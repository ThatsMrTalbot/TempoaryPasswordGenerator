using System.Text;
using Xunit;

namespace TempoaryPasswordGenerator.Tests
{
    public class LongEncodeTests
    {
        public byte[] ValidSecret = Encoding.UTF8.GetBytes("SomeValidSecret");
        public byte[] InvalidSecret = Encoding.UTF8.GetBytes("SomeInvalidSecret");

        [Fact]
        public void LongEncode_CanEncodeLong()
        {
            // Arrange
            var encoder = new LongEncode(ValidSecret);
            var expected = (long)123456789;

            // Act
            var result = encoder.Encode(expected);

            // Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public void LongEncode_CanDecodeEncodedLong()
        {
            // Arrange
            var encoder = new LongEncode(ValidSecret);
            var expected = (long)123456789;

            // Act
            var encoded = encoder.Encode(expected);
            var result = encoder.Decode(encoded);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void LongEncode_CantDecodeDecodWithInvalidSecret()
        {
            // Arrange
            var validEncoder = new LongEncode(ValidSecret);
            var invalidEncoder = new LongEncode(InvalidSecret);
            var expected = (long)123456789;

            // Act
            var encoded = validEncoder.Encode(expected);
            var result = invalidEncoder.Decode(encoded);

            // Assert
            Assert.NotEqual(expected, result);
        }
    }
}
