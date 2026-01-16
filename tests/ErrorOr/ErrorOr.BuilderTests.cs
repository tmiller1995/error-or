using ErrorOr;
using FluentAssertions;

namespace Tests;

public class BuilderTests
{
    [Fact]
    public void Create_WhenErrorsProvided_ShouldReturnErrorOrWithErrors()
    {
        // Arrange
        var error1 = Error.Validation("Error.1", "First error");
        var error2 = Error.Validation("Error.2", "Second error");

        // Act
#if NET8_0_OR_GREATER
        var result = ErrorOrBuilder.Create<int>(new[] { error1, error2 });
#else
        var result = ErrorOrBuilder.Create<int>(new[] { error1, error2 });
#endif

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(2);
        result.Errors.Should().Contain(error1);
        result.Errors.Should().Contain(error2);
    }

    [Fact]
    public void Create_WhenSingleErrorProvided_ShouldReturnErrorOrWithError()
    {
        // Arrange
        var error = Error.NotFound("Error.NotFound", "Resource not found");

        // Act
#if NET8_0_OR_GREATER
        var result = ErrorOrBuilder.Create<string>(new[] { error });
#else
        var result = ErrorOrBuilder.Create<string>(new[] { error });
#endif

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.FirstError.Should().Be(error);
    }

    [Fact]
    public void Create_WhenEmptyErrorsProvided_ShouldThrowArgumentException()
    {
        // Act
#if NET8_0_OR_GREATER
        Action act = () => ErrorOrBuilder.Create<int>(Array.Empty<Error>());
#else
        Action act = () => ErrorOrBuilder.Create<int>(Array.Empty<Error>());
#endif

        // Assert
        act.Should().ThrowExactly<ArgumentException>()
           .And.ParamName.Should().Be("errors");
    }

#if !NET8_0_OR_GREATER
    [Fact]
    public void Create_WhenNullErrorsProvided_ShouldThrowArgumentException()
    {
        // Act
        Action act = () => ErrorOrBuilder.Create<int>(null!);

        // Assert
        act.Should().ThrowExactly<ArgumentException>()
           .And.ParamName.Should().Be("errors");
    }
#endif
}
