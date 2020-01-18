using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDesktopUI.EventModels;
using TRMDesktopUI.Helpers;
using TRMDesktopUI.Library.Api;

namespace TRMDesktopUI.ViewModels
{
    public class LoginViewModel : Screen
    {
		private string _userName;
		private string _password;
		private IAPIHelper _apiHelper;
		private IEventAggregator _events;

		public LoginViewModel(IAPIHelper apiHelper, IEventAggregator events)
		{
			_apiHelper = apiHelper;
			_events = events;

			InitTestUserdata();
		}

		private void InitTestUserdata()
		{
#if DEBUG
			var currentDir = Environment.CurrentDirectory;
			var topDir = currentDir.Replace("TRMDesktopUI\\bin\\Debug", "");
			string path = topDir + "myLoginForDebug.txt";

			// This text is added only once to the file.
			if (File.Exists(path))
			{
				string[] lines = File.ReadAllLines(path);
				UserName = lines[0];
				Password = lines[1];
			}
#endif
		}

		public string UserName
		{
			get { return _userName; }
			set
			{
				_userName = value;
				NotifyOfPropertyChange(() => UserName);
			}
		}

		public string Password
		{
			get { return _password; }
			set
			{
				_password = value;
				NotifyOfPropertyChange(() => Password);
				NotifyOfPropertyChange(() => CanLogIn);
			}
		}

		public bool IsErrorVisible
		{
			get 
			{
				var output = false;
				if (_errorMessage?.Length > 0)
					output = true;
				return output;
			}
		}

		private string _errorMessage;

		public string ErrorMessage
		{
			get { return _errorMessage; }
			set 
			{
				_errorMessage = value;
				NotifyOfPropertyChange(() => ErrorMessage);
				NotifyOfPropertyChange(() => IsErrorVisible);
			}
		}

		public bool CanLogIn
		{
			get
			{
				var output = false;
				if (UserName?.Length > 0 && Password?.Length > 0)
					output = true;
				return output;
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

		public async Task LogIn()
		{
			try
			{
				ErrorMessage = string.Empty;
				IsProcessing = true;
				var result = await _apiHelper.Authenticate(UserName, Password);

				// capture more information about the user
				await _apiHelper.GetLoggedInUserInfo(result.Access_Token);

				await _events.PublishOnUIThreadAsync(new LogOnEvent());
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}
			finally
			{
				IsProcessing = false;
			}
		}
	}
}
