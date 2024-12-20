# WebSettingsManager
Тестовый проект
Автор: Гулин Алексей Александрович

WebSettingsManager представляет из себя ASP.NET Web-API, позволяющее:
1) Создавать новых пользователей
2) Создавать конфигурации текста для каждого из пользователей
3) Сохранять и восстанавливать конфигурации на момент времени

Хранение данных реализовано в SQLite через EntityFramework.
Добавлена поддержка Swagger:
http://localhost:5197/swagger/index.html       - UI
http://localhost:5197/swagger/v1/swagger.json  - json
http://localhost:5197/swagger/v1/swagger.yaml  - yaml
