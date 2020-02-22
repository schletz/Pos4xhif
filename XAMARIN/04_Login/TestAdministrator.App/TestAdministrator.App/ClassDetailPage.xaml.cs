using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TestAdministrator.Dto;
using TestAdministrator.App.ViewModels;

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
        public ClassDetailPage(string currentId): this()
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
            ClassDetailViewModel vm = BindingContext as ClassDetailViewModel;
            await vm.LoadClassDetails();
        }
    }
}