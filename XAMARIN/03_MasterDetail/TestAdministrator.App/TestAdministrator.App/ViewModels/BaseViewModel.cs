using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using TestAdministrator.App.Services;
using Xamarin.Forms;

namespace TestAdministrator.App.ViewModels
{
    /// <summary>
    /// Basisklasse für alle ViewModels der Applikation. Implementiert INotifyPropertyChanged
    /// und lädt das RestService vom ServiceLocator.
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected RestService RestService { get; }

        public BaseViewModel()
        {
            RestService = DependencyService.Get<RestService>();
        }
        /// <summary>
        /// Ruft den PropertyChanged Event auf. Der Propertyname wird vom Compiler bestimmt, daher
        /// muss OnPropertyChanged() direkt im set des Properties aufgerufen werden.
        /// </summary>
        /// <param name="propertyName">Vom Compiler ermittelter Propertyname.</param>
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
