using StateDemo.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace StateDemo.ViewModel
{
    class DetailModel : INotifyPropertyChanged
    {
        public string Username { get; set; }
        public DetailModel()
        {
            // Das UserChanged Event des StateManegers abonnieren und die Logik im VM ausführen.
            StateManager.Instance.UserChanged += (username) =>
             {
                 Username = username;
                 PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Username)));
             };
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
