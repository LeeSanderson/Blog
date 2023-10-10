using System.IO.Abstractions;

namespace BlogToHtml.Commands.BuildBlog.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using YamlDotNet.Serialization;

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

        [YamlIgnore]
        public IFileInfo? OutputFileInfo { get; set; }

        public SummaryModel Clone()
        {
            return new SummaryModel 
            { 
                Title = this.Title,
                Abstract = this.Abstract,
                Tags = this.Tags,
                PublicationDate = this.PublicationDate,
                OutputFileInfo = this.OutputFileInfo
            }; 
        }
    }
}