using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EnGee.Models;

public partial class TMember
{
    public int MemberId { get; set; }
    [DisplayName("註冊帳號")]
    [Required(ErrorMessage = ("註冊帳號必填"))]
    [StringLength(10, ErrorMessage = ("帳號格式:英文數字共5-10字元"), MinimumLength = 5)]
    public string Username { get; set; } = null!;
    [DisplayName("註冊密碼")]
    [Required(ErrorMessage = ("註冊密碼必填"))]
    [StringLength(10, ErrorMessage = ("密碼格式:英文數字共5-10字元"), MinimumLength = 5)]
    public string Password { get; set; } = null!;
    [DisplayName("註冊信箱(驗證)")]
    [DataType(DataType.EmailAddress, ErrorMessage = ("信箱格式不符合"))]
    [Required(ErrorMessage = ("註冊信箱必填"))]
    public string Email { get; set; } = null!;
    [DisplayName("名字全名")]
    public string? Fullname { get; set; }
   
    [DisplayName("性別")]
    public string? Gender { get; set; }
    [DisplayName("通訊地址")]
    [Required(ErrorMessage = ("通訊地址必填"))]
    public string Address { get; set; } = null!;
    [DisplayName("手機號碼")]
    [Required(ErrorMessage = ("手機號碼必填"))]
    [StringLength(10, ErrorMessage = ("手機位數共10位:09xxxxxxxx"), MinimumLength = 10)]

    public string Phone { get; set; } = null!;
    [DisplayName("註冊日期")]
    public DateTime? RegistrationDate { get; set; }
    [DisplayName("出生日期")]
    public DateTime? Birth { get; set; }
    [DisplayName("權限")]
    public int? Access { get; set; }
    [DisplayName("點數")]
    public double? Point { get; set; }
    [DisplayName("會員照片")]
    public string? PhotoPath { get; set; }
    [DisplayName("會員自介")]
    public string? Introduction { get; set; }
    [DisplayName("公益團體證明(文件上傳)")]
    public string? CharityProof { get; set; }

    public virtual ICollection<TCase> TCases { get; set; } = new List<TCase>();

    public virtual ICollection<TCollect> TCollects { get; set; } = new List<TCollect>();

    public virtual ICollection<TDemand> TDemandDemanders { get; set; } = new List<TDemand>();

    public virtual ICollection<TDemand> TDemandGivers { get; set; } = new List<TDemand>();

    public virtual ICollection<TDonationOrder> TDonationOrders { get; set; } = new List<TDonationOrder>();

    public virtual ICollection<TMemberFavorite> TMemberFavorites { get; set; } = new List<TMemberFavorite>();

    public virtual ICollection<TMemberPoint> TMemberPoints { get; set; } = new List<TMemberPoint>();

    public virtual ICollection<TMessage> TMessages { get; set; } = new List<TMessage>();

    public virtual ICollection<TOrder> TOrderBuyers { get; set; } = new List<TOrder>();

    public virtual ICollection<TOrderDetail> TOrderDetailBuyers { get; set; } = new List<TOrderDetail>();

    public virtual ICollection<TOrderDetail> TOrderDetailSellers { get; set; } = new List<TOrderDetail>();

    public virtual ICollection<TOrder> TOrderSellers { get; set; } = new List<TOrder>();

    public virtual ICollection<TProduct> TProducts { get; set; } = new List<TProduct>();

    public virtual ICollection<TRating> TRatingBuyers { get; set; } = new List<TRating>();

    public virtual ICollection<TRating> TRatingSellers { get; set; } = new List<TRating>();

    public virtual ICollection<TShoppingCart> TShoppingCarts { get; set; } = new List<TShoppingCart>();
}
