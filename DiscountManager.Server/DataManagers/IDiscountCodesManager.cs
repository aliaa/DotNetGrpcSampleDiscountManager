using DiscountManager.Server.Entities;

namespace DiscountManager.Server.DataManagers;
public interface IDiscountCodesManager
{
    ValueTask<DiscountCode> GetByCode(string code);
    Task<DiscountCode> GetReadyToUseAndSetAsAssigned();
    Task<int> Insert(IEnumerable<DiscountCode> items);
    Task<int> SetAsUsed(DiscountCode item);
}