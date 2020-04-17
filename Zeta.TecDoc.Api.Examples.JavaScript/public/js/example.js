const example = (function () {

    /** ПАвторизоваться и найти запчасти по коду (артикулу) */
    const find = async function () {

        try {
            const email = $("#email").val();
            const password = $("#password").val();
            const vendorCode = $("#vendorCode").val();

            // настройки SwaggerClient
            const swaggerOptions = {
                url: 'http://api.tecdoc.zetasoft.ru/swagger/v1/swagger.json',
                requestInterceptor: req => {
                    req.headers["Content-Type"] = "application/json";
                }
            };

            // запуск задания получения клиента
            const swaggerClient = await new SwaggerClient(swaggerOptions);

            const tokenRequest = {
                requestBody: {
                    "email": email,
                    "password": password
                }
            };

            // получение token для авторизации в методах                
            const tokenResponse = await swaggerClient.apis.User.GetUserToken({}, tokenRequest);

            // чтение ответа
            const token = tokenResponse.obj.value;

            // установка заголовка авторизации
            const currentRequestInterceptor = swaggerClient.requestInterceptor;
            swaggerClient.requestInterceptor = req => {
                currentRequestInterceptor(req);
                req.headers["Authorization"] = `Bearer ${token}`;
            };

            // запуска задания на получение уточнений по коду                            
            const partsResponse = await swaggerClient.apis.CrossCode.GetCrossCodesByVendorCode({
                vendorCode: vendorCode,
                languageCode: "RU",
                regionCode: "RU"
            });

            // чтение ответа
            var parts = partsResponse.obj.data;

            // вывод списка кроссов
            let html = `
<table class="table">
  <thead>
    <tr>
      <th scope="col">Код</th>
      <th scope="col">Производитель</th>
    </tr>
  </thead>
  <tbody>`;
            for (let i = 0; i < parts.length; i++) {
                html += `
    <tr>      
      <td>${parts[i].vendorCode}</td>
      <td>${parts[i].vendorName}</td>
    </tr>`;
            }
            html += `
  </tbody>
</table>`;
            var resultContainer = $("[data-result]");
            resultContainer.html(html);
        }
        catch (error) {
            if (error.status === 401)
                alert("Вы указали неправильный логин и пароль или ваша учетная запись неактивна");
            else if (error.status === 404)
                alert("По вашему запросу ничего не найдено");
            else
                alert(error);
        }
    };
    
    const init = function () {
        $(document).on("click", ".btn", find);
    };

    init();

    return {
    };

})();
