namespace BlogToHtml.Commands.BuildBlog.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using YamlDotNet.Serialization;

    public class ArticleModel
    {
        [YamlMember(Alias = "title")]
        public string Title { get; set; } = "Untitled Blogpost";

        [YamlMember(Alias = "abstract")]
        public string Abstract { get; set; } = string.Empty;

        [YamlMember(Alias = "tags")]
        public string? Tags { get; set; }

        public IEnumerable<string> GetTags() => 
            Tags?.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()) ?? Enumerable.Empty<string>(); 

        [YamlMember(Alias = "date")]
        public DateTime? PublicationDate { get; set; }

        public string Content { get; set; } = string.Empty;
    }
}