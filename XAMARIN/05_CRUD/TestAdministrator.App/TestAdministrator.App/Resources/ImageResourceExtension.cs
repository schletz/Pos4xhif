using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestAdministrator.App
{
    /// <summary>
    /// Definiert die Xaml Erweiterung, sodass mit 
    /// "{local:ImageResource TestAdministrator.App.Resources.filename.png}"
    /// auf das Bild zugegriffen werden kann. Dabei muss das Bild als Embedded resource in den
    /// Eigenschaften definiert werden.
    /// Quelle: https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/images?tabs=windows#xaml
    /// </summary>
    [ContentProperty(nameof(Source))]
    public class ImageResourceExtension : IMarkupExtension
    {
        public string Source { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null)
            {
                return null;
            }

            // Do your translation lookup here, using whatever method you require
            var imageSource = ImageSource.FromResource(Source, typeof(ImageResourceExtension).GetTypeInfo().Assembly);

            return imageSource;
        }
    }
}
