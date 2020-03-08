using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TestAdministrator.App.Services;
using TestAdministrator.Dto;
using Xamarin.Forms;

namespace TestAdministrator.App.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        public ObservableCollection<TestDto> TestInfos { get; private set; }

        public List<LessonDto> Lessons { get; private set; }

        private TestDto _selectedTest = new TestDto();
        public TestDto SelectedTest
        {
            get => _selectedTest;
            set
            {
                if (value != null)
                {
                    _selectedTest = value;
                    InvalidateProperty(nameof(SelectedTest));
                    InvalidateProperty(nameof(SelectedClass));
                    InvalidateProperty(nameof(SelectedSubject));
                }
            }
        }

        public List<string> Classes => Lessons.Select(l => l.Class).Distinct().ToList();

        public string SelectedClass
        {
            get => SelectedTest.Schoolclass;
            set
            {
                if (value != null)
                {
                    SelectedTest.Schoolclass = value;
                    SetProperty(
                        nameof(Subjects),
                        Lessons.Where(l => l.Class == SelectedClass).Select(l => l.Subject).ToList());
                    SetProperty(nameof(SelectedSubject), Subjects[0]);
                }

            }
        }

        public List<string> Subjects { get; set; }
        public string SelectedSubject
        {
            get => SelectedTest.Subject;
            set => SelectedTest.Subject = value;
        }

        public DateTime MinDate => DateTime.Now.Month >= 9 ? new DateTime(DateTime.Now.Year, 9, 1) : new DateTime(DateTime.Now.Year - 1, 9, 1);
        public DateTime MaxDate => DateTime.Now.Month >= 9 ? new DateTime(DateTime.Now.Year + 1, 9, 1) : new DateTime(DateTime.Now.Year, 9, 1);


        private DashboardViewModel()
        {
            DeleteItem = new RelayCommand(async (param) =>
            {
                try
                {
                    TestDto current = param as TestDto;
                    await RestService.Instance.SendAsync(HttpMethod.Delete, "test", current?.TestId.ToString(), null);
                    TestInfos.Remove(current);
                }
                catch (ServiceException e)
                {
                    await App.Current.MainPage.DisplayAlert("Fehler", e.Message, "OK");
                }
            });

            SaveTest = new RelayCommand(async () =>
            {
                HttpMethod method = SelectedTest.TestId == null ? HttpMethod.Post : HttpMethod.Put;
                TestDto newTest = await RestService.Instance.SendAsync<TestDto>(method, "test", RestService.Instance.CurrentUser.Username, SelectedTest);
                TestInfos.Remove(SelectedTest);
                TestInfos.Add(newTest);
                SelectedTest = new TestDto();
            });

            NewTest = new RelayCommand(() =>
            {
                SelectedTest = new TestDto();
            });
        }

        /// <summary>
        /// Lädt die Daten, die die Seite für die erste Darstellung benötigt, und gibt das
        /// initialisierte Viewmodel zurück.
        /// </summary>
        /// <returns></returns>
        public static async Task<DashboardViewModel> FactoryAsync()
        {
            DashboardViewModel vm = new DashboardViewModel();

            vm.TestInfos = await RestService.Instance.SendAsync<ObservableCollection<TestDto>>(
                HttpMethod.Get, "test", RestService.Instance.CurrentUser.Username);
            vm.Lessons = await RestService.Instance.SendAsync<List<LessonDto>>(
                HttpMethod.Get, "lessons", RestService.Instance.CurrentUser.Username);

            return vm;
        }

        public RelayCommand DeleteItem { get; }
        public RelayCommand SaveTest { get; }
        public RelayCommand NewTest { get; }
    }
}
