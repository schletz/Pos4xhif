using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TestAdministrator.App.Services;
using TestAdministrator.Dto;
using Xamarin.Forms;

namespace TestAdministrator.App.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly TestRepository _testRepository;
        private readonly INavigation _navigation;

        public ObservableCollection<TestDto> TestInfos => _testRepository?.Tests;
        public TestDto SelectedTest { get; set; }

        public ICommand DeleteItem { get; }
        public ICommand NewItem { get; }
        public ICommand EditItem { get; }

        private DashboardViewModel(TestRepository testRepository, INavigation navigation)
        {
            _testRepository = testRepository;
            _navigation = navigation;

            DeleteItem = new Command<TestDto>(async (current) =>
            {
                try
                {
                    if (SelectedTest != null) { await _testRepository.Remove((long)SelectedTest.TestId); }
                }
                catch (ServiceException e)
                {
                    await App.Current.MainPage.DisplayAlert("Fehler", e.Message, "OK");
                }
                SelectedTest = null;
            });

            NewItem = new Command(async () =>
            {
                await _navigation.PushAsync(new EditTestPage(await EditTestViewModel.FactoryAsync(_testRepository, _navigation)));
            });

            EditItem = new Command(async () =>
            {
                await _navigation.PushAsync(new EditTestPage(await EditTestViewModel.FactoryAsync(_testRepository, SelectedTest, _navigation)));
            });

        }

        /// <summary>
        /// Lädt die Daten, die die Seite für die erste Darstellung benötigt, und gibt das
        /// initialisierte Viewmodel zurück.
        /// </summary>
        /// <returns></returns>
        public static async Task<DashboardViewModel> FactoryAsync(INavigation navigation)
        {
            TestRepository testRepository = await TestRepository.LoadAsync(RestService.Instance.CurrentUser.Username);
            return new DashboardViewModel(testRepository, navigation);
        }


    }
}
