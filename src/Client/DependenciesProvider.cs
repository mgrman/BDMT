using System.Collections.Generic;

namespace BDMT.Client
{
    public static class DependenciesProvider
    {
        public static IReadOnlyList<string> Scripts { get; } = new[]
        {
            "_content/BDMT.Client/js/createAndSubmitForm.js"
        };
    }
}