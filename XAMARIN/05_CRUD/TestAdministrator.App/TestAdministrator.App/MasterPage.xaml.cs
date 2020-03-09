using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TestAdministrator.App.Services;
using TestAdministrator.App.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestAdministrator.App
{
    public class NavigationItem
    {
        public string Title { get; set; }
        public string IconSource { get; set; }
        public Type TargetType { get; set; }
    }

    // https://docs.microsoft.com/de-de/xamarin/xamarin-forms/app-fundamentals/navigation/master-detail-page
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterPage : ContentPage
    {
        public MasterPage()
        {
            InitializeComponent();
        }

        private async void NavigationList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            MasterDetailPage mainPage = App.Current.MainPage as MasterDetailPage;
            NavigationItem item = e.SelectedItem as NavigationItem;
            if (mainPage != null && item != null)
            {
                if (item.TargetType == typeof(DashboardPage))
                {
                    NavigationPage newNavigation = new NavigationPage();
                    TestRepository testRepository = await TestRepository.CreateAsync(RestService.Instance.CurrentUser, RestService.Instance);
                    await newNavigation.PushAsync(new DashboardPage(new DashboardViewModel(testRepository, newNavigation.Navigation, RestService.Instance.CurrentUser)));

                    mainPage.Detail = newNavigation;
                }
                else
                {
                    // Wir brauchen eine Navigation, sonst kommen wir in Android nicht mehr zurück wenn
                    // die Masterpage versteckt wird.
                    mainPage.Detail = new NavigationPage(
                        (Page)Activator.CreateInstance(item.TargetType));
                }
                // Die Masterpage soll nach dem Auswählen verschwinden.
                mainPage.IsPresented = false;
            }
        }
    }
}
