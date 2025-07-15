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
                        "source": [ "# Hello World\r\n" , "This is a test." ]
                    }
                ]
            }
            """);

        notebook.Markdown.Should().Be(
            """
            # Hello World
            This is a test.
            """);
    }

    [Fact]
    public void ExtractMarkdownFromMultilineCodeCell()
    {
        var notebook = converter.Convert(
            """
            {
                "cells": [
                    {
                        "cell_type": "code",
                        "source": [ "# Print Hello World\r\n" , "print(\"Hello world\")" ]
                    }
                ]
            }
            """);

        notebook.Markdown.Should().Be(
            """
            
            ``` python
            # Print Hello World
            print("Hello world")
            ```
            
            """);
    }

    [Fact]
    public void ExtractMarkdownFromCodeCellStreamOutput()
    {
        var notebook = converter.Convert(
            """
            {
                "cells": [
                    {
                        "cell_type": "code",
                        "source": [ "print(\"Hello world\")" ],
                        "outputs": [
                            {
                                "output_type": "stream",
                                "name": "stdout",
                                "text": [ "Hello world" ]
                            }
                        ]
                    }
                ]
            }
            """);

        notebook.Markdown.Should().Be(
            """
            
            ``` python
            print("Hello world")
            ```
            
            
            
            ``` text
            Hello world
            ```
            
            """);
    }

    [Fact]
    public void ExtractMarkdownFromCodeCellPlainTextExecutionResultOutput()
    {
        var notebook = converter.Convert(
            """
            {
                "cells": [
                    {
                        "cell_type": "code",
                        "source": [ "print(\"Hello world\")" ],
                        "outputs": [
                            {
                                "output_type": "execute_result",
                                "data": 
                                {
                                    "text/plain": [ "Hello world" ]
                                }
                            }
                        ]
                    }
                ]
            }
            """);

        notebook.Markdown.Should().Be(
            """
            
            ``` python
            print("Hello world")
            ```
            
            
            
            ``` text
            Hello world
            ```
            
            """);
    }

    [Fact]
    public void ExtractMarkdownFromCodeCellPlainTextDisplayDataOutput()
    {
        var notebook = converter.Convert(
            """
            {
                "cells": [
                    {
                        "cell_type": "code",
                        "source": [ "print(\"Hello world\")" ],
                        "outputs": [
                            {
                                "output_type": "display_data",
                                "data": 
                                {
                                    "text/plain": [ "Hello world" ]
                                }
                            }
                        ]
                    }
                ]
            }
            """);

        notebook.Markdown.Should().Be(
            """
            
            ``` python
            print("Hello world")
            ```

            
            
            ``` text
            Hello world
            ```
            
            """);
    }

    [Fact]
    public void ExtractMarkdownFromCodeCellHtmlExecutionResultOutput()
    {
        var notebook = converter.Convert(
            """
            {
                "cells": [
                    {
                        "cell_type": "code",
                        "source": [ "print(\"Hello world\")" ],
                        "outputs": [
                            {
                                "output_type": "execute_result",
                                "data": 
                                {
                                    "text/html": [ "<p>Hello world</p>" ]
                                }
                            }
                        ]
                    }
                ]
            }
            """);

        notebook.Markdown.Should().Be(
            """
            
            ``` python
            print("Hello world")
            ```

            
            <p>Hello world</p>
            """);
    }

    [Fact]
    public void ExtractMarkdownFromCodeCellImageExecutionResultOutput()
    {
        var notebook = converter.Convert(
            """
            {
                "cells": [
                    {
                        "cell_type": "code",
                        "source": [ "print(\"Hello world\")" ],
                        "outputs": [
                            {
                                "output_type": "execute_result",
                                "data": 
                                {
                                    "image/png": "bas64_encoded_image"
                                }
                            }
                        ]
                    }
                ]
            }
            """);

        notebook.Markdown.Should().Be(
            """
            
            ``` python
            print("Hello world")
            ```
            
            
            <img class="img-fluid" src="data:image/png;base64,bas64_encoded_image">
            
            """);
    }
}