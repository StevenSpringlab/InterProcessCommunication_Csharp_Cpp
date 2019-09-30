#pragma once

#include "MyInterops.h"

#if RUN_DLL != 1
// Console application entry point, used for debugging C++ side
int main(void)
{
    ActualCPPClass& myClass = ActualCPPClass::getInstance();
    myClass.RunConsole(true);

    return EXIT_SUCCESS;
}
#endif

/// Struct to contain shared pointer to ActualCPPClass
struct handle_ref
{
    std::shared_ptr<ActualCPPClass> shrPtrHandle;
};

/// Create reference to ActualCPPClass and do setup if needed
handle_ref* InitHandle()
{
    // Initialize handle_ref
    handle_ref* ref = new handle_ref{ std::make_shared<ActualCPPClass>() };
    
    if (ref != nullptr)
        // Do further setup after calling ctor of ActualCPPClass
        ref->shrPtrHandle->DoSetup();
        
    // Return reference to handle, to be used by subsequent calls to IPC
    return ref;
}

/// Set basic value type
void BasicFunction(handle_ref* ref, int myValue)
{
    ref->shrPtrSensor->BasicFunction(myValue);
}

/// Set simple struct
void SetSimpleStruct(handle_ref* ref, MyStruct* setStruct)
{
    ref->shrPtrSensor->SetSimpleStruct(setStruct);
}

/// Update simple struct
void UpdateSimpleStruct(handle_ref* ref, MyStruct** setStruct)
{
    ref->shrPtrSensor->UpdateSimpleStruct(setStruct);
}

/// Set struct array
void SetStructArray(handle_ref* ref, MyStruct** structArray, int arraySize)
{
    ref->shrPtrSensor->SetStructArray(structArray, arraySize);
}

// Get 1D array data
void GetArrayData(handle_ref* ref, MyStruct** structArray, int* arraySize)
{
    ref->shrPtrSensor->GetArrayData(structArray, arraySize);
}

/// Get 1D array for a struct array
void GetArrayStruct1ArrayData(handle_ref* ref,
    MyStruct** elements1Array, int* elements1Count, int** elements1SizesArray, int* elements1SizesArrayCount)
{
    return ref->shrPtrSensor->GetArrayStruct1ArrayData(
        elements1Array, elements1Count, elements1SizesArray, elements1SizesArrayCount);
}

/// Get 2 1D arrays for a struct array
void GetArrayStruct2ArrayData(handle_ref* ref,
    MyStruct** elements1Array, int* elements1Count, int** elements1SizesArray, int* elements1SizesArrayCount,
    MyStruct** elements2Array, int* elements2Count, int** elements2SizesArray, int* elements2SizesArrayCount)
{
    ref->shrPtrSensor->GetArrayStruct2ArrayData(
        elements1Array, elements1Count, elements1SizesArray, elements1SizesArrayCount,
        elements2Array, elements2Count, elements2SizesArray, elements2SizesArrayCount);
}