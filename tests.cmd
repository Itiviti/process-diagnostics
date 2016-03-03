msbuild /t:Build /p:Configuration=Release

cd procdiag.tests\bin\Debug
nunit3-console.exe procdiag.tests.dll

pause



