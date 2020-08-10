using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceGuard
{
    [Generator]
    public class ServiceGuardGenerator : ISourceGenerator
    {
        public void Initialize(InitializationContext context)
        {
            // Register a syntax receiver that will be created for each generation pass
            context.RegisterForSyntaxNotifications(() => new TypeDeclarationSyntaxReceiver());
        }

        public void Execute(SourceGeneratorContext context)
        {
            // retreive the populated receiver
            if (!(context.SyntaxReceiver is TypeDeclarationSyntaxReceiver receiver))
                return;

            // begin creating the source we'll inject into the users compilation
            var debugSourceBuilder = new StringBuilder(@"
using System;
namespace ServiceGuard
{
    public static class Debug
    {
        public static void LogGeneratedClasses( )
        {
");

            var typesToGenerate = new List<(ITypeSymbol implementationSymbol, INamedTypeSymbol interfaceSymbol)>();
            foreach (var typeDeclaration in receiver.DeclaredTypes)
            {
                var model = context.Compilation.GetSemanticModel(typeDeclaration.SyntaxTree);
                var typeSymbol = model.GetDeclaredSymbol(typeDeclaration) as ITypeSymbol;

                if (typeSymbol.IsAbstract)
                {
                    continue;
                }

                var typeFullName = typeSymbol.ToDisplayString();
                foreach (var interfaceSymbol in typeSymbol.AllInterfaces)
                {
                    bool hasServiceContract = interfaceSymbol.HasServiceContract();

                    if (hasServiceContract)
                    {
                        typesToGenerate.Add((typeSymbol, interfaceSymbol));
                    }
                }
            }

            foreach (var typeToGenerate in typesToGenerate)
            {
                (var implementationSymbol, var interfaceSymbol) = typeToGenerate;

                var interfaceAttrs = interfaceSymbol.GetAuthenticationAttributes();

                var guardedNameSpace = implementationSymbol.ToDisplayString()
                    .Split('.')
                    .Reverse()
                    .Skip(1)
                    .Reverse()
                    .StringJoin(".");
                var guardedName = implementationSymbol.Name + "_Guarded";
                var implementationFullName = implementationSymbol.ToDisplayString();
                var interfaceFullName = interfaceSymbol.ToDisplayString();

                var ctor = implementationSymbol.GetConstructor();

                var typeConstructorArgumentsDefinition = ctor.Parameters.Select((p, i) => $"{p.ToDisplayString()} arg{i}").Concat(new[] { "ServiceGuard.IAuthenticationService authService" }).StringJoin(", ");
                var typeConstructorArgumentsUsage = ctor.Parameters.Select((p, i) => $"arg{i}").StringJoin(", ");

                var guardedProperties = "";
                //interfaceSymbol.GetMembers().OfType<IPropertySymbol>()
                //.Where(p=>p.getse)
                //.Select(p => $"");

                var guardedVoidMethodsList = interfaceSymbol.GetMembers().OfType<IMethodSymbol>()
                    .Where(o => o.ReturnsVoid)
                    .Select(method =>
                    {
                        var argumentsDefinition = method.Parameters.Select((p, i) => $"{p.ToDisplayString()} arg{i}").StringJoin(", ");
                        var argumentsUsage = method.Parameters.Select((p, i) => $"arg{i}").StringJoin(", ");

                        var guards = GetGuardCalls(method, interfaceAttrs);

                        return $@"public void {method.Name}({argumentsDefinition})
                            {{
                                {guards}
                                this.implementation.{method.Name}({argumentsUsage});
                            }}";
                    });

                var guardedReturnMethodsList = interfaceSymbol.GetMembers().OfType<IMethodSymbol>()
                    .Where(o => !o.ReturnsVoid)
                    .Select(method =>
                    {
                        var argumentsDefinition = method.Parameters.Select((p, i) => $"{p.ToDisplayString()} arg{i}").StringJoin(", ");
                        var argumentsUsage = method.Parameters.Select((p, i) => $"arg{i}").StringJoin(", ");

                        var guards = GetGuardCalls(method, interfaceAttrs);

                        return $@"public {method.ReturnType?.ToDisplayString()} {method.Name}({argumentsDefinition})
                            {{
                                {guards}
                               return this.implementation.{method.Name}({argumentsUsage});
                            }}";
                    });

                var guardedMethods = guardedVoidMethodsList.Concat(guardedReturnMethodsList).StringJoin(Environment.NewLine);

                var guardedType = $@"
                    namespace {guardedNameSpace}
                    {{
                        internal class {guardedName} : {interfaceFullName}
                        {{
                            private readonly {implementationFullName} implementation;
                            private readonly ServiceGuard.IAuthenticationService authService;

                            public {guardedName}({typeConstructorArgumentsDefinition})
                            {{
                                this.authService=authService;
                                this.implementation=new {implementationFullName}({typeConstructorArgumentsUsage});
                            }}

                            {guardedProperties}

                            {guardedMethods}
                        }}
                    }}";

                debugSourceBuilder.AppendLine($@"Console.WriteLine(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(@""{Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(guardedType))}"")));");
                context.AddSource($"{guardedName}", SourceText.From(guardedType, Encoding.UTF8));
            }

            // finish creating the source to inject
            debugSourceBuilder.Append(@"
        }
    }
}");

            // inject the created source into the users compilation
            context.AddSource("serviceGuardGenerator", SourceText.From(debugSourceBuilder.ToString(), Encoding.UTF8));
        }

        private static string GetGuardCalls(IMethodSymbol m, IEnumerable<AttributeData> interfaceAttrs)
        {
            var methodAttrs = m.GetAuthenticationAttributes();
            var authAttrs = interfaceAttrs.Concat(methodAttrs);
            var guards = authAttrs.Select(a =>
            {
                var policy = a.NamedArguments.Where(o => o.Key == "Policy").Select(o => o.Value.ToCSharpString()).DefaultIfEmpty("\"\"").FirstOrDefault();
                var roles = a.NamedArguments.Where(o => o.Key == "Roles").Select(o => o.Value.ToCSharpString()).DefaultIfEmpty("\"\"").FirstOrDefault();
                var authenticationSchemes = a.NamedArguments.Where(o => o.Key == "AuthenticationSchemes").Select(o => o.Value.ToCSharpString()).DefaultIfEmpty("\"\"").FirstOrDefault();

                return $"authService.Validate({policy},{roles},{authenticationSchemes});";
            })
                .StringJoin(Environment.NewLine);
            return guards;
        }
    }

    internal static class SymbolUtils
    {
        public static IMethodSymbol GetConstructor(this ITypeSymbol typeSymbol)
        {
            foreach (var member in typeSymbol.GetMembers())
            {
                if (member is IMethodSymbol methodSymbol)
                {
                    if (methodSymbol.Name == ".ctor")
                    {
                        return methodSymbol;
                    }
                }
            }
            return null;
        }

        public static IEnumerable<AttributeData> GetAuthenticationAttributes(this ISymbol typeSymbol)
        {
            foreach (var attr in typeSymbol.GetAttributes())
            {
                bool isProtected = attr.AttributeClass.ToDisplayString().Contains("Authorize");
                if (isProtected)
                {
                    yield return attr;
                }
            }
        }

        public static bool HasServiceContract(this ISymbol typeSymbol)
        {
            foreach (var attr in typeSymbol.GetAttributes())
            {
                bool hasServiceContract = attr.AttributeClass.ToDisplayString().Contains("ServiceContract");
                if (hasServiceContract)
                {
                    return true;
                }
            }
            return false;
        }

        public static string StringJoin<T>(this IEnumerable<T> items, string separator)
        {
            return string.Join(separator, items);
        }
    }

    internal class TypeDeclarationSyntaxReceiver : ISyntaxReceiver
    {
        public List<TypeDeclarationSyntax> DeclaredTypes { get; } = new List<TypeDeclarationSyntax>();

        /// <summary>
        /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
        /// </summary>
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            // any field with at least one attribute is a candidate for property generation
            if (syntaxNode is TypeDeclarationSyntax fieldDeclarationSyntax)
            {
                DeclaredTypes.Add(fieldDeclarationSyntax);
            }
        }
    }
}