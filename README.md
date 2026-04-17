<p align="center">
  <img src="Api/wwwroot/resources/images/logo.svg" alt="Blueprint logo" width="180">
</p>

# Blueprint 
![Platform](https://img.shields.io/badge/platform-Web-green) ![Language](https://img.shields.io/badge/language-C%23-blue) ![Frontend](https://img.shields.io/badge/frontend-HTML%2FCSS%2FJS-orange) ![Database](https://img.shields.io/badge/database-PostgreSQL-316192) ![Monitoring](https://img.shields.io/badge/monitoring-Grafana%20%2B%20Prometheus-F46800) ![Status](https://img.shields.io/badge/status-in%20development-yellow)

Blueprint — платформа для хранения и поиска студенческих проектов прошлых лет с материалами по всем этапам разработки.

Этот репозиторий содержит веб-платформу, созданную в рамках курсов «Полезное приложение» и «Проектный практикум». Проект помогает студентам быстрее стартовать работу над своим продуктом, смотреть примеры прошлых лет и ориентироваться в требованиях курса.

# Contents
- [About](#about)
- [Проектные материалы](#проектные-материалы)
- [Шаги для установки](#шаги-для-установки)
- [Tech Stack](#tech-stack)
- [Monitoring](#monitoring)
- [Authors](#authors)

# About
Что можно делать в Blueprint:
- смотреть проекты прошлых лет в одном месте
- искать проекты по тегам, технологиям и параметрам
- открывать карточку проекта с описанием и составом команды
- переходить к материалам проекта: CustDev, MVP, roadmap, описание и ссылки на продукт
- вдохновляться чужими идеями и проверять, не реализована ли уже похожая

# Проектные материалы
- [Customer Development](https://buildin.ai/5321986b-ca6f-447f-ab72-edc9dac71aa7)
- [Метрики](https://buildin.ai/f0fa512c-5e80-4a0a-8cb7-c802af876810)
- [Роадмап разработки MVP](https://buildin.ai/8a22dd14-aefe-4956-9f7d-253f55ef2641)
- [MVP](https://buildin.ai/07ea870e-790b-4d38-a4b3-70f9e2b20c8f)

# Шаги для установки
1. Склонируйте репозиторий.
2. Создайте `.env` на основе файла `.env.example`.
3. Заполните переменные окружения для ASP.NET, PostgreSQL и Grafana.
4. Запустите проект:
   ```bash
   docker compose up -d --build
   ```
5. После запуска будут доступны:
   - backend: `http://localhost:8080`
   - Prometheus: `http://localhost:9090`
   - Grafana: `http://localhost:3000`

Для запуска тестов:
```bash
dotnet test
```

# Tech Stack
### Frontend
- HTML
- CSS
- JavaScript

Статический фронтенд лежит в `Api/wwwroot`. Логотип и графические ресурсы лежат в `Api/wwwroot/resources/images`. 

### Backend
- C#
- ASP.NET Core Web API
- MediatR
- Swagger

Основные ручки проекта:
- `GET /api/projects/projects`
- `GET /api/projects/project/{id}`
- `GET /api/projects/tags`
- `HEAD /api/metrics/rebuild_open_cohorts_retention`

### Database
- PostgreSQL
- Entity Framework Core

В проекте есть сущности для проектов, участников, тегов, файлов и таблиц метрик.

### Parsing and data
- CSV parser для загрузки проектов из таблицы
- seed data для тегов и типов тегов

### Monitoring
- OpenTelemetry
- Prometheus
- Grafana

### DevOps
- Docker
- Docker Compose
- Nginx
- GitHub Actions

### Tests
- NUnit

### Архитектура
Основные части решения:
- **Api** — ASP.NET Core приложение, контроллеры и статические файлы фронтенда
- **Infrastructure** — доступ к данным, репозитории, парсеры, конфигурация БД
- **Client.Models** — DTO и общие модели
- **Core** — доменная и прикладная логика
- **Test** — тесты парсера и метрик

# Monitoring
В проекте предусмотрен мониторинг пользовательской активности и технических метрик.

Используется связка Prometheus + Grafana, а также отдельная логика для метрик просмотров и retention. Для деплоя и проксирования запросов используется Nginx.

# Authors
- Арина Кискина — дизайн, верстка, пользовательский интерфейс, страницы каталога и проекта, интеграция фронта с API  
  - [reqied](https://github.com/reqied)
- Артем Скворок — основные project API-ручки, хендлеры запросов, обработка ошибок, работа с backend-логикой  
  - [fan4cz](https://github.com/fan4cz)
- Илья Котов — метрики, cookie/session tracking, metric backend, CSV-парсер таблицы проектов, интеграция аналитики в flow просмотра  
  - [Kitiketov](https://github.com/Kitiketov)
- Екатерина Толканюк — схема БД, сущности, миграции, сиды, теги и общая интеграция серверной части  
  - [Jlychee](https://github.com/Jlychee)
- Казанцева Илона — деплой, nginx, HTTPS, Docker и CI/CD  
  - [prlzrakk](https://github.com/prlzrakk)
