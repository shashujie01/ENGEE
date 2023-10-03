using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnGee.Hubs
{
    public class ChatHub : Hub
    {
        // 用戶 ID 字典，用於存儲用戶自定義的 ID
        public static Dictionary<string, string> UserIdDictionary = new Dictionary<string, string>();
        public static HashSet<string> UsedUserNames = new HashSet<string>();

        /// <summary>
        /// 連線事件
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            // 提示使用者輸入唯一的 ID
            await Clients.Client(Context.ConnectionId).SendAsync("PromptForUserId");

            await base.OnConnectedAsync();
        }

        /// <summary>
        /// 設定使用者的 ID
        /// </summary>
        public async Task SetUserId(string userId)
        {
            if (!UsedUserNames.Contains(userId))
            {
                UsedUserNames.Add(userId);
                UserIdDictionary[Context.ConnectionId] = userId;

                // 更新使用者的 ID
                await Clients.Client(Context.ConnectionId).SendAsync("UpdSelfID", userId);

                // 更新使用者 ID 列表
                string jsonString = JsonConvert.SerializeObject(UserIdDictionary.Values);
                await Clients.All.SendAsync("UpdList", jsonString);

                // 通知使用者已加入
                await Clients.All.SendAsync("UpdContent", "👋 " + userId + " 進入聊天室");
            }
            else
            {
                // 通知使用者所選擇的 ID 已經被使用
                await Clients.Client(Context.ConnectionId).SendAsync("UserIdInUse");
            }
        }

        /// <summary>
        /// 連線事件
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception ex)
        {
            if (UserIdDictionary.ContainsKey(Context.ConnectionId))
            {
                string userId = UserIdDictionary[Context.ConnectionId];
                UserIdDictionary.Remove(Context.ConnectionId);

                // 釋放使用者名稱
                UsedUserNames.Remove(userId);

                // 更新使用者 ID 列表
                string jsonString = JsonConvert.SerializeObject(UserIdDictionary.Values);
                await Clients.All.SendAsync("UpdList", jsonString);

                // 通知使用者已離開
                await Clients.All.SendAsync("UpdContent", "😭 " + userId + " 離開聊天室");
            }

            await base.OnDisconnectedAsync(ex);
        }

        /// <summary>
        /// 傳遞訊息
        /// </summary>
        public async Task SendMessage(string selfID, string message, string sendToID)
        {
            if (string.IsNullOrEmpty(sendToID))
            {
                await Clients.All.SendAsync("UpdContent", selfID + " 說：" + message);
            }
            else
            {
                // 檢查目標使用者是否存在
                if (UserIdDictionary.ContainsValue(sendToID))
                {
                    // 獲取目標使用者的連線 ID
                    var targetConnectionId = UserIdDictionary.FirstOrDefault(x => x.Value == sendToID).Key;

                    // 將訊息發送給目標使用者
                    await Clients.Client(targetConnectionId).SendAsync("UpdContent", selfID + " 私訊向你說：" + message);

                    // 向發送者發送確認訊息
                    await Clients.Client(Context.ConnectionId).SendAsync("UpdContent", "你向 " + sendToID + " 私訊說：" + message);
                }
                else
                {
                    // 通知發送者目標使用者不存在
                    await Clients.Client(Context.ConnectionId).SendAsync("UserNotFound");
                }
            }
        }
    }
}