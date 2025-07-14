using System;
using BlogToHtml.Generators;
using BlogToHtml.Models;
using FluentAssertions;
using Markdig.Parsers;
using Xunit;

namespace BlogToHtml.UnitTests.Generators;

public class FrontMatterProcessorShould
{
    [Fact]
    public void ParseMarkdownDocumentWithNoFrontMatter() =>
        ExtractModelFromFrontMatter(string.Empty).Should().BeEquivalentTo(new ArticleModel());


    private const string ArticleWithTitleMarkdown = @"---
title: An article with a title
---

# Heading 1
";
    [Fact]
    public void ParseTitleFromFrontMatter() =>
        ExtractModelFromFrontMatter(ArticleWithTitleMarkdown).Title.Should().BeEquivalentTo("An article with a title");


    private const string ArticleWithAbstractMarkdown = @"---
abstract: The abstract, i.e. the main point of the article
---

# Heading 1
";

    [Fact]
    public void ParseAbstractFromFrontMatter() =>
        ExtractModelFromFrontMatter(ArticleWithAbstractMarkdown)
            .Abstract
            .Should()
            .BeEquivalentTo("The abstract, i.e. the main point of the article");


    private const string ArticleWithTagsMarkdown = @"---
tags:
  - mini
  - k8s
---

# Heading 1
";
    [Fact]
    public void ParseTagsFromFrontMatter() =>
        ExtractModelFromFrontMatter(ArticleWithTagsMarkdown).Tags.Should().BeEquivalentTo("mini", "k8s");


    private const string ArticleWithDateMarkdown = @"---
date: 2023-10-09
---

# Heading 1
";
    [Fact]
    public void ParseDateFromFrontMatter() =>
        ExtractModelFromFrontMatter(ArticleWithDateMarkdown).PublicationDate.Should().Be(new DateTime(2023, 10, 9));

    private const string ArticleWithDraftStatusMarkdown = @"---
status: Draft
---

# Heading 1
";
    [Fact]
    public void ParseDraftStatusFromFrontMatter() =>
        ExtractModelFromFrontMatter(ArticleWithDraftStatusMarkdown).PublicationStatus.Should().Be(PublicationStatus.Draft);


    private const string ArticleWithPublishedStatusMarkdown = @"---
status: Published
---

# Heading 1
";
    [Fact]
    public void ParsePublishedStatusFromFrontMatter() =>
        ExtractModelFromFrontMatter(ArticleWithPublishedStatusMarkdown).PublicationStatus.Should().Be(PublicationStatus.Published);

    private static ArticleModel ExtractModelFromFrontMatter(string markdownSource)
    {
        var processor = new FrontMatterProcessor();
        var markdown =
            MarkdownParser.Parse(markdownSource, MarkdownPipelineFactory.GetStandardPipeline());
        var model = processor.GetFrontMatter<ArticleModel>(markdown);
        return model;
    }
}