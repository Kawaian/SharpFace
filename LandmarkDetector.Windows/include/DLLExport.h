#pragma once

#ifndef STATIC_LINK
#ifdef DLL_EXPORT
#define OPENFACE_API __declspec(dllexport)
#else
#define OPENFACE_API __declspec(dllimport)
#endif

#else
#define OPENFACE_API
#endif