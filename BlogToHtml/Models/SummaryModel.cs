﻿using System.IO.Abstractions;

namespace BlogToHtml.Models
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

        [YamlMember(Alias = "status")]
        public PublicationStatus PublicationStatus { get; set; }

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
}

public enum PublicationStatus
{
    Draft,
    Published
}
