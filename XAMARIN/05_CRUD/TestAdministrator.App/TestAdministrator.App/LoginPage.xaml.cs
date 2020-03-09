using System;

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TestAdministrator.App.Services;
using TestAdministrator.App.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestAdministrator.App
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void Login_Clicked(object sender, EventArgs e)
        {
            // Holt sich das Rest Service als Singleton. Nicht new verwenden,
            // da sonst der Token im Header beim Request nicht mitgesendet
            // wird.
            if (await RestService.Instance.TryLoginAsync(new Dto.UserDto
                {
                    Username = this.Username.Text,
                    Password = this.Password.Text
                }))
            {
                NavigationPage newNavigation = new NavigationPage();
                TestRepository testRepository = await TestRepository.CreateAsync(RestService.Instance.CurrentUser, RestService.Instance);
                await newNavigation.PushAsync(new DashboardPage(new DashboardViewModel(testRepository, newNavigation.Navigation)));

                Application.Current.MainPage = new MainPage(newNavigation);
            }
        }
    }
}