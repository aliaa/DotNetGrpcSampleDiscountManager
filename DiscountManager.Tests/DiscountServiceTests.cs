using DiscountManager.Server.DataManagers;
using DiscountManager.Server.Entities;
using DiscountManager.Server.Services;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moq;

namespace DiscountManager.Tests;

public class DiscountServiceTests
{
    Mock<IDiscountCodesManager> _discountCodesManagerMock;
    Mock<ServerCallContext> _contextMock;
    DiscountService _service;

    public DiscountServiceTests()
    {
        _discountCodesManagerMock = new Mock<IDiscountCodesManager>();
        var loggerMock = new Mock<ILogger<DiscountService>>();
        _service = new DiscountService(_discountCodesManagerMock.Object, loggerMock.Object);
        _contextMock = new Mock<ServerCallContext>();
    }

    [Theory]
    [InlineData(10, 8, true)]
    [InlineData(10, 7, true)]
    [InlineData(10, 6, false)]
    [InlineData(10, 9, false)]
    [InlineData(0, 7, false)]
    [InlineData(2001, 7, false)]
    public async Task Generate_ShouldReturnFalseResult_IfInputInvalid(uint count, uint length, bool result)
    {
        _discountCodesManagerMock.Setup(x =>
            x.Insert(It.IsAny<IEnumerable<DiscountCode>>()))
            .Returns((IEnumerable<DiscountCode> input) => Task.FromResult(input.Count()));

        var response = await _service.Generate(new ProtoDefinitions.GenerateRequest { Count = count, Length = length },
            _contextMock.Object);

        Assert.NotNull(response);
        Assert.Equal(response.Result, result);
    }

    [Theory]
    [InlineData(7)]
    [InlineData(8)]
    public void GenerateRandomCode_ShouldReturnSameSizeCode(uint length)
    {
        var code = DiscountService.GenerateRandomCode(length);
        Assert.Equal(length, (uint)code.Length);
    }
}