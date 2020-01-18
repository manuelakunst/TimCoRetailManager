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

		private UserModel _selectedUser;

		public UserModel SelectedUser
		{
			get { return _selectedUser; }
			set
			{
				_selectedUser = value;
				SelectedUserName = value.Email;

				UserRoles.Clear();
				var selRoles = value.Roles.Select(x => x.Value).ToList();
				UserRoles = new BindingList<string>(selRoles);
				LoadRoles();

				NotifyOfPropertyChange(() => SelectedUser);
			}
		}

		private string _selectedUserName;

		public string SelectedUserName
		{
			get { return _selectedUserName; }
			set
			{
				_selectedUserName = value;
				NotifyOfPropertyChange(() => SelectedUserName);
			}
		}

		private BindingList<string> _userRoles = new BindingList<string>();

		public BindingList<string> UserRoles
		{
			get { return _userRoles; }
			set
			{
				_userRoles = value;
				NotifyOfPropertyChange(() => UserRoles);
			}
		}

		private BindingList<string> _availableRoles = new BindingList<string>();

		public BindingList<string> AvailableRoles
		{
			get { return _availableRoles; }
			set
			{
				_availableRoles = value;
				NotifyOfPropertyChange(() => AvailableRoles);
			}
		}

		private string _selectedUserRole;

		public string SelectedUserRole
		{
			get { return _selectedUserRole; }
			set
			{
				_selectedUserRole = value;
				NotifyOfPropertyChange(() => SelectedUserRole);
				NotifyOfPropertyChange(() => AvailableRoles);
			}
		}

		private string _selectedRoleToAdd;

		public string SelectedAvailableRole
		{
			get { return _selectedRoleToAdd; }
			set
			{
				_selectedRoleToAdd = value;
				NotifyOfPropertyChange(() => SelectedAvailableRole);
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
					await _window.ShowDialogAsync(_status, null, settings);
				}
				else
				{
					_status.UpdateMessage("Fatal Exception", ex.Message);
					await _window.ShowDialogAsync(_status, null, settings);
				}

				await TryCloseAsync();
			}
		}

		private async Task LoadUsers()
		{
			var userList = await _userEndpoint.GetAllUsers();
			Users = new BindingList<UserModel>(userList);
		}

		private async Task LoadRoles()
		{
			var roles = await _userEndpoint.GetAllRoles();

			foreach(var role in roles)
			{
				if(!UserRoles.Contains(role.Value))
				{
					AvailableRoles.Add(role.Value);
				}
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

		public async void AddSelectedRole()
		{
			try
			{
				IsProcessing = true;
				await _userEndpoint.AddUserToRole(SelectedUser.Id, SelectedAvailableRole);

				UserRoles.Add(SelectedAvailableRole);
				AvailableRoles.Remove(SelectedAvailableRole);
			}
			catch (Exception)
			{
				throw;
			}
			finally
			{
				IsProcessing = false;
			}
		}

		public async void RemoveSelectedRole()
		{
			try
			{
				IsProcessing = true;
				await _userEndpoint.RemoveUserFromRole(SelectedUser.Id, SelectedUserRole);

				var selRole = SelectedUserRole;
				UserRoles.Remove(selRole);
				AvailableRoles.Add(selRole);
			}
			catch (Exception)
			{
				throw;
			}
			finally
			{
				IsProcessing = false;
			}
		}
	}
}
