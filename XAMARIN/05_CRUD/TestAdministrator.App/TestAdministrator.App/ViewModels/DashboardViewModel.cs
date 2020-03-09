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
    /// <summary>
    /// Viewmodel für die Übersichtsseite der Tests
    /// </summary>
    public class DashboardViewModel : BaseViewModel
    {
        private readonly TestRepository _testRepository;
        private readonly INavigation _navigation;

        // Binding für die Liste der Tests.
        public ObservableCollection<TestDto> TestInfos => _testRepository?.Tests;
        // Binding für den ausgewählten Test.
        public TestDto SelectedTest { get; set; }

        // Commands für die Menübuttons.
        public ICommand DeleteItem { get; }
        public ICommand NewItem { get; }
        public ICommand EditItem { get; }

        public DashboardViewModel(TestRepository testRepository, INavigation navigation)
        {
            _testRepository = testRepository;
            _navigation = navigation;

            // Löscht den aktiven Test über das Repository.
            DeleteItem = new Command<TestDto>(async (current) =>
            {
                // Damit die Methode auch mit CommandParameter funktioniert, wählen wir entweder
                // den übergebenen Test oder - wenn nichts gewählt wurde (im Menü) den über das
                // Binding gesetzten SelectedTest.
                current = current ?? SelectedTest;
                try
                {
                    if (SelectedTest != null) { await _testRepository.Remove(current); }
                }
                catch (ServiceException e)
                {
                    await App.Current.MainPage.DisplayAlert("Fehler", e.Message, "OK");
                }
                SelectedTest = null;
            });

            // Öffnet die EditTest Seite mit einem leeren Test.
            NewItem = new Command(async () =>
            {
                await _navigation.PushAsync(new EditTestPage(new EditTestViewModel(_testRepository, _navigation)));
            });

            // Öffnet die EditTest Seite mit dem ausgewähltem Test.
            EditItem = new Command(async () =>
            {
                await _navigation.PushAsync(new EditTestPage(new EditTestViewModel(_testRepository, _navigation, SelectedTest)));
            });

        }
    }
}
