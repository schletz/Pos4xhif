using StateDemo.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace StateDemo.ViewModel
{
    class MasterViewModel
    {
        public string Username { get; set; }
        public ICommand LoginCommand { get; }

        public MasterViewModel()
        {
            LoginCommand = new Command(
                () =>
                {
                    StateManager.Instance.ChangeUser(Username);
                });
        }
    }
}
