using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAdministrator.App.ViewModels;
using TestAdministrator.Dto;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestAdministrator.App
{
    /// <summary>
    /// Code Behind für die Seite ClassPage.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClassPage : ContentPage
    {
        public ClassPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Lädt die Daten vom Server, wenn die Seite angezeigt wird.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ContentPage_Appearing(object sender, EventArgs e)
        {
            ClassViewModel vm = BindingContext as ClassViewModel;
            await vm.LoadClasses();
        }

        /// <summary>
        /// Legt die Detailseite auf den Navigation Stack und lädt diese.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ClassList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ClassViewModel vm = BindingContext as ClassViewModel;
            await Navigation.PushAsync(new ClassDetailPage(vm.SelectedClass.Id));
        }
    }
}