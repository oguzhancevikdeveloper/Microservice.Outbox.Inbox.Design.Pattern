﻿using InOut.OrderAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace InOut.OrderAPI.Models.Contexts;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions options) : base(options) { }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<OrderOutbox> OrderOutboxes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) { }
}