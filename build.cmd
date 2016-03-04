nuget restore
msbuild /t:Build /p:Configuration=Release

mkdir build
del build\*.* /Q
pushd .
cd src\bin\Release
..\..\..\packages\ILRepack.2.0.10\tools\ILRepack.exe procdiag.exe CommandLine.dll Microsoft.Diagnostics.Runtime.dll /out=..\..\..\build\procdiag.exe
copy procdiag.x86.exe ..\..\..\build\
popd 
nuget pack procdiag.nuspec -o build
pause



