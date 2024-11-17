using DiscountManager.Server.Entities;
using Microsoft.EntityFrameworkCore;

namespace DiscountManager.Server.DataManagers;

public class DiscountCodesManager(AppDbContext dbContext) : IDiscountCodesManager
{
    public Task<int> Insert(IEnumerable<DiscountCode> items)
    {
        dbContext.DiscountCodes.AddRange(items);
        return dbContext.SaveChangesAsync();
    }

    public ValueTask<DiscountCode> GetByCode(string code)
    {
        return dbContext.DiscountCodes.FindAsync(code);
    }

    public async Task<DiscountCode> GetReadyToUseAndSetAsAssigned()
    {
        var item = await dbContext.DiscountCodes
            .Where(c => c.Status == CodeStatus.ReadyToUse)
            .OrderBy(c => EF.Functions.Random())
            .Take(1)
            .FirstOrDefaultAsync();

        if (item != null)
        {
            item.Status = CodeStatus.Assigned;
            await dbContext.SaveChangesAsync();
        }
        return item;
    }

    public Task<int> SetAsUsed(DiscountCode item)
    {
        item.Status = CodeStatus.Used;
        return dbContext.SaveChangesAsync();
    }
}
