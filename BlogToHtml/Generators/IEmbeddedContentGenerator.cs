using System.Threading.Tasks;

namespace BlogToHtml.Generators
{
    internal interface IEmbeddedContentGenerator
    {
        Task GenerateContentAsync();
    }
}