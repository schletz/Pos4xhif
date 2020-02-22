using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestAdministrator.App
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : MasterDetailPage
    {
        private MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initialisiert die Mainpage. Wird in App.OnStart() aufgerufen.
        /// </summary>
        /// <param name="detailPage">Die erste anzuzeigende Seite.</param>
        public MainPage(Page detailPage) : this()
        {
            Detail = detailPage;
        }
    }
}