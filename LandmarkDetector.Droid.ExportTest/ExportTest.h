// file: ExportTest.h
// info: Test for exporting functions with clang
#pragma once
#define DLL_EXPORT __attribute__((visibility("default")))

DLL_EXPORT void ExportTest();
DLL_EXPORT void ExportTest2();