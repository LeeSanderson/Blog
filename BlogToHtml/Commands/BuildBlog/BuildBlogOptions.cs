﻿using CommandLine;

namespace BlogToHtml.Commands.BuildBlog;

[Verb("build", isDefault: true, HelpText = "Build the HTML static site from a directory of Markdown files and images")]
// ReSharper disable once ClassNeverInstantiated.Global - created implicitly in main program code.
public class BuildBlogOptions
{
    [Option('c', "content", Required = true, HelpText = "The directory containing the content")]
    public string? ContentDirectory { get; set; }

    [Option('o', "output", Required = true, HelpText = "The directory to write the static content to")]
    public string? OutputDirectory { get; set; }

    [Option('x', "clean", Required = false, 
        HelpText = "Flag indicating if the output directory should be cleaned before content is generated. Defaults to True.")]
    public bool Clean { get; set; } = true;

    [Option('i', "images", Required = false,
        HelpText = "Flag indicating if hero images should be generated. Defaults to True.")]
    public bool GenerateHeroImages { get; set; } = true;
}