@echo off
SETLOCAL

SET FAKE_PATH=packages\build\FAKE\tools\Fake.exe
SET SCRIPT_PATH=build\Targets.fsx
"%FAKE_PATH%" "%SCRIPT_PATH%" %* 
