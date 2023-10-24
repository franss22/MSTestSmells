using Microsoft.VisualStudio.ApplicationInsights.DataContracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestSmells.Test
{
    internal class TestOptions
    {
        public static (string filename, string content) EnableSingleDiagnosticForCompendium(string diagnosticName)
        {
            return
                (
                "/.editorconfig",
                $@"root = true
[*.cs]
dotnet_diagnostic.AssertionRoulette.severity = {Severity(diagnosticName, "AssertionRoulette")}
dotnet_diagnostic.ConditionalTest.severity = {Severity(diagnosticName, "ConditionalTest")}
dotnet_diagnostic.EmptyTest.severity = {Severity(diagnosticName, "EmptyTest")}
dotnet_diagnostic.ExceptionHandling.severity = {Severity(diagnosticName, "ExceptionHandling")}
dotnet_diagnostic.IgnoredTest.severity = {Severity(diagnosticName, "IgnoredTest")}
dotnet_diagnostic.MagicNumber.severity = {Severity(diagnosticName, "MagicNumber")}
dotnet_diagnostic.RedundantAssertion.severity = {Severity(diagnosticName, "RedundantAssertion")}
dotnet_diagnostic.SleepyTest.severity = {Severity(diagnosticName, "SleepyTest")}
"
                );
        }

        private static string Severity(string name1, string name2)
        {
            return name1 == name2 ? "warning" : "silent";
        }


    }
}
