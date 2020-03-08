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
    public class TestRepository
    {
        public ObservableCollection<TestDto> Tests { get; }
        public string User { get; }

        private TestRepository(string user, ObservableCollection<TestDto> tests)
        {
            User = user;
            Tests = tests;
        }

        public static async Task<TestRepository> LoadAsync(string user)
        {
            var tests = await RestService.Instance.SendAsync<ObservableCollection<TestDto>>(
                HttpMethod.Get, "test", RestService.Instance.CurrentUser.Username);
            return new TestRepository(user, tests);
        }

        public async Task Add(TestDto test)
        {
            TestDto newTest = await RestService.Instance.SendAsync<TestDto>(HttpMethod.Post, "test", RestService.Instance.CurrentUser.Username, test);
            Tests.Add(newTest);
        }

        public async Task Update(long id, TestDto test)
        {
            TestDto newTest = await RestService.Instance.SendAsync<TestDto>(HttpMethod.Put, "test", RestService.Instance.CurrentUser.Username, test);
            Tests.Remove(Tests.FirstOrDefault(t => t.TestId == id));
            Tests.Add(newTest);
        }

        public async Task Remove(long id)
        {
            await RestService.Instance.SendAsync(HttpMethod.Delete, "test", id.ToString(), null);
            Tests.Remove(Tests.FirstOrDefault(t => t.TestId == id));
        }

    }
}
