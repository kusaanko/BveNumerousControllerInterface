cd src\bin\Release
if not exist "Input Devices" mkdir "Input Devices"
copy "Kusaanko.NumerousControllerInterface.dll" "Input Devices" /Y
"C:\Program Files\7-Zip\7z.exe" a NumerousControllerInterface.zip "Input Devices" LibUsbDotNet.dll
cd ..\..\..\
if not exist "bin" mkdir "bin"
copy "src\bin\Release\NumerousControllerInterface.zip" "bin" /Y
del "src\bin\Release\NumerousControllerInterface.zip"
cd Installer\bin\Release
"C:\Program Files\7-Zip\7z.exe" a Installer.zip NumerousControllerInterfaceInstaller.exe NumerousControllerInterfaceInstaller.exe.config
cd ..\..\..\
copy "NumerousControllerInterfaceAtsEXPlugin\bin\Release\NumerousControllerInterfaceAtsEXPlugin.zip" "bin" /Y
del "NumerousControllerInterfaceAtsEXPlugin\bin\Release\NumerousControllerInterfaceAtsEXPlugin.zip"
cd NumerousControllerInterfaceAtsEXPlugin\bin\Release
"C:\Program Files\7-Zip\7z.exe" a NumerousControllerInterfaceAtsEXPlugin.zip NumerousControllerInterfaceAtsEXPlugin.dll
cd ..\..\..\
copy "Installer\bin\Release\Installer.zip" "bin" /Y
del "Installer\bin\Release\Installer.zip"
"C:\Program Files\7-Zip\7z.exe" a bin/Installer.zip library_license.txt
"C:\Program Files\7-Zip\7z.exe" a bin/NumerousControllerInterface.zip library_license.txt
pause