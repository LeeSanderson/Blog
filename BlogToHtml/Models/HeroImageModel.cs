using System.Collections.Generic;
using System.Linq;

namespace BlogToHtml.Models;

public class HeroImageModel
{
    public string Title { get; set; } = "Untitled Blog post";
    public string[]? Tags { get; set; }
    public IEnumerable<string> GetTags() => Tags ?? Enumerable.Empty<string>();
    public bool OutputHtml { get; set; }
}