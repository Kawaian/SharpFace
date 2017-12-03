/** 
 * file: RuntimeInfo.cpp
 * author: Matyas Constans
 * info: Exports basic runtime information.
 */
#include <cstdio>

#ifdef _WIN32
#define RTI_EXPORT extern "C" __declspec(dllexport)
#define RTI_STD_CALL __stdcall

#else
#define RTI_EXPORT extern "C" __attribute__((visibility("default")))
#define RTI_STD_CALL

#endif

#define RTI_API(return_) RTI_EXPORT return_ RTI_STD_CALL

typedef unsigned char   int8;
typedef char            uint8;

RTI_API(int8) RTI_GetPointerSize()
{
    return sizeof(size_t);
}

RTI_API(uint8) TryInvoke()
{
    return 0x001;
}

#pragma GCC visibility push(default)

void MyTestFunc() { puts("LOL"); }
void MyTestFunc2() { puts("LOL"); }

#pragma GCC visibility pop