@echo off

swig -c++ -csharp -outdir csfiles -I../LandmarkDetector.Windows/include  SharpFace.i
pause