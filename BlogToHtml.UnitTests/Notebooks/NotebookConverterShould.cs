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
                        "name": "python"
                    }
                }
            }
            """);

        notebook.Language.Should().Be("python");
    }

    [Fact]
    public void AssumeLanguageIsPythonIfNotDefined()
    {
        var notebook = converter.Convert("{}");

        notebook.Language.Should().Be("python");
    }
}