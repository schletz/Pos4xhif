using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
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

        /// <summary>
        /// Setzt den Wert eines Properties und ruft das PropertyChanged Event auf, um die Bindings
        /// zu aktualisieren.
        /// </summary>
        /// <typeparam name="T">Typ des Properties.</typeparam>
        /// <param name="propertyName">Name des Properties. Wird mit nameof(Property) im Aufruf ermittelt.</param>
        /// <param name="value">Wert, auf den das Property gesetzt wird.</param>
        protected void SetProperty<T>(string propertyName, T value)
        {
            PropertyInfo prop = GetType().GetProperty(propertyName);
            prop.SetValue(this, value);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
