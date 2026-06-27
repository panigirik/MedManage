using MedManage.Domain.Entities;
using MedManage.Domain.Enums;
using MedManage.Domain.Interfaces;
using MedManage.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace MedManage.Persistence.Repositories;

public class PurchaseRequestRepository : IPurchaseRequestRepository
{
    private readonly IAppDbContext _context;

    public PurchaseRequestRepository(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PurchaseRequest> CreateAsync(PurchaseRequest purchaseRequest)
    {
        await _context.PurchaseRequests.AddAsync(purchaseRequest);
        await _context.SaveChangesAsync();

        var created = await _context.PurchaseRequests
            .Include(pr => pr.Announcement)
            .Include(pr => pr.BuyerUser)
            .Include(pr => pr.SellerUser)
            .FirstAsync(pr => pr.PurchaseRequestId == purchaseRequest.PurchaseRequestId);

        return created;
    }

    public async Task<PurchaseRequest?> GetByIdAsync(Guid purchaseRequestId)
    {
        return await _context.PurchaseRequests
            .Include(pr => pr.Announcement)
            .Include(pr => pr.BuyerUser)
            .Include(pr => pr.SellerUser)
            .FirstOrDefaultAsync(pr => pr.PurchaseRequestId == purchaseRequestId);
    }

    public async Task<IEnumerable<PurchaseRequest>> GetBySellerAsync(Guid sellerUserId)
    {
        return await _context.PurchaseRequests
            .Include(pr => pr.Announcement)
            .Include(pr => pr.BuyerUser)
            .Include(pr => pr.SellerUser)
            .Where(pr => pr.SellerUserId == sellerUserId)
            .OrderByDescending(pr => pr.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<PurchaseRequest>> GetByBuyerAsync(Guid buyerUserId)
    {
        return await _context.PurchaseRequests
            .Include(pr => pr.Announcement)
            .Include(pr => pr.BuyerUser)
            .Include(pr => pr.SellerUser)
            .Where(pr => pr.BuyerUserId == buyerUserId)
            .OrderByDescending(pr => pr.CreatedAt)
            .ToListAsync();
    }

    public async Task UpdateAsync(PurchaseRequest purchaseRequest)
    {
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(PurchaseRequest purchaseRequest)
    {
        _context.PurchaseRequests.Remove(purchaseRequest);
        await _context.SaveChangesAsync();
    }
}
