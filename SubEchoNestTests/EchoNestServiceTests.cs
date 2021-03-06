﻿namespace SubEchoNestTests
{
    using Common.Mocks;

    using FluentAssertions;
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using SubEchoNest;

    [TestClass]
    public class EchoNestServiceTests
    {
        private EchoNestService _subject;

        [TestInitialize]
        public void Setup()
        {
            _subject = new EchoNestService(new MockEchoNestConfigurationProvider());
        }

        [TestMethod]
        public void GetArtistDetails_Always_ReturnsAGetArtistDetailsResultWithTheGivenArtistName()
        {
            var getBiographiesResult = _subject.GetArtistBiographies("testArtist");

            getBiographiesResult.ArtistName.Should().Be("testArtist");
        }
    }
}