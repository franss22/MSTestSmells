using Microsoft.CodeAnalysis.Testing;
using System.Collections.Immutable;
using static Microsoft.CodeAnalysis.Testing.ReferenceAssemblies;

namespace TestSmells.Test
{
    public class TestSmellReferenceAssembly
    {
        public static Microsoft.CodeAnalysis.Testing.ReferenceAssemblies Assemblies()
        {
            return Net.Net70
                .AddPackages(ImmutableArray.Create(new PackageIdentity("MSTest.TestFramework", "3.1.1")))
                .AddAssemblies(ImmutableArray.Create("Microsoft.VisualStudio.UnitTesting"));
        }
    }
}
