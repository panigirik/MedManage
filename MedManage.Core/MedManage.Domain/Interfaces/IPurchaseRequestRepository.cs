using MedManage.Domain.Entities;
using MedManage.Domain.Enums;

namespace MedManage.Domain.Interfaces;

public interface IPurchaseRequestRepository
{
    Task<PurchaseRequest> CreateAsync(PurchaseRequest purchaseRequest);
    Task<PurchaseRequest?> GetByIdAsync(Guid purchaseRequestId);
    Task<IEnumerable<PurchaseRequest>> GetBySellerAsync(Guid sellerUserId);
    Task<IEnumerable<PurchaseRequest>> GetByBuyerAsync(Guid buyerUserId);
    Task UpdateAsync(PurchaseRequest purchaseRequest);
    Task DeleteAsync(PurchaseRequest purchaseRequest);
}
