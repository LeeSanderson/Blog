using YamlDotNet.Serialization;

namespace BlogToHtml.Models;

public class ArticleModel : SummaryModel
{
    [YamlIgnore]
    public string Content { get; set; } = string.Empty;
}