
namespace BlogToHtml.Commands.GenerateHeroImage
{
    using CommandLine;
    using System.Collections.Generic;

    [Verb("hero", isDefault: false, HelpText = "Generate a hero image from the standard template using the given title and tage")]
    // ReSharper disable once ClassNeverInstantiated.Global - created implicitly in main program code.
    public class GenerateHeroImageOptions
    {
        [Option('o', "output", Required = true, HelpText = "The name of the file to create/overwrite")]
        public string? OutputFileName { get; set; }

        [Option('t', "title", Required = true, HelpText = "The title of the hero image")]
        public string? Title { get; set; }

        [Option('g', "tags", Required = false, HelpText = "Optional list of tags")]
        public IEnumerable<string>? Tags { get; set; }
    }
}