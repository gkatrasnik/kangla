@echo off
setlocal

set "API_BASE_URL=https://localhost:7049"
echo Starting the simulator with its built-in device access key against %API_BASE_URL%...
dotnet run --project "%~dp0src\kangla.DeviceSimulator\kangla.DeviceSimulator.csproj" -- "%API_BASE_URL%"
