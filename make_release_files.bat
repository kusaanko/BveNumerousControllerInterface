cd src\bin\Release
if not exist "Input Devices" mkdir "Input Devices"
copy "Kusaanko.NumerousControllerInterface.dll" "Input Devices" /Y
"C:\Program Files\7-Zip\7z.exe" a NumerousControllerInterface.zip "Input Devices" LibUsbDotNet.dll
cd ..\..\..\
if not exist "bin" mkdir "bin"
copy "src\bin\Release\NumerousControllerInterface.zip" "bin" /Y
del "src\bin\Release\NumerousControllerInterface.zip"
cd Installer\bin\Release
"C:\Program Files\7-Zip\7z.exe" a NumerousControllerInterfaceInstaller.zip NumerousControllerInterfaceInstaller.exe NumerousControllerInterfaceInstaller.exe.config
cd ..\..\..\
copy "Installer\bin\Release\NumerousControllerInterfaceInstaller.zip" "bin" /Y
del "Installer\bin\Release\NumerousControllerInterfaceInstaller.zip"
"C:\Program Files\7-Zip\7z.exe" a bin/NumerousControllerInterfaceInstaller.zip library_license.txt
"C:\Program Files\7-Zip\7z.exe" a bin/NumerousControllerInterface.zip library_license.txt
"C:\Program Files\7-Zip\7z.exe" a bin/NumerousControllerInterfaceInstaller.zip LICENSE
"C:\Program Files\7-Zip\7z.exe" a bin/NumerousControllerInterface.zip LICENSE
pause