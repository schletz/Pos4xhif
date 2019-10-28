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
    public class RestService
    {
        private readonly HttpClientHandler _handler;           // Genauere Steuerung des HttpClient
        private readonly HttpClient _client;                   // Einzige Instanz des HttpClient
        private UserDto _currentUser;                          // Aktuell angemeldeter Benutzer.
        // Properties werden von System.Text.Json in camelCase umgewandelt. Daher muss bei der
        // Deserialisierung Case Sensitive deaktiviert werden.
        private readonly JsonSerializerOptions _jsonOptions =
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        /// <summary>
        /// Konstruktor. Legt die Http Einstellungen fest.
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
        public Task<IEnumerable<SchoolclassDto>> GetClassesAsync() => SendAsync<IEnumerable<SchoolclassDto>>(HttpMethod.Get, "classes");
        public Task<SchoolclassDto> GetClassDetailsAsync(string classId) => SendAsync<SchoolclassDto>(HttpMethod.Get, "classes", classId);

        /// <summary>
        /// Meldet den User an der Adresse (baseUrl)/user/login an und setzt den Token als
        /// Default Request Header für zukünftige Anfragen.
        /// </summary>
        /// <param name="user">Benutzer, der angemeldet werden soll.</param>
        /// <returns>Userobjekt mit Token wenn erfolgreich, null bei ungültigen Daten.</returns>
        public async Task<bool> TryLoginAsync(UserDto user)
        {
            try
            {
                UserDto sentUser = await SendAsync<UserDto>(HttpMethod.Post, "user/login", user);
                _client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", sentUser.Token);
                _currentUser = sentUser;
                return true;
            }
            catch (ServiceException e) when (e.HttpStatusCode == (int)HttpStatusCode.Unauthorized)
            {
                return false;
            }
        }

        /// <summary>
        /// Löscht den Token aus den HTTP Headern. Ein Logout Request in der API ist nicht nötig.
        /// </summary>
        /// <returns></returns>
        public void Logout()
        {
            _currentUser = null;
            _client.DefaultRequestHeaders.Authorization = null;
        }

        Task<T> SendAsync<T>(HttpMethod method, string actionUrl) => SendAsync<T>(method, actionUrl, "", null);
        Task<T> SendAsync<T>(HttpMethod method, string actionUrl, object requestData) => SendAsync<T>(method, actionUrl, "", requestData);
        Task<T> SendAsync<T>(HttpMethod method, string actionUrl, string idParam) => SendAsync<T>(method, actionUrl, idParam, null);
        /// <summary>
        /// Sendet einen Request an die REST API und gibt das Ergebnis zurück.
        /// </summary>
        /// <typeparam name="T">Typ, in den die JSON Antwort des Servers umgewandelt wird.</typeparam>
        /// <param name="method">HTTP Methode, die zum Senden des Requests verwendet wird.</param>
        /// <param name="actionUrl">Adresse, die in {baseUrl}/{actionUrl}/{idParam} ersetzt wird.</param>
        /// <param name="idParam">Adresse, die in {baseUrl}/{actionUrl}/{idParam} ersetzt wird.</param>
        /// <param name="requestData">Daten, die als JSON Request Body bzw. als Parameter bei GET Requests gesendet werden.</param>
        /// <returns></returns>
        public async Task<T> SendAsync<T>(HttpMethod method, string actionUrl, string idParam, object requestData)
        {
            string baseUrl = DependencyService.Get<AppSettingsService>().Get("ServiceUrl");
            string url = $"{baseUrl}/{actionUrl}/{idParam}";

            // Wurde der Zugriff auf das Internet im Manifest erlaubt? Es muss
            // <uses-permission android:name="android.permission.INTERNET" />
            // in Androidprojekt/Properties/AndroidManifest.xml gesetzt werden.
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                throw new ServiceException("No permission for Internet connecions.") { Url = url };
            }

            try
            {
                // Die Daten für den Request Body als JSON serialisieren und mitsenden.
                string jsonContent = JsonSerializer.Serialize(requestData);
                StringContent request = new StringContent(
                    jsonContent, Encoding.UTF8, "application/json"
                );

                HttpResponseMessage response;
                if (method == HttpMethod.Get)
                {
                    string parameters = requestData as string;
                    if (!string.IsNullOrEmpty(parameters))
                        url = $"{url}?{parameters}";
                    response = await _client.GetAsync(url);
                }
                else if (method == HttpMethod.Post)
                { response = await _client.PostAsync(url, request); }
                else if (method == HttpMethod.Put)
                { response = await _client.PutAsync(url, request); }
                else if (method == HttpMethod.Delete)
                { response = await _client.DeleteAsync(url); }
                else
                {
                    throw new ServiceException("Unsupported Request Method") { Url = url };
                }

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
                    return JsonSerializer.Deserialize<T>(result, _jsonOptions);
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
                    Url = url,
                };
            }
        }
    }
}
