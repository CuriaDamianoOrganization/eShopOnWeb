# Testing and quality

The solution separates tests by scope:

- `tests/UnitTests`: fast tests for ApplicationCore and Web behavior, using xUnit and NSubstitute.
- `tests/IntegrationTests`: persistence and infrastructure integration tests, including EF Core's in-memory provider.
- `tests/FunctionalTests`: end-to-end HTTP and page scenarios for Web and PublicApi.
- `tests/PublicApiIntegrationTests`: Public API integration coverage.

## Recommended coverage

When changing a feature, cover the business rule in a unit test, the persistence mapping or query in an integration test when applicable, and the externally visible route or endpoint in a functional test when applicable.

Cross-cutting changes should include focused tests for:

- authorization requirements and role boundaries;
- exception-to-response mappings;
- logging or health-check behavior where it affects operations;
- SQL and in-memory configuration paths.

The repository's CI workflow is `.github/workflows/dotnetcore.yml`. Run the existing solution restore, build, and test commands used by that workflow before merging changes.
