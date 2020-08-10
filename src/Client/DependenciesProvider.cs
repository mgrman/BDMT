using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BDMT.Client
{
    public static class DependenciesProvider
    {
        public static IReadOnlyList<string> Scripts { get; } = new[]
        {
            "_content/BDMT.Client/js/createAndSubmitForm.js",
            "_content/Fluxor.Blazor.Web/scripts/index.js",
        };
    }
}