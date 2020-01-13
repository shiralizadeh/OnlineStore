using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.Providers.Controllers;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using OnlineStore.DataLayer;
using System.Drawing;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class PricesController : AdminController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ExportToExcel(string jsonGroups)
        {
            var pricesdt = new System.Data.DataTable("Prices");

            var groups = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int?>>(jsonGroups);

            var productprices = ProductPrices.GetByGroupIDs(groups);
            var varientPrices = ProductVarientPrices.GetByGroupIDs(groups);

            var prices = productprices.Union(varientPrices).Where(item => item.PriceID.HasValue || item.VarientID.HasValue);

            pricesdt.Columns.Add("ProductID", typeof(string));
            pricesdt.Columns.Add("PriceID", typeof(string));
            pricesdt.Columns.Add("VarientID", typeof(string));
            pricesdt.Columns.Add("PriceCode", typeof(string));
            pricesdt.Columns.Add("Title", typeof(string));
            pricesdt.Columns.Add("PriceType", typeof(string));
            pricesdt.Columns.Add("Price", typeof(string));
            pricesdt.Columns.Add("NewPrice", typeof(string));

            foreach (var item in prices)
            {
                pricesdt.Rows.Add(item.ProductID, item.PriceID, item.VarientID, item.PriceCode, item.Title, item.PriceType, item.Price, "");
            }

            var grid = new GridView();
            grid.Font.Name = "tahoma";
            grid.Font.Size = new FontUnit(11, UnitType.Pixel);
            grid.DataSource = pricesdt;
            grid.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Prices.xls");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View("MyView");
        }
    }
}