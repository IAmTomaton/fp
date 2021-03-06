﻿using Autofac;
using NUnit.Framework;
using System.Linq;
using TagsCloudContainer.Readers;
using TagsCloudContainer.TokensAndSettings;
using TagsCloudContainer.WordPreprocessors;
using FluentAssertions;

namespace TagsCloudContainerTests
{
    [TestFixture]
    class SimpleWordPreprocessorTests
    {
        private ContainerBuilder containerBuilder;

        [SetUp]
        public void SetUp()
        {
            containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterType<SimpleWordPreprocessor>().As<IWordPreprocessor>();
        }

        [TestCase("Олень", ExpectedResult = new[] { "олень" })]
        [TestCase("лЕС", ExpectedResult = new[] { "лес" })]
        [TestCase("ДОРОГА", ExpectedResult = new[] { "дорога" })]
        public string[] CountWords_ToLower(string word)
        {
            var container = containerBuilder.Build();
            var simpleWordPreprocessor = container.Resolve<IWordPreprocessor>();

            var result = simpleWordPreprocessor.PreprocessWords(new[] { word });

            return result.GetValueOrThrow().Select(resultWord => resultWord.Word).ToArray();
        }

        [TestCase("бегают", ExpectedResult = new[] { "бегать" })]
        [TestCase("прыгают машут", ExpectedResult = new[] { "прыгать", "махать" })]
        public string[] CountWords_InitialForm(string word)
        {
            var container = containerBuilder.Build();
            var simpleWordPreprocessor = container.Resolve<IWordPreprocessor>();

            var result = simpleWordPreprocessor.PreprocessWords(new[] { word });

            return result.GetValueOrThrow().Select(resultWord => resultWord.Word).ToArray();
        }

        [Test]
        public void CountWords_FromTxt()
        {
            var container = containerBuilder.Build();
            var simpleWordPreprocessor = container.Resolve<IWordPreprocessor>();
            var simpleReader = new SimpleReader(System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, "WordsRus.txt"));

            var result = simpleWordPreprocessor.PreprocessWords(simpleReader.ReadAllLines().GetValueOrThrow());

            result.GetValueOrThrow().Should().BeEquivalentTo(new[] {
                new ProcessedWord("огонь", "S"),
                new ProcessedWord("полено", "S") });
        }
    }
}
