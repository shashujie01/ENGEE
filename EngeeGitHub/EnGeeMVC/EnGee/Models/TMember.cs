using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace EnGee.Models;

public partial class TMember


{

	public int MemberId { get; set; }
	[DisplayName("註冊帳號")]
	[Required(ErrorMessage = ("註冊帳號必填"))]


	public string Username { get; set; } = null!;
	[DisplayName("註冊密碼")]
	[Required(ErrorMessage = ("註冊密碼必填"))]
	[RegularExpression(@"^[a-zA-Z0-9]{5,10}$", ErrorMessage = ("密碼格式:英文數字共5-10字元"))]
	[StringLength(10, ErrorMessage = ("密碼格式:英文數字共5-10字元"), MinimumLength = 5)]

	public string Password { get; set; } = null!;
	[NotMapped] //不存至SQL
	[DisplayName("確認密碼")]
	[Required(ErrorMessage = ("確認密碼必填"))]
	[StringLength(10, ErrorMessage = ("密碼格式:英文數字共5-10字元"), MinimumLength = 5)]
	[Compare("Password", ErrorMessage = "兩組密碼必須相同")]
	public string RePassword { get; set; } = null!;
	[DisplayName("註冊信箱(驗證)")]
	[Required(ErrorMessage = ("註冊信箱必填"))]
	[RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "請輸入有效的電子郵件地址")]

	public string Email { get; set; } = null!;
	[DisplayName("全名")]
	[Required(ErrorMessage = ("全名必填"))]
	public string Fullname { get; set; } = null!;


	[Required(ErrorMessage = ("性別必填"))]
	public int Gender { get; set; }


	[DisplayName("通訊地址")]
	[Required(ErrorMessage = ("通訊地址必填"))]
	public string Address { get; set; } = null!;
	[DisplayName("通訊電話")]
	[Required(ErrorMessage = ("手機號碼必填"))]
	[StringLength(10, ErrorMessage = ("手機號碼10位/電話號碼9位"), MinimumLength = 9)]

	public string Phone { get; set; } = null!;

	[DisplayName("註冊日期")]
	[DataType(DataType.Date)]
	[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
	public DateTime? RegistrationDate { get; set; }
	[DisplayName("出生日期")]
	[Required(ErrorMessage = ("出生日期必填"))]
	[DataType(DataType.Date)]
	[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
	public DateTime Birth { get; set; } = DateTime.MinValue;
	[DisplayName("權限")]
	public int? Access { get; set; }
	[DisplayName("點數")]
	public double? Point { get; set; }
	[DisplayName("會員照片")]
	public string? PhotoPath { get; set; }
	[DisplayName("會員自介")]
	public string? Introduction { get; set; }
	[DisplayName("公益團體證明(文件上傳)")]

	//--0916新增--//
	[NotMapped]
	public string? RandomToken { get; set; }
	public string? CharityProof { get; set; }
	[DisplayName("公益團體認證")]
	public bool? IsValidCharity { get; set; }




	public virtual ICollection<TCase> TCases { get; set; } = new List<TCase>();

	public virtual ICollection<TCollect> TCollects { get; set; } = new List<TCollect>();

	public virtual ICollection<TDemand> TDemandDemanders { get; set; } = new List<TDemand>();

	public virtual ICollection<TDemand> TDemandGivers { get; set; } = new List<TDemand>();

	public virtual ICollection<TDonationOrder> TDonationOrders { get; set; } = new List<TDonationOrder>();

	public virtual ICollection<TMemberFavorite> TMemberFavorites { get; set; } = new List<TMemberFavorite>();

	public virtual ICollection<TMemberPoint> TMemberPoints { get; set; } = new List<TMemberPoint>();

	public virtual ICollection<TMessage> TMessages { get; set; } = new List<TMessage>();

	public virtual ICollection<TProduct> TProducts { get; set; } = new List<TProduct>();

	public virtual ICollection<TRating> TRatingBuyers { get; set; } = new List<TRating>();

	public virtual ICollection<TRating> TRatingSellers { get; set; } = new List<TRating>();

	public virtual ICollection<TShoppingCart> TShoppingCarts { get; set; } = new List<TShoppingCart>();
}