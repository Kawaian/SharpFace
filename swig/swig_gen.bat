@echo off

echo ======================================
echo Generating C# wrappers with SWIG
echo ======================================

swig -c++ -csharp -outdir csfiles^
	 -I../LandmarkDetector.Windows/include^
	 SharpFace.i

echo Generated wrappers!
echo.
echo ======================================
echo Formatting code with AStyle
echo ======================================

tools\AStyle.exe  --style=allman --mode=c -n .\SharpFace_wrap.cxx

tools\AStyle.exe  --style=allman --mode=cs -n .\csfiles\*.cs

echo.
pause