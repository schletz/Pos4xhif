using System;
using System.Collections.Generic;
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
    public class EditTestViewModel : BaseViewModel
    {
        private readonly TestRepository _testRepository;
        private readonly INavigation _navigation;

        public List<LessonDto> Lessons => _testRepository.Lessons;

        public ICommand SaveTest { get; }
        public TestDto Test { get; }

        public List<string> Classes => Lessons.Select(l => l.Class).Distinct().ToList();

        public string SelectedClass
        {
            get => Test.Schoolclass;
            set
            {
                if (value != null)
                {
                    Test.Schoolclass = value;
                    SetProperty(
                        nameof(Subjects),
                        Lessons.Where(l => l.Class == SelectedClass).Select(l => l.Subject).ToList());
                    if (Subjects.Count == 1)
                        SetProperty(nameof(SelectedSubject), Subjects[0]);
                }
            }
        }

        public List<string> Subjects { get; set; }

        public string SelectedSubject { 
            get => Test.Subject;
            set => Test.Subject = value;
        }

        public DateTime MinDate => DateTime.Now.Month >= 9 ? new DateTime(DateTime.Now.Year, 9, 1) : new DateTime(DateTime.Now.Year - 1, 9, 1);
        public DateTime MaxDate => DateTime.Now.Month >= 9 ? new DateTime(DateTime.Now.Year + 1, 9, 1) : new DateTime(DateTime.Now.Year, 9, 1);

        public EditTestViewModel(TestRepository testRepository, INavigation navigation)
            : this(testRepository, navigation, null)
        { }

        public EditTestViewModel(TestRepository testRepository, INavigation navigation, TestDto test)
        {
            _testRepository = testRepository;
            _navigation = navigation;
            // Legt einen leeren Test mit der entsprechenden Lehrer ID an.
            Test = test ?? _testRepository.CreateTest();

            SaveTest = new Command(async () =>
            {
                try
                {
                    if (Test.TestId == null)
                    {
                        await _testRepository.Add(Test);
                    }
                    else
                    {
                        await _testRepository.Update(Test);
                    }
                }
                catch (ServiceException e)
                {
                    await App.Current.MainPage.DisplayAlert("Fehler", e.Message, "OK");
                }
                finally
                {
                    await _navigation.PopAsync();
                }
            });
        }
    }
}
