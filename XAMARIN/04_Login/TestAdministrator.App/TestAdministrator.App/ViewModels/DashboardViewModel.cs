using System;
using System.Collections.Generic;
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
        public List<TestinfoDto> TestInfos { get; private set; }

        private DashboardViewModel() { }

        /// <summary>
        /// Lädt die Daten, die die Seite für die erste Darstellung benötigt, und gibt das
        /// initialisierte Viewmodel zurück.
        /// </summary>
        /// <returns></returns>
        public static async Task<DashboardViewModel> FactoryAsync()
        {
            // Das RestService über das Xamarin Dependency Service holen.
            RestService restService = RestService.Instance;
            DashboardViewModel vm = new DashboardViewModel();

            vm.TestInfos = await restService.SendAsync<List<TestinfoDto>>(
                HttpMethod.Get, "dashboard", restService.CurrentUser.Username);
            return vm;
        }
    }
}
