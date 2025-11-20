@echo off
REM Build script for Sup.Modern - Windows

echo ==========================================
echo   Sup!? Modern - Build Script (Windows)
echo ==========================================
echo.

set PROJECT_DIR=Sup.Desktop
set OUTPUT_DIR=dist
set VERSION=1.0.0

REM Clean previous builds
echo Cleaning previous builds...
if exist %OUTPUT_DIR% rmdir /s /q %OUTPUT_DIR%
mkdir %OUTPUT_DIR%

echo.
echo Building for Windows (x64)...
dotnet publish %PROJECT_DIR% ^
    -c Release ^
    -r win-x64 ^
    --self-contained true ^
    -p:PublishSingleFile=true ^
    -p:Version=%VERSION% ^
    -o %OUTPUT_DIR%\windows-x64

if %ERRORLEVEL% NEQ 0 (
    echo Build failed!
    exit /b %ERRORLEVEL%
)

echo.
echo ==========================================
echo Build completed successfully!
echo ==========================================
echo.
echo Build output: %OUTPUT_DIR%\windows-x64\Sup.exe
echo.
echo To run: cd %OUTPUT_DIR%\windows-x64 ^& Sup.exe
echo.

pause
