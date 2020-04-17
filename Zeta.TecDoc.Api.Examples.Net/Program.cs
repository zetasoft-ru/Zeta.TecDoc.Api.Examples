using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Zeta.TecDoc.Api.Examples.Net
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Пожалуйста, введите ваш логин (email) и нажмите Enter");
            var email = Console.ReadLine();

            Console.WriteLine("Пожалуйста, введите ваш пароль и нажмите Enter");
            var password = Console.ReadLine();

            Console.WriteLine("Пожалуйста, введите код (артикул) запчасти и нажмите Enter");
            var vendorCode = Console.ReadLine();

            Find(email, password, vendorCode)
                .GetAwaiter()
                .GetResult();
        }

        private static async Task Find(string email, string password, string vendorCode)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://api.tecdoc.zetasoft.ru");

                var tokenRequest = new
                {
                    Email = email,
                    Password = password
                };

                var tokenRequestJson = JsonConvert.SerializeObject(tokenRequest);

                // получение token для авторизации в методах
                var tokenResponse = await client.PostAsync("/api/User/Token", new StringContent(tokenRequestJson, Encoding.UTF8, "application/json"));

                // проверка ответа
                if (!ValidateResponse(tokenResponse))
                    return;

                // чтение ответа
                var token = await ReadObject(tokenResponse);

                // установка заголовка авторизации
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token["value"].Value<string>());

                // получение уточнений по коду
                var partsResponse = await client.GetAsync($"/api/RU-RU/CrossCode/Part?vendorCode={vendorCode}");

                // проверка ответа
                if (!ValidateResponse(partsResponse))
                    return;

                // чтение ответа
                var parts = await ReadObject(partsResponse);

                // вывод списка кроссов
                foreach (var part in parts["data"].AsEnumerable())
                {
                    Console.WriteLine($"{part["vendorCode"].Value<string>()} {part["vendorName"].Value<string>()}");
                }

                Console.ReadLine();
            }
        }

        /// <summary>
        /// Проверка ответа
        /// </summary>
        /// <param name="httpResponse">Ответ от API</param>
        /// <returns>true если статус ответ 200, иначе false</returns>
        private static bool ValidateResponse(HttpResponseMessage httpResponse)
        {
            if (!httpResponse.IsSuccessStatusCode)
            {
                if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    Console.WriteLine("Вы указали неправильный логин и пароль или ваша учетная запись неактивна");
                }
                if (httpResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Console.WriteLine("По вашему запросу ничего не найдено");

                }
                else
                {
                    var content = httpResponse.Content.ReadAsStringAsync().Result;
                    Console.WriteLine(content);
                }
                Console.ReadLine();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Чтение ответа JObject
        /// </summary>
        /// <param name="httpResponse">Ответ от API</param>
        /// <returns>JObject</returns>
        private static async Task<JObject> ReadObject(HttpResponseMessage httpResponse)
        {
            var json = await httpResponse.Content.ReadAsStringAsync();
            var jObject = JObject.Parse(json);
            return jObject;
        }
    }
}
