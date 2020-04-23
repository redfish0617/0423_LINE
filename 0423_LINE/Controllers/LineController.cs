using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using _0423_LINE.Models;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace _0423_LINE.Controllers
{
    public class LineController : Controller
    {
        public ActionResult Index()
        {
            //取得LineToken
            HttpCookie cookie = Request.Cookies["LineToken"];
            //如果沒有token 先取得自己Notify的資料
            if (cookie == null || String.IsNullOrEmpty(cookie.Value))
            {
                ViewBag.Title = "Line Notify";
                LineAuthorizeModel model = new LineAuthorizeModel()
                {
                    response_type = "code",
                    client_id = ConfigurationManager.AppSettings["ClientID"],
                    redirect_uri = ConfigurationManager.AppSettings["RedirectUri"],
                    scope = "notify",
                    state = Guid.NewGuid().ToString(),
                    responsemode = "form_post"
                };
                HttpCookie cookieState = new HttpCookie("State", model.state);
                Response.Cookies.Add(cookieState);
                return View(model);
            } 
            //有token的話 前往Notify功能
            return RedirectToAction("Notification", "Line");
        }
        public ActionResult CodeReturn(string code = "", string state = "")
        {
            //取得Code後要建立Token資料
            HttpCookie cookie = Request.Cookies["State"];
            if (state.Equals(cookie.Value.ToString()))
            {
                LineTokenModel tokenModel = new LineTokenModel()
                {
                    code= code,
                    client_id = ConfigurationManager.AppSettings["ClientID"],
                    client_secret = ConfigurationManager.AppSettings["ClientSecret"],
                    redirect_uri = ConfigurationManager.AppSettings["RedirectUri"],
                    grant_type = "authorization_code"
                };
                return View(tokenModel);
            }
            return Content("<H1>State不正確</H1>");
        }
        public ActionResult SendMessage(string Message ="")
        {
            
            HttpCookie cookie = Request.Cookies["LineToken"];
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://notify-api.line.me");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", cookie.Value);
                var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("message", Message) });
                using (HttpResponseMessage message = httpClient.PostAsync("/api/notify", content).Result)
                {
                    //傳送訊息
                    return new HttpStatusCodeResult(message.StatusCode);
                }           
            }
        }
        public ActionResult Notification(LineTokenModel m)
        {
            string result = "";
            string token = "";
            HttpCookie cookie = Request.Cookies["LineToken"];
            //如果沒取得LineToken的Code
            if (cookie == null || String.IsNullOrEmpty(cookie.Value))
            {
                using (HttpClient http = new HttpClient())
                {
                    //跟Line取得Token的code
                    http.BaseAddress = new Uri("https://notify-bot.line.me");
                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("grant_type", m.grant_type),
                        new KeyValuePair<string, string>("redirect_uri", m.redirect_uri),
                        new KeyValuePair<string, string>("code", m.code),
                        new KeyValuePair<string, string>("client_id", m.client_id),
                        new KeyValuePair<string, string>("client_secret", m.client_secret),
                    });
                    using(HttpResponseMessage message = http.PostAsync("/oauth/token", content).Result)
                    {
                        if (message.StatusCode == HttpStatusCode.OK)
                        {
                            result = message.Content.ReadAsStringAsync().Result;
                            token = JObject.Parse(result)["access_token"].ToString();
                        }
                        else
                        {
                            return View("Error");
                        }
                    }
                    cookie = new HttpCookie("LineToken", token);
                    Response.Cookies.Add(cookie);
                }
            }
            return View();
        }
    }
}