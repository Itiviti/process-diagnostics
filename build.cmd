nuget restore
msbuild /t:Build /p:Configuration=Release

mkdir build
del build\*.* /Q
pushd .
cd src\bin\Release
..\..\..\packages\ILRepack.2.0.10\tools\ILRepack.exe procdiag.exe CommandLine.dll Microsoft.Diagnostics.Runtime.dll /out=..\..\..\build\procdiag.exe

popd 
mkdir contentFiles
copy build\procdiag.exe contentFiles\

nuget pack procdiag.nuspec -OutputDirectory build
pause



