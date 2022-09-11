@echo off
if exist "script.ps1" echo.>script.ps1:Zone.Identifier
if exist "Newtonsoft.Json.dll" echo.>Newtonsoft.Json.dll:Zone.Identifier
if exist "LibUsbDotNet.dll" echo.>LibUsbDotNet.dll:Zone.Identifier
if exist "Input Devices\Kusaanko.NumerousControllerInterface.dll" echo.>"Input Devices\Kusaanko.NumerousControllerInterface.dll:Zone.Identifier"
if exist "Input Devices\Kusaanko.NumerousControllerInterface.NET4.dll" echo.>"Input Devices\Kusaanko.NumerousControllerInterface.NET4.dll:Zone.Identifier"
powershell -NoProfile -ExecutionPolicy Unrestricted -File ".\script.ps1" '%~dp0' '%1' '%2'
