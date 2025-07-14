using Xunit;

using BlogToHtml.Notebooks;
using FluentAssertions;

namespace BlogToHtml.UnitTests.Notebooks;

public class NotebookConverterShould
{
    [Fact]
    public void IdentifyLanguageOfNotebook()
    {
        var converter = new NotebookConverter();
        
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
        var converter = new NotebookConverter();

        var notebook = converter.Convert("{}");

        notebook.Language.Should().Be("python");
    }
}