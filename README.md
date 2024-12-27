# WebSettingsManager
Тестовый проект
Автор: Гулин Алексей Александрович

WebSettingsManager представляет из себя ASP.NET Web-API, позволяющее:
1) Создавать новых пользователей
2) Создавать конфигурации текста для каждого из пользователей
3) Сохранять и восстанавливать конфигурации на момент времени
Создан интерфейс пользователя, позволяющий подписываться на обновления множества конфигураций
Доступ через http://localhost:5197

Хранение данных реализовано в SQLite через EntityFramework.
Добавлена поддержка Swagger:
http://localhost:5197/swagger/index.html       - UI,
http://localhost:5197/swagger/v1/swagger.json  - json,
http://localhost:5197/swagger/v1/swagger.yaml  - yaml
