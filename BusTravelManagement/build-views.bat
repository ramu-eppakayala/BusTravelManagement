@echo off
cd /d D:\Openclaude\Travels\BusTravelManagement
"C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\amd64\MSBuild.exe" BusTravelManagement.csproj /t:Build /p:Configuration=Debug /v:m /nologo
