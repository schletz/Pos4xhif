using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TestAdministrator.Dto;

namespace TestAdministrator.App.Services
{
    /// <summary>
    /// Repository für den Zugriff auf den Controller Test
    /// </summary>
    public class TestRepository
    {
        private RestService _restService;
        private UserDto _user { get; }

        /// <summary>
        /// Beinhaltet die Tests eines Lehrers oder eines Schülers.
        /// </summary>
        public ObservableCollection<TestDto> Tests { get; }
        /// <summary>
        /// Beinhaltet die Unterrichtsstunden (Kombination Klasse und Fach), die ein Lehrer unterrichtet.
        /// Ist bei Schülern leer. Diese Collection wird verwendet, damit ein Lehrer nur aus den Fächern,
        /// die er unterrichtet, bei Anlegen eines Testes auswählen kann.
        /// </summary>
        public List<LessonDto> Lessons { get; }

        private TestRepository(RestService restService, UserDto user, ObservableCollection<TestDto> tests) :
            this(restService, user, tests, new List<LessonDto>())
        { }

        private TestRepository(RestService restService, UserDto user, ObservableCollection<TestDto> tests, List<LessonDto> lessons)
        {
            _restService = restService;
            _user = user;
            Tests = tests;
            Lessons = lessons;
        }

        /// <summary>
        /// Erstellt einen leeren Test mit dem aktuellen User aus eingetragenem Lehrer.
        /// </summary>
        /// <returns></returns>
        public TestDto CreateTest() => new TestDto { Teacher = _user.TeacherId };

        /// <summary>
        /// Lädt die Tests vom Server. Bei Lehrern werden auch die unterrichteten Fächer geladen.
        /// Gibt das initialisierte Repository zurück.
        /// </summary>
        /// <param name="user">User, für den die Tests geladen werden sollen.</param>
        /// <param name="restService">Das Rest Service für den Zugriff auf den Server.</param>
        /// <returns></returns>
        public static async Task<TestRepository> CreateAsync(UserDto user, RestService restService)
        {
            var tests = await RestService.Instance.SendAsync<ObservableCollection<TestDto>>(
                HttpMethod.Get, "testsbyuser", user.Username);

            if (!string.IsNullOrEmpty(user.TeacherId))
            {
                var lessons = await RestService.Instance.SendAsync<List<LessonDto>>(
                    HttpMethod.Get, "lessons", user.TeacherId);
                return new TestRepository(restService, user, tests, lessons);
            }
            return new TestRepository(restService, user, tests);
        }

        /// <summary>
        /// Ruft POST /api/test mit dem übergebenen Testobjekt auf.
        /// Fügt danach den Test zur ObservableCollection hinzu.
        /// </summary>
        /// <param name="test"></param>
        /// <returns></returns>
        public async Task Add(TestDto test)
        {
            TestDto newTest = await RestService.Instance.SendAsync<TestDto>(HttpMethod.Post, "test", test);
            Tests.Add(newTest);
        }

        /// <summary>
        /// Ruft PUT /api/test/id mit dem übergebenen Testobjekt auf.
        /// Löscht danach das Objekt aus der ObservableCollection und fügt das vom Server gelieferte
        /// neue Testobjekt hinzu.
        /// </summary>
        /// <param name="id">Die ID des Testes, die für den HTTP Request verwendet wird.</param>
        /// <param name="test">Die neuen Daten des Testes.</param>
        /// <returns></returns>
        public async Task Update(TestDto test)
        {
            TestDto newTest = await RestService.Instance.SendAsync<TestDto>(HttpMethod.Put, "test", test.TestId.ToString(), test);
            Tests.Remove(test);
            Tests.Add(newTest);
        }

        /// <summary>
        /// Ruft DELETE /api/test/id mit dem übergebenen Id auf.
        /// Löscht danach das Objekt aus der ObservableCollection.
        /// </summary>
        /// <param name="test">Der Test, der gelöscht werden soll.</param>
        /// <returns></returns>
        public async Task Remove(TestDto test)
        {
            await RestService.Instance.SendAsync(HttpMethod.Delete, "test", test.TestId?.ToString(), null);
            Tests.Remove(test);
        }

    }
}
