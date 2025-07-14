using System.Text.Json;

namespace BlogToHtml.Notebooks;

internal class NotebookConverter
{
    private const string DefaultLanguage = "python";

    public Notebook Convert(string notebookJson)
    {
        using var document = JsonDocument.Parse(notebookJson);
        var root = document.RootElement;

        // Example: extract language_info.name
        string? language = null;
        if (root.TryGetProperty("metadata", out var metadata) &&
            metadata.TryGetProperty("language_info", out var languageInfo) &&
            languageInfo.TryGetProperty("name", out var nameProp))
        {
            language = nameProp.GetString();
        }

        return new Notebook { Language = language ?? DefaultLanguage };
    }
}

internal class Notebook
{
    public string Language { get; set; }
}