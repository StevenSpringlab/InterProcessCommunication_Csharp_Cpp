/* Inter Process Communication Templates - C# 
 * 
 * Generics (i.e. templates) to process 2D data as array of structs/classes with 1 or 2 arrays inside
 */

/// <summary>
/// Fill a list of the given struct/class type, which contains a single dynamic array
/// The data is received from C++ in the form of a 1D array.
/// </summary>
/// <typeparam name="T"> The struct/class to be reconstructed </typeparam>
/// <typeparam name="U"> The dynamic array type inside the struct/class T </typeparam>
/// <param name="myStructList"> The output which stores the reconstructed list of struct/class T </param>
/// <param name="funcVar">
/// A function variable to handle storing the dynamic array in the output list.
/// Handle casting (such as char[] to string) if needed
/// </param>
/// <param name="elementsArray"> Array of type U, contains all elements of type U </param>
/// <param name="elementsCount"> Size of the elementsArray </param>
/// <param name="elementGroupSizesArray"> Array of sizes, to allow splitting up the elementsArray by its groups </param>
/// <param name="elementGroupSizesCount"> Size of the elementGroupSizesArray </param>
private void FillStructList_1<T, U>(ref List<T> myStructList, ActionRefArray1<T, U[]> funcVar,
    ref IntPtr elementsArray, ref int elementsCount, ref IntPtr elementGroupSizesArray, ref int elementGroupSizesCount)
{
    // 1. Ensure output array is empty. Empty can be valid result
    myStructList.Clear();

    // 2. Check if any results were found
    if (elementGroupSizesCount == 0)
        return;

    // 3. Create the aray of sizes per group
    int[] elementGroupSizesTemp = new int[elementGroupSizesCount];
    Marshal.Copy(elementGroupSizesArray, elementGroupSizesTemp, 0, elementGroupSizesCount);

    // 4. Initialise variables
    int counter = 0;
    int groupIndex = 0;
    int groupContentIndex = 0;
    U tempElement;
    U[] tempGroupArray = new U[elementGroupSizesTemp[0]];
    int elementStructSize = Marshal.SizeOf(typeof(U));

    // 5. Traverse all elements of array and fill groups into tempGroupArray, into output struct
    while (counter < elementsCount)
    {
        // 6a. If completed points in group, continue with next group, if any exists
        if (groupContentIndex >= elementGroupSizesTemp[groupIndex])
        {
            groupIndex++;
            groupContentIndex = 0;
            tempGroupArray = new U[elementGroupSizesTemp[groupIndex]];
        }

        // 6b. Create element T and assign to tempArray
        IntPtr elementPtr = new IntPtr(elementsArray.ToInt64() + elementStructSize * counter);
        tempElement = (U)Marshal.PtrToStructure(elementPtr, typeof(U));
        tempGroupArray[groupContentIndex] = tempElement;

        counter++;
        groupContentIndex++;

        // 6c. At end of group
        if (groupContentIndex >= elementGroupSizesTemp[groupIndex])
        {
            // Add group to output list, using the functionVariable to handle type conversion if needed
            myStructList.Add(funcVar(ref tempGroupArray));
        }
    }
}

/// <summary>
/// Fill a list of the given struct/class type, which contains 2 dynamic arrays
/// The data is received from C++ in the form of 2 1D array.
/// </summary>
/// <typeparam name="T"> The struct/class to be reconstructed </typeparam>
/// <typeparam name="U"> The 1th dynamic array type inside the struct/class T </typeparam>
/// <typeparam name="V"> The 2nd dynamic array type inside the struct/class T </typeparam>
/// <param name="myStructList"> The output which stores the reconstructed list of struct/class T </param>
/// <param name="funcVar">
/// A function variable to handle storing the dynamic array in the output list.
/// Handle casting (such as char[] to string) if needed
/// </param>
/// <param name="elementsArray1"> Array of type U, passed from IPC </param>
/// <param name="elementsCount1"> Size of the elementsArray1 </param>
/// <param name="elementGroupSizesArray1"> 1th array of sizes, to allow splitting up the elementsArray1 by its groups </param>
/// <param name="elementGroupSizesCount1"> Size of the elementGroupSizesArray1 </param>
/// <param name="elementsArray2"> Array of type V, passed from IPC </param>
/// <param name="elementsCount2"> Size of the elementsArray2 </param>
/// <param name="elementGroupSizesArray2"> 2nd array of sizes, to allow splitting up the elementsArray2 by its groups </param>
/// <param name="elementGroupSizesCount2"> Size of the elementGroupSizesArray2 </param>
private void FillStructList_2<T, U, V>(ref List<T> myStructList, ActionRefArray2<T, U[], V[]> funcVar,
    ref IntPtr elementsArray1, ref int elementsCount1, ref IntPtr elementGroupSizesArray1, ref int elementGroupSizesCount1,
    ref IntPtr elementsArray2, ref int elementsCount2, ref IntPtr elementGroupSizesArray2, ref int elementGroupSizesCount2)
{
    /* INFO:
     * This function fills the local variables tempGroupArray1 and tempGroupArray2 with the arrays from C++
     * Each time a group of both is ready, call the function variable to fill an element of T in the struct with both arrays
     * 
     * EXTRA / FUTURE:
     * Consider expanding from 2 dynamic variables to a setup with X dynamic variables
     */

    // 1. Only run if results were found in both groups, as each should contain the same number of size groups
    // So each array may contain different amounts of data and elements, but the number of groups must match!
    if (elementGroupSizesCount1 == 0 || elementGroupSizesCount2 == 0)
        return;

    // 2. Create the arays of sizes per group
    int[] elementGroupSizesTemp1 = new int[elementGroupSizesCount1];
    Marshal.Copy(elementGroupSizesArray1, elementGroupSizesTemp1, 0, elementGroupSizesCount1);
    int[] elementGroupSizesTemp2 = new int[elementGroupSizesCount2];
    Marshal.Copy(elementGroupSizesArray2, elementGroupSizesTemp2, 0, elementGroupSizesCount2);

    // 3. Initialise variables
    int counter1 = 0, counter2 = 0;
    int groupIndex1 = 0, groupContentIndex1 = 0;
    int groupIndex2 = 0, groupContentIndex2 = 0;
    U tempElement1;
    U[] tempGroupArray1 = new U[elementGroupSizesTemp1[0]];
    V tempElement2;
    V[] tempGroupArray2 = new V[elementGroupSizesTemp2[0]];
    int elementStructSize1 = Marshal.SizeOf(typeof(U));
    int elementStructSize2 = Marshal.SizeOf(typeof(V));
    bool groupCompleted1 = false, groupCompleted2 = false;
    bool arrayCompleted1 = false, arrayCompleted2 = false;

    // 4. Check all elements of both groups until both arrays have finished
    while (true)
    {
        // 5a. If completed all elements in a group, prepare for next group (but first wait for other array)
        if (!arrayCompleted1 && !groupCompleted1 && groupContentIndex1 >= elementGroupSizesTemp1[groupIndex1])
        {
            groupIndex1++;
            groupCompleted1 = true;
        }
        if (!arrayCompleted2 && !groupCompleted2 && groupContentIndex2 >= elementGroupSizesTemp2[groupIndex2])
        {
            groupIndex2++;
            groupCompleted2 = true;
        }

        // 5b. Create and fill element from C++ pointer data. Then assign to tempGroupArray at current group index
        if (!arrayCompleted1 && !groupCompleted1)
        {
            IntPtr elementPtr1 = new IntPtr(elementsArray1.ToInt64() + elementStructSize1 * counter1);
            tempElement1 = (U)Marshal.PtrToStructure(elementPtr1, typeof(U));
            tempGroupArray1[groupContentIndex1] = tempElement1;

            counter1++;
            groupContentIndex1++;
        }
        if (!arrayCompleted2 && !groupCompleted2)
        {
            IntPtr elementPtr2 = new IntPtr(elementsArray2.ToInt64() + elementStructSize2 * counter2);
            tempElement2 = (V)Marshal.PtrToStructure(elementPtr2, typeof(V));
            tempGroupArray2[groupContentIndex2] = tempElement2;

            counter2++;
            groupContentIndex2++;
        }

        // 5c. After parsing both groups, add both to output struct and if not finished, initialise next groups
        if (groupCompleted1 && groupCompleted2)
        {
            // Assign content of group array to current struct
            myStructList.Add(funcVar(ref tempGroupArray1, ref tempGroupArray2));

            // Reset booleans to allow next group being parsed
            groupCompleted1 = false;
            groupCompleted2 = false;

            // Reset tempGroupArray for next group, if not last group
            if (groupIndex1 < elementGroupSizesTemp1.Length)
            {
                tempGroupArray1 = new U[elementGroupSizesTemp1[groupIndex1]];
                groupContentIndex1 = 0;
            }
            else
                arrayCompleted1 = true;
            if (groupIndex2 < elementGroupSizesTemp2.Length)
            {
                tempGroupArray2 = new V[elementGroupSizesTemp2[groupIndex2]];
                groupContentIndex2 = 0;
            }
            else
                arrayCompleted2 = true;

            // If both arrays have finished, terminate loop
            if (arrayCompleted1 && arrayCompleted2)
                break;
        }
    }
}