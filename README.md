# CurrencyConverter

A modern .NET 8 Minimal API for currency conversion, featuring distributed caching, OpenTelemetry tracing, Serilog logging, and robust business logic separation.

## Setup Instructions

1. **Clone the repository:**
   ```
   git clone https://github.com/MaddiSomasekharaReddy/CurrencyConverter.git
   ```
2. **Restore dependencies:**
   ```
   dotnet restore
   ```
3. **Build the solution:**
   ```
   dotnet build
   ```
4. **Run the API:**
   ```
   cd CurrencyConverter.API
   dotnet run
   ```
5. **Run tests and view coverage:**
   ```
   dotnet test CurrencyConverter.Test --collect:"Code Coverage"
   ```
   - All tests are passing and the solution achieves **100% code coverage**.
   - Coverage reports are included and can be converted to HTML using [ReportGenerator](https://github.com/danielpalme/ReportGenerator).

6. **Run with Docker:**
   ```
   docker build -t currencyconverter .
   docker run -p 8080:8080 -p 5259:5259 currencyconverter
   ```
   - The provided Dockerfile builds and runs the API with all dependencies.

## Assumptions Made
- Currency exclusion rules (TRY, PLN, THB, MXN) are hardcoded in the domain layer for demonstration.
- Transaction fee is a flat 1% for all conversions.
- Distributed caching uses in-memory cache for simplicity.
- Minimal API endpoints are grouped for clarity and future extensibility with AOT compiler.

## Possible Future Enhancements
- Support for external distributed cache providers (e.g., Redis).
- Dynamic currency exclusion rules from configuration or database.
- More granular tracing and metrics (e.g., Prometheus, Jaeger exporters).
- More advanced business rules and structures.

## Highlights
- **Minimal APIs:** Modern, fast, and compatible with AOT compilation.
- **Distributed Caching:** Improves performance and scalability.
- **OpenTelemetry Tracing:** Observability for distributed systems.
- **Serilog Logging:** Structured logging for diagnostics.
- **Business Logic Separation:** Domain layer enforces rules and calculations.
- **Integration & Unit Tests:** High coverage and maintainability.
- **Docker Support:** Easily build and run the solution in any environment.
- **100% Code Coverage:** All tests are passing and the solution is fully covered by automated tests.

---
