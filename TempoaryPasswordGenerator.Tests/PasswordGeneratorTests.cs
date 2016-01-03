using System;
using Xunit;
using System.Text;

namespace TempoaryPasswordGenerator.Tests
{
    public class PasswordGeneratorTests
    {
        public TimeSpan ThirtySeconds = TimeSpan.FromSeconds(30);

        public byte[] ValidSecret = Encoding.UTF8.GetBytes("SomeValidSecret");
        public byte[] InvalidSecret = Encoding.UTF8.GetBytes("SomeInvalidSecret");

        public string ValidUid = "SomeValidUid";
        public string InvalidUid = "SomeInvalidUid";
        
        [Fact]
        public void Password_CanGeneratePassword()
        {
            // Arrange
            var generator = new PasswordGenerator(ThirtySeconds, ValidSecret);

            // Act
            var password = generator.Generate(ValidUid);

            // Assert
            Assert.NotEmpty(password);
        }

        [Fact]
        public void Password_CanValidateGeneratedPassword()
        {
            // Arrange
            var generator = new PasswordGenerator(ThirtySeconds, ValidSecret);

            // Act
            var password = generator.Generate(ValidUid);
            var valid = generator.Validate(ValidUid, password);

            // Assert
            Assert.True(valid);
        }

        [Fact]
        public void Password_CantValidateExpiredPassword()
        {
            // Arrange
            var timeProvider = new MockTimeProvider();
            var generator = new PasswordGenerator(ThirtySeconds, ValidSecret, timeProvider);

            // Act
            var password = generator.Generate(ValidUid);

            timeProvider.Set(DateTime.UtcNow.AddSeconds(31));
            var valid = generator.Validate(ValidUid, password);

            // Assert
            Assert.False(valid);
        }

        [Fact]
        public void Password_CantValidatePasswordAgainstIncorrectUid()
        {
            // Arrange
            var generator = new PasswordGenerator(ThirtySeconds, ValidSecret);

            // Act
            var password = generator.Generate(ValidUid);
            var valid = generator.Validate(InvalidUid, password);

            // Assert
            Assert.False(valid);
        }

        [Fact]
        public void Password_CantValidatePasswordAgainstIncorrectSecret()
        {
            // Arrange
            var validGenerator = new PasswordGenerator(ThirtySeconds, ValidSecret);
            var invalidGenerator = new PasswordGenerator(ThirtySeconds, InvalidSecret);

            // Act
            var password = validGenerator.Generate(ValidUid);
            var valid = invalidGenerator.Validate(ValidUid, password);

            // Assert
            Assert.False(valid);
        }
    }
}
