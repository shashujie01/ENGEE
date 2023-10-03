using EnGee.Data;
using Microsoft.AspNetCore.Mvc;
using EnGee.Models;
using EnGee.ViewModels;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using System.Globalization;
using static EnGee.Models.TMember;
using Microsoft.AspNetCore.Components.Web;

namespace EnGee.Controllers
{
	public class Min_backmanageController : Controller
	{
		public IActionResult List(CkeywordViewModel vm, int? minPoint, int? maxPoint, int? accessFilter, int? txtkeywordfor, int? genderFilter)  //把預設int genderFilter=2拿掉
		{
			// 建立 EngeeContext 物件，用於與資料庫進行交互
			EngeeContext db = new EngeeContext();

			// 建立查詢，初始為 TMembers 資料表的所有資料//AsQueryable() 方法將 TMembers 轉換為 IQueryable<TMember> 介面，並將結果賦值給 query 變數
			IQueryable<TMember> query = db.TMembers.AsQueryable();


			// 根據選擇的搜尋對象進行不同的搜尋
			if (txtkeywordfor.HasValue && !string.IsNullOrEmpty(vm.txtKeyword))
			{
				string keyword = vm.txtKeyword;  //為了datetime轉換使用的變數

				switch (txtkeywordfor.Value)
				{
					case 0: // 帳號
						query = query.Where(t => t.Username.Contains(vm.txtKeyword));
						break;
					case 1: // 信箱
						query = query.Where(t => t.Email.Contains(vm.txtKeyword));
						break;
					case 2: // 全名
						query = query.Where(t => t.Fullname.Contains(vm.txtKeyword));
						break;
					case 3: // 地址
						query = query.Where(t => t.Address.Contains(vm.txtKeyword));
						break;
					case 4: // 電話
						query = query.Where(t => t.Phone.Contains(vm.txtKeyword));
						break;

					case 5: // 註冊時間//無法將 ToString 方法直接用於 LINQ 查詢//並分別比較日期的年、月、日部分，來進行查詢

						DateTime keywordDate;

						if (DateTime.TryParseExact(keyword, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out keywordDate))
						{
							query = query.Where(t =>
							t.RegistrationDate.HasValue &&
							t.RegistrationDate.Value.Date == keywordDate.Date);
						}
						else if (DateTime.TryParseExact(keyword, "yyyyMM", CultureInfo.InvariantCulture, DateTimeStyles.None, out keywordDate))
						{
							query = query.Where(t =>
							t.RegistrationDate.HasValue &&
							t.RegistrationDate.Value.Year == keywordDate.Year &&
							t.RegistrationDate.Value.Month == keywordDate.Month);
						}
						else if (DateTime.TryParseExact(keyword, "MMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out keywordDate))
						{
							Console.WriteLine("Parsed month: " + keywordDate.Month);
							Console.WriteLine("Parsed day: " + keywordDate.Day);

							query = query.Where(t =>
							t.RegistrationDate.HasValue &&
							t.RegistrationDate.Value.Month == keywordDate.Month &&
							t.RegistrationDate.Value.Day == keywordDate.Day);
						}
						else if (DateTime.TryParseExact(keyword, "yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out keywordDate))
						{
							query = query.Where(t =>
							t.RegistrationDate.HasValue &&
							t.RegistrationDate.Value.Year == keywordDate.Year);
						}

						break;

					case 6: // 生日
							// 將 DateTime 轉換為 yyyymmdd 格式並進行比較
						query = query.Where(t => t.Birth.ToString("yyyyMMdd").Contains(keyword));
						break;

				}
			}

			//加入Gender篩選條件
			if (genderFilter.HasValue)
			{
				query = query.Where(t => t.Gender == genderFilter);          //原本錯誤修正如下
			}

			// 加入Access篩選條件，只有當 accessFilter 有值時才進行篩選
			if (accessFilter.HasValue)
			{
				query = query.Where(t => t.Access == accessFilter.Value);
			}


			// 加入Point篩選條件
			if (minPoint.HasValue)
			{
				query = query.Where(t => t.Point >= minPoint.Value);
			}

			if (maxPoint.HasValue)
			{
				query = query.Where(t => t.Point <= maxPoint.Value);
			}

			// 將最終查詢的結果轉為 IEnumerable<TMember> 並存儲在 datas 變數中
			IEnumerable<TMember> datas = query.ToList();

			// 將資料傳遞給 View 並返回相應的 View
			return View(datas);
		}


		public IActionResult Delete(int? id)
		{
			if (id == null)
			{
				return RedirectToAction("List");
			}
			else
			{
				using (var db = new EngeeContext())
				{
					var checkdata = db.TMembers.FirstOrDefault(t => t.MemberId == id);
					if (checkdata != null)
					{
						db.TMembers.Remove(checkdata);
						db.SaveChanges();
					}
					return RedirectToAction("List");
				}
			}
			return View();
		}

		public IActionResult Edit(int? id)
		{
			if (id == null)
			{
				return RedirectToAction("List");
			}
			else
			{
				using (var db = new EngeeContext())
				{
					var checkDbid = db.TMembers.FirstOrDefault(t => t.MemberId == id);
					if (checkDbid != null)
					{
						return View(checkDbid);
					}
					return RedirectToAction("List");
				}
			}
		}

		[HttpPost]
		public IActionResult Edit(TMember model, bool IsValidCharity)
		{
			model.IsValidCharity = IsValidCharity;
			if (IsValidCharity == true)
			{
				model.Access = 3;
			}
			else if (IsValidCharity != true)
			{
				model.Access = 1;
			}

			using (var db = new EngeeContext())
			{
				var checkDbId = db.TMembers.FirstOrDefault(t => t.MemberId == model.MemberId);
				if (checkDbId != null)
				{

					checkDbId.Username = model.Username;
					checkDbId.Phone = model.Phone;
					checkDbId.Email = model.Email;
					checkDbId.Fullname = model.Fullname;
					checkDbId.Gender = model.Gender;
					checkDbId.Address = model.Address;
					checkDbId.Birth = model.Birth;
					checkDbId.IsValidCharity = model.IsValidCharity;
					checkDbId.Access = model.Access;
				}


				db.SaveChanges();

			}
			return RedirectToAction("List");
		}

	}
}