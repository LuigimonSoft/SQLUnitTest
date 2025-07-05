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

### Test Case Model

The base `TestCase` model contains common BDD fields:

- `Description` – description of the feature or scenario
- `Context` – additional context information
- `Mock` (`MockBlock`) – preconditions used to seed data
- `Should` – nested expectations
