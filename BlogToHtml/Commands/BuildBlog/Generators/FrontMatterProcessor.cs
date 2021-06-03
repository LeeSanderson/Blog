namespace BlogToHtml.Commands.BuildBlog.Generators
{
    using Markdig.Extensions.Yaml;
    using Markdig.Syntax;
    using System;
    using System.Linq;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.Converters;
    using YamlDotNet.Serialization.NamingConventions;

    internal class FrontMatterProcessor
    {
        private const string YamlDateFormat = "yyyy-MM-dd HH:mm:ss";
        private readonly IDeserializer yamlDeserialiser;

        public FrontMatterProcessor()
        {
            var builder = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeConverter(new DateTimeConverter(DateTimeKind.Unspecified, null, YamlDateFormat));

            this.yamlDeserialiser = builder.Build();
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

            return this.yamlDeserialiser.Deserialize<T>(yaml);
        }
   }
}
