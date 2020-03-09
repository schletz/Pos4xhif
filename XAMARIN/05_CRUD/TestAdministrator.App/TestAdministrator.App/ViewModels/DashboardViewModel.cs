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
        private readonly UserDto _user;

        public ObservableCollection<TestDto> TestInfos => _testRepository?.Tests;
        public TestDto SelectedTest { get; set; }

        public ICommand DeleteItem { get; }
        public ICommand NewItem { get; }
        public ICommand EditItem { get; }

        public DashboardViewModel(TestRepository testRepository, INavigation navigation, UserDto user)
        {
            _testRepository = testRepository;
            _navigation = navigation;
            _user = user;

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
                await _navigation.PushAsync(new EditTestPage(new EditTestViewModel(_testRepository, _navigation, _user)));
            });

            EditItem = new Command(async () =>
            {
                await _navigation.PushAsync(new EditTestPage(new EditTestViewModel(_testRepository, _navigation, _user, SelectedTest)));
            });

        }
    }
}
