namespace BlogToHtml.Commands.BuildBlog.Models
{
    using YamlDotNet.Serialization;

    public class ArticleModel : SummaryModel
    {
        [YamlIgnore]
        public string Content { get; set; } = string.Empty;
    }
}