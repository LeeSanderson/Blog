using System.Threading.Tasks;

namespace BlogToHtml.Commands.BuildBlog.Generators
{
    internal interface IEmbeddedContentGenerator
    {
        Task GenerateContentAsync();
    }
}