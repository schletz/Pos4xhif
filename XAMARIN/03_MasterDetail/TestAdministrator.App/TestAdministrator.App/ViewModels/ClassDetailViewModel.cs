using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using TestAdministrator.Dto;

namespace TestAdministrator.App.ViewModels
{
    /// <summary>
    /// Viewmodel für die Page ClassDetailViewPage
    /// </summary>
    public class ClassDetailViewModel : BaseViewModel
    {
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
            if (await RestService.TryLoginAsync(
                new UserDto { Username = "schueler", Password = "pass" }))
            {
                SchoolclassDto classDetails = await RestService.GetClassDetailsAsync(CurrentId);
                SetProperty(nameof(CurrentClassDetails), classDetails);
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Fehler", "Login nicht erfolgreich", "OK");
            }
        }
    }
}
