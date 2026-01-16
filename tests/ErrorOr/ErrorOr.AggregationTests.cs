using ErrorOr;
using FluentAssertions;

namespace Tests;

public class AggregationTests
{
    [Fact]
    public void AppendErrors_WhenErrorOrIsValue_ShouldReturnErrorsOnly()
    {
        // Arrange
        ErrorOr<int> errorOrValue = 5;
        var error1 = Error.Validation("Error.1", "First error");
        var error2 = Error.Validation("Error.2", "Second error");

        // Act
        var result = errorOrValue.AppendErrors(error1, error2);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(2);
        result.Errors.Should().Contain(error1);
        result.Errors.Should().Contain(error2);
    }

    [Fact]
    public void AppendErrors_WhenErrorOrIsError_ShouldCombineErrors()
    {
        // Arrange
        var existingError = Error.NotFound("Existing.Error", "Existing error");
        ErrorOr<int> errorOrErrors = existingError;
        var newError1 = Error.Validation("Error.1", "First error");
        var newError2 = Error.Validation("Error.2", "Second error");

        // Act
        var result = errorOrErrors.AppendErrors(newError1, newError2);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(3);
        result.Errors.Should().Contain(existingError);
        result.Errors.Should().Contain(newError1);
        result.Errors.Should().Contain(newError2);
    }

    [Fact]
    public void AppendErrors_WhenErrorsArrayIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        ErrorOr<int> errorOrValue = 5;
        Error[]? nullErrors = null;

        // Act
        Action act = () => errorOrValue.AppendErrors(nullErrors!);

        // Assert
        act.Should().ThrowExactly<ArgumentNullException>()
           .And.ParamName.Should().Be("errors");
    }

    [Fact]
    public void AppendErrors_WhenErrorsArrayIsEmpty_ShouldThrowArgumentException()
    {
        // Arrange
        ErrorOr<int> errorOrValue = 5;

        // Act
        Action act = () => errorOrValue.AppendErrors(Array.Empty<Error>());

        // Assert
        act.Should().ThrowExactly<ArgumentException>()
           .And.ParamName.Should().Be("errors");
    }

    [Fact]
    public void AppendErrorsList_WhenErrorOrIsValue_ShouldReturnErrorsOnly()
    {
        // Arrange
        ErrorOr<int> errorOrValue = 5;
        var errors = new List<Error>
        {
            Error.Validation("Error.1", "First error"),
            Error.Validation("Error.2", "Second error"),
        };

        // Act
        var result = errorOrValue.AppendErrors(errors);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(2);
        result.Errors.Should().BeEquivalentTo(errors);
    }

    [Fact]
    public void AppendErrorsList_WhenErrorOrIsError_ShouldCombineErrors()
    {
        // Arrange
        var existingError = Error.NotFound("Existing.Error", "Existing error");
        ErrorOr<int> errorOrErrors = existingError;
        var newErrors = new List<Error>
        {
            Error.Validation("Error.1", "First error"),
            Error.Validation("Error.2", "Second error"),
        };

        // Act
        var result = errorOrErrors.AppendErrors(newErrors);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(3);
        result.Errors[0].Should().Be(existingError);
        result.Errors[1].Should().Be(newErrors[0]);
        result.Errors[2].Should().Be(newErrors[1]);
    }

    [Fact]
    public void AppendErrorsList_WhenErrorsListIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        ErrorOr<int> errorOrValue = 5;
        List<Error>? nullErrors = null;

        // Act
        Action act = () => errorOrValue.AppendErrors(nullErrors!);

        // Assert
        act.Should().ThrowExactly<ArgumentNullException>()
           .And.ParamName.Should().Be("errors");
    }

    [Fact]
    public void AppendErrorsList_WhenErrorsListIsEmpty_ShouldThrowArgumentException()
    {
        // Arrange
        ErrorOr<int> errorOrValue = 5;

        // Act
        Action act = () => errorOrValue.AppendErrors(new List<Error>());

        // Assert
        act.Should().ThrowExactly<ArgumentException>()
           .And.ParamName.Should().Be("errors");
    }

    [Fact]
    public void Combine_WhenAllAreValues_ShouldReturnFirstValue()
    {
        // Arrange
        ErrorOr<int> errorOr1 = 1;
        ErrorOr<int> errorOr2 = 2;
        ErrorOr<int> errorOr3 = 3;

        // Act
        var result = ErrorOrExtensions.Combine(errorOr1, errorOr2, errorOr3);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(1);
    }

    [Fact]
    public void Combine_WhenSomeAreErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var error1 = Error.Validation("Error.1", "First error");
        var error2 = Error.Validation("Error.2", "Second error");
        ErrorOr<int> errorOr1 = 1;
        ErrorOr<int> errorOr2 = error1;
        ErrorOr<int> errorOr3 = error2;

        // Act
        var result = ErrorOrExtensions.Combine(errorOr1, errorOr2, errorOr3);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(2);
        result.Errors.Should().Contain(error1);
        result.Errors.Should().Contain(error2);
    }

    [Fact]
    public void Combine_WhenAllAreErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var error1 = Error.Validation("Error.1", "First error");
        var error2 = Error.Validation("Error.2", "Second error");
        var error3 = Error.Validation("Error.3", "Third error");
        ErrorOr<int> errorOr1 = error1;
        ErrorOr<int> errorOr2 = error2;
        ErrorOr<int> errorOr3 = error3;

        // Act
        var result = ErrorOrExtensions.Combine(errorOr1, errorOr2, errorOr3);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(3);
        result.Errors.Should().Contain(error1);
        result.Errors.Should().Contain(error2);
        result.Errors.Should().Contain(error3);
    }

    [Fact]
    public void Combine_WhenErrorsArrayIsNull_ShouldThrowArgumentNullException()
    {
        // Act
        Action act = () => ErrorOrExtensions.Combine<int>(null!);

        // Assert
        act.Should().ThrowExactly<ArgumentNullException>()
           .And.ParamName.Should().Be("errorOrs");
    }

    [Fact]
    public void Combine_WhenErrorsArrayIsEmpty_ShouldThrowArgumentException()
    {
        // Act
        Action act = () => ErrorOrExtensions.Combine(Array.Empty<ErrorOr<int>>());

        // Assert
        act.Should().ThrowExactly<ArgumentException>()
           .And.ParamName.Should().Be("errorOrs");
    }

    [Fact]
    public void CombineAll_WhenAllAreValues_ShouldReturnAllValues()
    {
        // Arrange
        ErrorOr<int> errorOr1 = 1;
        ErrorOr<int> errorOr2 = 2;
        ErrorOr<int> errorOr3 = 3;

        // Act
        var result = ErrorOrExtensions.CombineAll(errorOr1, errorOr2, errorOr3);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(new[] { 1, 2, 3 });
    }

    [Fact]
    public void CombineAll_WhenSomeAreErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var error1 = Error.Validation("Error.1", "First error");
        var error2 = Error.Validation("Error.2", "Second error");
        ErrorOr<int> errorOr1 = 1;
        ErrorOr<int> errorOr2 = error1;
        ErrorOr<int> errorOr3 = error2;

        // Act
        var result = ErrorOrExtensions.CombineAll(errorOr1, errorOr2, errorOr3);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(2);
        result.Errors.Should().Contain(error1);
        result.Errors.Should().Contain(error2);
    }

    [Fact]
    public void CombineAll_WhenAllAreErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var error1 = Error.Validation("Error.1", "First error");
        var error2 = Error.Validation("Error.2", "Second error");
        var error3 = Error.Validation("Error.3", "Third error");
        ErrorOr<int> errorOr1 = error1;
        ErrorOr<int> errorOr2 = error2;
        ErrorOr<int> errorOr3 = error3;

        // Act
        var result = ErrorOrExtensions.CombineAll(errorOr1, errorOr2, errorOr3);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(3);
        result.Errors.Should().Contain(error1);
        result.Errors.Should().Contain(error2);
        result.Errors.Should().Contain(error3);
    }

    [Fact]
    public void CombineAll_WhenErrorsArrayIsNull_ShouldThrowArgumentNullException()
    {
        // Act
        Action act = () => ErrorOrExtensions.CombineAll<int>(null!);

        // Assert
        act.Should().ThrowExactly<ArgumentNullException>()
           .And.ParamName.Should().Be("errorOrs");
    }

    [Fact]
    public void CombineAll_WhenErrorsArrayIsEmpty_ShouldThrowArgumentException()
    {
        // Act
        Action act = () => ErrorOrExtensions.CombineAll(Array.Empty<ErrorOr<int>>());

        // Assert
        act.Should().ThrowExactly<ArgumentException>()
           .And.ParamName.Should().Be("errorOrs");
    }
}
