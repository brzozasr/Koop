using System;
using Koop.models;
using Koop.Models.RepositoryModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

#nullable disable

namespace Koop.Models
{
    public partial class KoopDbContext : IdentityDbContext<User, Role, Guid>
    {
        public KoopDbContext()
        {
        }

        public KoopDbContext(DbContextOptions<KoopDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AvailableQuantity> AvailableQuantities { get; set; }
        public virtual DbSet<Basket> Baskets { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<CoopOrderHistoryView> CoopOrderHistoryViews { get; set; }
        public virtual DbSet<Favority> Favorities { get; set; }
        public virtual DbSet<Fund> Funds { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderStatus> OrderStatuses { get; set; }
        public virtual DbSet<OrderedItem> OrderedItems { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<Unit> Units { get; set; }
        public virtual DbSet<Work> Works { get; set; }
        public virtual DbSet<WorkType> WorkTypes { get; set; }
        public virtual DbSet<UserOrdersHistoryView> UserOrdersHistoryView { get; set; }
        public virtual DbSet<OrderView> OrderViews { get; set; }
        public virtual DbSet<FnListForPacker> FnListForPackers { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json")
                    .Build();

                optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                    x => x.UseNodaTime().UseNetTopologySuite());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.HasPostgresExtension("uuid-ossp")
                .HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<AvailableQuantity>(entity =>
            {
                entity.ToTable("available_quantities");

                entity.HasIndex(e => e.ProductId, "IX_available_quantities_product_id");

                entity.Property(e => e.AvailableQuantityId)
                    .HasColumnName("available_quantity_id")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.AvailableQuantities)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_available_quantities_product_id");
            });

            modelBuilder.Entity<Basket>(entity =>
            {
                entity.ToTable("baskets");

                entity.HasIndex(e => e.CoopId, "IX_baskets_coop_id");

                entity.Property(e => e.BasketId)
                    .HasColumnName("basket_id")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.BasketName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("basket_name");

                entity.HasOne(d => d.Coop)
                    .WithMany(p => p.Baskets)
                    .HasForeignKey(d => d.CoopId)
                    .HasConstraintName("fk_baskets_coop_id");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("categories");

                entity.Property(e => e.CategoryId)
                    .HasColumnName("category_id")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("category_name");
            });

            modelBuilder.Entity<CoopOrderHistoryView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("coop_order_history_view");

                entity.Property(e => e.Id).HasColumnName("coop_id");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("first_name");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("last_name");

                entity.Property(e => e.OrderStatusName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("order_status_name");

                entity.Property(e => e.OrderStopDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("order_stop_date");

                entity.Property(e => e.Price)
                    .IsRequired()
                    .HasMaxLength(44)
                    .IsUnicode(false)
                    .HasColumnName("price");
            });
            
            modelBuilder.Entity<Favority>(entity =>
            {
                entity.HasKey(e => e.FavoriteId)
                    .HasName("pk_favorities");

                entity.ToTable("favorities");

                entity.HasIndex(e => e.ProductId, "IX_favorities_product_id");

                entity.Property(e => e.FavoriteId)
                    .HasColumnName("favorite_id")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.CoopId).HasColumnName("coop_id");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.HasOne(d => d.Coop)
                    .WithMany(p => p.Favorities)
                    .HasForeignKey(d => d.CoopId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_favorities_coop_id");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Favorities)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_favorities_product_id");
            });

            modelBuilder.Entity<Fund>(entity =>
            {
                entity.ToTable("funds");

                entity.Property(e => e.FundId)
                    .HasColumnName("fund_id")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.Value).HasColumnName("value");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("orders");

                entity.Property(e => e.OrderId)
                    .HasColumnName("order_id")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.OrderStartDate)
                    .HasColumnType("timestamp")
                    .HasColumnName("order_start_date");

                entity.Property(e => e.OrderStatusId).HasColumnName("order_status_id");

                entity.Property(e => e.OrderStopDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("order_stop_date");

                entity.HasOne(d => d.OrderStatus)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.OrderStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("orders_order_status_order_status_id_fk");
            });

            modelBuilder.Entity<OrderStatus>(entity =>
            {
                entity.ToTable("order_status");

                entity.Property(e => e.OrderStatusId)
                    .HasColumnName("order_status_id")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.OrderStatusName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("order_status_name");
            });

            modelBuilder.Entity<OrderedItem>(entity =>
            {
                entity.ToTable("ordered_items");

                entity.HasIndex(e => e.ProductId, "IX_ordered_items_product_id");

                entity.Property(e => e.OrderedItemId)
                    .HasColumnName("ordered_item_id")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.CoopId).HasColumnName("coop_id");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.OrderStatusId).HasColumnName("order_status_id");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.HasOne(d => d.Coop)
                    .WithMany(p => p.OrderedItems)
                    .HasForeignKey(d => d.CoopId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_ordered_items_coop_id");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderedItems)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_ordered_items_order_id");

                entity.HasOne(d => d.OrderStatus)
                    .WithMany(p => p.OrderedItems)
                    .HasForeignKey(d => d.OrderStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_ordered_items_order_status_id");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderedItems)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_ordered_items_product_id");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("products");

                entity.HasIndex(e => e.UnitId, "IX_products_unit_id");

                entity.Property(e => e.ProductId)
                    .HasColumnName("product_id")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.AmountInMagazine).HasColumnName("amount_in_magazine");

                entity.Property(e => e.AmountMax).HasColumnName("amount_max");

                entity.Property(e => e.Available).HasColumnName("available");

                entity.Property(e => e.Blocked).HasColumnName("blocked");

                entity.Property(e => e.Deposit)
                    .HasColumnName("deposit")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Description)
                    .HasColumnType("text")
                    .HasColumnName("description");

                entity.Property(e => e.Magazine).HasColumnName("magazine");

                entity.Property(e => e.Picture)
                    .HasColumnType("text")
                    .HasColumnName("picture")
                    .HasDefaultValueSql("''::text");

                entity.Property(e => e.Price).HasColumnName("price");

                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("product_name");

                entity.Property(e => e.SupplierId).HasColumnName("supplier_id");

                entity.Property(e => e.UnitId).HasColumnName("unit_id");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.SupplierId)
                    .HasConstraintName("fk_products_supplier_id");

                entity.HasOne(d => d.Unit)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.UnitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_products_unit_id");
            });

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.ToTable("product_categories");

                entity.HasIndex(e => e.ProductId, "IX_product_categories_product_id");

                entity.Property(e => e.ProductCategoryId)
                    .HasColumnName("product_category_id")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.ProductCategories)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_product_categories_category_id");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductCategories)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_product_categories_product_id");
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.ToTable("suppliers");

                entity.HasIndex(e => e.OproId, "IX_suppliers_opro_id");

                entity.Property(e => e.SupplierId)
                    .HasColumnName("supplier_id")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.OproId).HasColumnName("opro_id");

                entity.Property(e => e.OrderClosingDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("order_closing_date");

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("phone");

                entity.Property(e => e.Picture).HasColumnName("picture");

                entity.Property(e => e.Receivables)
                    .HasColumnName("receivables")
                    .HasDefaultValueSql("0.00");

                entity.HasIndex(e => e.SupplierAbbr).IsUnique();
                entity.Property(e => e.SupplierAbbr)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("supplier_abbr");

                entity.Property(e => e.SupplierName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("supplier_name");

                entity.HasOne(d => d.Opro)
                    .WithMany(p => p.Suppliers)
                    .HasForeignKey(d => d.OproId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_suppliers_coop_id");
            });

            modelBuilder.Entity<Unit>(entity =>
            {
                entity.ToTable("units");

                entity.Property(e => e.UnitId)
                    .HasColumnName("unit_id")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.UnitName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("unit_name");
            });

            modelBuilder.Entity<UserOrdersHistoryView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("user_orders_history_view");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.OrderStatusName)
                    .HasMaxLength(100)
                    .HasColumnName("order_status_name");

                entity.Property(e => e.OrderStopDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("order_stop_date");

                entity.Property(e => e.Price).HasColumnName("price");
            });

            modelBuilder.Entity<Work>(entity =>
            {
                entity.ToTable("works");

                entity.HasIndex(e => e.WorkTypeId, "IX_works_work_type_id");

                entity.Property(e => e.WorkId)
                    .HasColumnName("work_id")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.CoopId).HasColumnName("coop_id");

                entity.Property(e => e.Duration).HasColumnName("duration");

                entity.Property(e => e.WorkDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("work_date");

                entity.Property(e => e.WorkTypeId).HasColumnName("work_type_id");

                entity.HasOne(d => d.Coop)
                    .WithMany(p => p.Works)
                    .HasForeignKey(d => d.CoopId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_works_coop_id");

                entity.HasOne(d => d.WorkType)
                    .WithMany(p => p.Works)
                    .HasForeignKey(d => d.WorkTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_works_work_type_id");
            });

            modelBuilder.Entity<WorkType>(entity =>
            {
                entity.ToTable("work_types");

                entity.Property(e => e.WorkTypeId)
                    .HasColumnName("work_type_id")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.WorkType1)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("work_type");
            });

            modelBuilder.Entity<FnListForPacker>(entity =>
            {
                entity.HasNoKey();

                entity.ToFunction("list_for_packer");
                
                entity.Property(e => e.ProductName)
                    .HasColumnName("product_name");

                entity.Property(e => e.ProductsInBaskets)
                    .HasColumnName("products_in_baskets");
            });

            modelBuilder.Entity<OrderView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("order_view");

                entity.Property(e => e.AmountInMagazine).HasColumnName("amount_in_magazine");

                entity.Property(e => e.Available).HasColumnName("available");

                entity.Property(e => e.BasketName)
                    .HasMaxLength(100)
                    .HasColumnName("basket_name");

                entity.Property(e => e.Blocked).HasColumnName("blocked");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.FundValue).HasColumnName("fund_value");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.OrderStartDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("order_start_date");

                entity.Property(e => e.OrderStatusName)
                    .HasMaxLength(100)
                    .HasColumnName("order_status_name");

                entity.Property(e => e.OrderStopDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("order_stop_date");

                entity.Property(e => e.OrderedItemId).HasColumnName("ordered_item_id");

                entity.Property(e => e.Price).HasColumnName("price");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.ProductName)
                    .HasMaxLength(100)
                    .HasColumnName("product_name");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.FundValue).HasColumnName("fund_value");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
