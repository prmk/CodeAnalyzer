:run.bat
@echo off

@echo =========================================================
@echo Demonstrating the functionalities of Code Analyser Tool
@echo =========================================================

@echo Testing the tool for the current directory
cd Executive/bin/Debug
Executive.exe *.cs .

@echo Testing the tool for a directory input with recursive definition
cd Executive/bin/Debug 
Executive.exe /S ../../

@echo Testing the tool with a directory input and with relationships definition
cd Executive/bin/Debug  
Executive.exe /S /R .