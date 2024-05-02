namespace BlogToHtml.Generators
{
    using Models;
    using RazorEngine.Templating;

    internal class HeroImagesTemplate : TemplateBase<HeroImageModel>
    {
        public HeroImagesTemplate(IRazorEngineService razorEngineService) : base(razorEngineService, "HeroImage")
        {
        }
    }
}