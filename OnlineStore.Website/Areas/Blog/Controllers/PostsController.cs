using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.DataLayer;
using OnlineStore.Providers;
using OnlineStore.Models.Public;
using OnlineStore.Providers.Controllers;
using OnlineStore.Models.Enums;
using OnlineStore.Models;
using OnlineStore.EntityFramework;
using OnlineStore.Services;
using System.Threading;

namespace OnlineStore.Website.Areas.Blog.Controllers
{
    public class PostsController : PublicController
    {
        private GroupType _groupType;
        readonly int pageSize = 5;
        const string url = "/Areas/Blog/Views/Posts/";

        public PostsController()
        {
            _groupType = GroupType.Blogs;
        }

        [Route("وبلاگ")]
        public ActionResult RedirectIndex(int? index, int? groupID)
        {
            return RedirectPermanent("/Blog");
        }

        [Route("وبلاگ/{id:int}/{title}")]
        public ActionResult RedirectDetails(int id)
        {
            var article = Articles.GetByID(id);

            var group = Groups.GetByID(article.GroupID.Value);

            var url = UrlProvider.GetPostUrl(id, article.Title, group.TitleEn);

            return RedirectPermanent(url);
        }

        [HttpGet]
        [Route("Blog")]
        [Route("Blog/{pageIndex:int}")]

        [Route("Blog/{groupTitle}")]
        [Route("Blog/{groupTitle}/{pageIndex:int}")]
        public ActionResult List(string groupTitle = "", int pageIndex = 0)
        {
            groupTitle = groupTitle.DeNormalizeForUrl();

            var group = Groups.GetByTitle(groupTitle, _groupType);
            int? groupID = null;

            if (group != null)
                groupID = group.ID;

            if (pageIndex > 0)
            {
                pageIndex = pageIndex - 1;
            }
            else
                pageIndex = 0;

            var list = Articles.GetBlogList(pageIndex, pageSize, OnlineStore.Models.Enums.ArticleType.Blog, DateTime.Now, groupID);
            var latestPosts = Articles.GetLatestPosts(groupID.HasValue ? groupID.Value : (int?)null);
            var latestComments = ArticleComments.GetLatestComments(ArticleType.Blog, 6);

            var count = Articles.CountBlogList(OnlineStore.Models.Enums.ArticleType.Blog, DateTime.Now, groupID);
            var totalPages = (int)Math.Ceiling((decimal)count / pageSize);
            var paging = Utilities.MakePaging(totalPages, pageIndex + 1);

            foreach (var item in list)
            {
                try
                {
                    var user = Identity.OSUsers.GetByID(item.UserID);
                    item.UserTitle = user.Firstname + " " + user.Lastname;
                }
                catch (Exception ex)
                {
                    item.UserTitle = StaticValues.HomeTitle;
                }

            }

            var model = new BlogList
            {
                DataList = list,
                GroupID = groupID,
                Paging = paging,
                TotalPages = totalPages,
                CurrentPageIndex = pageIndex,
                LatestPosts = latestPosts,
                LatestComments = latestComments
            };

            return View(url + "Index.cshtml", model: model);
        }

        [Route("Blog/{group}/{title}-{id:int}")]
        public ActionResult Details(int id)
        {
            #region Details

            var blogDetails = Articles.GetBlogByID(id);

            if (blogDetails == null)
                return HttpNotFound();

            try
            {
                var user = Identity.OSUsers.GetByID(blogDetails.UserID);
                blogDetails.UserTitle = user.Firstname + " " + user.Lastname;
            }
            catch (Exception)
            {
                blogDetails.UserTitle = StaticValues.HomeTitle;
            }

            blogDetails.Text = HttpUtility.HtmlDecode(blogDetails.Text);

            #endregion Details

            var latestPosts = Articles.GetLatestPosts(blogDetails.GroupID);
            var latestComments = ArticleComments.GetLatestComments(ArticleType.Blog, 6, blogDetails.GroupID);
            var comments = ArticleComments.GetByArticleID(id);
            var relatedPosts = RelatedArticles.GetRelatedArticles(id);
            var group = Groups.GetByID(blogDetails.GroupID);

            #region Products

            var products = Products.GetRandom();
            Products.FillProductItems(UserID, products, StaticValues.RelatedProductImageSize);

            var shopProducts = new RelatedProductSettings
            {
                Products = products,
                Title = "فروشگاه آنلاین استور"
            };

            #endregion Products

            // increase Visits
            increaseVisits(id);

            BlogDetail model = new BlogDetail
            {
                BlogDetails = blogDetails,
                LatestComments = latestComments,
                LatestPosts = latestPosts,
                Comments = comments,
                RelatedPosts = relatedPosts,
                Products = shopProducts
            };

            ViewBag.Title = blogDetails.Title;
            ViewBag.Description = blogDetails.Summary;
            ViewBag.Keywords = group.Title + ", " + group.TitleEn +
                               ", " + blogDetails.Title.Split(' ').Aggregate((a, b) => b + ", " + a);
            ViewBag.OGType = "article";
            ViewBag.OGImage = StaticValues.WebsiteUrl + StaticPaths.ArticleImages + blogDetails.Image;

            return View(url + "Details.cshtml", model: model);
        }

        [HttpPost]
        [Route("Blog/Posts/AddComment")]
        public JsonResult AddComment(int id, string userName, string email, string subject, string text)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                ArticleComment comment = new ArticleComment
                {
                    ArticleID = id,
                    CommentStatus = ArticleCommentStatus.NotChecked,
                    UserName = userName,
                    Email = email,
                    Subject = subject,
                    Text = text,
                    LastUpdate = DateTime.Now,
                    UserID = User.Identity.IsAuthenticated ? UserID : null
                };

                ArticleComments.Insert(comment);

                // اطلاع رسانی به مدیر
                #region Apprise Admin 

                string body = "مدیریت محترم، در بخش نظرات وبلاگ، نظر جدیدی ثبت شد:";
                body += "<br/>";
                body += String.Format("ایمیل: {0} <br/> موضوع: {1} <br/> پیام: {2}", email, subject, text);

                EmailServices.NotifyAdminsByEmail(AdminEmailType.NewBlogComment, body, null);

                #endregion Apprise Admin

                jsonSuccessResult.Success = true;
            }
            catch (DbException ex)
            {
                jsonSuccessResult.Errors = ex.Errors.ToArray();
                jsonSuccessResult.Success = false;
            }
            catch (Exception ex)
            {
                jsonSuccessResult.Errors = new string[] { ex.Message };
                jsonSuccessResult.Success = false;
            }

            return new JsonResult()
            {
                Data = jsonSuccessResult
            };
        }

        private void increaseVisits(int id)
        {
            var ip = Utilities.GetIP();
            var now = DateTime.Now;

            var threadStart = new ThreadStart(() =>
            {
                Articles.AddVisits(id);
                ArticleVisit visit = new ArticleVisit
                {
                    IP = ip,
                    LastUpdate = now,
                    UserID = UserID,
                    ArticleID = id
                };

                ArticleVisits.Insert(visit);
            });
            Thread thread = new Thread(threadStart);
            thread.Start();
        }

        [HttpPost]
        [Route("Blog/Posts/InsertRate")]
        public JsonResult InsertRate(int articleID, float rate)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                string ip = Utilities.GetIP();
                var rpt = ArticleRates.RepeatPreferentials(articleID, ip, UserID);

                if (rpt < 5)
                {
                    var articleRate = new ArticleRate
                    {
                        ArticleID = articleID,
                        UserID = UserID,
                        Rate = rate,
                        IP = ip,
                        LastUpdate = DateTime.Now
                    };

                    ArticleRates.Insert(articleRate);

                    var rates = Articles.GetBlogByID(articleID);
                    var total = Utilities.CalcRaty(rates.SumScore.Value, rates.ScoreCount, rates.ArticleScore).ToString("0.0");

                    var data = new
                    {
                        CountScore = rates.ScoreCount + 1,
                        TotalScore = total
                    };

                    jsonSuccessResult.Success = true;
                    jsonSuccessResult.Data = data;
                }
                else
                {
                    jsonSuccessResult.Success = false;
                    jsonSuccessResult.Errors = new string[] { "Repeat" };
                }
            }
            catch (DbException ex)
            {
                jsonSuccessResult.Errors = ex.Errors.ToArray();
                jsonSuccessResult.Success = false;
            }
            catch (Exception ex)
            {
                jsonSuccessResult.Errors = new string[] { ex.Message };
                jsonSuccessResult.Success = false;
            }

            return new JsonResult()
            {
                Data = jsonSuccessResult
            };
        }

    }
}