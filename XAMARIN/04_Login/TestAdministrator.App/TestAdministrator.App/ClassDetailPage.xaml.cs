using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TestAdministrator.Dto;
using TestAdministrator.App.ViewModels;
using TestAdministrator.App.Services;
using System.Net;

namespace TestAdministrator.App
{
    /// <summary>
    /// Code Behind für ClassDetailPage.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClassDetailPage : ContentPage
    {
        public ClassDetailPage()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Konstruktor, der die Page und das ViewModel mit der übergebenen Klasse initialisiert.
        /// </summary>
        /// <param name="currentId">ID der Klasse, dessen Details angezeigt werden sollen.</param>
        public ClassDetailPage(string currentId) : this()
        {
            // Das Viewmodel mit der anzuzeigenden Klasse initialisieren.
            BindingContext = new ClassDetailViewModel(currentId);
            Title = currentId;
        }

        /// <summary>
        /// Lädt die Daten vom Server, wenn die Seite angezeigt wird. Das ist auch beim 
        /// Zurücknavigieren der Fall. Wenn das zu lange dauert, muss im RestService ein Cache
        /// einprogrammiert werden.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ContentPage_Appearing(object sender, EventArgs e)
        {
            try
            {
                ClassDetailViewModel vm = BindingContext as ClassDetailViewModel;
                await vm?.LoadClassDetails();
            }
            // Diese Fehler dürften hier gar nicht mehr auftreten, da das Laden der Seite nur mit
            // gültiger Rolle zu ermöglichen ist.
            catch (ServiceException err) when (err.HttpStatusCode == (int)HttpStatusCode.Unauthorized)
            {
                await App.Current.MainPage.DisplayAlert("Fehler", "Nicht angemeldet", "OK");
            }
            catch (ServiceException err) when (err.HttpStatusCode == (int)HttpStatusCode.Forbidden)
            {
                await App.Current.MainPage.DisplayAlert("Fehler", "Keine Berechtigung", "OK");
            }
        }
    }
}