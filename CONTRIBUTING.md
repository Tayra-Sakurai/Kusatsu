# Contributing to This Project

Thank you for your interest in contributing! We welcome contributions from everyone, whether you are fixing a bug, proposing a feature, improving documentation, or refactoring code.

Please take a moment to review this document to ensure a smooth and effective collaboration process.

---

## Table of Contents

1. [Code of Conduct](#code-of-conduct)
2. [How Can I Contribute?](#how-can-i-contribute)
   - [Reporting Bugs](#reporting-bugs)
   - [Suggesting Features](#suggesting-features)
   - [Submitting Pull Requests](#submitting-pull-requests)
3. [Development Setup](#development-setup)
4. [Coding & Design Standards](#coding--design-standards)
5. [Commit & Pull Request Conventions](#commit--pull-request-conventions)
6. [License & Intellectual Property](#license--intellectual-property)

---

## Code of Conduct

We aim to foster a welcoming, inclusive, and safe environment for all contributors. Please maintain a respectful and courteous tone in all discussions, issues, and pull requests. Harassment, abusive language, or unacceptable behavior will not be tolerated.

---

## How Can I Contribute?

### Reporting Bugs

Before creating a bug report, please check the existing issues to ensure the bug hasn't already been reported.

When creating a bug report, please include as much detail as possible:
* **Title**: A clear and concise summary of the issue.
* **Environment**:
  * OS Version (e.g., Windows 11 23H2)
  * Application Version
  * .NET SDK / Runtime version
* **Steps to Reproduce**: Detailed step-by-step instructions to reproduce the behavior.
* **Expected vs. Actual Behavior**: Explain what you expected to happen and what actually happened.
* **Logs / Screenshots**: Include relevant error logs, stack traces, or screenshots if applicable.

### Suggesting Features

Feature requests are always welcome! When submitting a feature proposal:
* Use a clear and descriptive title.
* Explain the **use case** and why this feature would be useful to users.
* Describe the **desired behavior** or UI design as specifically as possible.

### Submitting Pull Requests

1. **Check Existing Work**: Search existing Pull Requests (PRs) and Issues before starting. For large features or major breaking changes, please open an Issue to discuss your proposal first.
2. **Fork & Branch**: Fork the repository and create a feature branch off `main` (or `develop`):
   ```bash
   git checkout -b feature/your-feature-name
   ```
3. **Keep Changes Focused**: Keep your PR focused on a single logical change. Do not bundle unrelated refactoring or formatting changes into the same PR.
4. **Test**: Ensure your changes build cleanly without warnings/errors and pass all unit/integration tests.
5. **Submit**: Open a Pull Request with a clear title and description explaining what was changed and why.

---

## Development Setup

### Prerequisites

* **OS**: Windows 11
* **IDE**: [Visual Studio 2026](https://visualstudio.microsoft.com/) (version 18.11 or newer recommended)
* Workloads required:
  * **.NET desktop development**
  * **Windows application development** (if using WinUI 3 / Windows App SDK)
  * **SDK**: .NET SDK (matching the project target version specified in `.csproj` / `global.json`)

### Building the Solution

1. Clone the repository:
   ```bash
   git clone https://github.com/Tayra-Sakurai/Kusatsu.git
   cd ./Kusatsu
   ```
2. Open the solution file (`.slnx`) in Visual Studio 2026.
3. Restore NuGet packages:
   ```bash
   dotnet restore
   ```
4. Build and run the project (`F5`).

---

## Coding & Design Standards

To keep the codebase maintainable and readable, please follow these guidelines:

### C# / .NET Conventions

* Follow standard Microsoft [.NET Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions).
* Use `PascalCase` for class names, method names, and public properties.
* Use `camelCase` or `_camelCase` for private fields and local variables.
* Use explicit and clear naming; avoid unclear abbreviations.
* Prefer nullable reference types enabled (`#nullable enable`).
* Resolve all compiler warnings wherever possible.

### Architecture & UI (Windows / WinUI 3)

* Follow the **MVVM (Model-View-ViewModel)** architectural pattern.
* Maintain clean separation of concerns: keep UI logic inside Views/XAML and business logic inside ViewModels/Services.
* Follow Windows UI design principles for fluent, native, and accessible user experiences.

---

## Commit & Pull Request Conventions

### Commit Messages

We encourage clear and concise commit messages. A good format to follow:

```
<type>: <short description>

[optional longer body explaining 'why' rather than 'what']

```

Common types:

* `feat`: A new feature
* `fix`: A bug fix
* `docs`: Documentation changes
* `style`: Code style changes (formatting, missing semi-colons, etc.)
* `refactor`: Refactoring code without changing external behavior
* `test`: Adding or updating tests
* `chore`: Build process, dependencies, or tooling updates

### PR Review Process

* All submitted PRs will undergo code review before merging.
* Maintainers may request changes or clarity. Please address feedback promptly.
* Automated CI checks (builds, tests, formatting) must pass before merging.

---

## License & Intellectual Property

By contributing to this repository, you agree that your contributions will be licensed under the project's [LICENSE](./LICENSE.txt). Ensure that you have the right to contribute any code, assets, or third-party libraries you submit.