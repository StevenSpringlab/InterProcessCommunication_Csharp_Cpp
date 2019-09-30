using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/// <summary>
/// Exposes C# functions, providing access through IPC to C++ implementations
/// 
/// Access hierarchy for external to internal:
/// ExternalIPC.cs -> NativeMethods.cs -> SensorInterops.h/.cpp -> ActualCPPClass.h/.cpp
/// </summary>
namespace InteropsLibrary
{
    /// <summary>
    /// Class to communicate with C++ <for your application>.
    /// 
    /// NOTE: Referenced content of this region from Frame.cs in Realsense project
    /// HandleRef variable, Release function, IDisposable region, Marshaler class
    /// </summary>
    public class ExternalIPC : IDisposable
    {
        // Function pointers for complex generics
        delegate T ActionRefArray1<T, U>(ref U myArray);
        delegate T ActionRefArray2<T, U, V>(ref U myArray1, ref V myArray2);
        
        // Handle for IPC
        internal HandleRef m_instance;
        
        // Returns whether sensor was properly validated and initialized
        private static readonly ExternalIPC instance = new ExternalIPC();

        // Note: constructor is 'private'
        private ExternalIPC() { }

        // Get Singleton instance
        public static ExternalIPC GetInstance()
        {
            return instance;
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                //Release();
                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~ExternalIPC()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion

        // Free sensor handle. Unused, added for completeness
        public void Release()
        {
            m_instance = new HandleRef(this, IntPtr.Zero);
        }

        #region Dll Setup

        /// <summary>
        /// Create instance of sensor, store instance reference and check sensor status.
        /// Includes debug output of initialisation and boolean for showing openCV windows.
        /// </summary>
        public void InitIPCHandle()
        {
            // Check if handle already exists
            if (m_instance.Handle != IntPtr.Zero)
                return;

            // Set reference handle
            m_instance = new HandleRef(this, NativeMethods.InitHandle());
        }

        #endregion
        
        #region Function Examples
        
        /// <summary> ... </summary>
        public bool BasicFunction(int myInt)
        {
            if (!HandleValidated)
                return false;

            NativeMethods.BasicFunction(m_instance.Handle, myInt);
            return true;
        }
        
        /// <summary> ... </summary>
        public bool SetSimpleStruct(MyStruct myStruct)
        {
            if (!HandleValidated)
                return false;

            NativeMethods.SetSimpleStruct(m_instance.Handle, myStruct);
            return true;
        }
        
        /// <summary> ... </summary>
        public bool UpdateSimpleStruct(ref MyStruct myStruct)
        {
            if (!HandleValidated)
                return false;
                
            bool success = false;
            IntPtr myPtr = new IntPtr();
            try
            {
                // Allocate unmanaged memory for MyStruct
                myPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MyStruct)));
                Marshal.StructureToPtr(myStruct, myPtr, false /*Don't erase struct*/);

                // Internally updates the calibration settings and color cropping, then recalculates
                // the cropped resolutions. Finally returns the updated tracker calibration settings
                NativeMethods.UpdateCalibrationSettings(m_instance.Handle, ref myPtr);

                // Assign returned data from unmanaged code
                myStruct = (MyStruct)Marshal.PtrToStructure(myPtr, typeof(MyStruct));

                success = true;
            }
            finally
            {
                // Free unmanaged memory
                Marshal.FreeHGlobal(myPtr);
            }
            
            return success;
        }
        
        /// <summary> ... </summary>
        public bool SetStructArray(List<MyStruct> myStructList)
        {
            if (!HandleValidated)
                return false;

            bool success = false;
            IntPtr[] listPtr = new IntPtr[myStructList.Count];
            try
            {
                // 2. Allocate unmanaged memory in a list of pointers per item
                for (int i = 0; i < myStructList.Count; i++)
                {
                    listPtr[i] = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MyStruct)));
                    Marshal.StructureToPtr(myStructList[i], listPtr[i], false /*No need to erase struct*/);
                }

                // 3. Set color blobs to tracker
                NativeMethods.SetStructArray(m_instance.Handle, listPtr, myStructList.Count);
                success = true;
            }
            finally
            {
                // 4. Free unmanaged memory
                for (int i = 0; i < listPtr.Length; i++)
                    Marshal.FreeHGlobal(listPtr[i]);
            }
            return success;
        }
        
        /// <summary> ... </summary>
        public bool GetArrayData(ref List<MyStruct> myStructList)
        {
            if (!HandleValidated)
                return false;

            // Initialise variables
            MyStruct tempStruct;
            int structSizeBlobData = Marshal.SizeOf(typeof(MyStruct));

            // Get MyStruct from tracker
            NativeMethods.GetArrayData(m_instance.Handle, out IntPtr blobArrayData, out int blobCount);

            // Recreate array of MyStruct from unmanaged code
            for (int i = 0; i < blobCount; i++)
            {
                IntPtr data = new IntPtr(blobArrayData.ToInt64() + structSizeBlobData * i);
                tempStruct = (MyStruct)Marshal.PtrToStructure(data, typeof(MyStruct));
                myStructList.Add(tempStruct);
            }

            return true;
        }
        
        /// <summary> ... </summary>
        public bool GetArrayStruct1ArrayData(ref List<MyStruct> myStructList)
        {
            if (!HandleValidated)
                return false;

            // Initialize variables and get MyStruct
            IntPtr debugOutput = NativeMethods.GetArrayStruct1ArrayData(m_instance.Handle,
                out IntPtr myData1Array,      out int myData1ArrayCount,
                out IntPtr myData1SizesArray, out int myData1SizesArrayCount);

            // Clear old data
            myStructList.Clear();

            // Assign function to function variable to be called during filled of struct
            ActionRefArray1<MyStruct, Point_2f[]> myFuncVar = FillMyStructA;
            
            // Have given struct filled with data from C++
            FillStructList_1(ref myStructList, myFuncVar,
                ref myDataArray, ref myDataArrayCount, ref myDataSizesArray, ref myDataSizesArrayCount);

            return true;
        }

        /// <summary> ... </summary>
        public bool GetArrayStruct2ArrayData(ref List<MyStruct> myStructList)
        {
            if (!HandleValidated)
                return false;

            // 2. Initialize variables and get MyStruct from tracker
            NativeMethods.GetArrayStruct2ArrayData(m_instance.Handle,
                out IntPtr myData1Array,      out int myData1ArrayCount,
                out IntPtr myData1SizesArray, out int myData1SizesArrayCount,
                out IntPtr myData2Array,      out int myData2ArrayCount,
                out IntPtr myData2SizesArray, out int myData2SizesArrayCount,
                );

            // Clear old data
            myStructList.Clear();

            // Assign function to function variable to be called during filled of struct
            ActionRefArray2<MyStruct, char[], Point_2f[]> myFuncVar = FillMyStructB;
            
            // Have given struct filled with data from C++
            FillStructList_2(ref myStructList, myFuncVar,
                ref myData1Array, ref myData1ArrayCount, ref myData1SizesArray, ref myData1SizesArrayCount,
                ref myData2Array, ref myData2ArrayCount, ref myData2SizesArray, ref myData2SizesArrayCount);

            return true;
        }
        
        #endregion
    }
}