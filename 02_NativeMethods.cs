/* The following code contains various examples of possible functions for sending and receiving
 * data between C# and C++
 */

using System;
using System.Runtime.InteropServices;

namespace TrackerInteropsLibrary
{
    internal static class NativeMethods
    {
        public const string dllName = "MyDllName";
        
        // Info on marshalling strings: https://docs.microsoft.com/en-us/dotnet/framework/interop/default-marshaling-for-strings
        // Info on marshalling arrays:  https://docs.microsoft.com/en-us/dotnet/framework/interop/marshaling-different-types-of-arrays
        
        // NOTE: Boolean values are passed as int. Multiple consecutive booleans cause problems due to C# - C++ interpretation differences

        // Dll setup
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr InitHandle();
        
        // Basic call
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void BasicFunction(IntPtr handleRef, int myValue);
        
        // Set simple struct
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetSimpleStruct(IntPtr handleRef, MyStruct setStruct);
        
        // Update simple struct
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void UpdateSimpleStruct(IntPtr handleRef, ref IntPtr setStruct);
        
        // Set array data
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetStructArray(IntPtr handleRef, IntPtr[] newStructArray, int newArraySize);
        
        // Get 1D array data - CharSet Ansi needed?
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void GetArrayData(IntPtr handleRef, out IntPtr arrayData, out int arraySize);

        // Get 1D array for a struct array - CharSet Ansi needed?
        // See extra documentation on using this function to receive dynamic arrays over IPC
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr GetArrayStruct1ArrayData(IntPtr handleRef,
            out IntPtr elements1Array, out int elements1Count,
            out IntPtr elements1SizesArray, out int elements1SizesArrayCount);

        // Get 2 1D arrays for a struct array - CharSet Ansi needed?
        // See extra documentation on using this function to receive dynamic arrays over IPC
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr GetArrayStruct2ArrayData(IntPtr handleRef,
            out IntPtr elements1Array, out int elements1Count,
            out IntPtr elements1SizesArray, out int elements1SizesArrayCount,
            out IntPtr elements2Array, out int elements2Count,
            out IntPtr elements2SizesArray, out int elements2SizesArrayCount);
    }
}