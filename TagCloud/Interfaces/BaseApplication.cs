﻿using System.Drawing;
using System.Linq;
using TagCloud.CloudVisualizerSpace;
using TagCloud.CloudVisualizerSpace.CloudViewConfigurationSpace;
using TagCloud.WordsPreprocessing.DocumentParsers;

namespace TagCloud.Interfaces
{
    public class BaseApplication
    {
        public ApplicationSettings AppSettings { get; }
        private readonly CloudVisualizer visualizer;
        public CloudViewConfiguration CloudConfiguration { get; }
        public IDocumentParser[] Parsers { get; }

        public BaseApplication(CloudVisualizer visualizer, CloudViewConfiguration cloudConfiguration,
            IDocumentParser[] parsers, ApplicationSettings settings)
        {
            AppSettings = settings;
            this.visualizer = visualizer;
            CloudConfiguration = cloudConfiguration;
            Parsers = parsers;
        }

        public Result<Image> GetImage()
        {
            var format = $".{AppSettings.FilePath.Split('.').Last()}";
            var parser = Parsers.First(p => p.AllowedTypes.Contains(format));
            var bitmapResult = parser.GetWords(AppSettings)
                .Then(e => AppSettings.CurrentTextAnalyzer
                    .GetWords(e, CloudConfiguration.WordsCount))
                .Then(visualizer.GetCloud);
            return bitmapResult.IsSuccess
                ? Result.Ok((Image) bitmapResult.Value)
                : Result.Fail<Image>(bitmapResult.Error);
        }

        public void Close()
        {
            foreach (var parser in Parsers)
            {
                parser.Close();
            }
        }

        public void SetFontFamily(string fontFamily)
        {
            CloudConfiguration.FontFamily = new FontFamily(fontFamily);
        }
    }
}
