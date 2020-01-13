using OnlineStore.Models.Admin;
using OnlineStore.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;


namespace OnlineStore.DataLayer
{
    public class GroupBanner : EntityBase
    {
        [MaxLength(200)]
        public string Filename { get; set; }

        public GroupBannerType GroupBannerType { get; set; }

        [MaxLength(300)]
        public string Link { get; set; }

        [ForeignKey("Group")]
        public int GroupID { get; set; }
        public Group Group { get; set; }
    }

    public static class GroupBanners
    {
        public static List<EditGroupBanner> GetByGroupID(int groupID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.GroupBanners
                            where item.GroupID == groupID
                            select new EditGroupBanner
                            {
                                ID = item.ID,
                                Filename = item.Filename,
                                GroupBannerType = item.GroupBannerType,
                                Link = item.Link
                            };

                return query.ToList();
            }
        }

        public static GroupBanner GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var groupBanner = db.GroupBanners.Where(item => item.ID == id).Single();

                return groupBanner;
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var groupBanner = (from item in db.GroupBanners
                                   where item.ID == id
                                   select item).Single();

                db.GroupBanners.Remove(groupBanner);

                db.SaveChanges();
            }
        }

        public static void Insert(GroupBanner groupBanner)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.GroupBanners.Add(groupBanner);

                db.SaveChanges();
            }
        }

        public static void UpdateGroupBannerType(int id, GroupBannerType groupBannerType)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgGroupBanner = db.GroupBanners.Where(item => item.ID == id).Single();

                orgGroupBanner.GroupBannerType = groupBannerType;

                db.SaveChanges();
            }
        }
    }

}
