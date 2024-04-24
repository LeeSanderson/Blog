﻿using System.Linq;
using System.Threading.Tasks;
using BlogToHtml.Commands.BuildBlog.Models;
using FluentAssertions;

namespace BlogToHtml.UnitTests.Commands.BuildBlog.Generators
{
    using BlogToHtml.Commands.BuildBlog.Generators;
    using BlogToHtml.Commands.BuildBlog;
    using System.IO.Abstractions.TestingHelpers;
    using Xunit;
    using System.Collections.Generic;
    using System;

    public class HeroImageGeneratorShould
    {
        [Fact]
        public async Task GenerateAnImage()
        {
            var generateImageFile = await GenerateImage(new HeroImageModel {Title = "A test image", Tags = new[] {"Tag1", "Tag2"}});
            var image = generateImageFile.Contents;

            image.Length.Should().BeGreaterThan(0);
            IsValidPngImage(image).Should().BeTrue();
        }

        private static HeroImageGenerator CreateHeroImageGenerator(MockFileSystem fileSystem)
        {
            var contentDirectory = fileSystem.DirectoryInfo.New(@"c:\Content\");
            var outputDirectory = fileSystem.DirectoryInfo.New(@"c:\Output\");
            var generatorContext = new GeneratorContext(RazorEngineFactory.CreateRazorEngineService(), fileSystem,
                contentDirectory, outputDirectory);
            var generator = new HeroImageGenerator(generatorContext);
            return generator;
        }

        private static async Task<MockFileData> GenerateImage(HeroImageModel model)
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\Content\Test1.md", "Generator does not use source file content. Model is used instead" }
            });
            var generator = CreateHeroImageGenerator(fileSystem);
            var sourceFile = fileSystem.FileInfo.New(@"c:\Content\Test1.md");

            await generator.GenerateImageAsync(sourceFile, model);

            return fileSystem.GetFile(@"c:\Output\Test1.png");
        }

        private static bool IsValidPngImage(byte[] b)
        {
            var png = new byte[] { 137, 80, 78, 71 };
            var buffer = new byte[png.Length];
            Buffer.BlockCopy(b, 0, buffer, 0, buffer.Length);
            return png.SequenceEqual(buffer.Take(png.Length));
        }
    }
}
