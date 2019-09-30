/* Inter Process Communication Templates - C++
 * 
 * The following function with overload handles parsing 2D input into a 1D array, with an optional function pointer
 * to handle any desired conversion from the stored data to the transmitted data
 *
 * To prepare the data for sending to C#, the 2D data is split up. An elements array and its size,
 * and array of sizes for the groups inside the elements array and the size of this array.
 * 
 * To use this function for a struct/class with multiple dynamic arrays inside, first move that data
 * into its own 2D array and then call this function.
 */

template <class T>
void ParseArrayIPC_2D(std::vector<std::vector<T>>* inputArray,
    std::vector<T>* retainElementsArray, std::vector<int>* retainElementsGroupSizesArray,
    T** elementsArray, int* elementsCount, int** elementGroupSizesArray, int* elementGroupSizesCount);
template <class Tin, class Tout>
void ParseArrayIPC_2D(std::vector<std::vector<Tin>>* inputArray,
    std::vector<Tout>* retainElementsArray, std::vector<int>* retainElementsGroupSizesArray,
    Tout** elementsArray, int* elementsCount, int** elementGroupSizesArray, int* elementGroupSizesCount,
    Tout(SensorCommLayer::*pFunction)(const Tin));