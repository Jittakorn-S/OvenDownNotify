# OvenDownNotify

OvenDownNotify is a .NET application that monitors the status of servers and ovens, sending notifications to LINE when specific conditions are met.

## Features

  * **Server Status Check**: Pings a predefined list of servers to check if they are online or offline.
  * **Overdue Oven Lot Detection**: Queries a database to find oven lots that have been running longer than their standard time.
  * **LINE Notifications**: Sends alerts to a LINE group with details about overdue lots or other system events.
  * **Error Logging**: Logs any errors encountered during its operation to a text file.
  * **Scheduled Execution**: Designed to be run as a scheduled task to provide continuous monitoring.

## How it Works

The application works in a two-step process:

1.  **Ping Check**: A PowerShell script (`PingCheck.ps1`) reads a list of IP addresses or hostnames from `IP.txt` and pings each one. The results (online/offline status) are saved to `Result.txt`.

2.  **Main Application**: The .NET application (`OvenDownNotify.exe`) is executed.

      * It reads the `Result.txt` file.
      * It connects to a SQL database and executes a query to find lots in the `SPOvenData` table that have been processing for longer than the recipe specifies.
      * If any overdue lots are found, it constructs a message listing the Lot Number and Machine Number for each.
      * This message is then sent as a notification to a LINE chat or channel using the LINE Notify API.
      * The application then exits.

## Configuration

To use the OvenDownNotify application, you need to configure the following files:

### `IP.txt`

This file, located in `BatFilePing/PingCheck/`, should contain the list of server IP addresses or hostnames that you want to monitor, with one entry per line.

**Example:**

```
SP-OV-04
SP-OV-05
SP-OV-06
```

### `App.config` / `OvenDownNotify.exe.config`

This is the main configuration file for the .NET application.

  * **ConnectionDB**: The connection string for the database. This includes the server address, database name, username, and password.
    ```xml
    <add name="OvenDownNotify.Properties.Settings.ConnectionDB" connectionString="Data Source=ip;Initial Catalog=APCSProDB;Persist Security Info=True;User ID=username;Password=password;" />
    ```
  * **Token**: The access token for the LINE Notify API. You can get this from the LINE Notify website.
    ```xml
    <add key="Token" value="YOUR_LINE_NOTIFY_TOKEN"/>
    ```
  * **LogFile**: The full path to the file where error logs should be saved.
    ```xml
    <add key="LogFile" value="C:\Path\To\Your\LogError.txt"/>
    ```

## Installation and Setup

1.  **Configure the application**:
      * Edit `BatFilePing/PingCheck/IP.txt` to include the servers you want to monitor.
      * Edit `OvenDownNotify/App.config` with your database connection string, LINE Notify token, and desired log file path.
2.  **Set up the database**: Ensure the application has the necessary permissions to read from the `[DBx].[dbo].[SPOvenData]`, `APCSProDB.trans.lots`, `APCSProDB.method.packages`, `[DBx].[dbo].[OvenRecipe]`, and `[APCSProDB].[trans].[lot_process_records]` tables.
3.  **Schedule the task**: Create a task in Windows Task Scheduler to run the `PingCheck.ps1` script and then the `OvenDownNotify.exe` application at your desired interval.

## Dependencies

  * .NET Framework 4.8
  * Windows PowerShell
