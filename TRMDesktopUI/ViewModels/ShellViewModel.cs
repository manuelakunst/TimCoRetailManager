using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using TRMDesktopUI.EventModels;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
    public class ShellViewModel : Conductor<object>, IHandle<LogOnEvent>
    {
        private SalesViewModel _salesVM;
        private IEventAggregator _events;
        private ILoggedInUserModel _user;

        public ShellViewModel(SalesViewModel salesVM, IEventAggregator events, ILoggedInUserModel user)
        {            
            _salesVM = salesVM;
            _events = events;

            _events.Subscribe(this);

            _user = user;

            ActivateItem(IoC.Get<LoginViewModel>());
        }

        public void Handle(LogOnEvent message)
        {
            ActivateItem(_salesVM);

            NotifyOfPropertyChange(() => IsUserLoggedIn);
        }

        public void ExitApplication()
        {
            TryClose();
        }

        public void LogOut()
        {
            // TODO reset logged user
            _user.LogOffUser();
            NotifyOfPropertyChange(() => IsUserLoggedIn);
            ActivateItem(IoC.Get<LoginViewModel>());
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
