using Microsoft.Extensions.Logging;
using Moq;

namespace PaymentAPI.Tests.Mocks;

/// <summary>
/// Provides mock loggers for testing purposes.
/// </summary>
public static class LoggerMock
{
    /// <summary>
    /// Creates a mock ILogger instance that can be used in unit tests.
    /// </summary>
    /// <typeparam name="T">The category type for the logger.</typeparam>
    /// <returns>A mocked ILogger instance that doesn't perform any actual logging.</returns>
    public static ILogger<T> Create<T>()
    {
        return new Mock<ILogger<T>>().Object;
    }
}
