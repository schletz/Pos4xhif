using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;

using System.Threading.Tasks;
using System.Linq;
using System.Text;
using Plf4chif.Test.Model;
using System.Collections.Generic;

namespace Plf4chif.Test
{
    class Program
    {
        static readonly HttpClient client = new HttpClient();
        static readonly string baseUrl = "http://localhost:8088";
        static void Main(string[] args)
        {
            int points = 0, result = 0;
            SeedDb();
            result = TestGetPruefung().Result;
            points += result;
            Console.WriteLine($"Teste GET /api/pruefung:           {result} P von 2");

            result = TestBadRequest().Result;
            points += result;
            Console.WriteLine($"Teste GET /api/pruefung/abc:       {result} P von 1");

            result = TestGetPruefung(1001).Result;
            points += result;
            Console.WriteLine($"Teste GET /api/pruefung/1001:      {result} P von 2");

            result = TestPostPruefung().Result;
            points += result;
            Console.WriteLine($"Teste POST /api/pruefung:          {result} P von 2");

            result = TestDeletePruefung().Result;
            points += result;
            Console.WriteLine($"Teste DELETE /api/pruefung:        {result} P von 2");

            result = TestPutPruefung().Result;
            points += result;
            Console.WriteLine($"Teste PUT /api/pruefung:           {result} P von 2");

            result = TestGetPruefung(null, true).Result;
            points += result;
            Console.WriteLine($"Teste GET /api/pruefung/negative:  {result} P von 2");

            double percent = points / 13.0;
            int mark = percent > 7.0 / 8 ? 1 : percent > 6.0 / 8 ? 2 : percent > 5.0 / 8 ? 3 : percent > 4.0 / 8 ? 4 : 5;
            Console.WriteLine($"Erreichte Punkte: {points} von 13. Note: {mark}");
            Console.WriteLine("Test beendet. Drücke ENTER.");
            Console.ReadLine();
        }
        static async Task<int> TestGetPruefung(long? pruefId = null, bool onlyNegative = false)
        {
            int points = 0;
            string url = $"{baseUrl}/api/pruefung"
              + (onlyNegative ? "/negative" : "")
              + (pruefId != null ? $"/{pruefId}" : "");

            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                points++;
                try
                {
                    string data = await response.Content.ReadAsStringAsync();
                    if (pruefId != null) { data = "[" + data + "]"; }
                    JsonDocument doc = JsonDocument.Parse(data);
                    var pruefungen = doc.RootElement.EnumerateArray()
                        .Select(elem => new
                        {
                            P_ID = elem.GetProperty("P_ID").GetInt64(),
                            P_Note = elem.TryGetProperty("P_Note", out JsonElement noteElem) ? noteElem.GetInt64() : (long?)null
                        })
                        .Where(p => (!onlyNegative || p.P_Note == 5) && (pruefId == null || p.P_ID == pruefId));
                    using (SchuleContext db = new SchuleContext())
                    {
                        if (pruefungen.Count() == db.Pruefung.Where(p => (!onlyNegative || p.P_Note == 5) && (pruefId == null || p.P_ID == pruefId)).Count()) 
                        { 
                            points++; 
                        }
                    }
                }
                catch { }
            }
            return points;
        }
        static async Task<int> TestPostPruefung()
        {
            int points = 0;
            DateTime date = DateTime.Now.Date.AddDays(2);
            StringContent content = new StringContent(
                JsonSerializer.Serialize(new { P_Schueler = 1002, P_Datum = DateTime.Now.Date, P_Fach = "AM", P_Note = 2 }),
                Encoding.UTF8,
                "application/json"
            );
            HttpResponseMessage response = await client.PostAsync($"{baseUrl}/api/pruefung", content);
            if (response.IsSuccessStatusCode)
            {
                points++;
                using (SchuleContext db = new SchuleContext())
                {

                    if (db.Pruefung.FirstOrDefault(p => p.P_Schueler == 1002) != null)
                        points++;
                }
            }
            return points;
        }
        static async Task<int> TestPutPruefung()
        {
            int points = 0;
            string id = "";
            using (SchuleContext db = new SchuleContext())
            {
                id = db.Pruefung.FirstOrDefault(p => p.P_Schueler == 2001).P_ID.ToString();
            }
            StringContent content = new StringContent(
                JsonSerializer.Serialize(new { P_Schueler = 2001, P_Datum = DateTime.Now.Date, P_Fach = "E", P_Note = 2 }),
                Encoding.UTF8,
                "application/json"
            );

            HttpResponseMessage response = await client.PutAsync($"{baseUrl}/api/pruefung/{id}", content);
            if (response.IsSuccessStatusCode)
            {
                points++;
                using (SchuleContext db = new SchuleContext())
                {

                    if (db.Pruefung.Find(long.Parse(id)).P_Fach == "E")
                        points++;
                }
            }
            return points;
        }
        static async Task<int> TestDeletePruefung()
        {
            int points = 0;
            string id = "";
            using (SchuleContext db = new SchuleContext())
            {
                id = db.Pruefung.FirstOrDefault(p => p.P_Schueler == 2002).P_ID.ToString();
            }
            HttpResponseMessage response = await client.DeleteAsync($"{baseUrl}/api/pruefung/{id}");
            if (response.IsSuccessStatusCode)
            {
                points++;
                using (SchuleContext db = new SchuleContext())
                {
                    if (db.Pruefung.Find(long.Parse(id)) == null) { points++; }
                }
            }
            return points;
        }

        static async Task<int> TestBadRequest()
        {
            HttpResponseMessage response = await client.GetAsync($"{baseUrl}/api/pruefung/abc");
            if (response.StatusCode != HttpStatusCode.BadRequest) { return 0; }
            return 1;

        }


        static void SeedDb()
        {
            using (SchuleContext db = new SchuleContext())
            {
                db.Pruefung.RemoveRange(db.Pruefung);
                db.Pruefung.Add(new Pruefung
                {
                    P_ID = 1001,
                    P_Datum = DateTime.Now.Date.AddDays(0),
                    P_Note = 5L,
                    P_SchuelerNavigation = db.Schueler.Find(1001L),
                    P_FachNavigation = db.Fach.Find("POS1")
                });
                db.Pruefung.Add(new Pruefung
                {
                    P_ID = 1002,
                    P_Datum = DateTime.Now.Date.AddDays(1),
                    P_Note = 3L,
                    P_SchuelerNavigation = db.Schueler.Find(2001L),
                    P_FachNavigation = db.Fach.Find("AM")
                });
                db.Pruefung.Add(new Pruefung
                {
                    P_ID = 1003,
                    P_Datum = DateTime.Now.Date.AddDays(2),
                    P_Note = 2L,
                    P_SchuelerNavigation = db.Schueler.Find(2002L),
                    P_FachNavigation = db.Fach.Find("AM")
                });
                db.SaveChanges();
            }
        }
    }
}
