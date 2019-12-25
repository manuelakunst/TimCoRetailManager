using AutoMapper;
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
using TRMDesktopUI.Models;

namespace TRMDesktopUI.ViewModels
{
	public class SalesViewModel : Screen
	{
		private readonly IProductEndpoint _productEndpoint;
		private readonly ISaleEndpoint _saleEndpoint;
		private IConfigHelper _configHelper;
		private IMapper _mapper;

		public SalesViewModel(IProductEndpoint productEndpoint, IConfigHelper configHelper, 
			ISaleEndpoint saleEndpoint, IMapper mapper)
		{
			_productEndpoint = productEndpoint;
			_saleEndpoint = saleEndpoint;
			_configHelper = configHelper;
			_mapper = mapper;
		}

		protected override async void OnViewLoaded(object view)
		{
			base.OnViewLoaded(view);
			await LoadProducts();
		}

		private async Task LoadProducts()
		{
			var productList = await _productEndpoint.GetAll();
			var productDisplayList = _mapper.Map<List<ProductDisplayModel>>(productList);
			Products = new BindingList<ProductDisplayModel>(productDisplayList);
		}

		private BindingList<ProductDisplayModel> _products;

		public BindingList<ProductDisplayModel> Products
		{
			get { return _products; }
			set
			{
				_products = value;
				NotifyOfPropertyChange(() => Products);
			}
		}

		private ProductDisplayModel _selectedProduct;

		public ProductDisplayModel SelectedProduct
		{
			get { return _selectedProduct; }
			set
			{
				_selectedProduct = value;
				NotifyOfPropertyChange(() => SelectedProduct);
				NotifyOfPropertyChange(() => CanAddToCart);
			}
		}

		private CartItemDisplayModel _selectedCartItem;
		public CartItemDisplayModel SelectedCartItem
		{
			get { return _selectedCartItem; }
			set
			{
				_selectedCartItem = value;
				NotifyOfPropertyChange(() => SelectedCartItem);
				NotifyOfPropertyChange(() => CanRemoveFromCart);
			}
		}


		private BindingList<CartItemDisplayModel> _cart = new BindingList<CartItemDisplayModel>();

		public BindingList<CartItemDisplayModel> Cart
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
				var item = new CartItemDisplayModel
				{
					Product = SelectedProduct,
					QuantityInCart = ItemQuantity
				};
				Cart.Add(item);
			}
			else
			{
				existingItemInCart.QuantityInCart += ItemQuantity;

				//// HACK: update the DisplayText of existing item in Cart
				//Cart.Remove(existingItemInCart);
				//Cart.Add(existingItemInCart);
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

				if (SelectedCartItem != null && SelectedCartItem.QuantityInCart > 0)
					output = true;
				return output;
			}
		}

		public void RemoveFromCart()
		{
			SelectedCartItem.Product.QuantityInStock += 1;
			if (SelectedCartItem.QuantityInCart > 1)
			{
				// reduce Qty by 1
				SelectedCartItem.QuantityInCart -= 1;
			}
			else
			{
				Cart.Remove(SelectedCartItem);
			}

			NotifyOfPropertyChange(() => SubTotal);
			NotifyOfPropertyChange(() => Tax);
			NotifyOfPropertyChange(() => Total);
			NotifyOfPropertyChange(() => Cart);
			NotifyOfPropertyChange(() => CanAddToCart);
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
