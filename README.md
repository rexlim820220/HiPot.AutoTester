# HiPot AutoTester Desktop

A professional Windows-based automation tool designed for HiPot (High Potential) safety testing. The application synchronizes real-time test data with SFIS (Shop Floor Integrated System) and remote SFTP storage to ensure data integrity and traceability.

## 1. System Architecture & Modules

The application follows a modular design to ensure scalability and maintainability:

### 🛰️ Communication Modules
*   **HiPot Service (SCPI)**: Handles low-level serial communication with HiPot instruments using SCPI commands. Features a **Mock Mode** for offline development and simulation.
*   **SFIS Integration (SOAP/Web Service)**: Manages automated login/logout and result uploading to the factory SFIS system. Implements an auto-retry mechanism during initialization.
*   **SFTP Service (SSH.NET)**: Provides secure file transfer capabilities to back up raw test logs to Linux-based servers (`10.197.189.143`).

### ⚙️ Core Logic Modules
*   **Batch Test Manager**: Handles multi-PSU testing sequences. It ensures that if a re-test is required, the entire batch (PSU1, PSU2, etc.) is re-evaluated to maintain data consistency.
*   **Result Aggregator**: Formats individual test parameters into standardized CSV-style strings (`TEST, STATUS, VALUE, UCL, LCL`) as required by MIS specifications.
*   **Local Logger**: A thread-safe diagnostic module that records system events and exceptions into the Windows `%TEMP%` directory for troubleshooting.

---

## 2. Interface & Operation Guide

### 🖥️ Main User Interface
*   **ISN Input**: Supports barcode scanning or manual entry of the Device Internal Serial Number.
*   **Model Selection**: A dropdown list populated via `DeviceConfig`, which automatically determines the required number of PSU tests for the selected model.
*   **Real-time Status Board**: Large color-coded indicators (PASS/FAIL/TESTING) with synchronized system sounds for immediate operator feedback.

### 🛠️ Operation Flow
1.  **System Initialization**: On startup, the app automatically attempts to log in to the SFIS server. If the connection fails, it will enter an intelligent retry loop.
2.  **Configuration Selection**: The operator selects the appropriate test model.
3.  **The Testing Cycle**:
    *   The operator triggers the test.
    *   For multi-PSU devices, the app prompts the operator to switch connections between items.
    *   **Logic**: The system only proceeds to the upload stage if **all items** in the batch pass.
4.  **Data Synchronization**:
    *   **SFIS**: Uploads the latest consolidated test result.
    *   **SFTP**: Generates and uploads a detailed `.log` file containing raw instrument readings and high-precision timestamps.
5.  **Error Handling**: If a test fails, the operator can choose to "Restart" (which clears previous results and restarts the full batch) or "Cancel".

---

## 3. Technical Specifications
*   **Framework**: .NET Framework / .NET (WinForms)
*   **Dependencies**: 
    *   [SSH.NET](https://github.com) (for SFTP support)
    *   SOAP Web Services (for SFIS communication)
*   **Log Path**: `C:\Users\AppData\Local\Temp\HiPot_AutoTester_Log.txt`
