#ifndef MY_INTEROPS_H
#define MY_INTEROPS_H

#include "ActualCPPClass.h"

// Prevent name mangling of C++
#ifdef __cplusplus
extern "C"
{
#endif
    // ************** CPP ENTRY CLASS FOR MARSHALLING **************

    typedef struct handle_ref handle_ref;
    // Default zero return value
    int zeroValue = 0;
    int* zeroPtr = &zeroValue;
    // Default debug return value (better to use vector<char> setup?)
    std::string invalidRefTextStd = "Reference not set";
    const char* invalidRefText = invalidRefTextStd.c_str();

    // Dll setup
    __declspec(dllexport) handle_ref* InitHandle();
    
    // Basic call
    __declspec(dllexport) void BasicFunction(handle_ref* ref, int myValue);
    
    // Set simple struct
    __declspec(dllexport) void SetSimpleStruct(handle_ref* ref, MyStruct* setStruct);
    
    // Update simple struct (set and get)
    __declspec(dllexport) void UpdateSimpleStruct(handle_ref* ref, MyStruct** refStruct);
    
    // Set struct array
    __declspec(dllexport) void SetStructArray(handle_ref* ref, MyStruct** structArray, int arraySize);

    // Get 1D array data
    __declspec(dllexport) void GetArrayData(handle_ref* ref, MyStruct** structArray, int* arraySize);
    
    // Get 1D array for a struct array (with a pointer to return debug text)
    __declspec(dllexport) const char* GetArrayStruct1ArrayData(handle_ref* ref,
        MyStruct** elements1Array, int* elements1Count,             // Flattened 2D MyStruct array
        int** elements1SizesArray, int* elements1SizesArrayCount);  // Array of sizes
        
    // Get 2 1D arrays for a struct array
    __declspec(dllexport) void GetArrayStruct2ArrayData(handle_ref* ref,
        MyStruct** elements1Array, int* elements1Count,             // Flattened 2D MyStruct array
        int** elements1SizesArray, int* elements1SizesArrayCount,   // Array of sizes
        MyStruct** elements2Array, int* elements2Count,             // Flattened 2D MyStruct array
        int** elements2SizesArray, int* elements2SizesArrayCount);  // Array of sizes

#ifdef __cplusplus
}
#endif

#endif // INTEROPS_TRACKER_H