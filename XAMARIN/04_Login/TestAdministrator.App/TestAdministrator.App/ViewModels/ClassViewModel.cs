﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TestAdministrator.App.Services;
using TestAdministrator.Dto;
using Xamarin.Forms;

namespace TestAdministrator.App.ViewModels
{
    public class ClassViewModel : BaseViewModel
    {
        /// <summary>
        /// Liste der gelesenen Klassen.
        /// </summary>
        public IEnumerable<SchoolclassDto> Classes { get; set; }
        /// <summary>
        /// Die in der ListView ausgewählte Klasse. Wird über Binding geschrieben.
        /// </summary>
        public SchoolclassDto SelectedClass { get; set; }

        /// <summary>
        /// Lädt eine Liste der Klassen von der REST API.
        /// </summary>
        /// <returns></returns>
        public async Task LoadClasses()
        {
            try
            {
                SetProperty(nameof(Classes), await RestService.Instance.GetClassesAsync());
            }
            catch (Exception e)
            {
#if DEBUG
                await App.Current.MainPage.DisplayAlert("Fehler", e.ToString(), "OK");
#endif
            }
        }
    }
}
