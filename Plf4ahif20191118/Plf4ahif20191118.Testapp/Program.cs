using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Plf4ahif20191118.Testapp.Model;
using Plf4ahif20191118.Dto;

namespace Plf4ahif20191118.Testapp
{
    class Program
    {
        static readonly HttpClient _client = new HttpClient();
        static void Main(string[] args)
        {
            MainAsync().Wait();
        }
        static async Task MainAsync()
        {
            int score = 0;
            string baseUrl = "http://localhost:5000/api";

            SeedDb();
            Console.Write("MUSTERTEST: GET api/schoolrating/test ");
            await AssertTrueAsync(async () =>
            {
                IEnumerable<SchoolDto> result = await SendAsync<IEnumerable<SchoolDto>>(
                    HttpMethod.Get, 
                    $"{baseUrl}/schoolrating/test", 
                    null);
                return result.Any(r => r.Nr == 9999);
            });

            // *************************************************************************************
            Console.Write("GET /api/schoolrating ");
            score += await AssertTrueAsync(async () =>
            {
                IEnumerable<SchoolDto> result = await SendAsync<IEnumerable<SchoolDto>>(
                    HttpMethod.Get, 
                    $"{baseUrl}/schoolrating", 
                    null);
                return result.Where(r => r.Nr == 9999 && r.AvgRating == 5).Count() == 1;
            });

            // *************************************************************************************
            Console.Write("POST /api/register ");
            score += await AssertTrueAsync(async () =>
            {

                UserDto result = await SendAsync<UserDto>(
                    HttpMethod.Post,
                    $"{baseUrl}/register", new UserDto
                    {
                        PhoneNr = "09994",
                        School = 9999
                    });
                using (RatingContext db = new RatingContext())
                {
                    return db.User.FirstOrDefault(u => 
                        u.U_School == result.School && 
                        u.U_Phonenr == result.PhoneNr) != null;
                }
            });

            // *************************************************************************************
            Console.Write("POST /api/register liefert HTTP 400 bei existierendem User? ");
            score += await AssertTrueAsync(async () =>
            {
                try
                {
                    UserDto result = await SendAsync<UserDto>(
                        HttpMethod.Post,
                        $"{baseUrl}/register", new UserDto
                        {
                            PhoneNr = "09991",
                            School = 9999
                        });
                }
                catch (HttpException e) when (e.Status == 400)
                {
                    return true;
                }
                return false;
            });


            // *************************************************************************************
            Console.Write("POST /api/schoolrating ");
            score += await AssertTrueAsync(async () =>
            {

                RatingDto result = await SendAsync<RatingDto>(
                    HttpMethod.Post,
                    $"{baseUrl}/schoolrating", new RatingDto
                    {
                        School = 9999,
                        PhoneNr = "09992",
                        Value = 1
                    });
                using (RatingContext db = new RatingContext())
                {
                    return db.School_Rating.Find(result.Id) != null;
                }
            });

            // *************************************************************************************
            Console.Write("POST /api/schoolrating liefert HTTP 400, wenn der User die Schule schon bewertet hat? ");
            score += await AssertTrueAsync(async () =>
            {
                try
                {
                    RatingDto result = await SendAsync<RatingDto>(
                        HttpMethod.Post,
                        $"{baseUrl}/schoolrating", new RatingDto
                        {
                            School = 9999,
                            PhoneNr = "09991",
                            Value = 1
                        });
                }
                catch (HttpException e) when (e.Status == 400)
                { return true; }
                return false;
            });

            // *************************************************************************************
            Console.Write("PUT /api/schoolrating/(ratingId) ");
            score += await AssertTrueAsync(async () =>
            {
                long ratingId = 0;
                using (RatingContext db = new RatingContext())
                {
                    ratingId = db.School_Rating.First(sr => sr.SR_User_Phonenr == "09990").SR_ID;
                }
                RatingDto result = await SendAsync<RatingDto>(
                    HttpMethod.Put,
                    $"{baseUrl}/schoolrating/{ratingId}", new RatingDto
                    {
                        Value = 2
                    });
                using (RatingContext db = new RatingContext())
                {
                    return db.School_Rating.Find(ratingId).SR_Value == 2;
                }
            });

            // *************************************************************************************
            Console.Write("PUT /api/schoolrating/(ratingId) liefert HTTP 400, wenn der User eine Bewertung des aktuellen Tages ändern will? ");
            score += await AssertTrueAsync(async () =>
            {
                long ratingId = 0;
                using (RatingContext db = new RatingContext())
                {
                    ratingId = db.School_Rating.First(sr => sr.SR_User_Phonenr == "09991").SR_ID;
                }
                try
                {
                    RatingDto result = await SendAsync<RatingDto>(
                        HttpMethod.Put,
                        $"{baseUrl}/schoolrating/{ratingId}", new RatingDto
                        {
                            Value = 2
                        });
                }
                catch (HttpException e) when (e.Status == 400)
                { return true; }
                return false;
            });

            // *************************************************************************************
            Console.Write("DELETE /api/schoolrating/(ratingId) ");
            score += await AssertTrueAsync(async () =>
            {
                long ratingId = 0;
                using (RatingContext db = new RatingContext())
                {
                    ratingId = db.School_Rating.First(sr => sr.SR_User_Phonenr == "09990").SR_ID;
                }
                await SendAsync(HttpMethod.Delete, $"{baseUrl}/schoolrating/{ratingId}", null);
                using (RatingContext db = new RatingContext())
                {
                    return db.School_Rating.Find(ratingId) == null;
                }
            });

            // *************************************************************************************
            double percent = score / 8.0;
            int note = 
                percent > 0.875 ? 1 : 
                percent > 0.75 ? 2 : 
                percent > 0.625 ? 3 : 
                percent > 0.5 ? 4 : 5;
            Console.WriteLine($"{score} von 8 Punkten erreicht. Note: {note}.");
            Console.ReadLine();

        }

        static async Task<int> AssertTrueAsync(Func<Task<bool>> action)
        {
            try
            {
                if (await action())
                {
                    Console.WriteLine("1");
                    return 1;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
            Console.WriteLine("0");
            return 0;
        }

        static async Task<string> SendAsync(HttpMethod method, string url, object requestData)
        {
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
                throw new ArgumentException("Invalid method");
            }
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpException($"{url}: HTTP {response.StatusCode}")
                {
                    Status = (int)response.StatusCode
                };
            }
            string result = await response.Content.ReadAsStringAsync();
            return result;
        }
        static async Task<T> SendAsync<T>(HttpMethod method, string url, object requestData)
        {
            string result = await SendAsync(method, url, requestData);
            return JsonSerializer.Deserialize<T>(result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        }

        static void SeedDb()
        {
            using (RatingContext db = new RatingContext())
            {
                db.School_Rating.RemoveRange(db.School_Rating.Where(sr => sr.SR_User_School <= 9999));
                db.User.RemoveRange(db.User.Where(u => u.U_School <= 9999));
                db.School.RemoveRange(db.School.Where(s => s.S_Nr <= 9999));
                db.SaveChanges();
            }
            using (RatingContext db = new RatingContext())
            {
                db.School.Add(new School { S_Nr = 9998, S_Name = "Testschule 1" });
                db.School.Add(new School { S_Nr = 9999, S_Name = "Testschule 2" });
                var users = Enumerable.Range(0, 3).Select(i => new User()
                {
                    U_School = 9999,
                    U_Phonenr = "0999" + i
                }).ToArray();
                db.User.AddRange(users);
                db.School_Rating.Add(new School_Rating
                {
                    SR_Date = DateTime.Now.Date.AddDays(-1),
                    SR_User_Navigation = users[0],
                    SR_Value = 5
                });
                db.School_Rating.Add(new School_Rating
                {
                    SR_Date = DateTime.Now.Date.AddDays(0),
                    SR_User_Navigation = users[1],
                    SR_Value = 5
                });
                db.SaveChanges();
            }
        }
    }
}
