namespace Fieldbrief.Domain.Checks;

/// <summary>
/// Pure evaluation helper — keeps pass/fail logic unit-testable without HTTP.
/// </summary>
public static class HealthCheckEvaluator
{
    public static bool Passed(int? statusCode, int expectedStatusCode, string? error) =>
        error is null && statusCode == expectedStatusCode;
}
