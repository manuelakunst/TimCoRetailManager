using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRMDesktopUI.ViewModels
{
	public class SalesViewModel : Screen
	{
		private BindingList<string> _products;

		public BindingList<string> Products
		{
			get { return _products; }
			set
			{
				_products = value;
				NotifyOfPropertyChange(() => Products);
			}
		}

		private BindingList<string> _cart;

		public BindingList<string> Cart
		{
			get { return _cart; }
			set
			{
				_cart = value;
				NotifyOfPropertyChange(() => Cart);
			}
		}
		
		private string _itemQty;

		public string ItemQuantity
		{
			get { return _itemQty; }
			set
			{
				_itemQty = value;
				NotifyOfPropertyChange(() => ItemQuantity);
			}
		}

		public string SubTotal
		{
			get
			{
				return "$0.00";
			}
		}
		public string Tax
		{
			get
			{
				return "$0.00";
			}
		}
		public string Total
		{
			get
			{
				return "$0.00";
			}
		}

		public bool CanAddToCart
		{
			get
			{
				var output = false;

				// check, if something is selected and quantity > 0
				if (true)
					output = true;
				return output;
			}
		}

		public void AddToCart()
		{ }

		public bool CanRemoveFromCart
		{
			get
			{
				var output = false;

				// check, if something is selected in cart
				if (true)
					output = true;
				return output;
			}
		}

		public void RemoveFromCart()
		{

		}

		public bool CanCheckOut
		{
			get
			{
				var output = false;

				// make sure, something is in the cart ???
				if (true)
					output = true;
				return output;
			}
		}

		public void CheckOut()
		{

		}
	}
}
