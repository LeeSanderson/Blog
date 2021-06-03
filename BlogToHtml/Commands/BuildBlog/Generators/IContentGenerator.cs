namespace BlogToHtml.Commands.BuildBlog.Generators
{
    using System.IO;
    using System.Threading.Tasks;

    public interface IContentGenerator
    {
        Task GenerateContentAsync(FileInfo sourceFileInfo);
    }
}
