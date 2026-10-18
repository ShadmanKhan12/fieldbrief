using Fieldbrief.Domain.Checks;

namespace Fieldbrief.UnitTests;

public class HealthCheckEvaluatorTests
{
    [Fact]
    public void Passes_when_status_matches_and_no_error()
    {
        Assert.True(HealthCheckEvaluator.Passed(200, 200, null));
    }

    [Fact]
    public void Fails_on_unexpected_status()
    {
        Assert.False(HealthCheckEvaluator.Passed(401, 200, null));
    }

    [Fact]
    public void Fails_when_error_present()
    {
        Assert.False(HealthCheckEvaluator.Passed(200, 200, "timeout"));
    }
}
