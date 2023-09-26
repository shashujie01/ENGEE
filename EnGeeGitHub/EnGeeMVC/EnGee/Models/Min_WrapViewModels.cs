using EnGee.Models;
using Microsoft.Identity.Client;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EnGee.Models
{
    public class Min_WrapViewModels
    {
        public class Favor
        {
            public int FavorId { get; set; }
            public int ProductId { get; set; }

            public DateTime AddFavorDate { get; set; }

            public string ProductImagePath { get; set; }


            //===================================//
            public TMember tmemberjoin { get; set; }

            public TProduct productjoin { get; set; }


        }
    }
}
