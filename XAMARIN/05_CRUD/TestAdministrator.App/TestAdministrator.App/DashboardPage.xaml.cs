using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAdministrator.App.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestAdministrator.App
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DashboardPage : ContentPage
    {
        /// <summary>
        /// Defaultkonstruktor. Ist private, denn die Seite benötigt ein Initialisiertes ViewModel.
        /// </summary>
        private DashboardPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initialisiert die Seite und setzt das ViewModel.
        /// </summary>
        /// <param name="vm">Fertig initialisiertes Viewmodel mit den Daten zur Darstellung der Seite.</param>
        public DashboardPage(DashboardViewModel vm) : this()
        {
            BindingContext = vm;
        }

    }
}