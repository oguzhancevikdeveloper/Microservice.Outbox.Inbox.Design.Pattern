using InOut.StockAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace InOut.StockAPI.Models.Contexts;

public class StockDbContext : DbContext
{
    public StockDbContext(DbContextOptions<StockDbContext> options) : base(options) { }

    public DbSet<OrderInbox> OrderInboxes { get; set; }
}