@echo off
echo.>script.ps1:Zone.Identifier
echo.>Newtonsoft.Json.dll:Zone.Identifier
echo.>LibUsbDotNet.dll:Zone.Identifier
if exist "Input Devices\Kusaanko.NumerousControllerInterface.dll" echo.>"Input Devices\Kusaanko.NumerousControllerInterface.dll:Zone.Identifier"
if exist "Input Devices\Kusaanko.NumerousControllerInterface.NET4.dll" echo.>"Input Devices\Kusaanko.NumerousControllerInterface.NET4.dll:Zone.Identifier"
powershell -NoProfile -ExecutionPolicy Unrestricted ".\script.ps1"
