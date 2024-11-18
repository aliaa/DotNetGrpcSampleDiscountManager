using DiscountManager.ProtoDefinitions;
using DiscountManager.Server.DataManagers;
using DiscountManager.Server.Entities;
using Grpc.Core;

namespace DiscountManager.Server.Services;

public class DiscountService(IDiscountCodesManager discountCodesManager, ILogger<DiscountService> logger)
    : Discount.DiscountBase
{
    private const string CODE_CHARS = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public override async Task<GenerateResponse> Generate(GenerateRequest request, ServerCallContext context)
    {
        if (request.Count is > 2000 or < 1 || request.Length is > 8 or < 7)
        {
            logger.LogError("Asked to generate with invalid input. Count: {count}, Length: {length}", request.Count, request.Length);
            return new GenerateResponse { Result = false };
        }

        DiscountCode[] codes = new DiscountCode[request.Count];
        for (int i = 0; i < request.Count; i++)
        {
            codes[i] = new DiscountCode
            {
                Code = GenerateRandomCode(request.Length),
                Status = CodeStatus.ReadyToUse
            };
        }
        try
        {
            var insertedCount = await discountCodesManager.Insert(codes);
            return new GenerateResponse { Result = (insertedCount == request.Count) };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while inserting new codes");
            return new GenerateResponse { Result = false };
        }
    }

    public override async Task<GetCodeResponse> GetCode(Empty request, ServerCallContext context)
    {
        DiscountCode item = null;
        try
        {
            item = await discountCodesManager.GetReadyToUseAndSetAsAssigned();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while getting a ready-to-use code");
            return new GetCodeResponse { Result = false };
        }

        if (item == null)
        {
            logger.LogWarning("No ready-to-use code is remaining in database.");
            return new GetCodeResponse { Result = false };
        }

        return new GetCodeResponse { Code = item.Code, Result = true };
    }

    public override async Task<UseCodeResponse> UseCode(UseCodeRequest request, ServerCallContext context)
    {
        var item = await discountCodesManager.GetByCode(request.Code);
        if (item == null || item.Status != CodeStatus.Assigned)
        {
            logger.LogError("code not found to use or status is not assigned. code:{code}, status: {status}", request.Code, item?.Status);
            return new UseCodeResponse { Result = false };
        }
        await discountCodesManager.SetAsUsed(item);
        return new UseCodeResponse { Result = true };
    }

    public static string GenerateRandomCode(uint length)
    {
        var chars = new char[length];
        for (var i = 0; i < length; i++)
        {
            chars[i] = CODE_CHARS[Random.Shared.Next(CODE_CHARS.Length)];
        }
        return new string(chars);
    }
}
