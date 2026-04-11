# Routing Patterns Demo

A sample ASP.NET minimal API project that accompanies two articles on ASP.NET minimal API development:

1. **[Routing Patterns in ASP.NET Minimal APIs]()** — _link to be added_
2. **[OpenAPI Support in ASP.NET Minimal APIs]()** — _link to be added_

The project demonstrates:

- Organising minimal API handlers into `Handlers` classes with a `MapRoutesAndDescribe` method (routing pattern)
- Generating typed `Created`/`Ok`/`NotFound` responses
- Code-first OpenAPI schema generation using `Microsoft.AspNetCore.OpenApi`
- Enriching the schema with XML documentation comments, `.WithSummary()`, `.WithTags()`, and `.WithName()`
- Serving an interactive API browser using [Scalar](https://scalar.com/)

---

## Running the Project

### Locally

**Prerequisites:** [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)

```bash
git clone <repo-url>
cd routing-patterns-demo/RoutingPatternsDemo
dotnet run
```

Then open `http://localhost:5237/scalar` in your browser to browse the interactive API documentation.

Alternatively, open the solution in VS Code (with the C# Dev Kit extension) and press **F5**. The browser will open the Scalar UI automatically.

---

### In GitHub Codespaces

You do not need to install anything. GitHub Codespaces provides a fully pre-configured cloud development environment in your browser or in VS Code.

**Steps:**

1. Navigate to the repository on GitHub.
2. Click the green **Code** button.
3. Select the **Codespaces** tab and click **Create codespace on main**.
4. Wait for the Codespace to build (typically under a minute).
5. Once VS Code opens in the browser, press **F5** to run the project.
6. A notification will appear asking whether to open the forwarded port in your browser — click **Open in Browser**. The Scalar UI will load.

#### Free Allowance and Cost

GitHub provides a free monthly Codespaces allowance to every account:

| Account plan | Free core-hours/month | Free storage/month |
|---|---|---|
| GitHub Free | 120 core-hours | 15 GB |
| GitHub Pro | 180 core-hours | 20 GB |

This repo's devcontainer is configured to use **2 cores**, so a GitHub Free account gets roughly **60 hours/month** of free usage on this repo.

If you exceed the free allowance, usage is billed at approximately **$0.18 per hour** for a 2-core machine (storage is billed separately at $0.07 per GB per month). [Current pricing is listed here](https://docs.github.com/en/billing/managing-billing-for-your-products/managing-billing-for-github-codespaces/about-billing-for-github-codespaces#pricing-for-paid-usage).

#### Preventing unexpected charges

To ensure you are never billed for Codespaces beyond the free tier, go to **GitHub Settings → Billing and plans → Spending limits** and set the **Codespaces** spending limit to **$0**. With a $0 limit, Codespaces will stop rather than incur charges once your free allowance is exhausted.

> **Remember to stop your Codespace** when you are done. Codespaces count time while running, regardless of whether you are actively using them. Go to [github.com/codespaces](https://github.com/codespaces) to stop or delete your Codespaces.

---

## Project Structure

```
RoutingPatternsDemo/
├── Program.cs                          # Startup: DI registration, route mapping, OpenAPI & Scalar setup
├── Handlers/
│   └── ProductHandlers.cs              # Endpoint handlers + MapRoutesAndDescribe
├── ApplicationServices/
│   ├── CreateProductArgs.cs            # Request body record (with XML docs for OpenAPI)
│   ├── IProductService.cs
│   └── ProductService.cs               # In-memory product store
├── Domain/
│   └── Product.cs                      # Domain model (with XML docs for OpenAPI)
└── Properties/
    └── launchSettings.json
```

---

## Key Endpoints

| Method | Path | Description |
|--------|------|-------------|
| `POST` | `/v1/products/` | Create a new product |
| `GET` | `/v1/products/{id}` | Fetch a product by GUID |
| `GET` | `/openapi/v1.json` | Raw OpenAPI schema (JSON) |
| `GET` | `/scalar` | Interactive Scalar API browser |
