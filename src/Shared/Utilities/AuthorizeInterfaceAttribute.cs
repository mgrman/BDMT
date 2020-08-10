using Microsoft.AspNetCore.Authorization;
using System;

namespace BDMT.Shared
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
    public class AuthorizeInterfaceAttribute : AuthorizeAttribute
    {
    }
}