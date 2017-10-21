#include "stdafx.h"
#include <exception>

#if defined(_MSC_VER) || defined(_WIN32)
#define FUNCSTUB_AGGRESSIVEINLINE __forceinline
#else
#define FUNCSTUB_AGGRESSIVEINLINE // not supported, not implemented
#endif

#define FUNCSTUB_DECORTYPE __cdecl

namespace boost {
    // Should get inlined. aggressive inlining breaks compilation for some reason.
    void FUNCSTUB_DECORTYPE throw_exception(std::exception const&) {}
}