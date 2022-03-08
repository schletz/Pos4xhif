rd /S /Q ".vs"
rd /S /Q ".vscode"
  
FOR /D /R %%d IN (*) DO (
  rd /S /Q "%%d\bin"
  rd /S /Q "%%d\obj"
  rd /S /Q "%%d\.vs"
  rd /S /Q "%%d\.vscode"
)

cd ExamManager.Webapp
dotnet watch run
