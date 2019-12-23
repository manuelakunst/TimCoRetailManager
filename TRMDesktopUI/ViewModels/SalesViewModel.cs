using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Helper;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
	public class SalesViewModel : Screen
	{
		private readonly IProductEndpoint _productEndpoint;
		private readonly ISaleEndpoint _saleEndpoint;
		private IConfigHelper _configHelper;

		public SalesViewModel(IProductEndpoint productEndpoint, IConfigHelper configHelper, ISaleEndpoint saleEndpoint)
		{
			_productEndpoint = productEndpoint;
			_saleEndpoint = saleEndpoint;
			_configHelper = configHelper;
		}

		protected override async void OnViewLoaded(object view)
		{
			base.OnViewLoaded(view);
			await LoadProducts();
		}

		private async Task LoadProducts()
		{
			var productList = await _productEndpoint.GetAll();
			Products = new BindingList<ProductModel>(productList);
		}

		private BindingList<ProductModel> _products;

		public BindingList<ProductModel> Products
		{
			get { return _products; }
			set
			{
				_products = value;
				NotifyOfPropertyChange(() => Products);
			}
		}

		private ProductModel _selectedProduct;

		public ProductModel SelectedProduct
		{
			get { return _selectedProduct; }
			set
			{
				_selectedProduct = value;
				NotifyOfPropertyChange(() => SelectedProduct);
				NotifyOfPropertyChange(() => CanAddToCart);
			}
		}


		private BindingList<CartItemModel> _cart = new BindingList<CartItemModel>();

		public BindingList<CartItemModel> Cart
		{
			get { return _cart; }
			set
			{
				_cart = value;
				NotifyOfPropertyChange(() => Cart);
			}
		}

		private int _itemQty = 1;

		public int ItemQuantity
		{
			get { return _itemQty; }
			set
			{
				_itemQty = value;
				NotifyOfPropertyChange(() => ItemQuantity);
				NotifyOfPropertyChange(() => CanAddToCart);
			}
		}

		public string SubTotal
		{
			get
			{
				return CalculateSubTotal().ToString("C");
			}
		}

		private decimal CalculateSubTotal()
		{
			decimal subTotal = 0;
			foreach (var item in Cart)
			{
				subTotal += item.Product.RetailPrice * item.QuantityInCart;
			}
			return subTotal;
		}

		public string Tax
		{
			get
			{
				return CalculateTax().ToString("C");
			}
		}

		private decimal CalculateTax()
		{
			decimal taxAmount = 0;
			var taxRate = _configHelper.GetTaxRate();

			taxAmount = Cart
						.Where(x => x.Product.IsTaxable)
						.Sum(x => x.Product.RetailPrice * x.QuantityInCart * taxRate);

			//foreach (var item in Cart)
			//{
			//	if (item.Product.IsTaxable)
			//		taxAmount += item.Product.RetailPrice * item.QuantityInCart * taxRate / 100;
			//}

			return taxAmount;
		}

		public string Total
		{
			get
			{
				return (CalculateSubTotal() + CalculateTax()).ToString("C");
			}
		}

		public bool CanAddToCart
		{
			get
			{
				var output = false;
				if (ItemQuantity > 0)
				{
					if (SelectedProduct?.QuantityInStock >= ItemQuantity)
						output = true;
				}
				return output;
			}
		}

		public void AddToCart()
		{
			var existingItemInCart = Cart.FirstOrDefault(x => x.Product == SelectedProduct);
			if (existingItemInCart == null)
			{
				var item = new CartItemModel
				{
					Product = SelectedProduct,
					QuantityInCart = ItemQuantity
				};
				Cart.Add(item);
			}
			else
			{
				existingItemInCart.QuantityInCart += ItemQuantity;

				// HACK: update the DisplayText of existing item in Cart
				Cart.Remove(existingItemInCart);
				Cart.Add(existingItemInCart);
			}

			SelectedProduct.QuantityInStock -= ItemQuantity;
			ItemQuantity = 1;

			NotifyOfPropertyChange(() => SubTotal);
			NotifyOfPropertyChange(() => Tax);
			NotifyOfPropertyChange(() => Total);
			NotifyOfPropertyChange(() => Cart);
			NotifyOfPropertyChange(() => CanCheckOut);
		}

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
			NotifyOfPropertyChange(() => SubTotal);
			NotifyOfPropertyChange(() => Tax);
			NotifyOfPropertyChange(() => Total);
			NotifyOfPropertyChange(() => Cart);
			NotifyOfPropertyChange(() => CanCheckOut);
		}

		public bool CanCheckOut
		{
			get
			{
				var output = false;
				if (Cart.Count > 0)
					output = true;
				return output;
			}
		}

		public async Task CheckOut()
		{
			var sale = new SaleModel();
			foreach (var item in Cart)
			{
				sale.SaleDetails.Add(new SaleDetailsModel
				{
					ProductId = item.Product.Id,
					Quantity = item.QuantityInCart
				});
			}

			// all to the API
			try {
				IsProcessing = true;
				await _saleEndpoint.PostSale(sale);
			}
			catch (Exception ex)
			{ 
			}
			finally 
			{
				IsProcessing = false;
			}
		}

		private bool _isProcessing = false;
		public bool IsProcessing
		{
			get { return _isProcessing; }
			set
			{
				_isProcessing = value;
				NotifyOfPropertyChange(() => IsProcessing);
			}
		}
	}
}
