using NUnit.Framework;
using FluentAssertions;

namespace Thum.io.Tests
{
    public class ImageModifierOptionsTests
    {
        [Test]
        public void Options_Should_Be_Empty_If_No_Values_Exist()
        {
            var options = new ImageModifierOptions();

            options.ToString().Should().BeEmpty();
        }

        [Test]
        public void Options_Should_Be_Set_If_Flags_Exist()
        {
            var options = new ImageModifierOptions
            {
                AllowJpg = true,
                NoAnimate = true
            };

            var optionsAsString = options.ToString();

            optionsAsString.Should().NotBeEmpty();
            optionsAsString.Should().Be($"{nameof(ImageModifierOptions.AllowJpg)}/{nameof(ImageModifierOptions.NoAnimate)}");
        }

        [Test]
        public void Options_Should_Be_Set_If_Values_Exist()
        {
            var options = new ImageModifierOptions
            {
                Width = 1000,
                Crop = 600
            };

            var optionsAsString = options.ToString();

            optionsAsString.Should().NotBeEmpty();
            optionsAsString.Should().Be($"{nameof(ImageModifierOptions.Width)}/{options.Width}/{nameof(ImageModifierOptions.Crop)}/{options.Crop}");
        }

        [Test]
        public void Options_Should_Be_Set()
        {
            var options = new ImageModifierOptions
            {
                AllowJpg = true,
                NoAnimate = true,
                Width = 1000,
                Crop = 600
            };

            var optionsAsString = options.ToString();

            optionsAsString.Should().NotBeEmpty();

            optionsAsString.Should().Contain($"{nameof(ImageModifierOptions.AllowJpg)}");
            optionsAsString.Should().Contain($"{nameof(ImageModifierOptions.NoAnimate)}");
            optionsAsString.Should().Contain($"{nameof(ImageModifierOptions.Width)}/{options.Width}");
            optionsAsString.Should().Contain($"{nameof(ImageModifierOptions.Crop)}/{options.Crop}");
        }
    }
}