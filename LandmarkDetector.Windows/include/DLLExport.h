#pragma once

#ifdef DLL_EXPORT
#define OPENFACE_API __declspec(dllexport)
#else
#define OPENFACE_API __declspec(dllimport)
#endif

