#include "stdafx.h"
#include <exception>

#if defined(_MSC_VER) || defined(_WIN32)
#define FUNCSTUB_AGGRESSIVEINLINE __forceinline
#else
#define FUNCSTUB_AGGRESSIVEINLINE // not supported, not implemented
#endif

#define FUNCSTUB_DECORTYPE __cdecl

// ----------------------------------
//              BOOST
// ----------------------------------

namespace boost {
    // Should get inlined. aggressive inlining breaks compilation for some reason.
    void FUNCSTUB_DECORTYPE throw_exception(std::exception const&) {}
}

// // TODO(MattMatt2000): Implement eastl SWIG wrappers
// // ----------------------------------
// //              EASTL
// // ----------------------------------
// #include <EABase/eabase.h>
// #include <stddef.h>
// #include <new>
// 
// #if defined(EA_COMPILER_NO_EXCEPTIONS) && (!defined(__MWERKS__) || defined(_MSL_NO_THROW_SPECS)) && !defined(EA_COMPILER_RVCT)
// #define THROW_SPEC_0    // Throw 0 arguments
// #define THROW_SPEC_1(x) // Throw 1 argument
// #else
// #define THROW_SPEC_0    // throw()
// #define THROW_SPEC_1(x) // throw(x)
// #endif
// 
// // Custom operator new using malloc
// void* operator new[](size_t size, const char* /*name*/, int /*flags*/,
//     unsigned /*debugFlags*/, const char* /*file*/, int /*line*/) THROW_SPEC_1(std::bad_alloc)
// {
//     return malloc(size);
// }
// 
// void* operator new[](size_t size, size_t alignment, size_t alignmentOffset, const char* /*name*/,
//     int flags, unsigned /*debugFlags*/, const char* /*file*/, int /*line*/) THROW_SPEC_1(std::bad_alloc)
// {
//     return _aligned_offset_malloc(size, alignment, alignmentOffset);
// }
// 
// void* operator new(size_t size) THROW_SPEC_1(std::bad_alloc)
// {
//     return malloc(size);
// }
// 
// void* operator new[](size_t size) THROW_SPEC_1(std::bad_alloc)
// {
//     return malloc(size);
// }
// 
// 
// // Custom operator delete using free
// void operator delete(void* p) THROW_SPEC_0
// {
//     if (p) // The standard specifies that 'delete NULL' is a valid operation.
//         free(p);
// }
// 
// void operator delete[](void* p) THROW_SPEC_0
// {
//     if (p)
//         free(p);
// }
// 