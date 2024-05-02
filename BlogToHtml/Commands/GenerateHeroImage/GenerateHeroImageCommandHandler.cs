﻿using System.Linq;
using BlogToHtml.Generators;
using BlogToHtml.Models;

namespace BlogToHtml.Commands.GenerateHeroImage
{
    using System;
    using System.Threading.Tasks;
    using Serilog;
    using System.IO.Abstractions;

    internal class GenerateHeroImageCommandHandler
    {
        private readonly ILogger logger;
        private readonly IFileSystem fileSystem;
        private readonly GenerateHeroImageOptions options;

        public GenerateHeroImageCommandHandler(IFileSystem fileSystem, GenerateHeroImageOptions options)
        {
            logger = Log.ForContext<GenerateHeroImageCommandHandler>();
            this.fileSystem = fileSystem;
            this.options = options;
        }

        public async Task<int> RunAsync()
        {
            try
            {
                var outputFile = fileSystem.ToFileInfo(options.OutputFileName, false);
                var outputDirectory = outputFile.Directory!.CreateIfNotExists();
                var generatorContext = 
                    new GeneratorContext(
                        RazorEngineFactory.CreateRazorEngineService(), 
                        fileSystem, 
                        outputDirectory, 
                        outputDirectory);
                var generator = new HeroImageGenerator(generatorContext);

                var model = new HeroImageModel {Title = options.Title ?? string.Empty, Tags = options.Tags?.ToArray()};

                await generator.GenerateImageAsync(outputFile, model);
                return 0; // Success
            }
            catch (Exception e)
            {
                logger.Error(e, "Generate Hero Image Command Failed!");
                return 1; // Failure
            }
        }
    }
}
