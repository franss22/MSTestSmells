using CommandLine;

namespace TestSmells.Console
{

    internal static partial class Program
    {
        private class ConsoleProgramOptions
        {
            [Option('s', "solution", Required = true, HelpText = "Absolute path to solution to be analyzed.")]
            public string Solution { get; set; }

            [Option('o', "output", Required = false, Default = null, HelpText = "File path for diagnostic csv output. If left empty, results are printed in stdout")]
            public string? Output { get; set; }

            [Option('m', "method_output", Required = false, Default = null, HelpText = "File path for method summary csv output. If left empty, results are not printed")]
            public string? OutputMethods { get; set; }

            [Option('c', "config", Required = false, Default = null, HelpText = "File path for global JSON config file")]
            public string? Config { get; set; }

            [Option('i', "ignore_default_config", Required = false, Default = false, HelpText = "Override of default analyzer configuration. If enabled, analyzers will use default configuration instead of any local editorconfig values. Does not apply to diagnosis severity.")]
            public bool IgnoreExistingConfig { get; set; }

            [Option('l', "method_list_output", Required = false, Default = null, HelpText = "File path for method list csv output. If left empty, results are not printed")]
            public string? MethodListOutput { get; set; }


        }
    }
}