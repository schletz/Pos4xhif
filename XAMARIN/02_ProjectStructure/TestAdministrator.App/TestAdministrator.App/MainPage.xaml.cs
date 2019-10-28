using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using TestAdministrator.App.Services;
using TestAdministrator.App.ViewModels;

namespace TestAdministrator.App
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Load_Clicked(object sender, EventArgs e)
        {
            MainViewModel vm = BindingContext as MainViewModel;

            await vm.LoadClasses();
        }
    }
}
