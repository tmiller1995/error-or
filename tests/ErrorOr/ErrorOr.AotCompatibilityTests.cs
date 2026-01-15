namespace Tests;

using ErrorOr;
using FluentAssertions;

/// <summary>
/// Tests that verify AOT (Ahead-of-Time) compilation compatibility.
/// These tests exercise all public APIs without relying on reflection-based features.
/// </summary>
public class ErrorOrAotCompatibilityTests
{
    [Fact]
    public void ErrorOr_ShouldWorkWithValueTypes_WithoutReflection()
    {
        // Arrange & Act
        ErrorOr<int> errorOrInt = 42;
        ErrorOr<double> errorOrDouble = 3.14;
        ErrorOr<bool> errorOrBool = true;
        ErrorOr<Guid> errorOrGuid = Guid.NewGuid();
        ErrorOr<DateTime> errorOrDateTime = DateTime.UtcNow;

        // Assert
        errorOrInt.IsError.Should().BeFalse();
        errorOrInt.Value.Should().Be(42);

        errorOrDouble.IsError.Should().BeFalse();
        errorOrDouble.Value.Should().Be(3.14);

        errorOrBool.IsError.Should().BeFalse();
        errorOrBool.Value.Should().BeTrue();

        errorOrGuid.IsError.Should().BeFalse();
        errorOrGuid.Value.Should().NotBeEmpty();

        errorOrDateTime.IsError.Should().BeFalse();
    }

    [Fact]
    public void ErrorOr_ShouldWorkWithReferenceTypes_WithoutReflection()
    {
        // Arrange & Act
        ErrorOr<string> errorOrString = "test";
        ErrorOr<List<int>> errorOrList = new List<int> { 1, 2, 3 };
        ErrorOr<object> errorOrObject = new object();

        // Assert
        errorOrString.IsError.Should().BeFalse();
        errorOrString.Value.Should().Be("test");

        errorOrList.IsError.Should().BeFalse();
        errorOrList.Value.Should().HaveCount(3);

        errorOrObject.IsError.Should().BeFalse();
        errorOrObject.Value.Should().NotBeNull();
    }

    [Fact]
    public void AllErrorFactoryMethods_ShouldWork_WithoutReflection()
    {
        // Arrange & Act
        var failure = Error.Failure("code", "desc");
        var unexpected = Error.Unexpected("code", "desc");
        var validation = Error.Validation("code", "desc");
        var conflict = Error.Conflict("code", "desc");
        var notFound = Error.NotFound("code", "desc");
        var unauthorized = Error.Unauthorized("code", "desc");
        var forbidden = Error.Forbidden("code", "desc");
        var custom = Error.Custom(100, "code", "desc");

        // Assert
        failure.Type.Should().Be(ErrorType.Failure);
        unexpected.Type.Should().Be(ErrorType.Unexpected);
        validation.Type.Should().Be(ErrorType.Validation);
        conflict.Type.Should().Be(ErrorType.Conflict);
        notFound.Type.Should().Be(ErrorType.NotFound);
        unauthorized.Type.Should().Be(ErrorType.Unauthorized);
        forbidden.Type.Should().Be(ErrorType.Forbidden);
        custom.NumericType.Should().Be(100);
    }

    [Fact]
    public void ErrorWithMetadata_ShouldWork_WithoutReflection()
    {
        // Arrange
        var metadata = new Dictionary<string, object>
        {
            ["key1"] = "value1",
            ["key2"] = 42,
            ["key3"] = true,
        };

        // Act
        var error = Error.Validation("code", "desc", metadata);

        // Assert
        error.Metadata.Should().NotBeNull();
        error.Metadata.Should().HaveCount(3);
        error.Metadata!["key1"].Should().Be("value1");
        error.Metadata!["key2"].Should().Be(42);
        error.Metadata!["key3"].Should().Be(true);
    }

    [Fact]
    public void Match_ShouldWork_WithoutReflection()
    {
        // Arrange
        ErrorOr<int> valueResult = 42;
        ErrorOr<int> errorResult = Error.Validation();

        // Act
        var valueMatchResult = valueResult.Match(
            onValue: v => $"Value: {v}",
            onError: e => $"Errors: {e.Count}");

        var errorMatchResult = errorResult.Match(
            onValue: v => $"Value: {v}",
            onError: e => $"Errors: {e.Count}");

        // Assert
        valueMatchResult.Should().Be("Value: 42");
        errorMatchResult.Should().Be("Errors: 1");
    }

    [Fact]
    public void Then_ShouldWork_WithoutReflection()
    {
        // Arrange
        ErrorOr<int> result = 10;

        // Act
        var chainedResult = result
            .Then(v => v * 2)
            .Then(v => v + 5)
            .Then(v => $"Result: {v}");

        // Assert
        chainedResult.IsError.Should().BeFalse();
        chainedResult.Value.Should().Be("Result: 25");
    }

    [Fact]
    public void Else_ShouldWork_WithoutReflection()
    {
        // Arrange
        ErrorOr<int> errorResult = Error.Validation();

        // Act
        var elseValue = errorResult.Else(42);
        var elseFunc = errorResult.Else(errors => errors.Count * 10);

        // Assert
        elseValue.Value.Should().Be(42);
        elseFunc.Value.Should().Be(10);
    }

    [Fact]
    public void FailIf_ShouldWork_WithoutReflection()
    {
        // Arrange
        ErrorOr<int> result = 10;

        // Act
        var failedResult = result.FailIf(v => v > 5, Error.Validation("TooLarge", "Value is too large"));
        var passedResult = result.FailIf(v => v > 100, Error.Validation("TooLarge", "Value is too large"));

        // Assert
        failedResult.IsError.Should().BeTrue();
        failedResult.FirstError.Code.Should().Be("TooLarge");

        passedResult.IsError.Should().BeFalse();
        passedResult.Value.Should().Be(10);
    }

    [Fact]
    public void Switch_ShouldWork_WithoutReflection()
    {
        // Arrange
        ErrorOr<int> valueResult = 42;
        ErrorOr<int> errorResult = Error.Validation();
        var switchedValue = 0;
        var switchedErrorCount = 0;

        // Act
        valueResult.Switch(
            onValue: v => switchedValue = v,
            onError: e => switchedErrorCount = e.Count);

        errorResult.Switch(
            onValue: v => switchedValue = v,
            onError: e => switchedErrorCount = e.Count);

        // Assert
        switchedValue.Should().Be(42);
        switchedErrorCount.Should().Be(1);
    }

    [Fact]
    public void ResultTypes_ShouldWork_WithoutReflection()
    {
        // Arrange & Act
        ErrorOr<Success> success = Result.Success;
        ErrorOr<Created> created = Result.Created;
        ErrorOr<Updated> updated = Result.Updated;
        ErrorOr<Deleted> deleted = Result.Deleted;

        // Assert
        success.IsError.Should().BeFalse();
        success.Value.Should().Be(Result.Success);

        created.IsError.Should().BeFalse();
        created.Value.Should().Be(Result.Created);

        updated.IsError.Should().BeFalse();
        updated.Value.Should().Be(Result.Updated);

        deleted.IsError.Should().BeFalse();
        deleted.Value.Should().Be(Result.Deleted);
    }

    [Fact]
    public void ErrorOrFactory_ShouldWork_WithoutReflection()
    {
        // Arrange & Act
        var result = ErrorOrFactory.From("test value");

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be("test value");
    }

    [Fact]
    public void MultipleErrors_ShouldWork_WithoutReflection()
    {
        // Arrange
        var errors = new List<Error>
        {
            Error.Validation("Error1", "First error"),
            Error.Validation("Error2", "Second error"),
            Error.Validation("Error3", "Third error"),
        };

        // Act
        ErrorOr<int> result = errors;

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(3);
        result.FirstError.Code.Should().Be("Error1");
        result.ErrorsOrEmptyList.Should().HaveCount(3);
    }

    [Fact]
    public void ErrorEquality_ShouldWork_WithoutReflection()
    {
        // Arrange
        var error1 = Error.Validation("code", "desc");
        var error2 = Error.Validation("code", "desc");
        var error3 = Error.Validation("other", "desc");

        // Assert
        error1.Should().Be(error2);
        error1.Should().NotBe(error3);
        error1.GetHashCode().Should().Be(error2.GetHashCode());
    }

    [Fact]
    public void ToErrorOr_ExtensionMethod_ShouldWork_WithoutReflection()
    {
        // Arrange & Act
        var result = "test".ToErrorOr();

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be("test");
    }

    [Fact]
    public void GenericStructs_ShouldWork_WithoutReflection()
    {
        // This test verifies that the library works with user-defined structs
        // which is important for AOT scenarios where the JIT cannot generate code at runtime

        // Arrange
        var point = new Point(10, 20);

        // Act
        ErrorOr<Point> result = point;

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.X.Should().Be(10);
        result.Value.Y.Should().Be(20);
    }

    [Fact]
    public void NestedGenericTypes_ShouldWork_WithoutReflection()
    {
        // Arrange
        var nested = new Dictionary<string, List<int>>
        {
            ["key"] = new List<int> { 1, 2, 3 },
        };

        // Act
        ErrorOr<Dictionary<string, List<int>>> result = nested;

        // Assert
        result.IsError.Should().BeFalse();
        result.Value["key"].Should().HaveCount(3);
    }

    private readonly record struct Point(int X, int Y);
}
