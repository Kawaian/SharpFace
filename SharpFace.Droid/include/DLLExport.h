#pragma once

#ifdef DLL_EXPORT
#define OPENFACE_API __attribute__ ((visibility ("default")))
#else
#define OPENFACE_API __attribute__ ((visibility ("default")))
#endif

