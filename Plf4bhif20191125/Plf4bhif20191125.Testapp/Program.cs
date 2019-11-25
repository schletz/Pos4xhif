using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Plf4bhif20191125.Testapp.Model;
using Plf4bhif20191125.Dto;

namespace Plf4bhif20191125.Testapp
{
    class Program
    {
        static readonly HttpClient _client = new HttpClient();
        static void Main(string[] args)
        {
            SeedDb();
            MainAsync().Wait();
        }
        static void SeedDb()
        {
            using (RegistrationContext db = new RegistrationContext())
            {
                db.Registration.RemoveRange(db.Registration.Where(r => r.R_Lastname.StartsWith("Testlastname")));
                db.SaveChanges();
            }
            using (RegistrationContext db = new RegistrationContext())
            {
                db.Registration.Add(new Registration
                {
                    R_Firstname = "Testfirstname",
                    R_Lastname = "Testlastname",
                    R_Email = "test@mail.at",
                    R_Registration_Date = new DateTime(2019, 11, 23),
                    R_DepartmentNavigation = db.Department.Find("HIF"),
                    R_Date_of_Birth = new DateTime(2006, 1, 1),
                });
                db.Registration.Add(new Registration
                {
                    R_Firstname = "Testfirstname2",
                    R_Lastname = "Testlastname2",
                    R_Email = "test2@mail.at",
                    R_Registration_Date = new DateTime(2019, 11, 23),
                    R_DepartmentNavigation = db.Department.Find("HIF"),
                    R_Date_of_Birth = new DateTime(2006, 1, 2),
                });
                db.Registration.Add(new Registration
                {
                    R_Firstname = "Testfirstname3",
                    R_Lastname = "Testlastname3",
                    R_Email = "test3@mail.at",
                    R_Admitted_Date = new DateTime(2020, 3, 21),
                    R_Registration_Date = new DateTime(2019, 11, 23),
                    R_DepartmentNavigation = db.Department.Find("HIF"),
                    R_Date_of_Birth = new DateTime(2006, 1, 2),
                });
                db.SaveChanges();
            }
        }


        static async Task MainAsync()
        {
            int score = 0;
            string baseUrl = "http://localhost:5000/api";

            // *************************************************************************************
            // Mustertest: Liefert alle in der DB gespeicherten Vor- und Zunamen.
            // *************************************************************************************
            Console.Write("MUSTERTEST: GET api/registration/test ");
            await AssertTrueAsync(async () =>
            {
                IEnumerable<RegistrationDto> result = await SendAsync<IEnumerable<RegistrationDto>>(
                    HttpMethod.Get,
                    $"{baseUrl}/registration/test",
                    null);
                using (RegistrationContext db = new RegistrationContext())
                {
                    return
                        result.Any(r => r.Lastname == "Testlastname") &&
                        result.Count() == db.Registration.Count();
                }
            });

            // *************************************************************************************
            // Liefert die für die HIF Abteilung angemeldeten Schüler
            // *************************************************************************************
            Console.Write("GET /api/registration/hif ");
            score += await AssertTrueAsync(async () =>
            {
                IEnumerable<RegistrationDto> result = await SendAsync<IEnumerable<RegistrationDto>>(
                    HttpMethod.Get,
                    $"{baseUrl}/registration/hif",
                    null);
                using (RegistrationContext db = new RegistrationContext())
                {
                    var registrations = db.Registration.Where(r => r.R_Department == "HIF");
                    return
                        result.Count() == db.Registration.Where(r => r.R_Department == "HIF").Count() &&
                        result.All(r => r.Department == "HIF");
                }

            });

            // *************************************************************************************
            // Liefert die für die HBG Abteilung bestätigten Schüler 
            // (R_Admitted_Date ist nicht null)
            // *************************************************************************************
            Console.Write("GET /api/admitted/hbg ");
            score += await AssertTrueAsync(async () =>
            {
                IEnumerable<RegistrationDto> result = await SendAsync<IEnumerable<RegistrationDto>>(
                    HttpMethod.Get,
                    $"{baseUrl}/admitted/hbg",
                    null);
                using (RegistrationContext db = new RegistrationContext())
                {
                    int dbCount = db.Registration.Where(r =>
                        r.R_Department == "HBG" &&
                        r.R_Admitted_Date != null).Count();
                    return
                        result.Count() == dbCount &&
                        result.All(r => r.Department == "HBG");
                }

            });

            // *************************************************************************************
            // Schreiben der Anmeldung: Wird der Datensatz geschrieben, R_Registration_Date
            // korrekt gesetzt und R_Admitted_Date leer gelassen?
            // *************************************************************************************
            Console.Write("POST /api/registration ");
            score += await AssertTrueAsync(async () =>
            {
                RegistrationDto result = await SendAsync<RegistrationDto>(
                    HttpMethod.Post,
                    $"{baseUrl}/registration", new RegistrationDto
                    {
                        Firstname = "AAA1",
                        Lastname = "Testlastname10",
                        Email = "CCC@mail.at",
                        Department = "HIF",
                        Date_of_Birth = new DateTime(2006, 1, 1)
                    });
                using (RegistrationContext db = new RegistrationContext())
                {
                    Registration newReg = db.Registration.FirstOrDefault(r => r.R_Lastname == "Testlastname10");
                    if (newReg == null) { return false; }
                    return
                        result.ID != 0 &&
                        result.Registration_Date > DateTime.Now.AddMinutes(-1) &&
                        newReg.R_Registration_Date > DateTime.Now.AddMinutes(-1) &&
                        newReg.R_Admitted_Date == null;
                }
            });

            // *************************************************************************************
            // Schreiben der Anmeldung: Wird HTTP 400 geliefert, wenn Daten die DB Constraints
            // (FK, NOT NULL, ...) verletzen, gesendet wurden?
            // *************************************************************************************
            Console.Write("POST /api/registration liefert HTTP 400 bei ungültigen Daten? ");
            score += await AssertTrueAsync(async () =>
            {
                int exceptionCount = 0;
                try
                {
                    await SendAsync(
                        HttpMethod.Post,
                        $"{baseUrl}/registration", new RegistrationDto
                        {
                            Firstname = "AAA2",
                            Lastname = "Testlastname11",
                            Email = "CCC2@mail.at",
                            Department = "?",             // Diese Abteilung gibt es nicht.
                            Date_of_Birth = new DateTime(2006, 1, 1)
                        });
                }
                catch (HttpException e) when (e.Status == 400)
                {
                    exceptionCount++;
                }
                try
                {
                    await SendAsync(
                            HttpMethod.Post,
                            $"{baseUrl}/registration", new RegistrationDto
                            {
                                Firstname = "AAA3",                  // Lastname ist null
                                Email = "CCC3@mail.at",
                                Department = "HIF",
                                Date_of_Birth = new DateTime(2006, 1, 1)
                            });
                }
                catch (HttpException e) when (e.Status == 400)
                {
                    exceptionCount++;
                }
                return exceptionCount == 2;
            });


            // *************************************************************************************
            // Bestätigung des Schulplatzes: Ist das aktuelle Datum in R_Admitted_Date eingetragen?
            // *************************************************************************************
            Console.Write("PUT /api/registration/admit/(regId) ");
            score += await AssertTrueAsync(async () =>
            {
                long id = 0;
                using (RegistrationContext db = new RegistrationContext())
                {
                    id = db.Registration.First(r => r.R_Lastname == "Testlastname2").R_ID;
                }
                RegistrationDto result = await SendAsync<RegistrationDto>(
                    HttpMethod.Put,
                    $"{baseUrl}/registration/admit/{id}",
                    null);
                using (RegistrationContext db = new RegistrationContext())
                {
                    return
                        result.ID == id &&
                        result.Admitted_Date > DateTime.Now.AddMinutes(-1) &&
                        db.Registration.Find(id).R_Admitted_Date > DateTime.Now.AddMinutes(-1);
                }
            });

            // *************************************************************************************
            // Bestätigung des Schulplatzes: Wird HTTP 400 geliefert, wenn der Schüler nicht existiert?
            // *************************************************************************************
            Console.Write("PUT /api/registration/admit/(regId) liefert HTTP 400, wenn Schüler nicht existiert? ");
            score += await AssertTrueAsync(async () =>
            {
                try
                {
                    RegistrationDto result = await SendAsync<RegistrationDto>(
                        HttpMethod.Put,
                        $"{baseUrl}/registration/admit/99999",
                        null);
                }
                catch (HttpException e) when (e.Status == 400)
                { return true; }
                return false;
            });

            // *************************************************************************************
            // Ändern der Daten: Werden die Daten aktualisiert, das Anmeldedatum aber nicht?
            // *************************************************************************************
            Console.Write("PUT /api/registration/(regId) ");
            score += await AssertTrueAsync(async () =>
            {
                long id = 0;
                DateTime oldDate = DateTime.Now;
                using (RegistrationContext db = new RegistrationContext())
                {
                    Registration reg = db.Registration.First(r => r.R_Lastname == "Testlastname");
                    id = reg.R_ID;
                    oldDate = reg.R_Registration_Date;
                }
                RegistrationDto result = await SendAsync<RegistrationDto>(
                    HttpMethod.Put,
                    $"{baseUrl}/registration/{id}",
                    new RegistrationDto
                    {
                        Firstname = "Testfirstname",
                        Lastname = "Testlastname",
                        Email = "test4@mail.at",           // Email wird geändert
                        Department = "HWIT",               // Abteilung wird geändert
                        Date_of_Birth = new DateTime(2006, 1, 1)
                    });
                using (RegistrationContext db = new RegistrationContext())
                {
                    Registration reg = db.Registration.Find(id);
                    return
                        result.ID == id &&
                        reg.R_Registration_Date == oldDate &&
                        reg.R_Department == "HWIT" &&
                        reg.R_Email == "test4@mail.at";
                }
            });

            // *************************************************************************************
            // Ändern der Daten: Wird HTTP 400 geliefert, wenn der Schüler nicht existiert?
            // *************************************************************************************
            Console.Write("PUT /api/registration/(regId) liefert HTTP 400, wenn Schüler nicht existiert? ");
            score += await AssertTrueAsync(async () =>
            {
                try
                {
                    RegistrationDto result = await SendAsync<RegistrationDto>(
                        HttpMethod.Put,
                        $"{baseUrl}/registration/99999",
                        null);
                }
                catch (HttpException e) when (e.Status == 400)
                { return true; }
                return false;
            });

            // *************************************************************************************
            // Ändern der Daten: Wird HTTP 400 (BadRequest) geliefert, wenn der Schüler schon 
            // aufgenommen wurde, die Abteilung jedoch geändert werden soll?
            // *************************************************************************************
            Console.Write("PUT /api/registration/(regId) liefert HTTP 400 bei Abteilungsänderung von Aufgenommenen? ");
            score += await AssertTrueAsync(async () =>
            {
                long id = 0;
                using (RegistrationContext db = new RegistrationContext())
                {
                    id = db.Registration.First(r => r.R_Lastname == "Testlastname3").R_ID;
                }
                // Diese Änderung soll funktionieren (nur die Mail wird geändert)
                await SendAsync(
                    HttpMethod.Put,
                    $"{baseUrl}/registration/{id}",
                    new RegistrationDto
                    {
                        Firstname = "Testfirstname3",
                        Lastname = "Testlastname3",
                        Email = "test31@mail.at",
                        Department = "HIF",
                        Date_of_Birth = new DateTime(2006, 1, 2),
                    });
                using (RegistrationContext db = new RegistrationContext())
                {
                    if (db.Registration.Find(id).R_Email != "test31@mail.at") { return false; }
                }
                // Die Änderung der Abteilung darf nicht mehr funktionieren, da dieser Schüler
                // schon ein R_Admitted_Date gesetzt hat.
                try
                {
                    await SendAsync(
                        HttpMethod.Put,
                        $"{baseUrl}/registration/{id}",
                        new RegistrationDto
                        {
                            Firstname = "Testfirstname3",
                            Lastname = "Testlastname3",
                            Email = "test31@mail.at",
                            Department = "FIT",
                            Date_of_Birth = new DateTime(2006, 1, 2),
                        });
                }
                catch (HttpException e) when (e.Status == 400)
                {
                    return true;
                }
                return false;
            });

            // *************************************************************************************
            // Löschen einer Anmeldung
            // *************************************************************************************
            Console.Write("DELETE /api/registration/(regId) ");
            score += await AssertTrueAsync(async () =>
            {
                long id = 0;
                using (RegistrationContext db = new RegistrationContext())
                {
                    id = db.Registration.First(r => r.R_Lastname == "Testlastname").R_ID;
                }
                await SendAsync(
                    HttpMethod.Delete,
                    $"{baseUrl}/registration/{id}",
                    null);
                using (RegistrationContext db = new RegistrationContext())
                {
                    return
                        db.Registration.Find(id) == null;
                }
            });

            // *************************************************************************************
            // Löschen einer Anmeldung: Liefert HTTP 400 (BadRequest), wenn der Schüler nicht existiert.
            // *************************************************************************************
            Console.Write("DELETE /api/registration/(regId) liefert HTTP 400 wenn nicht vorhanden? ");
            score += await AssertTrueAsync(async () =>
            {
                try
                {
                    await SendAsync(
                        HttpMethod.Delete,
                        $"{baseUrl}/registration/99999",
                        null);
                }
                catch (HttpException e) when (e.Status == 400)
                { return true; }
                return false;
            });

            // *************************************************************************************
            // Löschen einer Anmeldung: Liefert HTTP 400 (BadRequest), wenn der Schüler schon 
            // aufgenommen wurde.
            // *************************************************************************************
            Console.Write("DELETE /api/registration/(regId) liefert HTTP 400 wenn schon aufgenommen? ");
            score += await AssertTrueAsync(async () =>
            {
                long id = 0;
                using (RegistrationContext db = new RegistrationContext())
                {
                    id = db.Registration.First(r => r.R_Lastname == "Testlastname3").R_ID;
                }
                try
                {
                    await SendAsync(
                        HttpMethod.Delete,
                        $"{baseUrl}/registration/{id}",
                        null);
                }
                catch (HttpException e) when (e.Status == 400)
                { return true; }
                return false;
            });

            // *************************************************************************************
            int[] note = { 5, 5, 5, 5, 5, 5, 5, 4, 4, 3, 3, 2, 1 };
            Console.WriteLine($"{score} von 12 Punkten erreicht. Note: {note[score]}.");
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
    }
}
