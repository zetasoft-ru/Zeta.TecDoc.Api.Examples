# Zeta.TecDoc.Api.Examples.NET

Для удобной работы с Zeta TecDoc API в среде .NET вы можете скачать нагу библиотеку.

### Создание клиента
```csharp
var client = new HttpClient();
client.BaseAddress = new Uri("http://api.tecdoc.zetasoft.ru");
```

### Авторизация
```csharp
var tokenRequest = new
{
    Email = email,
    Password = password
};
var tokenRequestJson = JsonConvert.SerializeObject(tokenRequest);
var tokenResponse = await client.PostAsync("/api/User/Token", new StringContent(tokenRequestJson, Encoding.UTF8, "application/json"));
var tokenResponseContent = await tokenResponse.Content.ReadAsStringAsync();
var tokenObject = JObject.Parse(tokenResponseContent);
client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenObject["value"].Value<string>());
```

### Вызов действия
```csharp 
var partsResponse = await client.GetAsync($"/api/RU-RU/CrossCode/Part?vendorCode={vendorCode}");
var partsResponseContent = await partsResponse.Content.ReadAsStringAsync();
var partsObject = JObject.Parse(partsResponseContent);
```
> где __CrossCode__ наименование контроллера, а __Part__ - наименование действия

Описание всех доступных методов можно найти [здесь](http://api.tecdoc.zetasoft.ru/api/index.html)
