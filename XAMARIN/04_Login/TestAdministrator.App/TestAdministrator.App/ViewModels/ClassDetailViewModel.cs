using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
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
            SchoolclassDto classDetails = await RestService.Instance.GetClassDetailsAsync(CurrentId);
            SetProperty(nameof(CurrentClassDetails), classDetails);
        }
    }
}
