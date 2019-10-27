using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TestAdministrator.Dto;
using Xamarin.Forms;
using System.Text.Json;
using System.Net;
using Xamarin.Essentials;

// Registriert das Service, sodass die Instanz in der App mit DependencyService.Get<RestService>()
// abgerufen werden kann. Es wird nur 1 Instanz erstellt, falls keine Option für 
// DependencyFetchTarget.GlobalInstance in DependencyService angegeben wird.
// Vgl. https://docs.microsoft.com/en-us/xamarin/xamarin-forms/app-fundamentals/dependency-service/introduction
[assembly: Dependency(typeof(TestAdministrator.App.Services.RestService))]
namespace TestAdministrator.App.Services
{
    /// <summary>
    /// Stellt ein Service bereit, welches alle Abfragen an die REST API zur Verfügung stellt.
    /// Dafür muss die Permission INTERNET in Android bei externen Servern gesetzt werden. Außerdem
    /// muss die Firewall des Servers Verbindungen von diesem Port akzeptieren.
    /// </summary>
    class RestService
    {
        private readonly HttpClientHandler _handler;           // Genauere Steuerung des HttpClient
        private readonly HttpClient _client;
        private UserDto _currentUser;

        /// <summary>
        /// Erstellt das Objekt und legt die Http Einstellungen fest.
        /// </summary>
        public RestService()
        {
            // Akzeptiert das selbst ausgestellte Zertifikat der REST API.
            // Sollte nicht im Produktionscode sein, deswegen ist das Akteptieren von ungültigen
            // Zertifikaten nur im Debugmodus aktiviert.
            _handler = new HttpClientHandler() { ClientCertificateOptions = ClientCertificateOption.Manual };
#if DEBUG
            _handler.ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return true;
                };
#endif
            _client = new HttpClient(_handler);
        }
        // Hier werden die angeboteten Serviceses der REST Schnittstelle auf die C# Methoden
        // abgebildet.
        public Task<IEnumerable<SchoolclassDto>> GetClassesAsync() => GetAsync<IEnumerable<SchoolclassDto>>("classes");

        /// <summary>
        /// Meldet den User an der Adresse (baseUrl)/user/login an.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Userobjekt mit Token wenn erfolgreich, null bei ungültigen Daten.</returns>
        public async Task<bool> TryLoginAsync(UserDto user)
        {
            if (_currentUser != null) { return true; }
            if (user == null) { throw new ServiceException("User to log in is null."); }

            string baseUrl = DependencyService.Get<AppSettingsService>().Get("ServiceUrl");
            string url = $"{baseUrl}/user/login";

            // Wurde der Zugriff auf das Internet im Manifest erlaubt? Es muss
            // <uses-permission android:name="android.permission.INTERNET" />
            // in Androidprojekt/Properties/AndroidManifest.xml gesetzt werden.
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                throw new ServiceException("No permission for Internet connecions.") { Url = url };
            }

            try
            {
                string jsonContent = JsonSerializer.Serialize(user);
                StringContent request = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _client.PostAsync(url, request);
                if (response.StatusCode == HttpStatusCode.Unauthorized) { return false; }
                if (!response.IsSuccessStatusCode)
                {
                    throw new ServiceException("Login Request not successful.")
                    {
                        Url = url,
                        HttpStatusCode = (int)response.StatusCode
                    };
                }
                string token = await response.Content.ReadAsStringAsync();
                // Setzt den Token in den Authorizationheader des Clients. Somit wird er
                // bei allen Anfragen mitgesendet.
                _client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                user.Password = "";
                user.Token = token;
                _currentUser = user;
                return true;
            }
            catch (ServiceException) { throw; }
            catch (Exception e)
            {
                throw new ServiceException("Login Request not successful.")
                {
                    Url = url,
                };
            }
        }

        /// <summary>
        /// Sendet einen Request mit der URL {actionUrl}/{param} an die unter ServiceUrl in der
        /// Datei appsettings.json angegebene Adresse und gibt die Antwort deserialisiert
        /// zurück.
        /// </summary>
        /// <typeparam name="T">
        /// Typ für die Deserialisierung. Collections können mit IEnumerable<...> angegeben werden.
        /// </typeparam>
        /// <param name="actionUrl"></param>
        /// <param name="param">Optional. Leerstring wenn nicht angegeben.</param>
        /// <returns></returns>
        private async Task<T> GetAsync<T>(string actionUrl, string param = "")
        {
            // Den  Eintrag ServiceUrl aus der Datei appsettings.json holen.
            string baseUrl = DependencyService.Get<AppSettingsService>().Get("ServiceUrl");
            string url = $"{baseUrl}/{actionUrl}/{param}";

            // Wurde der Zugriff auf das Internet im Manifest erlaubt? Es muss
            // <uses-permission android:name="android.permission.INTERNET" />
            // in Androidprojekt/Properties/AndroidManifest.xml gesetzt werden.
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                throw new ServiceException("No permission for Internet connecions.") { Url = url };
            }

            try
            {
                HttpResponseMessage response = await _client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    throw new ServiceException("Request not successful.")
                    {
                        Url = url,
                        HttpStatusCode = (int)response.StatusCode
                    };
                }
                string result = await response.Content.ReadAsStringAsync();
                try
                {
                    return JsonSerializer.Deserialize<T>(result);
                }
                catch (Exception e)
                {
                    throw new ServiceException("Cannot parse result", e)
                    {
                        Url = url
                    };
                }
            }
            catch (ServiceException) { throw; }
            catch (Exception e)
            {
                throw new ServiceException("Request not successful.", e)
                {
                    Url = url
                };
            }
        }


    }
}
