using System;
using Xunit;
using photography_gallery.Services;

namespace photography_gallery.tests
{
    public class ImageConversionServiceTests
    {
        private readonly ImageConversionService ImageConversion;

        public ImageConversionServiceTests()
        {
            ImageConversion = new ImageConversionService();
        }

        [Fact] 
        void ImagesAreDifferent_true_1()
        {
            Assert.True(ImageConversion.ImagesAreDifferent("./Data/original_image.jpg", "./Data/different_image.jpg"));
        }

        [Fact]
        void ImagesAreDifferent_true_2()
        {
            Assert.True(ImageConversion.ImagesAreDifferent("./Data/original_image.jpg", "./Data/original_image_edited.jpg"));

        }

        [Fact]
        void ImagesAreDifferent_false()
        {
            Assert.False(ImageConversion.ImagesAreDifferent("./Data/original_image.jpg", "./Data/original_image_identical.jpg"));
        }

        [Fact]
        public void FixFNumber_true_1()
        {
            Assert.Equal(actual: ImageConversion.FixFNumber("1"), expected: "1");
        }

        [Fact]
        public void FixFNumber_true_2()
        {
            Assert.Equal(actual: ImageConversion.FixFNumber("1/10"), expected: "0.1");
        }

        [Fact]
        public void FixFNumber_false_1()
        {
            Assert.NotEqual(actual: ImageConversion.FixFNumber("1/10"), expected: "1");
        }

        [Fact]
        public void FixFNumber_false_2()
        {
            Assert.NotEqual(actual: ImageConversion.FixFNumber("1/10"), expected: "1/10");
        }

        [Fact]
        public void GetImageRatio_true()
        {
            Assert.Equal(actual: ImageConversion.GetImageRatio(2, 3), expected: 3.0/2.0);
        }

        [Fact]
        public void GetImageRatio_false()
        {
            Assert.NotEqual(actual: ImageConversion.GetImageRatio(2, 3), expected: 2.0 / 3.0);
        }
    }
}
