nuget restore
msbuild /t:Build /p:Configuration=Release

mkdir build
pushd .
cd src\bin\Release
..\..\..\packages\ILRepack.2.0.10\tools\ILRepack.exe procdiag.exe CommandLine.dll Microsoft.Diagnostics.Runtime.dll /out=..\..\..\build\procdiag.exe
popd
nuget pack procdiag.1.0.0.nuspec -o build
pause



