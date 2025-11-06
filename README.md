# 🌍 Disaster Alleviation Web Application

## 📘 Overview
The **Disaster Alleviation Web Application** is a .NET Core MVC-based system designed to streamline the process of managing donations, tracking incidents, and coordinating volunteer activities during disaster relief operations.  
The system provides an organized platform for managing essential data, ensuring transparency, efficiency, and accountability within disaster response efforts.

---

## 🧩 Key Features
- **Donation Management**  
  Collects and tracks donations from individuals and organizations, including donor information, resource types, and quantities.

- **Incident Tracking**  
  Records and manages disaster incidents, including location, severity, and reporter details, allowing for better coordination and response.

- **User Management**  
  Maintains user accounts with defined roles such as Admin, Volunteer, and Donor.  
  Includes authentication and validation to ensure secure system access.

- **Volunteer Coordination (Future Extension)**  
  Allows volunteers to register, view ongoing incidents, and assist where needed.

---

## ⚙️ Technologies Used
- **ASP.NET Core MVC (Web Framework)**
- **C# (Programming Language)**
- **Entity Framework Core (Data Access)**
- **xUnit (Unit Testing Framework)**
- **SQL Server (Database)**
- **Visual Studio 2022 (IDE)**

---

## 🧪 Unit Testing with xUnit

### 🧰 Purpose
Unit tests are implemented to ensure that all model classes and their data annotations behave correctly before integrating other application layers.  
These tests verify:
- Validation attributes (e.g., `[Required]`, `[StringLength]`, `[EmailAddress]`, `[Range]`, `[Phone]`)
- Default property values (e.g., `Status`, `Role`, `CreatedAt`)
- Correct handling of relationships between entities (e.g., `User` ↔ `Incident` ↔ `Volunteer`)
- Proper error detection for invalid data input

---

## 🧱 Test Project Structure

**Project Name:** `Disaster_AlleviationTests`  
**Test Framework:** `xUnit`  

### Included Test Classes
| Test File | Model Tested | Description |
|------------|--------------|--------------|
| `DonationModelTests.cs` | `Donation` | Verifies donor details, required fields, and quantity validation. |
| `IncidentModelTests.cs` | `Incident` | Ensures incidents are reported correctly, with valid title, location, and timestamps. |
| `UserModelTests.cs` | `User` | Validates user creation, email/phone formats, default roles, and navigation properties. |

Each test suite uses the `.NET Core` validation system to simulate model validation just like the framework does at runtime.

---

## 🚀 How to Run the Tests

1. Open the solution in **Visual Studio 2022**.
2. Right-click the test project folder (**Disaster_AlleviationTests**) and select:  
   ➤ **Set as Startup Project**
3. Open the **Test Explorer** (`Test > Windows > Test Explorer`).
4. Click **Run All Tests** or run individual test files.
5. Review test outcomes (✅ Pass or ❌ Fail) in the results panel.

---

## 🧾 Expected Outcomes
If all models and annotations are configured correctly:
- All **positive tests** (valid data) should pass.
- **Negative tests** (invalid data or missing required fields) should fail as expected, confirming that validation works correctly.

This approach ensures that your web application’s data layer is robust and reliable before adding controllers, views, or business logic.

---

## 👩‍💻 Author
**Developer:** Amahle Msomi  
**Project:** Disaster Alleviation Web Application  
**Test Framework:** xUnit (.NET Core)  
**IDE:** Visual Studio 2022  

---

## 📜 License
This project is for educational and development purposes.  
Feel free to extend it with new models, controllers, or integration tests as the system evolves.
