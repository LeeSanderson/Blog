﻿using System.IO.Abstractions;
using BlogToHtml.Models;
using FluentAssertions;
using System;
using System.Linq;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace BlogToHtml.UnitTests.Commands.BuildBlog;

public class BuildBlogCommandHandlerShould
{
    [Fact]
    public async Task Generate_expected_HTML_from_markdown()
    {
        using var blogOutput = 
            await new BlogBuilder(new FileSystem())
                .AddContent("Example.md", "# Heading 1", tags: ["test", "with", "tags"])
                .GenerateAsync(false);

        var generatedHtmlFile = blogOutput.GeneratedFiles.First(f => f.Name == "Example.html");
        await Verifier.VerifyFile(generatedHtmlFile.FullName);
    }

    [Fact]
    public async Task Generate_expected_hero_image()
    {
        using var blogOutput =
            await new BlogBuilder(new FileSystem())
                .AddContent("Example.md", "# Heading 1")
                .GenerateAsync(true);
        blogOutput.GeneratedFiles.First(f => f.Name == "Example.png").Should().NotBeNull();
    }


    [Fact]
    public async Task Generate_index_page_with_most_recent_10_blog_articles()
    {
        using var blogOutput = 
            await new BlogBuilder(new FileSystem())
                .AddContent("Example1.md", "# Heading 1", publicationDate: new DateTime(2022, 1, 1))
                .AddContent("Example2.md", "# Heading 2", publicationDate: new DateTime(2022, 1, 2))
                .AddContent("Example3.md", "# Heading 3", publicationDate: new DateTime(2022, 1, 3))
                .AddContent("Example4.md", "# Heading 4", publicationDate: new DateTime(2022, 1, 4))
                .AddContent("Example5.md", "# Heading 5", publicationDate: new DateTime(2022, 1, 5))
                .AddContent("Example6.md", "# Heading 6", publicationDate: new DateTime(2022, 1, 6))
                .AddContent("Example7.md", "# Heading 7", publicationDate: new DateTime(2022, 1, 7))
                .AddContent("Example8.md", "# Heading 8", publicationDate: new DateTime(2022, 1, 8))
                .AddContent("Example9.md", "# Heading 9", publicationDate: new DateTime(2022, 1, 9))
                .AddContent("Example10.md", "# Heading 10", publicationDate: new DateTime(2022, 1, 10))
                .AddContent("Example11.md", "# Heading 11", publicationDate: new DateTime(2022, 1, 11))
                .GenerateAsync(false);

        var generatedIndexHtmlFile = blogOutput.GeneratedFiles.First(f => f.Name == "index.html");
        await Verifier.VerifyFile(generatedIndexHtmlFile.FullName);
    }

    [Fact]
    public async Task Not_generate_pages_with_draft_status()
    {
        using var blogOutput =
            await new BlogBuilder(new FileSystem())
                .AddContent("Example1.md", "# Heading 1", publicationDate: new DateTime(2022, 1, 1))
                .AddContent("Example2.md", "# Heading 2", publicationDate: new DateTime(2022, 1, 2), publicationStatus: PublicationStatus.Draft)
                .GenerateAsync(false);

        var generatedIndexHtmlFile = blogOutput.GeneratedFiles.First(f => f.Name == "index.html");
        await Verifier.VerifyFile(generatedIndexHtmlFile.FullName);
    }

    [Fact]
    public async Task Generate_all_page_all_blog_articles()
    {
        using var blogOutput =
            await new BlogBuilder(new FileSystem())
                .AddContent("Example1.md", "# Heading 1", publicationDate: new DateTime(2022, 1, 1))
                .AddContent("Example2.md", "# Heading 2", publicationDate: new DateTime(2022, 1, 2))
                .AddContent("Example3.md", "# Heading 3", publicationDate: new DateTime(2022, 1, 3))
                .AddContent("Example4.md", "# Heading 4", publicationDate: new DateTime(2022, 1, 4))
                .AddContent("Example5.md", "# Heading 5", publicationDate: new DateTime(2022, 1, 5))
                .AddContent("Example6.md", "# Heading 6", publicationDate: new DateTime(2022, 1, 6))
                .AddContent("Example7.md", "# Heading 7", publicationDate: new DateTime(2022, 1, 7))
                .AddContent("Example8.md", "# Heading 8", publicationDate: new DateTime(2022, 1, 8))
                .AddContent("Example9.md", "# Heading 9", publicationDate: new DateTime(2022, 1, 9))
                .AddContent("Example10.md", "# Heading 10", publicationDate: new DateTime(2022, 1, 10))
                .AddContent("Example11.md", "# Heading 11", publicationDate: new DateTime(2022, 1, 11))
                .GenerateAsync(false);

        var generatedIndexHtmlFile = blogOutput.GeneratedFiles.First(f => f.Name == "all.html");
        await Verifier.VerifyFile(generatedIndexHtmlFile.FullName);
    }

    [Fact]
    public async Task Generate_rss_for_all_blog_articles()
    {
        using var blogOutput =
            await new BlogBuilder(new FileSystem())
                .AddContent("Example1.md", "# Heading 1", publicationDate: new DateTime(2022, 1, 1))
                .AddContent("Example2.md", "# Heading 2", publicationDate: new DateTime(2022, 1, 2))
                .AddContent("Example3.md", "# Heading 3", publicationDate: new DateTime(2022, 1, 3))
                .AddContent("Example4.md", "# Heading 4", publicationDate: new DateTime(2022, 1, 4))
                .AddContent("Example5.md", "# Heading 5", publicationDate: new DateTime(2022, 1, 5))
                .AddContent("Example6.md", "# Heading 6", publicationDate: new DateTime(2022, 1, 6))
                .AddContent("Example7.md", "# Heading 7", publicationDate: new DateTime(2022, 1, 7))
                .AddContent("Example8.md", "# Heading 8", publicationDate: new DateTime(2022, 1, 8))
                .AddContent("Example9.md", "# Heading 9", publicationDate: new DateTime(2022, 1, 9))
                .AddContent("Example10.md", "# Heading 10", publicationDate: new DateTime(2022, 1, 10))
                .AddContent("Example11.md", "# Heading 11", publicationDate: new DateTime(2022, 1, 11))
                .GenerateAsync(false);

        var rssFeedFile = blogOutput.GeneratedFiles.First(f => f.Name == "rss.xml");
        await Verifier
            .VerifyFile(rssFeedFile.FullName)
            .ScrubLinesContaining("<lastBuildDate>")
            .ScrubLinesContaining("<pubDate>"); // Build server run different local time, which means time offsets are different from dev.
    }

    [Fact]
    public async Task Generate_blog_article_from_remote_notebook()
    {
        const string notebookUrl = "https://raw.githubusercontent.com/LeeSanderson/MLByExample/refs/heads/main/DecisionTreeClassifier.ipynb";
        using var blogOutput =
            await new BlogBuilder(new FileSystem())
                .AddContent(
                    "NotebookExample.md", 
                    "# Heading 1", 
                    notebookUrl: notebookUrl)
                .AddExternalNotebookContent(
                    notebookUrl,
                    """
                    {
                        "cells": [
                            {
                                "cell_type": "markdown",
                                "source": "# Content from notebook"
                            }
                        ]
                    }
                    """)
                .GenerateAsync(false);

        var generatedHtmlFile = blogOutput.GeneratedFiles.First(f => f.Name == "NotebookExample.html");
        await Verifier.VerifyFile(generatedHtmlFile.FullName);
    }
}