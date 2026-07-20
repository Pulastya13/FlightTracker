# Reflection

What I would improve with more time:

---

## Architecture Improvements

1. **Add integration tests** — Test the full HTTP pipeline using `WebApplicationFactory<Program>` to validate endpoint behaviour, CORS headers, and content negotiation.

2. **Add a caching layer** — Even with stubs, introducing `IMemoryCache` or a simple caching decorator around providers demonstrates extensibility.

3. **Structured logging** — Add Serilog with structured log events for each provider call, merge decision, and API response. Useful for debugging in production.

4. **Health checks** — Add `/health` endpoint that verifies provider connectivity (even for stubs, establishes the pattern).

---

## Code Quality

5. **FluentValidation** — Replace inline validation with a `FlightStatusRequestValidator` for cleaner separation of validation logic from the endpoint.

6. **Result pattern** — Use a `Result<T>` type instead of returning nullable `FlightStatusResult?` from providers. Makes error handling more explicit.

7. **OpenAPI/Swagger** — Add `Swashbuckle` for API documentation and interactive testing during development.

---

## Frontend Improvements

8. **Loading skeleton** — Replace the "Searching..." text with a skeleton card animation for better perceived performance.

9. **Form validation UX** — Add inline validation messages and visual feedback on input fields.

10. **Responsive design** — Further optimize for mobile viewports and add PWA support.

11. **Angular unit tests** — Add Jasmine/Karma tests for the service and components.

---

## Operational Readiness

12. **Docker Compose** — Add a `docker-compose.yml` that runs both backend and frontend for truly reproducible local setup.

13. **CI pipeline** — Add GitHub Actions workflow for build, test, and lint on every push.

14. **Error boundaries** — Add global error handling middleware in the API and a global Angular error handler.

---

## Design Decisions I'm Satisfied With

- **Strategy + DI for providers** — Adding a third provider requires only a new class and one DI registration line
- **Normalisation as a service** — Vocabulary mappings are isolated and testable
- **Deterministic stubs with overlapping data** — Covers all merge scenarios without complexity
- **spec.md before code** — Forced upfront thinking about contracts and models
