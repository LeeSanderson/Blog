using System.Text;
using System.Text.Json;

namespace BlogToHtml.Notebooks;


/// <summary>
/// Processes Jupyter Notebook JSON to extract Markdown content and language information.
/// Uses format defined here: https://nbformat.readthedocs.io/en/latest/format_description.html
/// </summary>
internal class NotebookConverter
{
    private readonly StringBuilder markdownBuilder = new();
    private const string DefaultLanguage = "python";

    public Notebook Convert(string notebookJson)
    {
        using var document = JsonDocument.Parse(notebookJson);
        var root = document.RootElement;

        var language = GetNotebookLanguage(root);
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
                            ProcessMarkdownCell(cell);
                            break;
                    }
                }
            }
        }
    }

    private void ProcessMarkdownCell(JsonElement cell)
    {
        if (cell.TryGetProperty("source", out var source))
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