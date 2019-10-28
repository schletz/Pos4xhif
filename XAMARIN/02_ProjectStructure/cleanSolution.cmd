@echo off
REM Löscht alle temporären Visual Studio Dateien
rd /S /Q .vs
FOR /D %%d IN (*) DO (
    rd /S /Q %%d\bin
    rd /S /Q %%d\obj
    FOR /D %%s IN (%%d\*) DO (
        rd /S /Q %%s\bin
        rd /S /Q %%s\obj
    )    
)
