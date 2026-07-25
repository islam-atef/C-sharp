========================================================================
Huawei & Nokia Commissioning Automation Tool - Documentation
========================================================================

This application is a cross-platform desktop GUI (Graphical User Interface) 
built in C# using Avalonia UI and the MVVM (Model-View-ViewModel) pattern. 
It automates the generation of commissioning configuration files (.cfg) for 
Huawei (MA5818, MA5600, GPON300, GPON_T500, MSAN500) and Nokia (MODEL_B) 
cabinets, securing templates and data centrally in the cloud using Firebase.

------------------------------------------------------------------------
1. PROJECT ARCHITECTURE OVERVIEW
------------------------------------------------------------------------
The application is structured into decoupled components to ensure 
maintainability and allow future extensions:

- Views (Classes/Views/): XAML markup for UI pages (Login, Commission 
  Generator, IP Plans Uploader, and Admin Templates Manager).
- ViewModels (Classes/ViewModels/): UI controllers managing page states 
  and async operations (preventing screen freezing during cloud fetches).
- Strategies (Classes/Strategies/): Interfaces implementing data fetching 
  and writing (Firebase, In-Memory demo, and local Excel reading).
- Modifiers (Classes/Modifiers/): Pipeline stages modifying specific parts 
  of template files (IP configuration, cabinet naming, SNMP settings, 
  Link Aggregation, and Auto-negotiation / Ports speed).
- Services (Classes/Services/): Cloud integration services for loading 
  templates dynamically and writing audit logs.

------------------------------------------------------------------------
2. FIREBASE SETUP REQUIREMENTS
------------------------------------------------------------------------
To use the application in cloud-connected (online) mode, you need to set 
up a Firebase project (Google Console). The free Spark plan is 100% sufficient.

A. Firebase Realtime Database:
   1. Go to Firebase Console -> Build -> Realtime Database -> Create Database.
   2. Select a database location and start in "locked mode".
   3. Go to the "Rules" tab and paste these security rules (allows reading 
      for keys, and writing only for authorized keys):
      
      {
        "rules": {
          ".read": "auth != null",
          ".write": "auth != null",
          "keys": {
            ".read": true,
            ".write": false
          },
          "ipplans": {
            ".read": true,
            ".write": true
          },
          "logs": {
            ".read": false,
            ".write": true
          }
        }
      }

   4. Go to the "Data" tab and seed the database structure. Example:
      
      {
        "keys": {
          "ADMIN_KEY_1": {
            "accessLevel": "Admin",
            "region": "All"
          },
          "STAFF_KEY_1": {
            "accessLevel": "Staff",
            "region": "11"
          },
          "OUTSOURCE_KEY_1": {
            "accessLevel": "Outsource",
            "region": "12"
          }
        },
        "ipplans": {
          "11-2-12-14": {
            "popName": "Sohag-Decent-Life",
            "tedMgGatewayIp": "10.24.120.1",
            "tedMgSH1Ip": "10.24.120.2",
            "tedMgSH2Ip": "10.24.120.3",
            "mgGatewayIp": "10.88.40.1",
            "mgSH1Ip": "10.88.40.2",
            "mgSH2Ip": "10.88.40.3",
            "mgSH3Ip": "10.88.40.4",
            "sigGatewayIp": "10.90.15.1",
            "sigSH1Ip": "10.90.15.2",
            "sigSH2Ip": "10.90.15.3",
            "fvnoEmGatewayIp": "10.95.80.1",
            "fvnoEmSH1Ip": "10.95.80.2",
            "fvnoEmSH2Ip": "10.95.80.3"
          }
        }
      }

B. Firebase Storage (Cloud Templates):
   1. Go to Firebase Console -> Build -> Storage -> Get Started.
   2. Set up default bucket.
   3. Create a folder named "templates" in the storage root.
   4. Upload your original reference configuration templates directly inside 
      the "templates/" folder. File names MUST match:
      - sh1MA5818.cfg
      - sh2MA5818.cfg
      - MSAN-500-UPPER-2023.cfg
      - GPON-300.cfg
      - GPON-T500.cfg
      - INDOOR 4 PORT.cfg
   5. Admin users will be able to manage, delete, or upload files directly 
      to this folder from the desktop application.

------------------------------------------------------------------------
3. LOCAL CONFIGURATION
------------------------------------------------------------------------
Configure the "appsettings.json" file located in the application root directory:

{
  "Firebase": {
    "DatabaseUrl": "https://your-project-id.firebaseio.com",
    "AuthSecret": "your-firebase-database-secret",
    "StorageBucket": "your-project-id.appspot.com"
  }
}

- DatabaseUrl: The URL shown at the top of your Firebase Realtime Database tab.
- AuthSecret: Found in Project Settings -> Service Accounts -> Database Secrets.
- StorageBucket: Found at the top of your Firebase Storage tab (e.g., project-id.appspot.com).

------------------------------------------------------------------------
4. HOW TO RUN AND TEST THE PROJECT
------------------------------------------------------------------------
A. Run Command:
   Open a terminal (Command Prompt, PowerShell, or macOS Terminal) inside the 
   project root directory and execute:
   
   dotnet run

   (Alternatively, you can open the folder in Visual Studio or Rider and 
   press the "Start/Play" button).

B. Testing in Offline/Demo Mode (Firebase Unconfigured):
   If the settings in appsettings.json contain default placeholders, the 
   app automatically runs in Offline/Demo Mode. Use these keys to test roles:
   
   - Key: "ADMIN" -> Logs in as Admin (access to Generator, IP plans, Templates)
   - Key: "STAFF" -> Logs in as Staff (access to Generator, IP plans)
   - Key: "OUTSOURCE" -> Logs in as Outsource (access to Generator tab only)

C. Output Location:
   Generated commissioning files will be saved under the chosen output path 
   (by default, in an "Outputs/" folder next to the app executable).

------------------------------------------------------------------------
5. PROJECT CLEANUP DETAILS
------------------------------------------------------------------------
To clean the project and save space, duplicate/unnecessary archive files 
(like "references/Models.rar" and "references/Models/Models.zip") have been 
removed as they were redundant copies of already extracted template folders.
========================================================================
