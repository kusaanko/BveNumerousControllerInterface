cd src\bin\Release
if not exist "Input Devices" mkdir "Input Devices"
copy "Kusaanko.NumerousControllerInterface.dll" "Input Devices" /Y
"C:\Program Files\7-Zip\7z.exe" a NumerousControllerInterface_Bve5.zip "Input Devices" LibUsbDotNet.dll
cd ..\..\NumerousControllerInterface.NET4\bin\Release
if not exist "Input Devices" mkdir "Input Devices"
copy "Kusaanko.NumerousControllerInterface.NET4.dll" "Input Devices" /Y
"C:\Program Files\7-Zip\7z.exe" a NumerousControllerInterface_Bve6.zip "Input Devices" LibUsbDotNet.dll
cd ..\..\..\..\
if not exist "bin" mkdir "bin"
copy "src\bin\Release\NumerousControllerInterface_Bve5.zip" "bin" /Y
copy "src\NumerousControllerInterface.NET4\bin\Release\NumerousControllerInterface_Bve6.zip" "bin" /Y
del "src\bin\Release\NumerousControllerInterface_Bve5.zip"
del "src\NumerousControllerInterface.NET4\bin\Release\NumerousControllerInterface_Bve6.zip"
"C:\Program Files\7-Zip\7z.exe" a "bin\NumerousControllerInterface_Bve5.zip" install.bat script.ps1
"C:\Program Files\7-Zip\7z.exe" a "bin\NumerousControllerInterface_Bve6.zip" install.bat script.ps1