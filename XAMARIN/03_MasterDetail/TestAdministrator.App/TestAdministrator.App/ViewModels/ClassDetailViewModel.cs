using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using TestAdministrator.App.Services;
using TestAdministrator.Dto;
using Xamarin.Forms;

namespace TestAdministrator.App.ViewModels
{
    /// <summary>
    /// Viewmodel für die Page ClassDetailViewPage
    /// </summary>
    public class ClassDetailViewModel : BaseViewModel
    {
        private readonly RestService _restService = DependencyService.Get<RestService>();

        public string CurrentId { get; }
        /// <summary>
        /// Details zur geladenen Klasse.
        /// </summary>
        public SchoolclassDto CurrentClassDetails { get; set; }
        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="currentId">Die ID der Klasse, dessen Details angezeigt werden.</param>
        public ClassDetailViewModel(string currentId)
        {
            CurrentId = currentId;
        }
        /// <summary>
        /// Lädt die Details zur angezeigten Klasse über das RestService.
        /// </summary>
        /// <returns></returns>
        public async Task LoadClassDetails()
        {
            // Für die Klassendetails müssen wir uns anmelden.
            if (await _restService.TryLoginAsync(
                new UserDto { Username = "schueler", Password = "pass" }))
            {
                SchoolclassDto classDetails = await _restService.GetClassDetailsAsync(CurrentId);
                SetProperty(nameof(CurrentClassDetails), classDetails);
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Fehler", "Login nicht erfolgreich", "OK");
            }
        }
    }
}
