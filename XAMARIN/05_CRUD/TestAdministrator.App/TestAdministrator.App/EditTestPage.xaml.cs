using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAdministrator.App.ViewModels;
using TestAdministrator.Dto;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestAdministrator.App
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditTestPage : ContentPage
    {
        private EditTestPage()
        {
            InitializeComponent();
        }

        public EditTestPage(EditTestViewModel vm) : this()
        {
            BindingContext = vm;
        }
    }
}