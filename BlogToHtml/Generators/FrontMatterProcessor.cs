using System;
using System.Linq;
using System.Text.RegularExpressions;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.Converters;
using YamlDotNet.Serialization.NamingConventions;

namespace BlogToHtml.Generators;

internal class FrontMatterProcessor
{
    private const string YamlDateFormat = "yyyy-MM-dd HH:mm:ss";
    private readonly Regex frontMatterExtractor = new(@"^(---\s*\r?\n.*?\r?\n---)\s*\r?\n(.*)", RegexOptions.Singleline);
    private readonly IDeserializer yamlDeserialiser;

    public FrontMatterProcessor()
    {
        var builder = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .WithTypeConverter(new DateTimeConverter(DateTimeKind.Unspecified, formats: YamlDateFormat));

        yamlDeserialiser = builder.Build();
    }

    public T GetFrontMatter<T>(MarkdownDocument document)
        where T : new()
    {
        var block = document
            .Descendants<YamlFrontMatterBlock>()
            .FirstOrDefault();

        if (block == null)
            return new T();

        var yaml =
            block
                // this is not a mistake
                // we have to call .Lines 2x
                .Lines // StringLineGroup[]
                .Lines // StringLine[]
                .OrderByDescending(x => x.Line)
                .Select(x => $"{x}\n")
                .ToList()
                .Select(x => x.Replace("---", string.Empty))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Aggregate((s, agg) => agg + s);

        return yamlDeserialiser.Deserialize<T>(yaml);
    }

    public (string frontMatter, string markdown) RemoveFrontMatter(string source)
    {
        var match = frontMatterExtractor.Match(source);
        if (match.Success)
        {
            var frontMatter = match.Groups[1].Value.Trim();
            var markdown = match.Groups[2].Value.Trim();
            return (frontMatter, markdown);
        }

        return (string.Empty, source);
    }
}