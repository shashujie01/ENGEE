using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Engee.Models;

public partial class EngeeContext : DbContext
{
    public EngeeContext()
    {
    }

    public EngeeContext(DbContextOptions<EngeeContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TBrand> TBrands { get; set; }

    public virtual DbSet<TCartsItem> TCartsItems { get; set; }

    public virtual DbSet<TCase> TCases { get; set; }

    public virtual DbSet<TCollect> TCollects { get; set; }

    public virtual DbSet<TCollectImage> TCollectImages { get; set; }

    public virtual DbSet<TCollectItem> TCollectItems { get; set; }

    public virtual DbSet<TCosmeticMainCategory> TCosmeticMainCategories { get; set; }

    public virtual DbSet<TCosmeticSubcategory> TCosmeticSubcategories { get; set; }

    public virtual DbSet<TDeliveryType> TDeliveryTypes { get; set; }

    public virtual DbSet<TDemand> TDemands { get; set; }

    public virtual DbSet<TDonationOrder> TDonationOrders { get; set; }

    public virtual DbSet<TDonationOrderDetail> TDonationOrderDetails { get; set; }

    public virtual DbSet<TMember> TMembers { get; set; }

    public virtual DbSet<TMemberFavorite> TMemberFavorites { get; set; }

    public virtual DbSet<TMemberPoint> TMemberPoints { get; set; }

    public virtual DbSet<TMessage> TMessages { get; set; }

    public virtual DbSet<TOrder> TOrders { get; set; }

    public virtual DbSet<TOrderDetail> TOrderDetails { get; set; }

    public virtual DbSet<TProduct> TProducts { get; set; }

    public virtual DbSet<TRating> TRatings { get; set; }

    public virtual DbSet<TShoppingCart> TShoppingCarts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-KCLL746\\MSSQLSERVER_2022;Initial Catalog=Engee;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TBrand>(entity =>
        {
            entity.HasKey(e => e.BrandId).HasName("PK_Table_1");

            entity.ToTable("tBrand");

            entity.Property(e => e.BrandId).HasColumnName("BrandID");
            entity.Property(e => e.BrandCategory).HasMaxLength(50);
            entity.Property(e => e.BrandName).HasMaxLength(100);
        });

        modelBuilder.Entity<TCartsItem>(entity =>
        {
            entity.HasKey(e => e.CartsItemId);

            entity.ToTable("tCartsItems");

            entity.Property(e => e.CartsItemId).HasColumnName("CartsItemID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.ShoppingCartId).HasColumnName("ShoppingCartID");

            entity.HasOne(d => d.Product).WithMany(p => p.TCartsItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tCartsItems_tProducts");

            entity.HasOne(d => d.ShoppingCart).WithMany(p => p.TCartsItems)
                .HasForeignKey(d => d.ShoppingCartId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tCartsItems_tShoppingCarts");
        });

        modelBuilder.Entity<TCase>(entity =>
        {
            entity.HasKey(e => e.CaseId);

            entity.ToTable("tCase");

            entity.Property(e => e.CaseId).HasColumnName("CaseID");
            entity.Property(e => e.CaseCategory)
                .HasMaxLength(1)
                .IsFixedLength();
            entity.Property(e => e.CaseEndDate).HasColumnType("datetime");
            entity.Property(e => e.CaseImagePath).HasMaxLength(1000);
            entity.Property(e => e.CaseStartDate).HasColumnType("datetime");
            entity.Property(e => e.CaseTitle).HasMaxLength(100);
            entity.Property(e => e.DisplayStatus)
                .HasMaxLength(1)
                .IsFixedLength();
            entity.Property(e => e.MemberId).HasColumnName("MemberID");

            entity.HasOne(d => d.Member).WithMany(p => p.TCases)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tCase_tMembers");
        });

        modelBuilder.Entity<TCollect>(entity =>
        {
            entity.HasKey(e => e.CollectId);

            entity.ToTable("tCollect");

            entity.Property(e => e.CollectId).HasColumnName("CollectID");
            entity.Property(e => e.CollectCaption).HasMaxLength(1000);
            entity.Property(e => e.CollectEndDate).HasColumnType("datetime");
            entity.Property(e => e.CollectStartDate).HasColumnType("datetime");
            entity.Property(e => e.CollectTitle).HasMaxLength(100);
            entity.Property(e => e.ConvenienNum).HasMaxLength(50);
            entity.Property(e => e.DeliveryAddress).HasMaxLength(100);
            entity.Property(e => e.DeliveryTypeId).HasColumnName("DeliveryTypeID");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");

            entity.HasOne(d => d.DeliveryType).WithMany(p => p.TCollects)
                .HasForeignKey(d => d.DeliveryTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tCollect_tDeliveryTypes");

            entity.HasOne(d => d.Member).WithMany(p => p.TCollects)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tCollect_tMembers");
        });

        modelBuilder.Entity<TCollectImage>(entity =>
        {
            entity.HasKey(e => e.CollectImageId);

            entity.ToTable("tCollectImage");

            entity.Property(e => e.CollectImageId).HasColumnName("CollectImageID");
            entity.Property(e => e.CollectId).HasColumnName("CollectID");
            entity.Property(e => e.CollectImagePath).HasMaxLength(1000);

            entity.HasOne(d => d.Collect).WithMany(p => p.TCollectImages)
                .HasForeignKey(d => d.CollectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tCollectImage_tCollect");
        });

        modelBuilder.Entity<TCollectItem>(entity =>
        {
            entity.HasKey(e => e.CollectIitemsId);

            entity.ToTable("tCollectItems");

            entity.Property(e => e.CollectIitemsId).HasColumnName("CollectIItemsID");
            entity.Property(e => e.CollectId).HasColumnName("CollectID");
            entity.Property(e => e.CollectItemName).HasMaxLength(50);
            entity.Property(e => e.MainCategoryId).HasColumnName("MainCategoryID");
            entity.Property(e => e.SubcategoryId).HasColumnName("SubcategoryID");

            entity.HasOne(d => d.Collect).WithMany(p => p.TCollectItems)
                .HasForeignKey(d => d.CollectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tCollectItems_tCollectItems");

            entity.HasOne(d => d.MainCategory).WithMany(p => p.TCollectItems)
                .HasForeignKey(d => d.MainCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tCollectItems_tCosmeticMainCategory");

            entity.HasOne(d => d.Subcategory).WithMany(p => p.TCollectItems)
                .HasForeignKey(d => d.SubcategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tCollectItems_tCosmeticSubcategory");
        });

        modelBuilder.Entity<TCosmeticMainCategory>(entity =>
        {
            entity.HasKey(e => e.MainCategoryId).HasName("PK_tbCosmeticMainCategory");

            entity.ToTable("tCosmeticMainCategory");

            entity.Property(e => e.MainCategoryId).HasColumnName("MainCategoryID");
            entity.Property(e => e.MainCategory).HasMaxLength(50);
        });

        modelBuilder.Entity<TCosmeticSubcategory>(entity =>
        {
            entity.HasKey(e => e.SubcategoryId).HasName("PK_tbCosmeticSubcategory");

            entity.ToTable("tCosmeticSubcategory");

            entity.Property(e => e.SubcategoryId).HasColumnName("SubcategoryID");
            entity.Property(e => e.MainCategoryId).HasColumnName("MainCategoryID");
            entity.Property(e => e.Subcategory).HasMaxLength(50);

            entity.HasOne(d => d.MainCategory).WithMany(p => p.TCosmeticSubcategories)
                .HasForeignKey(d => d.MainCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tCosmeticSubcategory_tCosmeticMainCategory");
        });

        modelBuilder.Entity<TDeliveryType>(entity =>
        {
            entity.HasKey(e => e.DeliveryTypeId);

            entity.ToTable("tDeliveryTypes");

            entity.Property(e => e.DeliveryTypeId).HasColumnName("DeliveryTypeID");
            entity.Property(e => e.DeliveryType).HasMaxLength(100);
        });

        modelBuilder.Entity<TDemand>(entity =>
        {
            entity.HasKey(e => e.DemandId).HasName("PK_Table_1_1");

            entity.ToTable("tDemand");

            entity.Property(e => e.DemandId).HasColumnName("DemandID");
            entity.Property(e => e.ConvienenNum).HasMaxLength(50);
            entity.Property(e => e.DeliveryAddress).HasMaxLength(100);
            entity.Property(e => e.DeliveryTypeId).HasColumnName("DeliveryTypeID");
            entity.Property(e => e.DemandDate).HasColumnType("datetime");
            entity.Property(e => e.DemandMessage).HasMaxLength(1000);
            entity.Property(e => e.DemanderId).HasColumnName("DemanderID");
            entity.Property(e => e.GiverId).HasColumnName("GiverID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.ReceiverName).HasMaxLength(50);
            entity.Property(e => e.ReceiverPhone)
                .HasMaxLength(10)
                .IsFixedLength();

            entity.HasOne(d => d.DeliveryType).WithMany(p => p.TDemands)
                .HasForeignKey(d => d.DeliveryTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tDemand_tDeliveryTypes");

            entity.HasOne(d => d.Demander).WithMany(p => p.TDemandDemanders)
                .HasForeignKey(d => d.DemanderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tDemand_tMembers1");

            entity.HasOne(d => d.Giver).WithMany(p => p.TDemandGivers)
                .HasForeignKey(d => d.GiverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tDemand_tMembers");

            entity.HasOne(d => d.Product).WithMany(p => p.TDemands)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tDemand_tProducts");
        });

        modelBuilder.Entity<TDonationOrder>(entity =>
        {
            entity.HasKey(e => e.DonationOrderId).HasName("PK_tbDonationOrder");

            entity.ToTable("tDonationOrder");

            entity.Property(e => e.DonationOrderId).HasColumnName("DonationOrderID");
            entity.Property(e => e.DeliveryTypeId).HasColumnName("DeliveryTypeID");
            entity.Property(e => e.DonarName).HasMaxLength(50);
            entity.Property(e => e.DonarPhone)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.DonationStatus)
                .HasMaxLength(1)
                .IsFixedLength();
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.OrderDate).HasColumnType("datetime");

            entity.HasOne(d => d.DeliveryType).WithMany(p => p.TDonationOrders)
                .HasForeignKey(d => d.DeliveryTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tDonationOrder_tDeliveryTypes");

            entity.HasOne(d => d.Member).WithMany(p => p.TDonationOrders)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tDonationOrder_tMembers");
        });

        modelBuilder.Entity<TDonationOrderDetail>(entity =>
        {
            entity.HasKey(e => e.DonationOrderDetailId);

            entity.ToTable("tDonationOrderDetail");

            entity.Property(e => e.DonationOrderDetailId).HasColumnName("DonationOrderDetailID");
            entity.Property(e => e.DonationOrderId).HasColumnName("DonationOrderID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.DonationOrder).WithMany(p => p.TDonationOrderDetails)
                .HasForeignKey(d => d.DonationOrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tDonationOrderDetail_tDonationOrderDetail");

            entity.HasOne(d => d.Product).WithMany(p => p.TDonationOrderDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tDonationOrderDetail_tProducts");
        });

        modelBuilder.Entity<TMember>(entity =>
        {
            entity.HasKey(e => e.MemberId);

            entity.ToTable("tMembers");

            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.Birth).HasColumnType("date");
            entity.Property(e => e.CharityProof).HasMaxLength(1000);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Fullname).HasMaxLength(100);
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsFixedLength();
            entity.Property(e => e.Introduction).HasMaxLength(200);
            entity.Property(e => e.Nickname).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(25);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.PhotoPath).HasMaxLength(1000);
            entity.Property(e => e.RegistrationDate).HasColumnType("datetime");
            entity.Property(e => e.Username).HasMaxLength(15);
        });

        modelBuilder.Entity<TMemberFavorite>(entity =>
        {
            entity.HasKey(e => e.FavoriteId);

            entity.ToTable("tMemberFavorite");

            entity.Property(e => e.FavoriteId).HasColumnName("FavoriteID");
            entity.Property(e => e.AddFavoriteDate).HasColumnType("datetime");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Member).WithMany(p => p.TMemberFavorites)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tMemberFavorite_tMembers");

            entity.HasOne(d => d.Product).WithMany(p => p.TMemberFavorites)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tMemberFavorite_tProducts");
        });

        modelBuilder.Entity<TMemberPoint>(entity =>
        {
            entity.HasKey(e => e.PointId);

            entity.ToTable("tMemberPoint");

            entity.Property(e => e.PointId).HasColumnName("PointID");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.Memo).HasMaxLength(200);
            entity.Property(e => e.TransactionDate).HasColumnType("datetime");

            entity.HasOne(d => d.Member).WithMany(p => p.TMemberPoints)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tMemberPoint_tMembers");
        });

        modelBuilder.Entity<TMessage>(entity =>
        {
            entity.HasKey(e => e.MessageId);

            entity.ToTable("tMessage");

            entity.Property(e => e.MessageId).HasColumnName("MessageID");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.MessageArea)
                .HasMaxLength(1)
                .IsFixedLength();
            entity.Property(e => e.MessageContent).HasMaxLength(1000);
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Member).WithMany(p => p.TMessages)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tMessage_tMembers");

            entity.HasOne(d => d.Product).WithMany(p => p.TMessages)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tMessage_tProducts");
        });

        modelBuilder.Entity<TOrder>(entity =>
        {
            entity.HasKey(e => e.OrderId);

            entity.ToTable("tOrders");

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.BuyerId).HasColumnName("BuyerID");
            entity.Property(e => e.ConvienenNum).HasMaxLength(50);
            entity.Property(e => e.DeliveryAddress).HasMaxLength(100);
            entity.Property(e => e.DeliveryTypeId).HasColumnName("DeliveryTypeID");
            entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.OrderStatus)
                .HasMaxLength(1)
                .IsFixedLength();
            entity.Property(e => e.SellerId).HasColumnName("SellerID");

            entity.HasOne(d => d.Buyer).WithMany(p => p.TOrderBuyers)
                .HasForeignKey(d => d.BuyerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tOrders_tMembers");

            entity.HasOne(d => d.DeliveryType).WithMany(p => p.TOrders)
                .HasForeignKey(d => d.DeliveryTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tOrders_tDeliveryTypes");

            entity.HasOne(d => d.Seller).WithMany(p => p.TOrderSellers)
                .HasForeignKey(d => d.SellerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tOrders_tMembers1");
        });

        modelBuilder.Entity<TOrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId);

            entity.ToTable("tOrderDetail");

            entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");
            entity.Property(e => e.BuyerId).HasColumnName("BuyerID");
            entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.SellerId).HasColumnName("SellerID");

            entity.HasOne(d => d.Buyer).WithMany(p => p.TOrderDetailBuyers)
                .HasForeignKey(d => d.BuyerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tOrderDetail_tMembers");

            entity.HasOne(d => d.Order).WithMany(p => p.TOrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tOrderDetail_tOrders");

            entity.HasOne(d => d.Product).WithMany(p => p.TOrderDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tOrderDetail_tOrderDetail");

            entity.HasOne(d => d.Seller).WithMany(p => p.TOrderDetailSellers)
                .HasForeignKey(d => d.SellerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tOrderDetail_tMembers1");
        });

        modelBuilder.Entity<TProduct>(entity =>
        {
            entity.HasKey(e => e.ProductId);

            entity.ToTable("tProducts");

            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.BrandId).HasColumnName("BrandID");
            entity.Property(e => e.DateOfSale).HasColumnType("datetime");
            entity.Property(e => e.DeliveryTypeId).HasColumnName("DeliveryTypeID");
            entity.Property(e => e.MainCategoryId).HasColumnName("MainCategoryID");
            entity.Property(e => e.ProductDescribe).HasMaxLength(100);
            entity.Property(e => e.ProductExpirationDate).HasColumnType("datetime");
            entity.Property(e => e.ProductImagePath).HasMaxLength(1000);
            entity.Property(e => e.ProductName).HasMaxLength(100);
            entity.Property(e => e.ProductUsageStatus).HasMaxLength(100);
            entity.Property(e => e.SellerId).HasColumnName("SellerID");
            entity.Property(e => e.SubcategoryId).HasColumnName("SubcategoryID");

            entity.HasOne(d => d.Brand).WithMany(p => p.TProducts)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tProducts_tBrand");

            entity.HasOne(d => d.DeliveryType).WithMany(p => p.TProducts)
                .HasForeignKey(d => d.DeliveryTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tProducts_tDeliveryTypes");

            entity.HasOne(d => d.MainCategory).WithMany(p => p.TProducts)
                .HasForeignKey(d => d.MainCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tProducts_tCosmeticMainCategory");

            entity.HasOne(d => d.Seller).WithMany(p => p.TProducts)
                .HasForeignKey(d => d.SellerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tProducts_tMembers");

            entity.HasOne(d => d.Subcategory).WithMany(p => p.TProducts)
                .HasForeignKey(d => d.SubcategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tProducts_tCosmeticSubcategory");
        });

        modelBuilder.Entity<TRating>(entity =>
        {
            entity.HasKey(e => e.RatingId);

            entity.ToTable("tRatings");

            entity.Property(e => e.RatingId).HasColumnName("RatingID");
            entity.Property(e => e.BuyerComment).HasMaxLength(200);
            entity.Property(e => e.BuyerId).HasColumnName("BuyerID");
            entity.Property(e => e.BuyerRatingDate).HasColumnType("datetime");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.SellerComment).HasMaxLength(200);
            entity.Property(e => e.SellerId).HasColumnName("SellerID");
            entity.Property(e => e.SellerRatingDate).HasColumnType("datetime");

            entity.HasOne(d => d.Buyer).WithMany(p => p.TRatingBuyers)
                .HasForeignKey(d => d.BuyerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tRatings_tMembers");

            entity.HasOne(d => d.Order).WithMany(p => p.TRatings)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tRatings_tOrders");

            entity.HasOne(d => d.Seller).WithMany(p => p.TRatingSellers)
                .HasForeignKey(d => d.SellerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tRatings_tMembers1");
        });

        modelBuilder.Entity<TShoppingCart>(entity =>
        {
            entity.HasKey(e => e.ShoppingCartId);

            entity.ToTable("tShoppingCarts");

            entity.Property(e => e.ShoppingCartId).HasColumnName("ShoppingCartID");
            entity.Property(e => e.CartCreatDate).HasColumnType("datetime");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");

            entity.HasOne(d => d.Member).WithMany(p => p.TShoppingCarts)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tShoppingCarts_tMembers");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
