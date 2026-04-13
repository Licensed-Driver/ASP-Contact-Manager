# Contact Manager - RIVA Take-Home Assignment

## Overview

I've made a modern, responsive SPA for the Contact Manager assignment. It sticks to the layered architecture (Controller, Service, Model) and uses an AJAX-driven design to remove the need for page reloads.

The baseline requirements set in the assignment were implemented and heavily tested. However, I was enjoying the process and decided to take it a few steps further to incorporate some production-geared considerations like thread safety, XSS prevention, fuzzy search algorithms, and strict domain validation.

_(One of the things I had some fun with was adding this README as dynamically served HTML directly within the app using a Markdown rendering pipeline and custom document management system.)_

---

## 1. Setup & Execution Instructions

**Prerequisites:**

- .NET 8.0 SDK
- Visual Studio 2022 (recommended) or VS Code

**To Run the Application:**

1. Clone the repository and open the solution file (`.sln`) in Visual Studio.
2. Ensure the main web project is set as the Startup Project.
3. Press `F5` (or use `dotnet run` via the CLI).
4. The server will launch, automatically opening the Contact Manager UI in default browser.
   - _Note: The TypeScript compiler is configured via `tsconfig.json` to automatically transpile the `.ts` files into the `wwwroot` folder on build._

**To Run the Tests:**

- Open the **Test Explorer** in Visual Studio and click "Run All", or use `dotnet test` from the root directory in terminal. The suite runs heavy unit tests covering model validation, service logic, and controller routing.

---

## 2. Architecture & Tech Stack

The application is implemented with a clean separation of concerns:

- **Backend:** ASP.NET Core 8 MVC / Web API (C#)
- **Frontend:** TypeScript, Native Web Components (`elements.ts`), HTML5
- **Styling:** Bootstrap 5.3
- **Testing:** xUnit & Moq
- **Key Libraries:** `Quickenshtein` (for super fact Levenshtein distance), `Markdig` (for Markdown to HTML rendering).

**Separation of Concerns:**

- **Controllers:** Strictly act as traffic cops. They handle HTTP request lifecycles, parse route/body data, and return standard HTTP status codes (`200 OK`, `400 Bad Request`). **No business logic in the controllers.**
- **Services:** All data manipulation, retrieval, and algorithm logic are housed in injected Service classes.
- **Models:** Define the data structure and handle data validation via Data Annotations and `IValidatableObject`.

---

## 3. Technical Decisions, Trade-offs & Optimizations

To keep within the constraints of the assignment (in-memory data storage) but still try to uphold real-world standards, a few technical decisions were made.

### A. Data Layer: Thread-Safe In-Memory Storage

- **Implementation:** Instead of using a basic `List<T>` or dealing with the overhead of ENtity Framework, the data layer uses a `ConcurrentDictionary<Guid, Contact>` injected as a Singleton service.
- **Trade-off:** While in-memory storage means data is lost upon a server restart, the `ConcurrentDictionary` makes sure we have complete thread safety. Multiple end-users can access and mutate contacts simultaneously without race conditions.

### B. Strict Domain Validation & Payload Sanitization

Standard ASP.NET validation is good, but adheres strictly to RFC definitions, which can fall short of some regular practices. So I implemented a few custom rules:

- **Email Regex:** The default `[EmailAddress]` attribute follows RFC standards, which technically allows emails without TLDs and certain invalid characters (e.g. `:` and `/`). I used Regex to just allow common-sense email formatting.
- **Phone Number Constraints:** Phone numbers are restricted to lengths between 7 and 20 characters. The shortest possible valid phone number is 7 digits, and the longest international number is 15. The 20-character cap gives some standard formatting characters (like `(`, `)`, `-`, and `+`).
- **Minimum Contact Info:** It doesn't really make sense to store a contact if you can't contact them, so I implemented `IValidatableObject` on the Model to make sure a user provides _either_ an email address or a phone number.
- **Whitespace Sanitization:** Custom property setters check empty or whitespace strings and convert them to `null` before validation, stopping empty strings from bypassing the Regex or Phone attributes.

### C. Advanced Search: Levenshtein Distance & Pagination

- **Implementation:** Rather than a rigid, exact-match `.Contains()` filter, the search endpoint uses fuzzy matching with the Levenshtein ratio (implemented with the `Quickenshtein` to reduce any overhead). It calculates the percent difference against the First Name, Last Name, Email, and combined Full Name, ordering the results by best match.
- **Trade-off:** Fuzzy logic provides a much better UX (accounts for typos and incomplete strings), but calculating string distance across a large dataset in memory is expensive. To help mitigate this, **server-side pagination** (using `Skip` and `Take`) was implemented alongside a **300ms debounce** on the frontend TypeScript input. This makes sure we have predictable response sizes and drastically reduces server load while typing.

### D. Frontend: TypeScript & Native Web Components

- **Module-Level State:** Instead of a regular MVC Razor application where the server renders the HTML, the frontend is a true SPA. `app.ts` maintains a stateful `currentContacts` array.
- **XSS Prevention:** A common pitfall in vanilla JS/AJAX apps is injecting raw JSON directly into HTML string literals, which breaks the DOM or risks cross site scripting attacks (XSS) if user data contains quotes. Here, the UI is rendered by binding entity IDs to the DOM elements. When a user clicks "Edit", the system safely looks up the entity from memory.
- **Component Encapsulation:** The `<update-contact-modal>` is built as a native Web Component (`HTMLElement`), encapsulating its template and logic away from the main view.

---

## 4. Testing Strategy

The solution includes an xUnit test suite focused on isolated and predictable states for testing.

- **Service Isolation:** Each `[Fact]` in `ContactServiceUnitTests` instantiates its own service instance to ensure isolated data.
- **Controller Mocking:** Controller tests verify routing and HTTP Status Code returns by mocking the `IContactService` with Moq. This is to make sure the HTTP layer is tested independently of the data layer.
- **Exhaustive Model Validation:** Tests explicitly target the complex edge cases of the `IValidatableObject` implementation, testing an array of malformed phone numbers and regex-busting email strings to guarantee data integrity before it ever touches the service layer.

---

## 5. Having Fun: Custom Markdown Rendering Pipeline

To serve this documentation dynamically within the app, I made a custom file-serving pipeline using some advanced C# features since I had some extra time and wanted to have some fun:

- **Curiously Recurring Template Pattern (CRTP) & Reflection:** I created an abstract `Extension` class that dynamically discovers supported file types (like `.md`) via Reflection on assembly load.
- **Implicit Casting:** The `Document` model utilizes implicit casting and generic constraints to safely parse and decode file contents based on their extension.
- **Markdig Integration:** The `DocumentationController` takes the parsed Markdown content and uses `Markdig` to render it directly into HTML, serving it to the browser.

It's way overkill for a README, but it was fun and hopefully demonstrates some good familiarity with C# generics, reflection, and the MVC request pipeline!
