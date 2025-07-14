using System.IO.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Serialization;

namespace BlogToHtml.Models;

public class SummaryModel
{
    [YamlMember(Alias = "title")]
    public string Title { get; set; } = "Untitled Blog post";

    [YamlMember(Alias = "abstract")]
    public string Abstract { get; set; } = string.Empty;

    [YamlMember(Alias = "tags")]
    public string[]? Tags { get; set; }

    public IEnumerable<string> GetTags() => Tags ?? Enumerable.Empty<string>();

    [YamlMember(Alias = "date")]
    public DateTime? PublicationDate { get; set; }

    [YamlMember(Alias = "status")]
    public PublicationStatus PublicationStatus { get; set; }

    /// <summary>
    /// Optional URL to a Jupyter notebook associated with the article.
    /// Content from this url will be merged with the article content.
    /// </summary>
    [YamlMember(Alias = "notebookUrl")]
    public string? NotebookUrl { get; set; }

    [YamlIgnore]
    public IFileInfo? OutputFileInfo { get; set; }

    public SummaryModel Clone()
    {
        return new SummaryModel
        {
            Title = Title,
            Abstract = Abstract,
            Tags = Tags,
            PublicationDate = PublicationDate,
            OutputFileInfo = OutputFileInfo
        };
    }
}


public enum PublicationStatus
{
    Draft,
    Published
}
