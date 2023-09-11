﻿namespace EnGee.ViewModels
{
    public class Rong_CollectIndexViewModel
    {
        public int CollectId { get; set; }
        public bool CollectStatus { get; set; }

        public string CollectTitle { get; set; }
        public string CollectStartDate { get; set; }
        public string CollectEndDate { get; set; }

        public string? CollectImagePath { get; set; }

        //  from 會員資料表 取得名稱
        public string Username { get; set; }

        public int MainCategoryId { get; set; }
        public int SubcategoryId { get; set; }


    }
}
