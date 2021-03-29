﻿// <auto-generated />
using System;
using Koop.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Koop.Migrations
{
    [DbContext(typeof(KoopDbContext))]
    partial class KoopDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS")
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.4")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("Koop.Models.AvailableQuantity", b =>
                {
                    b.Property<Guid>("AvailableQuantityId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("available_quantity_id");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid")
                        .HasColumnName("product_id");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer")
                        .HasColumnName("quantity");

                    b.HasKey("AvailableQuantityId");

                    b.HasIndex("ProductId");

                    b.ToTable("available_quantities");
                });

            modelBuilder.Entity("Koop.Models.Basket", b =>
                {
                    b.Property<Guid>("BasketId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("basket_id");

                    b.Property<string>("BasketName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("basket_name");

                    b.Property<Guid?>("CoopId")
                        .HasColumnType("uuid")
                        .HasColumnName("coop_id");

                    b.Property<string>("CoopName")
                        .HasColumnType("text");

                    b.HasKey("BasketId");

                    b.HasIndex("CoopId");

                    b.ToTable("baskets");
                });

            modelBuilder.Entity("Koop.Models.Category", b =>
                {
                    b.Property<Guid>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("category_id");

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("category_name");

                    b.HasKey("CategoryId");

                    b.ToTable("categories");
                });

            modelBuilder.Entity("Koop.Models.CoopOrderHistoryView", b =>
                {
                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("first_name");

                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("coop_id");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("last_name");

                    b.Property<string>("OrderStatusName")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("order_status_name");

                    b.Property<DateTime?>("OrderStopDate")
                        .HasColumnType("timestamp")
                        .HasColumnName("order_stop_date");

                    b.Property<string>("Price")
                        .IsRequired()
                        .HasMaxLength(44)
                        .IsUnicode(false)
                        .HasColumnType("character varying(44)")
                        .HasColumnName("price");

                    b.ToView("coop_order_history_view");
                });

            modelBuilder.Entity("Koop.Models.Favority", b =>
                {
                    b.Property<Guid>("FavoriteId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("favorite_id");

                    b.Property<Guid>("CoopId")
                        .HasColumnType("uuid")
                        .HasColumnName("coop_id");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid")
                        .HasColumnName("product_id");

                    b.HasKey("FavoriteId")
                        .HasName("pk_favorities");

                    b.HasIndex("CoopId");

                    b.HasIndex("ProductId");

                    b.ToTable("favorities");
                });

            modelBuilder.Entity("Koop.Models.Fund", b =>
                {
                    b.Property<Guid>("FundId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("fund_id");

                    b.Property<byte>("Value")
                        .HasColumnType("smallint")
                        .HasColumnName("value");

                    b.HasKey("FundId");

                    b.ToTable("funds");
                });

            modelBuilder.Entity("Koop.Models.Order", b =>
                {
                    b.Property<Guid>("OrderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("order_id");

                    b.Property<DateTime>("OrderStartDate")
                        .HasColumnType("timestamp")
                        .HasColumnName("order_start_date");

                    b.Property<Guid>("OrderStatusId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("OrderStopDate")
                        .HasColumnType("timestamp")
                        .HasColumnName("order_stop_date");

                    b.HasKey("OrderId");

                    b.HasIndex("OrderStatusId");

                    b.ToTable("orders");
                });

            modelBuilder.Entity("Koop.Models.OrderStatus", b =>
                {
                    b.Property<Guid>("OrderStatusId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("order_status_id");

                    b.Property<string>("OrderStatusName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("order_status_name");

                    b.HasKey("OrderStatusId");

                    b.ToTable("order_status");
                });

            modelBuilder.Entity("Koop.Models.OrderedItem", b =>
                {
                    b.Property<Guid>("OrderedItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("ordered_item_id");

                    b.Property<Guid>("CoopId")
                        .HasColumnType("uuid")
                        .HasColumnName("coop_id");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("uuid")
                        .HasColumnName("order_id");

                    b.Property<Guid>("OrderStatusId")
                        .HasColumnType("uuid")
                        .HasColumnName("order_status_id");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid")
                        .HasColumnName("product_id");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer")
                        .HasColumnName("quantity");

                    b.HasKey("OrderedItemId");

                    b.HasIndex("CoopId");

                    b.HasIndex("OrderId");

                    b.HasIndex("OrderStatusId");

                    b.HasIndex("ProductId");

                    b.ToTable("ordered_items");
                });

            modelBuilder.Entity("Koop.Models.Product", b =>
                {
                    b.Property<Guid>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("product_id");

                    b.Property<int>("AmountInMagazine")
                        .HasColumnType("integer")
                        .HasColumnName("amount_in_magazine");

                    b.Property<int?>("AmountMax")
                        .HasColumnType("integer")
                        .HasColumnName("amount_max");

                    b.Property<bool>("Available")
                        .HasColumnType("boolean")
                        .HasColumnName("available");

                    b.Property<bool>("Blocked")
                        .HasColumnType("boolean")
                        .HasColumnName("blocked");

                    b.Property<int?>("Deposit")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("deposit")
                        .HasDefaultValueSql("((0))");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<bool>("Magazine")
                        .HasColumnType("boolean")
                        .HasColumnName("magazine");

                    b.Property<string>("Picture")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text")
                        .HasColumnName("picture")
                        .HasDefaultValueSql("('')");

                    b.Property<double>("Price")
                        .HasColumnType("double precision")
                        .HasColumnName("price");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("product_name");

                    b.Property<Guid>("SupplierId")
                        .HasColumnType("uuid")
                        .HasColumnName("supplier_id");

                    b.Property<Guid>("UnitId")
                        .HasColumnType("uuid")
                        .HasColumnName("unit_id");

                    b.HasKey("ProductId");

                    b.HasIndex("SupplierId");

                    b.HasIndex("UnitId");

                    b.ToTable("products");
                });

            modelBuilder.Entity("Koop.Models.ProductCategory", b =>
                {
                    b.Property<Guid>("ProductCategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("product_category_id");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("uuid")
                        .HasColumnName("category_id");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid")
                        .HasColumnName("product_id");

                    b.HasKey("ProductCategoryId");

                    b.HasIndex("CategoryId");

                    b.HasIndex("ProductId");

                    b.ToTable("product_categories");
                });

            modelBuilder.Entity("Koop.Models.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Koop.Models.Supplier", b =>
                {
                    b.Property<Guid>("SupplierId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("supplier_id");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(30)
                        .IsUnicode(false)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("email");

                    b.Property<Guid>("OproId")
                        .HasColumnType("uuid")
                        .HasColumnName("opro_id");

                    b.Property<string>("OproName")
                        .HasColumnType("text");

                    b.Property<DateTime?>("OrderClosingDate")
                        .HasColumnType("timestamp")
                        .HasColumnName("order_closing_date");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("phone");

                    b.Property<string>("Picture")
                        .HasColumnType("text")
                        .HasColumnName("picture");

                    b.Property<string>("SupplierAbbr")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("supplier_abbr");

                    b.Property<string>("SupplierName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("supplier_name");

                    b.HasKey("SupplierId");

                    b.HasIndex("OproId");

                    b.ToTable("suppliers");
                });

            modelBuilder.Entity("Koop.Models.Unit", b =>
                {
                    b.Property<Guid>("UnitId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("unit_id");

                    b.Property<string>("UnitName")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("unit_name");

                    b.HasKey("UnitId");

                    b.ToTable("units");
                });

            modelBuilder.Entity("Koop.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer");

                    b.Property<Guid?>("BasketId")
                        .HasColumnType("uuid");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<double?>("Debt")
                        .HasColumnType("double precision");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<Guid?>("FundId")
                        .HasColumnType("uuid");

                    b.Property<string>("Info")
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("FundId");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Koop.Models.Work", b =>
                {
                    b.Property<Guid>("WorkId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("work_id");

                    b.Property<Guid>("CoopId")
                        .HasColumnType("uuid")
                        .HasColumnName("coop_id");

                    b.Property<double>("Duration")
                        .HasColumnType("double precision")
                        .HasColumnName("duration");

                    b.Property<DateTime>("WorkDate")
                        .HasColumnType("timestamp")
                        .HasColumnName("work_date");

                    b.Property<Guid>("WorkTypeId")
                        .HasColumnType("uuid")
                        .HasColumnName("work_type_id");

                    b.HasKey("WorkId");

                    b.HasIndex("CoopId");

                    b.HasIndex("WorkTypeId");

                    b.ToTable("works");
                });

            modelBuilder.Entity("Koop.Models.WorkType", b =>
                {
                    b.Property<Guid>("WorkTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("work_type_id");

                    b.Property<string>("WorkType1")
                        .IsRequired()
                        .HasMaxLength(200)
                        .IsUnicode(false)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("work_type");

                    b.HasKey("WorkTypeId");

                    b.ToTable("work_types");
                });

            modelBuilder.Entity("Koop.models.UserOrdersHistoryView", b =>
                {
                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<Guid?>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<Guid?>("OrderId")
                        .HasColumnType("uuid")
                        .HasColumnName("order_id");

                    b.Property<string>("OrderStatusName")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("order_status_name");

                    b.Property<DateTime?>("OrderStopDate")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("order_stop_date");

                    b.Property<string>("Price")
                        .HasColumnType("text")
                        .HasColumnName("price");

                    b.ToTable("user_orders_history_view");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("text");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("Koop.Models.AvailableQuantity", b =>
                {
                    b.HasOne("Koop.Models.Product", "Product")
                        .WithMany("AvailableQuantities")
                        .HasForeignKey("ProductId")
                        .HasConstraintName("fk_available_quantities_product_id")
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Koop.Models.Basket", b =>
                {
                    b.HasOne("Koop.Models.User", "Coop")
                        .WithMany("Baskets")
                        .HasForeignKey("CoopId")
                        .HasConstraintName("fk_baskets_coop_id");

                    b.Navigation("Coop");
                });

            modelBuilder.Entity("Koop.Models.Favority", b =>
                {
                    b.HasOne("Koop.Models.User", "Coop")
                        .WithMany("Favorities")
                        .HasForeignKey("CoopId")
                        .HasConstraintName("fk_favorities_coop_id")
                        .IsRequired();

                    b.HasOne("Koop.Models.Product", "Product")
                        .WithMany("Favorities")
                        .HasForeignKey("ProductId")
                        .HasConstraintName("fk_favorities_product_id")
                        .IsRequired();

                    b.Navigation("Coop");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Koop.Models.Order", b =>
                {
                    b.HasOne("Koop.Models.OrderStatus", "OrderStatus")
                        .WithMany()
                        .HasForeignKey("OrderStatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("OrderStatus");
                });

            modelBuilder.Entity("Koop.Models.OrderedItem", b =>
                {
                    b.HasOne("Koop.Models.User", "Coop")
                        .WithMany("OrderedItems")
                        .HasForeignKey("CoopId")
                        .HasConstraintName("fk_ordered_items_coop_id")
                        .IsRequired();

                    b.HasOne("Koop.Models.Order", "Order")
                        .WithMany("OrderedItems")
                        .HasForeignKey("OrderId")
                        .HasConstraintName("fk_ordered_items_order_id")
                        .IsRequired();

                    b.HasOne("Koop.Models.OrderStatus", "OrderStatus")
                        .WithMany("OrderedItems")
                        .HasForeignKey("OrderStatusId")
                        .HasConstraintName("fk_ordered_items_order_status_id")
                        .IsRequired();

                    b.HasOne("Koop.Models.Product", "Product")
                        .WithMany("OrderedItems")
                        .HasForeignKey("ProductId")
                        .HasConstraintName("fk_ordered_items_product_id")
                        .IsRequired();

                    b.Navigation("Coop");

                    b.Navigation("Order");

                    b.Navigation("OrderStatus");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Koop.Models.Product", b =>
                {
                    b.HasOne("Koop.Models.Supplier", "Supplier")
                        .WithMany("Products")
                        .HasForeignKey("SupplierId")
                        .HasConstraintName("fk_products_supplier_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Koop.Models.Unit", "Unit")
                        .WithMany("Products")
                        .HasForeignKey("UnitId")
                        .HasConstraintName("fk_products_unit_id")
                        .IsRequired();

                    b.Navigation("Supplier");

                    b.Navigation("Unit");
                });

            modelBuilder.Entity("Koop.Models.ProductCategory", b =>
                {
                    b.HasOne("Koop.Models.Category", "Category")
                        .WithMany("ProductCategories")
                        .HasForeignKey("CategoryId")
                        .HasConstraintName("fk_product_categories_category_id")
                        .IsRequired();

                    b.HasOne("Koop.Models.Product", "Product")
                        .WithMany("ProductCategories")
                        .HasForeignKey("ProductId")
                        .HasConstraintName("fk_product_categories_product_id")
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Koop.Models.Supplier", b =>
                {
                    b.HasOne("Koop.Models.User", "Opro")
                        .WithMany("Suppliers")
                        .HasForeignKey("OproId")
                        .HasConstraintName("fk_suppliers_coop_id")
                        .IsRequired();

                    b.Navigation("Opro");
                });

            modelBuilder.Entity("Koop.Models.User", b =>
                {
                    b.HasOne("Koop.Models.Fund", "Fund")
                        .WithMany("Cooperators")
                        .HasForeignKey("FundId");

                    b.Navigation("Fund");
                });

            modelBuilder.Entity("Koop.Models.Work", b =>
                {
                    b.HasOne("Koop.Models.User", "Coop")
                        .WithMany("Works")
                        .HasForeignKey("CoopId")
                        .HasConstraintName("fk_works_coop_id")
                        .IsRequired();

                    b.HasOne("Koop.Models.WorkType", "WorkType")
                        .WithMany("Works")
                        .HasForeignKey("WorkTypeId")
                        .HasConstraintName("fk_works_work_type_id")
                        .IsRequired();

                    b.Navigation("Coop");

                    b.Navigation("WorkType");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.HasOne("Koop.Models.Role", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.HasOne("Koop.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.HasOne("Koop.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.HasOne("Koop.Models.Role", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Koop.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.HasOne("Koop.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Koop.Models.Category", b =>
                {
                    b.Navigation("ProductCategories");
                });

            modelBuilder.Entity("Koop.Models.Fund", b =>
                {
                    b.Navigation("Cooperators");
                });

            modelBuilder.Entity("Koop.Models.Order", b =>
                {
                    b.Navigation("OrderedItems");
                });

            modelBuilder.Entity("Koop.Models.OrderStatus", b =>
                {
                    b.Navigation("OrderedItems");
                });

            modelBuilder.Entity("Koop.Models.Product", b =>
                {
                    b.Navigation("AvailableQuantities");

                    b.Navigation("Favorities");

                    b.Navigation("OrderedItems");

                    b.Navigation("ProductCategories");
                });

            modelBuilder.Entity("Koop.Models.Supplier", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("Koop.Models.Unit", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("Koop.Models.User", b =>
                {
                    b.Navigation("Baskets");

                    b.Navigation("Favorities");

                    b.Navigation("OrderedItems");

                    b.Navigation("Suppliers");

                    b.Navigation("Works");
                });

            modelBuilder.Entity("Koop.Models.WorkType", b =>
                {
                    b.Navigation("Works");
                });
#pragma warning restore 612, 618
        }
    }
}
