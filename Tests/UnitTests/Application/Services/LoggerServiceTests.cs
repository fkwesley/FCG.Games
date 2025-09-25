using Application.Services;
using Application.Settings;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;

namespace Tests.UnitTests.FCG.Tests.Application.Services
{
    public class LoggerServiceTests
    {
        private readonly Mock<ILoggerRepository> _loggerRepoMock;
        private readonly Mock<INewRelicLoggerRepository> _newRelicLoggerMock;
        private readonly Mock<IOptions<ExternalLoggerSettings>> _externalLoggerSettingsMock;
        private readonly LoggerService _loggerService;

        public LoggerServiceTests()
        {
            _loggerRepoMock = new Mock<ILoggerRepository>();
            _newRelicLoggerMock = new Mock<INewRelicLoggerRepository>();
            _externalLoggerSettingsMock = new Mock<IOptions<ExternalLoggerSettings>>();

            _loggerService = new LoggerService(_loggerRepoMock.Object, _newRelicLoggerMock.Object, _externalLoggerSettingsMock.Object);
        }

        [Fact]
        public async Task LogTraceAsync_ShouldThrow_WhenLogIdIsNull()
        {
            // Arrange
            var trace = new Trace { Message = "Test", Level = LogLevel.Info, LogId = null };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _loggerService.LogTraceAsync(trace));
        }

        [Fact]
        public async Task LogTraceAsync_ShouldKeepProvidedLogId()
        {
            var providedId = Guid.NewGuid();
            var trace = new Trace { LogId = providedId, Level = LogLevel.Info };

            _loggerRepoMock.Setup(r => r.LogTraceAsync(It.IsAny<Trace>()))
                .Returns(Task.CompletedTask)
                .Callback<Trace>(t => t.LogId.Should().Be(providedId));

            await _loggerService.LogTraceAsync(trace);

            _loggerRepoMock.Verify(r => r.LogTraceAsync(It.IsAny<Trace>()), Times.Once);
        }

        [Fact]
        public async Task LogTraceAsync_ShouldThrow_WhenTraceIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _loggerService.LogTraceAsync(null));
        }

        [Fact]
        public async Task LogRequestAsync_ShouldCallRepository()
        {
            var log = new RequestLog { LogId = Guid.NewGuid(), Path = "/test" };

            _loggerRepoMock.Setup(r => r.LogRequestAsync(log)).Returns(Task.CompletedTask);

            await _loggerService.LogRequestAsync(log);

            _loggerRepoMock.Verify(r => r.LogRequestAsync(log), Times.Once);
        }

        [Fact]
        public async Task UpdateRequestLogAsync_ShouldThrow_WhenLogIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _loggerService.UpdateRequestLogAsync(null));
        }
    }
}
