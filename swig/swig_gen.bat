@echo off

echo --------------------------------------
echo Generating C# wrappers with SWIG
echo --------------------------------------

swig -c++ -csharp -outdir csfiles^
	 -I../LandmarkDetector.Windows/include^
	 SharpFace.i

echo Generated SWIG files!
pause