using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.EntityFramework;
using System.Data.Entity.Validation;
using OnlineStore.Providers;

namespace OnlineStore.DataLayer
{
    public class OnlineStoreDbContext : DbContext
    {
        public OnlineStoreDbContext()
            : base(Global.ConnectionString)
        {

        }

        public static OnlineStoreDbContext Entity
        {
            get
            {
                var entity = new OnlineStoreDbContext();

                //entity.Database.CommandTimeout = 300;

                return entity;
            }
        }

        #region Tables

        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<GroupBanner> GroupBanners { get; set; }
        public virtual DbSet<AttrGroup> AttrGroups { get; set; }
        public virtual DbSet<AttrGroupGroup> AttrGroupGroups { get; set; }
        public virtual DbSet<Attribute> Attributes { get; set; }
        public virtual DbSet<AttributeGroup> AttributeGroups { get; set; }
        public virtual DbSet<AttributeOption> AttributeOptions { get; set; }
        public virtual DbSet<AttributeValue> AttributeValues { get; set; }
        public virtual DbSet<GiftCard> GiftCards { get; set; }
        public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }
        public virtual DbSet<SendMethod> SendMethods { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<CartItem> CartItems { get; set; }
        public virtual DbSet<CartItemGift> CartItemGifts { get; set; }
        public virtual DbSet<Producer> Producers { get; set; }
        public virtual DbSet<ProducerGroup> ProducerGroups { get; set; }
        public virtual DbSet<ProductComment> ProductComments { get; set; }
        public virtual DbSet<ProductQuestion> ProductQuestions { get; set; }
        public virtual DbSet<ProductDiscount> ProductDiscounts { get; set; }
        public virtual DbSet<ProductDonwload> ProductDonwloads { get; set; }
        public virtual DbSet<ProductFile> ProductFiles { get; set; }
        public virtual DbSet<ProductGroup> ProductGroups { get; set; }
        public virtual DbSet<ProductImage> ProductImages { get; set; }
        public virtual DbSet<ProductProfit> ProductProfits { get; set; }
        public virtual DbSet<ProductMark> ProductMarks { get; set; }
        public virtual DbSet<ProductGift> ProductGifts { get; set; }
        public virtual DbSet<ProductPrice> ProductPrices { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductSupply> ProductSupplies { get; set; }
        public virtual DbSet<ProductRequest> ProductRequests { get; set; }
        public virtual DbSet<ProductVisit> ProductVisits { get; set; }
        public virtual DbSet<SliderImage> SliderImages { get; set; }
        public virtual DbSet<UserNotification> UserNotifications { get; set; }
        public virtual DbSet<StaticContent> StaticContents { get; set; }
        public virtual DbSet<UserScore> UserScores { get; set; }
        public virtual DbSet<Article> Articles { get; set; }
        public virtual DbSet<ArticleComment> ArticleComments { get; set; }
        public virtual DbSet<ArticleVisit> ArticleVisits { get; set; }
        public virtual DbSet<ArticleRate> ArticleRates { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<ContactUsMessage> ContactUsMessages { get; set; }
        public virtual DbSet<UserWishe> UserWishes { get; set; }
        public virtual DbSet<UserAddress> UserAddresses { get; set; }
        public virtual DbSet<UserWork> UserWorks { get; set; }
        public virtual DbSet<CustomerComment> CustomerComments { get; set; }
        public virtual DbSet<Banner> Banners { get; set; }
        public virtual DbSet<NewsLetterMember> NewsLetterMembers { get; set; }
        public virtual DbSet<RelatedProduct> RelatedProducts { get; set; }
        public virtual DbSet<HomeBox> HomeBoxes { get; set; }
        public virtual DbSet<HomeBoxProduct> HomeBoxProducts { get; set; }
        public virtual DbSet<HomeBoxItem> HomeBoxItems { get; set; }
        public virtual DbSet<ScoreParameter> ScoreParameters { get; set; }
        public virtual DbSet<GroupScoreParameter> GroupScoreParameters { get; set; }
        public virtual DbSet<ScoreComment> ScoreComments { get; set; }
        public virtual DbSet<ScoreParameterValue> ScoreParameterValues { get; set; }
        public virtual DbSet<RelatedArticle> RelatedArticles { get; set; }
        public virtual DbSet<ProductVarient> ProductVarients { get; set; }
        public virtual DbSet<ProductVarientAttribute> ProductVarientAttributes { get; set; }
        public virtual DbSet<ProductKeyword> ProductKeywords { get; set; }
        public virtual DbSet<ProductVarientPrice> ProductVarientPrices { get; set; }
        public virtual DbSet<ProductAccessory> ProductAccessories { get; set; }
        public virtual DbSet<ProductCommentRate> ProductCommentRates { get; set; }
        public virtual DbSet<ProductRate> ProductRates { get; set; }
        public virtual DbSet<MenuItem> MenuItems { get; set; }
        public virtual DbSet<MenuItemBanner> MenuItemBanners { get; set; }
        public virtual DbSet<PaymentLog> PaymentLogs { get; set; }
        public virtual DbSet<ProductPoint> ProductPoints { get; set; }
        public virtual DbSet<ProductNote> ProductNotes { get; set; }
        public virtual DbSet<ProductPricesLink> ProductPricesLinks { get; set; }
        public virtual DbSet<Keyword> Keywords { get; set; }
        public virtual DbSet<ProductSuggestion> ProductSuggestions { get; set; }
        public virtual DbSet<SMSLog> SMSLogs { get; set; }
        public virtual DbSet<EmailLog> EmailLogs { get; set; }
        public virtual DbSet<OrderNote> OrderNotes { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<Email> Emails { get; set; }
        public virtual DbSet<EmailSend> EmailSends { get; set; }
        public virtual DbSet<SpecialOrder> SpecialOrders { get; set; }
        public virtual DbSet<UserTask> UserTasks { get; set; }
        public virtual DbSet<Employment> Employments { get; set; }
        public virtual DbSet<Colleague> Colleagues { get; set; }
        public virtual DbSet<Package> Packages { get; set; }
        public virtual DbSet<PackageProduct> PackageProducts { get; set; }
        public virtual DbSet<PackageImage> PackageImages { get; set; }
        public virtual DbSet<PriceListSection> PriceListSections { get; set; }
        public virtual DbSet<PriceListProduct> PriceListProducts { get; set; }
        public virtual DbSet<PriceListLog> PriceListLogs { get; set; }

        #endregion

        public override int SaveChanges()
        {
            this.InvalidateSecondLevelCache();

            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage).ToList();

                throw new DbException(errorMessages);
            }
            catch (Exception ex)
            {
                var errorMessages = new List<string>() { "Message: " + ex.Message + " StackTrace: " + ex.StackTrace };

                var innerException = ex.InnerException;

                while (innerException != null)
                {
                    errorMessages.Add("InnerExceptionMessage: " + ex.InnerException.Message + " InnerExceptionStackTrace: " + ex.InnerException.StackTrace);
                    innerException = innerException.InnerException;
                }

                throw new DbException(errorMessages);
            }
        }
    }
}
