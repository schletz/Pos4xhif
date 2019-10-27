using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using TestAdministrator.App.Services;
using Xamarin.Forms;
using TestAdministrator.Dto;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Globalization;

namespace TestAdministrator.App.ViewModels
{
    /// <summary>
    /// Viewmodel für die View MainPage
    /// https://docs.microsoft.com/en-us/xamarin/xamarin-forms/xaml/xaml-basics/data-bindings-to-mvvm
    /// </summary>
    class MainViewModel : INotifyPropertyChanged
    {
        private readonly RestService _restService;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Liste der gelesenen Klassen.
        /// </summary>
        private IEnumerable<SchoolclassDto> _classes;
        public IEnumerable<SchoolclassDto> Classes
        {
            get => _classes;
            set { _classes = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Flag, ob gerade ein Request läuft oder ob Controls gedrückt werden können.
        /// </summary>
        private bool _acceptInput = true;
        public bool AcceptInput
        {
            get => _acceptInput;
            set { _acceptInput = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Konstruktor. Holt alle benötigten Objekte aus dem DependencyService.
        /// </summary>
        public MainViewModel()
        {
            _restService = DependencyService.Get<RestService>();
        }
        /// <summary>
        /// Lädt eine Liste der Klassen von der REST API.
        /// </summary>
        /// <returns></returns>
        public async Task LoadClasses()
        {
            try
            {
                AcceptInput = false;
                // Zum Testen melden wir uns an.
                if (await _restService.TryLoginAsync(
                    new UserDto { Username = "schueler", Password = "pass" }
                ))
                {
                    Classes = await _restService.GetClassesAsync();
                    // Und zum Testen melden wir uns wieder ab.
                    _restService.Logout();
                }
            }
            catch (Exception e)
            {
#if DEBUG
                await Application.Current.MainPage.DisplayAlert("Fehler", e.ToString(), "OK");
#endif
            }
            AcceptInput = true;
        }

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Konvertiert ein Bindingfeld so, dass es true liefert wenn es ungleich null ist. 
    /// Ansonsten false. Wird z. B. für Sichtbarkeiten von Elementen benötigt.
    /// </summary>
    public class HasValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
