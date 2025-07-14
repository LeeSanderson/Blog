using System;
using System.Text;
using System.Text.Json;

namespace BlogToHtml.Notebooks;


/// <summary>
/// Processes Jupyter Notebook JSON to extract Markdown content and language information.
/// Uses format defined here: https://nbformat.readthedocs.io/en/latest/format_description.html
/// </summary>
internal class NotebookConverter
{
    private const string DefaultLanguage = "python";

    private readonly StringBuilder markdownBuilder = new();
    private string language = string.Empty;

    public Notebook Convert(string notebookJson)
    {
        using var document = JsonDocument.Parse(notebookJson);
        var root = document.RootElement;

        language = GetNotebookLanguage(root);
        markdownBuilder.Length = 0; // Reset the markdown builder
        BuildMarkdown(root);

        return new Notebook
        {
            Language = language, 
            Markdown = markdownBuilder.ToString()
        };
    }

    private void BuildMarkdown(JsonElement root)
    {
        if (root.TryGetProperty("cells", out var cells))
        {
            foreach (var cell in cells.EnumerateArray())
            {
                if (cell.TryGetProperty("cell_type", out var cellType))
                {
                    switch (cellType.GetString())
                    {
                        case "markdown":
                            ProcessCellSource(cell);
                            break;
                        case "code":
                            ProcessCodeCell(cell);
                            break;
                        default:
                            throw new NotSupportedException($"Unsupported cell type: {cellType.GetString()}");
                    }
                }
            }
        }
    }

    private void ProcessCodeCell(JsonElement cell)
    {
        WrapWithCodeSection(language, () => ProcessCellSource(cell));
        if (cell.TryGetProperty("outputs", out var outputs))
        {
            ProcessOutputsCell(outputs);
        }
    }

    private void ProcessOutputsCell(JsonElement outputs)
    {
        if (outputs.ValueKind == JsonValueKind.Array)
        {
            foreach (var output in outputs.EnumerateArray())
            {
                markdownBuilder.AppendLine();
                markdownBuilder.AppendLine();
                if (output.TryGetProperty("output_type", out var outputType))
                {
                    switch (outputType.GetString())
                    {
                        case "stream":
                            ProcessStreamOutput(output);
                            break;
                        case "execute_result":
                        case "display_data":
                            ProcessExecuteResultOutput(output);
                            break;
                        default:
                            throw new NotSupportedException($"Unsupported output type: {outputType.GetString()}");
                    }
                }
            }
        }

    }

    private void ProcessExecuteResultOutput(JsonElement output)
    {
        if (output.TryGetProperty("data", out var data))
        {
            if (data.TryGetProperty("text/plain", out var text))
            {
                WrapWithCodeSection("text", () => AppendToMarkdown(text));
            }

            if (data.TryGetProperty("text/html", out var htmlText))
            {
                AppendToMarkdown(htmlText);
            }
        }

    }

    private void ProcessStreamOutput(JsonElement output)
    {
        if (output.TryGetProperty("text", out var text))
        {
            WrapWithCodeSection("text", () => AppendToMarkdown(text));
        }
    }

    private void WrapWithCodeSection(string codeLanguage, Action writeMarkdownAction)
    {
        markdownBuilder.Append("``` ");
        markdownBuilder.AppendLine(codeLanguage);
        writeMarkdownAction();
        markdownBuilder.AppendLine();
        markdownBuilder.Append("```");

    }

    private void ProcessCellSource(JsonElement cell)
    {
        if (cell.TryGetProperty("source", out var source))
        {
            AppendToMarkdown(source);
        }
    }

    private void AppendToMarkdown(JsonElement source)
    {
        if (source.ValueKind == JsonValueKind.Array)
        {
            foreach (var line in source.EnumerateArray())
            {
                markdownBuilder.Append(line.GetString());
            }
        }
        else
        {
            markdownBuilder.Append(source.GetString());
        }
    }

    private static string GetNotebookLanguage(JsonElement root)
    {
        string? language = null;
        if (root.TryGetProperty("metadata", out var metadata) &&
            metadata.TryGetProperty("language_info", out var languageInfo) &&
            languageInfo.TryGetProperty("name", out var nameProp))
        {
            language = nameProp.GetString();
        }

        return language ?? DefaultLanguage;
    }
}

internal class Notebook
{
    public string Language { get; set; }
    public string Markdown { get; set; }
}