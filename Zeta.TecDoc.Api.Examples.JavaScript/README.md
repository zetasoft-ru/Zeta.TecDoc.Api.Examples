# Zeta.TecDoc.Api.Examples.JavaScript

Для удобной работы с Zeta TecDoc API в языке JavaScript используются библиотека [swagger-client](https://www.npmjs.com/package/swagger-client), которая позволяет получить описание всех методов по спецификации Swagger

### Создание клиента
```javascript
const swaggerOptions = {
    url: 'http://api.tecdoc.zetasoft.ru/swagger/v1/swagger.json',
    requestInterceptor: req => {
        req.headers["Content-Type"] = "application/json";
    }
};
const swaggerClient = await new SwaggerClient(swaggerOptions);
```

### Авторизация
```javascript
const tokenRequest = {
    requestBody: {
        "email": email,
        "password": password
    }
};               
const tokenResponse = await swaggerClient.apis.User.GetUserToken({}, tokenRequest);
const token = tokenResponse.obj.value;
const currentRequestInterceptor = swaggerClient.requestInterceptor;
swaggerClient.requestInterceptor = req => {
    currentRequestInterceptor(req);
    req.headers["Authorization"] = `Bearer ${token}`;
};
```

### Вызов операции клиента
```javascript 
const partsResponse = await swaggerClient.apis.CrossCode.GetCrossCodesByVendorCode({
        vendorCode: vendorCode,
        languageCode: "RU",
        regionCode: "RU"
    });
```
> где __CrossCode__ наименование контроллера, а __GetCrossCodesByVendorCode__ - наименование операции

Описание всех доступных методов можно найти [здесь](http://api.tecdoc.zetasoft.ru/api/index.html)
