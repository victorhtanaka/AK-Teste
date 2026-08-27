# Documentação da API RESTful

A API segue o padrão RESTful com versionamento semântico (`/api/v1/*`) e documentação interativa via Swagger / OpenAPI.

---

## 1. Endpoints de Capitais

### `GET /api/v1/cities`
Retorna a listagem das 27 capitais brasileiras cadastradas.

**Resposta de Sucesso (200 OK):**
```json
[
  {
    "id": 7,
    "name": "Brasília",
    "state": "DF",
    "latitude": -15.7797,
    "longitude": -47.9297,
    "openWeatherCityId": null,
    "fullDisplayName": "Brasília - DF"
  },
  {
    "id": 16,
    "name": "Curitiba",
    "state": "PR",
    "latitude": -25.4278,
    "longitude": -49.2731,
    "openWeatherCityId": null,
    "fullDisplayName": "Curitiba - PR"
  }
]
```

### `GET /api/v1/cities/{id}`
Retorna os dados de uma capital específica pelo seu ID numérico.

---

## 2. Endpoints Climáticos

### `GET /api/v1/weather/current?cityId={id}`
Retorna a última leitura climática registrada para a capital informada.

**Resposta de Sucesso (200 OK):**
```json
{
  "id": 104,
  "cityId": 7,
  "cityName": "Brasília",
  "state": "DF",
  "collectedAtUtc": "2026-08-25T14:30:00Z",
  "temperatureC": 26.5,
  "feelsLikeC": 26.0,
  "tempMinC": 22.0,
  "tempMaxC": 28.0,
  "humidity": 55,
  "pressureHpa": 1014.0,
  "windSpeedMs": 3.5,
  "weatherDescription": "Céu Limpo",
  "weatherIcon": "01d"
}
```

### `GET /api/v1/weather/stats/today?cityId={id}`
Retorna as estatísticas consolidadas do dia corrente (máxima, mínima, média ponderada, sensação térmica média, umidade média, velocidade média do vento e condição predominante).

**Resposta de Sucesso (200 OK):**
```json
{
  "cityId": 7,
  "cityName": "Brasília",
  "state": "DF",
  "dateUtc": "2026-08-25T00:00:00Z",
  "tempMaxC": 28.5,
  "tempMinC": 18.0,
  "tempAvgC": 24.2,
  "feelsLikeAvgC": 24.8,
  "humidityAvg": 58.0,
  "pressureAvgHpa": 1013.0,
  "windSpeedAvgMs": 3.8,
  "dominantWeatherDescription": "Céu Limpo",
  "dominantWeatherIcon": "01d",
  "totalReadings": 24,
  "lastUpdatedUtc": "2026-08-25T14:30:00Z"
}
```

### `GET /api/v1/weather/history?cityId={id}&startDateUtc={start}&endDateUtc={end}`
Retorna a série histórica de leituras climáticas dentro do intervalo especificado para alimentar os gráficos.

### `POST /api/v1/weather/sync`
Dispara sob demanda um ciclo completo de coleta climática para as 27 capitais.

---

## 3. Endpoint de Monitoramento (Health Check)

### `GET /health`
Retorna status `Healthy` com código `200 OK` indicando que a API e seus componentes estão operacionais.
