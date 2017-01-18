@echo off
SETLOCAL

SET FAKE_PATH=packages\build\FAKE\tools\Fake.exe
paket.exe restore

IF [%1]==[] (
    "%FAKE_PATH%" "build.fsx" "Default" 
) ELSE (
    "%FAKE_PATH%" "build.fsx" %* 
) 
