# SQLUnitTest

A .NET class library for automated testing of SQL Server stored procedures, functions, and queries.

This project implements a BDD-style JSON test format and includes utilities for generating markdown reports. It can be consumed from console apps, GUI applications, or CI/CD pipelines via dependency injection.

## Project Structure
- `Models` – classes for describing test cases
- `Repositories` – abstractions over database access
- `Services` – core logic for executing tests
- `Reporting` – helpers for generating markdown output
- `DependencyInjection` – registration extensions

The library targets **.NET Standard 2.0** so it can be used from .NET Framework and .NET Core applications.

### Design Principles

The library follows SOLID principles. Test execution logic is separated into discrete *handlers* that implement `ITestCaseHandler`, enabling new test types without modifying the core runner.

### Test Case Model

The base `TestCase` model contains common BDD fields:

- `Describe` – description of the feature or scenario
- `Context` – additional context information
- `Mock` (`MockBlock`) – preconditions used to seed data
- `Should` – nested expectations

## Features

* Stored procedure execution and comparison tests
* Output parameter verification
* Table result and table comparison helpers
* Cross-database comparisons
* Markdown reporting with BDD style output

## Sample JSON

```json
{
  "describe": "User report comparison from different data sources",
  "context": "When retrieving active users filtered by region",
  "mock": {
    "preConditions": [
      { "connection": "MainDb", "query": "INSERT ..." }
    ]
  },
  "should": [
    {
      "it": "Compare procedures",
      "type": "StoredProcedureCompareTestCase",
      "storedProcedure": "sp_GetUserReport",
      "parameters": { "ActiveOnly": true }
    }
  ]
}
```

The runner outputs a concise console summary and a detailed markdown report that can be stored in CI pipelines.

