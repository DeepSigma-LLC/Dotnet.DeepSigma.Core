using DeepSigma.Core.Enums;

namespace DeepSigma.Core.Inventory;

/// <summary>
/// Factory for creating inventory collections based on accounting cost methodology
/// </summary>
/// <typeparam name="T"></typeparam>
public static class InventoryCollectionFactory<T>
{
    /// <summary>
    /// Creates an inventory collection based on the specified accounting cost methodology
    /// </summary>
    /// <param name="costMethodology"></param>
    /// <param name="AverageCostFunction"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static InventoryCollectionAbstract<T> Create(AccountingCostMethodology costMethodology, Func<IEnumerable<T>,T?>? AverageCostFunction = null)
    {
        return costMethodology switch
        {
            AccountingCostMethodology.FirstInFirstOut => new InventoryQueueCollection<T>(),
            AccountingCostMethodology.LastInLastOut => new InventoryStackCollection<T>(),
            AccountingCostMethodology.AverageCost => AverageCostFunction is not null 
                ? new InventoryAverageCostCollection<T>(AverageCostFunction) 
                : throw new ArgumentNullException(paramName: nameof(AverageCostFunction), "Arguement cannot be null for the selected accounting methodology."),
            _ => throw new NotImplementedException(),
        };
    }
}
