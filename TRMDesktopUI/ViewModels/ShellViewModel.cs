using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using TRMDesktopUI.EventModels;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
    public class ShellViewModel : Conductor<object>, IHandle<LogOnEvent>
    {
        private SalesViewModel _salesVM;
        private IEventAggregator _events;
        private ILoggedInUserModel _user;
        private IAPIHelper _apiHelper;
        public ShellViewModel(SalesViewModel salesVM, IEventAggregator events, ILoggedInUserModel user, IAPIHelper apiHelper)
        {            
            _salesVM = salesVM;
            _events = events;

            _events.SubscribeOnPublishedThread(this);

            _user = user;
            _apiHelper = apiHelper;
            
            ActivateItemAsync(IoC.Get<LoginViewModel>(), new CancellationToken());
        }

        //public void Handle(LogOnEvent message)
        //{
        //    ActivateItem(_salesVM);

        //    NotifyOfPropertyChange(() => IsUserLoggedIn);
        //}

        public void ExitApplication()
        {
            TryCloseAsync();
        }
         
        public async Task UserManagement()
        {
            await ActivateItemAsync(IoC.Get<UserDisplayViewModel>(), new CancellationToken());
        }

        public async Task LogOut()
        {
            _user.ResetUser();
            _apiHelper.LogOffUser();
            NotifyOfPropertyChange(() => IsUserLoggedIn);
            await ActivateItemAsync(IoC.Get<LoginViewModel>(), new CancellationToken());
        }

        public async Task HandleAsync(LogOnEvent message, CancellationToken cancellationToken)
        {
            await ActivateItemAsync(_salesVM, cancellationToken);

            NotifyOfPropertyChange(() => IsUserLoggedIn);
        }

        public bool IsUserLoggedIn
        {
            get
            {
                var output = false;

                if (!string.IsNullOrEmpty(_user.Token))
                    output = true;

                return output;
            }
        }
    }
}
