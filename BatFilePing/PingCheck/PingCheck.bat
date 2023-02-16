@ECHO OFF
:Loop
TIMEOUT /T 1 /NOBREAK
ECHO -----------------------------------------------------------------------------------------------------------------------
ECHO Check Server Status
Powershell.exe -executionpolicy remotesigned -File C:\Users\user446\Desktop\OvenDownNotify\BatFilePing\PingCheck\PingCheck.ps1
ECHO -----------------------------------------------------------------------------------------------------------------------
goto Loop
pause >nul