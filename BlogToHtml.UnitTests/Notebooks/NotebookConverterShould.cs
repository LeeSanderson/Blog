using Xunit;

using BlogToHtml.Notebooks;
using FluentAssertions;

namespace BlogToHtml.UnitTests.Notebooks;

public class NotebookConverterShould
{
    private readonly NotebookConverter converter = new();

    [Fact]
    public void IdentifyLanguageOfNotebook()
    {
        var notebook = converter.Convert(
            """
            {
                "metadata": {
                    "language_info": {
                        "name": "fsharp"
                    }
                }
            }
            """);

        notebook.Language.Should().Be("fsharp");
    }

    [Fact]
    public void AssumeLanguageIsPythonIfNotDefined()
    {
        var notebook = converter.Convert("{}");

        notebook.Language.Should().Be("python");
    }

    [Fact]
    public void ExtractMarkdownFromSimpleMarkdownCell()
    {
        var notebook = converter.Convert(
            """
            {
                "cells": [
                    {
                        "cell_type": "markdown",
                        "source": "# Hello World"
                    }
                ]
            }
            """);

        notebook.Markdown.Should().Be("# Hello World");
    }

    [Fact]
    public void ExtractMarkdownFromMultilineMarkdownCell()
    {
        var notebook = converter.Convert(
            """
            {
                "cells": [
                    {
                        "cell_type": "markdown",
                        "source": [ "# Hello World\n" , "This is a test." ]
                    }
                ]
            }
            """);

        notebook.Markdown.Should().Be("# Hello World\nThis is a test.");
    }
}