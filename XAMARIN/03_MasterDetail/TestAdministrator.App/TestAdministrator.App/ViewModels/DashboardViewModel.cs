using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TestAdministrator.App.Services;
using Xamarin.Forms;

namespace TestAdministrator.App.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly RestService _restService = DependencyService.Get<RestService>();
        public string Response { get; set; }

        private DashboardViewModel() { }

        /// <summary>
        /// Lädt die Daten, die die Seite für die erste Darstellung benötigt, und gibt das
        /// initialisierte Viewmodel zurück.
        /// </summary>
        /// <returns></returns>
        public static async Task<DashboardViewModel> FactoryAsync()
        {
            DashboardViewModel vm = new DashboardViewModel();
            vm.Response = (await vm._restService.SendAsync<List<string>>(HttpMethod.Get, "dashboard"))[0];
            return vm;
        }
    }
}
