namespace EnGee.ViewModel
{
    public class SSJ_ShoppingListCombinedViewModel
    {
        public IEnumerable<SSJ_ShoppingListOrderViewModel> Orders { get; set; }
        public IEnumerable<SSJ_ShoppingListOrderDetailViewModel> OrderDetails { get; set; }
    }
}

