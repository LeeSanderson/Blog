using System;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using BlogToHtml.MarkdigExtensions;
using BlogToHtml.MarkdigExtensions.HeroHeadings;
using FluentAssertions;
using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using NSubstitute;
using Xunit;

namespace BlogToHtml.UnitTests.MarkdigExtensions.HeroHeadings;

public class HeroHeadingRendererShould
{
    private readonly MockFileSystem fileSystem = new();
    private readonly IMarkdownContext markdownContext;

    public HeroHeadingRendererShould()
    {
        markdownContext = Substitute.For<IMarkdownContext>();
        markdownContext.CurrentSourceFile = fileSystem.FileInfo.New(@"c:\Content\Test1.md");
    }

    [Fact]
    public void RenderHeading() => 
        RenderHeadingAsHtml("heading").Should().Contain(@"<h1>heading</h1>");

    [Fact]
    public void RenderHeroHeading() =>
        RenderHeadingAsHtml("heading", true)
            .Should()
            .Contain(@"<h1 data-hero-heading=""true"">heading</h1>");

    [Fact]
    public void RenderHeroImageAfterHeading() =>
        RenderHeadingAsHtml("heading", true)
            .Should()
            .Contain(@"<h1 data-hero-heading=""true"">heading</h1>")
            .And
            .Contain(@"<img class=""hero-image"" src=""Test1.png"" alt=""heading""/>");

    [Fact]
    public void ErrorWhenRenderingHeroImageIfContextNotSetup()
    {
        markdownContext.CurrentSourceFile = null;
        var render = () => RenderHeadingAsHtml("heading", true);
        render.Should().Throw<Exception>().WithMessage("Cannot render hero heading. CurrentSourceFile not set.");
    }


    private string RenderHeadingAsHtml(string heading, bool isHero = false)
    {
        var markdownDocument = Markdown.Parse($"# {heading}");
        var headingBlock = markdownDocument[0] as HeadingBlock;
        headingBlock.Should().NotBeNull();
        if (isHero)
        {
            headingBlock!.GetAttributes().AddPropertyIfNotExist("data-hero-heading", "true");
        }

        var heroHeadingRenderer = new HeroHeadingRenderer(null, markdownContext);
        var stringWriter = new StringWriter();
        var htmlRenderer = new HtmlRenderer(stringWriter);
        heroHeadingRenderer.Write(htmlRenderer, headingBlock!);
        var html = stringWriter.GetStringBuilder().ToString();

        return html;
    }
}