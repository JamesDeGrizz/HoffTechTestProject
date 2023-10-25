# HoffTechTestProject

### Классы для получения курса
- клиент сгенерил так: 
    - ПКМ на проекте -> Add... -> Service Reference -> WCF Web Service -> https://www.cbr.ru/DailyInfoWebServ/DailyInfo.asmx?WSDL
- dto сгенерил так:
    - при помощи сгенерённого клиента вызвал GetCursOnDateAsync и скопировал часть с xml с буфер
    - создал пустой файл в проекте
    - Edit -> Paste Special -> Paste XML As Classes
	
### Конфигурационные параметры
- CbrUrl - адрес API ЦБ РФ
- CircleRadius - радиус круга для проверки вхождения
- CurrencyCode - код валюты
- RetryIntervalMinutes - интервал в минутах для триггеров, которые заменяют стандартные, в случаях, когда не получается получить или обновить курс валюты

### Обновление курса валюты
Курсами занимаются 3 job'а:
- InitializeRatesJob - инициализирует курсы на все доступные дни. Срабатывает 1 раз при запуске приложения. Если что-то пошло не так, меняет свой триггер на работу раз в RetryIntervalMinutes до тех пор, пока курс не будет проинициализирован.
- TomorrowRateJob - получает курс на завтрашний день. Срабатывает 1 раз ежедневно по будним дням в 11:30. Если что-то пошло не так, меняет свой триггер на работу раз в RetryIntervalMinutes до тех пор, пока курс на завтрашний день не будет получен. В худшем случае остановится в 23:50, триггер будет заменён на обычный, со срабатыванием в 11:30.
- MoveRatesJob - смещает курс (курс на завтра становится сегодняшним, сегодняшний - вчерашним, вчерашний - позавчерашним). Запускается ежедневно в 00:00.

### Спорные моменты
Геометрическая часть:
- попадание в радиус считается вхождением
- попадание в ось работает с приоритетом направо и наверх. Например, координаты (0, 0) будут считаться попавшими в 1 четверть, а (0, -3) - в 4

Курс:
- базу решил не использовать, потому что база для 4 значений - это перебор
- не вытащил в настройки время запуска триггеров
- не знаю как поведёт себя апи ЦБ РФ в выходные и праздничные дни, в частности, GetLatestDate. Возможно, логика обновления курсов сломается
- не учтены выходные и праздничные дни, но чтобы сделать эту часть правильно, потребуется интеграция с каким-нибудь "Консультант плюс", а это уже, как говорится, отдельная тема для диплома

Контроллер:
- тексты ошибок придумывались как будто это публичное апи, а не для внутреннего использования, то есть без разглашения деталей. Если нужны детали - смотреть в логи