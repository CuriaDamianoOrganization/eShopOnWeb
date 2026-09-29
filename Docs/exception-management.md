# Exception management

## Web application

Development and Docker environments use the developer exception page to support local diagnosis. Other environments use `UseExceptionHandler("/Error")`, which routes unhandled failures to the Web error page without exposing the developer exception page.

## Public API

PublicApi installs `ExceptionMiddleware` before the rest of the request pipeline. The middleware converts:

- `DuplicateException` to HTTP 409 Conflict.
- Other unhandled exceptions to HTTP 500 Internal Server Error.

Responses are JSON `ErrorDetails` objects containing the status code and message. The API also uses the developer exception page in development.

## Application exceptions

Business-specific exceptions live in `ApplicationCore.Exceptions`, including duplicate, missing-basket, and empty-basket-on-checkout cases. Application services throw these exceptions when a business invariant cannot be satisfied; the host decides how to represent them at the transport boundary.

## Guidance

- Keep business exceptions independent of HTTP types.
- Map expected domain failures deliberately and consistently.
- Do not expose internal exception messages or stack traces to external clients in production.
- Log unexpected failures with the exception object and a request correlation identifier.
- Add tests for both the exception type and the externally visible status/response.

Relevant implementation points:

- `src/PublicApi/Middleware/ExceptionMiddleware.cs`
- `src/Web/Program.cs`
- `src/Web/Pages/Error.cshtml.cs`
- `src/ApplicationCore/Exceptions/`
