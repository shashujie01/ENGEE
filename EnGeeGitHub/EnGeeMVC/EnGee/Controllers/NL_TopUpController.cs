using EnGee.Models;
using Microsoft.AspNetCore.Mvc;
using prjMvcCoreDemo.Models;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Json;
using EnGee.ViewModels;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using static System.Net.WebRequestMethods;

namespace EnGee.Controllers
{
    public class NL_TopUpController : SuperController
    {
        private readonly IWebHostEnvironment _enviro;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
       
        public NL_TopUpController(IConfiguration configuration,
            IWebHostEnvironment p,
            CHI_CUserViewModel userViewModel,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor)
            : base(userViewModel)
        {
            _enviro = p;
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
            _httpClient.DefaultRequestHeaders.Add("X-LINE-ChannelId", "channelId");
            _httpClient.DefaultRequestHeaders.Add("X-LINE-ChannelSecret", "channelSecret");           
        }

        private TMember GetLoggedInUser()
        {
            string userJson = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            return JsonSerializer.Deserialize<TMember>(userJson);
        }

        public IActionResult Index()
        {
            EngeeContext db = new EngeeContext();
            TMember loggedInUser = GetLoggedInUser();

            TMember userFromDatabase = db.TMembers.FirstOrDefault(t => t.Email.Equals(loggedInUser.Email));
            if (userFromDatabase != null)
            {
                CHI_CMemberWrap memberWrap = new CHI_CMemberWrap
                {
                    member = userFromDatabase
                };       
                return View(memberWrap);
            }
            return View();
        }

        public async Task<IActionResult> Payment(decimal amount)
        {
            using (var client = new HttpClient())
            {
                var channelId = _configuration["LinePay:ChannelId"];
                var channelSecret = _configuration["LinePay:ChannelSecret"];
                var orderId = DateTime.Now.ToString("yyyyMMddHHmmssfff");                
                var orderData = new
                {
                    productName = "儲值",
                    productImageUrl = "https://i.imgur.com/9gxZy1W.png/84*84",
                    amount = amount,
                    currency = "TWD",
                    confirmUrl = "https://engee2023.azurewebsites.net/NL_TopUp/ConfirmPayment",                    
                    orderId = orderId
                };

                var requestJson = JsonSerializer.Serialize(orderData);

                var httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
                httpContent.Headers.Add("X-LINE-ChannelId", channelId);
                httpContent.Headers.Add("X-LINE-ChannelSecret", channelSecret);

                var response = await client.PostAsync("https://sandbox-api-pay.line.me/v2/payments/request", httpContent);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var item = JsonSerializer.Deserialize<Request_Rootobject>(result);

                    if (item.returnCode == "0000")
                    {
                        TempData["Amount"] = amount.ToString();
                        var paymentUrl = item.info.paymentUrl.web;
                        return Redirect(paymentUrl);  // 直接導向LINE Pay付款頁面
                    }
                    else
                    {
                        // 失敗的處理，您可以在這裡添加日誌、錯誤消息等
                    }
                }
            }
            return View("Error");
        }


        public async Task<IActionResult> ConfirmPayment([FromQuery] string transactionId)
        {
            using (var client = new HttpClient())
            {
                var channelId = _configuration["LinePay:ChannelId"];
                var channelSecret = _configuration["LinePay:ChannelSecret"];
                int retrievedAmount = Convert.ToInt32(decimal.Parse(TempData["Amount"].ToString()));

                var confirmData = new
                {
                    amount = retrievedAmount,
                    currency = "TWD"

                };

                var requestJson = JsonSerializer.Serialize(confirmData);

                var httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
                httpContent.Headers.Add("X-LINE-ChannelId", channelId);
                httpContent.Headers.Add("X-LINE-ChannelSecret", channelSecret);

                var response = await client.PostAsync("https://sandbox-api-pay.line.me/v2/payments/" + transactionId + "/confirm", httpContent);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var item = JsonSerializer.Deserialize<ConfirmResponse>(result);

                    Console.WriteLine("API Response: " + result);  // 輸出API的完整響應到控制台

                    if (item.returnCode == "0000")
                    {
                        // 根據API的回應更新您的資料庫
                        // 成功，更新會員點數                        
                        var loggedInUser = GetLoggedInUser();
                        var db = new EngeeContext();

                        var userFromDatabase = db.TMembers.FirstOrDefault(t => t.Email.Equals(loggedInUser.Email));

                        if (userFromDatabase != null)
                        {
                            // 更新點數
                            userFromDatabase.Point += retrievedAmount;
                            db.SaveChanges();
                            Console.WriteLine("付款成功!");
                        }
                    }
                    else
                    {
                        // 處理API返回的錯誤
                        Console.WriteLine($"付款失敗: {item.returnMessage}");
                    }
                }
                else
                {
                    // 處理HTTP請求的錯誤
                    Console.WriteLine("HTTP請求錯誤: " + response.StatusCode);  // 輸出HTTP的錯誤狀態碼
                }

            }            
            // 3. 重定向到確認頁面
            return RedirectToAction("Index");
        }
    }
}
