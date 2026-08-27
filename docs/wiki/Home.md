# Wiki Oficial — Dashboard Climático das Capitais Brasileiras

Bem-vindo à documentação oficial do projeto **Dashboard Climático das Capitais Brasileiras**, desenvolvido em **.NET 8 (C#)**, **Blazor WebAssembly**, **Entity Framework Core 8** e **SQL Server**.

---

## Navegação da Wiki

1. [Arquitetura Física e Lógica](Arquitetura-Fisica-e-Logica.md)
   - Diagramas vetoriais SVG e Mermaid da arquitetura.
   - Padrões de Clean Architecture, SOLID e Design Tokens.
   - Modelagem de dados e índices compostos.

2. [Instalação e Execução](Instalacao-e-Execucao.md)
   - Pré-requisitos de sistema.
   - Execução em 1 clique via scripts (`run-local.ps1` / `run-local.sh`).
   - Configuração de credenciais com .NET User Secrets.

3. [Deploy e Containerização](Deploy-e-Containers.md)
   - Execução completa com Docker Compose (App + SQL Server 2022).
   - Build multistage otimizado (.NET 8 SDK + ASP.NET 8 Runtime).
   - Configuração de variáveis de ambiente em produção.

4. [Referência da API REST](Referencia-da-API.md)
   - Especificação de todos os endpoints `/api/v1/*`.
   - DTOs de entrada e saída com exemplos JSON e exportação CSV.
   - Diagnósticos de status em tempo real da OpenWeatherMap.

5. [Testes Automatizados e Qualidade](Testes-e-Qualidade.md)
   - Pirâmide de testes (Unitários, Integração e Componentes bUnit).
   - Estratégia de Mocking e banco InMemory para isolamento.
   - Pipeline de Integração Contínua (GitHub Actions).

---

## Destaques do Projeto

- **Zero Frameworks Pesados de Terceiros no Frontend:** Gráficos vetoriais SVG nativos em componentes Razor puros, HTML5 semântico e CSS3 Grid/Flexbox responsivo.
- **Worker Resiliente (15 minutos):** Background service com `PeriodicTimer` e resiliência via políticas Polly (Retry exponencial).
- **Transparência de Provedor & Diagnóstico:** Status visual em tempo real do provedor OpenWeatherMap com telemetria detalhada de cada capital.
- **Exportação CSV & Panorama Nacional:** Download instantâneo de séries temporais e comparativo dinâmico das 27 capitais.
