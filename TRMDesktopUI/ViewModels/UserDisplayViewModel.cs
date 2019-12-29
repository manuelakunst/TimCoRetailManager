using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
    public class UserDisplayViewModel : Screen
    {
		private readonly IWindowManager _window;
		private readonly IUserEndpoint _userEndpoint;
		private readonly StatusInfoViewModel _status;

		private BindingList<UserModel> _users;

		public BindingList<UserModel> Users
		{
			get { return _users; }
			set 
			{
				_users = value;
				NotifyOfPropertyChange(() => Users);
			}
		}


		public UserDisplayViewModel(IUserEndpoint userEndpoint, IWindowManager window, StatusInfoViewModel status)
		{
			_userEndpoint = userEndpoint;
			_window = window;
			_status = status;

		}

		protected override async void OnViewLoaded(object view)
		{
			base.OnViewLoaded(view);
			try
			{
				await LoadUsers();
			}
			catch (Exception ex)
			{
				dynamic settings = new ExpandoObject();
				settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
				settings.ResizeMode = ResizeMode.NoResize;
				settings.Title = "System Error";

				if (ex.Message.Equals("Unauthorized"))
				{
					_status.UpdateMessage("Unauthorized Access", "You do not have permission to interact with the Sales Form.");
					_window.ShowDialog(_status, null, settings);
				}
				else
				{
					_status.UpdateMessage("Fatal Exception", ex.Message);
					_window.ShowDialog(_status, null, settings);
				}

				TryClose();
			}
		}

		private async Task LoadUsers()
		{
			var userList = await _userEndpoint.GetUsers();
			Users = new BindingList<UserModel>(userList);
		}

	}
}
