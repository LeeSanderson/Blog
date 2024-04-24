namespace BlogToHtml.Generators
{
    using Models;
    using RazorEngine.Templating;

    public class HeroImagesTemplate : TemplateBase<HeroImageModel>
    {
        public HeroImagesTemplate(IRazorEngineService razorEngineService) : base(razorEngineService, "HeroImage")
        {
        }
    }
}