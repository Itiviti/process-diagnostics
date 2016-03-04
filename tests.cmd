msbuild /t:Build /p:Configuration=Debug

cd procdiag.tests\bin\Debug
"%NUNIT_HOME%\nunit3-console.exe" procdiag.tests.dll

pause



