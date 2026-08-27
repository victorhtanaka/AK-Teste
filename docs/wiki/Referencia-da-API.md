# Referência da API REST

A API foi projetada segundo os padrões RESTful, com rotas versionadas sob `/api/v1/`, respostas fortemente tipadas em JSON e códigos de status HTTP semânticos.

---

## 1. Endpoints de Capitais

### `GET /api/v1/cities`
Retorna a lista completa das 27 capitais brasileiras cadastradas com UF, latitude e longitude.

**Exemplo de Resposta (200 OK):**
```json
[
  {
    "id": 7,
    "name": "Brasília",
    "state": "DF",
    "latitude": -15.7797,
    "longitude": -47.9297
  }
]
```

---

## 2. Endpoints Climáticos

### `GET /api/v1/weather/current?cityId={id}`
Retorna a última leitura climática persistida para a capital solicitada.

### `GET /api/v1/weather/stats/today?cityId={id}`
Retorna as estatísticas consolidadas do dia atual (mínima, máxima, média ponderada, sensação térmica, umidade e vento).

### `GET /api/v1/weather/history?cityId={id}&startDateUtc={start}&endDateUtc={end}`
Retorna a série temporal de pontos históricos para alimentação dos gráficos vetoriais.

### `GET /api/v1/weather/summary/national`
Retorna o panorama comparativo de todas as capitais no momento (capital mais quente, mais fria e médias nacionais).

### `GET /api/v1/weather/export?cityId={id}&startDateUtc={start}&endDateUtc={end}`
Exporta o histórico da capital selecionada em formato **CSV UTF-8** para download imediato.

### `POST /api/v1/weather/sync`
Dispara manualmente um ciclo imediato de sincronização climática para todas as 27 capitais.

### `GET /api/v1/weather/diagnostics`
Retorna o status em tempo real da conexão com a OpenWeatherMap e o detalhamento por capital.
