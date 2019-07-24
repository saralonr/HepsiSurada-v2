using HepsiBuradaBot.Models;
using HtmlAgilityPack;
using Jurassic;
using Jurassic.Library;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HepsiBuradaBot.Controllers
{
    public class CrawlController : ApiController
    {
        [HttpGet]
        public IHttpActionResult QueryByLink(string link)
        {
            ApiResponse res = new ApiResponse();
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(link);
                req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.157 Safari/537.36";
                req.Host = "www.hepsiburada.com";
                req.Referer = "www.hepsiburada.com";
                var response = req.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(responseString);
                var nodes = doc.DocumentNode.Descendants();

                List<Product> list = new List<Product>();
                var scripts = nodes.Where(x => x.Name == "script").ToList();
               var script = scripts.Where(x => x.InnerHtml.Contains("viewModelName")).FirstOrDefault();
                ScriptEngine eng = new ScriptEngine();
                var scriptContent = script.InnerHtml;

                var result = eng.Evaluate("(function(){ "+ scriptContent + "; return productModel; })()");
                var json = JSONObject.Stringify(eng, result);

                dynamic productObj = JsonConvert.DeserializeObject(json);
              var listings =  productObj.product.listings;
                foreach (var item in listings)
                {
                    Product p = new Product();
                    p.SellerName = item.merchantName;
                    p.Price = item.priceText;
                    list.Add(p);
                }

                res.Data = list;
                res.Status = true;
                res.Description = "Success";
                return Json(res);
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Description = "Hata Mesajı : "+ ex.Message;
                return Json(res);
            }
        }
    }
}
