using AutoMapper;
using OnlineStore.DataLayer;
using Admin = OnlineStore.Models.Admin;
using Public = OnlineStore.Models.Public;
using User = OnlineStore.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Owin;
using OnlineStore.Identity;

namespace OnlineStore.Website
{
    public class AutoMapperConfig
    {
        public static void Configure()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Product, Admin.EditProduct>();
                cfg.CreateMap<Admin.EditProduct, Product>();

                cfg.CreateMap<Admin.EditProductImage, ProductImage>();
                cfg.CreateMap<ProductImage, Admin.EditProductImage>();

                cfg.CreateMap<Admin.EditProductFile, ProductFile>();
                cfg.CreateMap<ProductFile, Admin.EditProductFile>();

                cfg.CreateMap<OSUser, Admin.EditOSUser>();
                cfg.CreateMap<Admin.EditOSUser, OSUser>();

                cfg.CreateMap<OSUser, User.EditOSUser>();
                cfg.CreateMap<User.EditOSUser, OSUser>();

                cfg.CreateMap<OSUser, Admin.UserShortInfo>();
                cfg.CreateMap<Admin.UserShortInfo, OSUser>();

                cfg.CreateMap<ProductMark, Admin.EditProductMark>();
                cfg.CreateMap<Admin.EditProductMark, ProductMark>();

                cfg.CreateMap<ProductPoint, Admin.EditProductPoint>();
                cfg.CreateMap<Admin.EditProductPoint, ProductPoint>();

                cfg.CreateMap<ProductPricesLink, Admin.EditProductPricesLink>();
                cfg.CreateMap<Admin.EditProductPricesLink, ProductPricesLink>();

                cfg.CreateMap<ProductNote, Admin.EditProductNote>();
                cfg.CreateMap<Admin.EditProductNote, ProductNote>();

                cfg.CreateMap<ProductKeyword, Admin.EditProductKeyword>();
                cfg.CreateMap<Admin.EditProductKeyword, ProductKeyword>();

                cfg.CreateMap<StaticContent, Admin.EditStaticContent>();
                cfg.CreateMap<Admin.EditStaticContent, StaticContent>();

                cfg.CreateMap<SliderImage, Public.ViewSliderImage>();
                cfg.CreateMap<Public.ViewSliderImage, SliderImage>();

                cfg.CreateMap<Banner, Public.ViewBanner>();
                cfg.CreateMap<Public.ViewBanner, Banner>();

                cfg.CreateMap<Producer, Public.ViewProducer>();
                cfg.CreateMap<Public.ViewProducer, Producer>();

                cfg.CreateMap<PaymentMethod, Public.ViewPaymentMethod>();
                cfg.CreateMap<Public.ViewPaymentMethod, PaymentMethod>();

                cfg.CreateMap<SendMethod, Public.ViewSendMethod>();
                cfg.CreateMap<Public.ViewSendMethod, SendMethod>();

                cfg.CreateMap<Cart, Admin.EditCart>();
                cfg.CreateMap<Admin.EditCart, Cart>();

                cfg.CreateMap<ScoreParameter, Admin.EditScoreParameter>();
                cfg.CreateMap<Admin.EditScoreParameter, ScoreParameter>();

                cfg.CreateMap<ScoreComment, Admin.EditScoreComment>();
                cfg.CreateMap<Admin.EditScoreComment, ScoreComment>();

                cfg.CreateMap<Public.ViewBuyerInfo, OSUser>();
                cfg.CreateMap<OSUser, Public.ViewBuyerInfo>();

                cfg.CreateMap<Admin.EditProductVarient, ProductVarient>();
                cfg.CreateMap<ProductVarient, Admin.EditProductVarient>();

                cfg.CreateMap<Admin.EditProductVarientAttribute, ProductVarientAttribute>();
                cfg.CreateMap<ProductVarientAttribute, Admin.EditProductVarientAttribute>();

                cfg.CreateMap<Admin.EditAttrGroup, AttrGroup>();
                cfg.CreateMap<AttrGroup, Admin.EditAttrGroup>();

                cfg.CreateMap<Admin.EditAttribute, DataLayer.Attribute>();
                cfg.CreateMap<DataLayer.Attribute, Admin.EditAttribute>();

                cfg.CreateMap<Admin.EditProducer, DataLayer.Producer>();
                cfg.CreateMap<DataLayer.Producer, Admin.EditProducer>();

                cfg.CreateMap<Public.SitePage, DataLayer.MenuItem>();
                cfg.CreateMap<DataLayer.MenuItem, Public.SitePage>();

                cfg.CreateMap<Public.BreadCrumbLink, DataLayer.MenuItem>();
                cfg.CreateMap<DataLayer.MenuItem, Public.BreadCrumbLink>();

                cfg.CreateMap<Public.ViewMenuItem, DataLayer.MenuItem>();
                cfg.CreateMap<DataLayer.MenuItem, Public.ViewMenuItem>();

                cfg.CreateMap<Public.JsonProductGroup, DataLayer.Group>();
                cfg.CreateMap<DataLayer.Group, Public.JsonProductGroup>();

                cfg.CreateMap<Public.JsonKeyword, DataLayer.Keyword>();
                cfg.CreateMap<DataLayer.Keyword, Public.JsonKeyword>();

                cfg.CreateMap<OrderNote, Admin.EditOrderNote>();
                cfg.CreateMap<Admin.EditOrderNote, OrderNote>();

                cfg.CreateMap<Admin.JsonProductRequest, ProductRequest>();
                cfg.CreateMap<ProductRequest, Admin.JsonProductRequest>();

                cfg.CreateMap<User.ViewOrderItem, Public.ViewCartItem>();
                cfg.CreateMap<Public.ViewCartItem, User.ViewOrderItem>();

                cfg.CreateMap<Public.ViewEmail, Email>();
                cfg.CreateMap<Email, Public.ViewEmail>();

                cfg.CreateMap<Admin.JsonUserTask, UserTask>();
                cfg.CreateMap<UserTask, Admin.JsonUserTask>();

                cfg.CreateMap<Admin.EditPackage, Package>();
                cfg.CreateMap<Package, Admin.EditPackage>();

                cfg.CreateMap<Admin.EditProductImage, PackageImage>();
                cfg.CreateMap<PackageImage, Admin.EditProductImage>();

                cfg.CreateMap<Admin.EditPackageProduct, PackageProduct>();
                cfg.CreateMap<PackageProduct, Admin.EditPackageProduct>();

                cfg.CreateMap<Admin.EditMenuItem, MenuItem>();
                cfg.CreateMap<MenuItem, Admin.EditMenuItem>();

                cfg.CreateMap<Admin.EditMenuItemBanner, MenuItemBanner>();
                cfg.CreateMap<MenuItemBanner, Admin.EditMenuItemBanner>();

                cfg.CreateMap<Admin.EditGroup, Group>();
                cfg.CreateMap<Group, Admin.EditGroup>();

                cfg.CreateMap<Admin.EditGroupBanner, GroupBanner>();
                cfg.CreateMap<GroupBanner, Admin.EditGroupBanner>();

            });
        }
    }
}