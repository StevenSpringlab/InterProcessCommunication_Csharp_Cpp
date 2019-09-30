// Parse 2D array into 1D array for IPC. Directly place input elements into output elements
// inputArray holds 2D array, other 4 variables contain IPC ready output variables
template <class T>
void SensorCommLayer::ParseArrayIPC_2D(std::vector<std::vector<T>>* inputArray,
    std::vector<T>* retainElementsArray, std::vector<int>* retainElementsGroupSizesArray,
    T** elementsArray, int* elementsCount, int** elementGroupSizesArray, int* elementGroupSizesCount)
{
    // Create function variable to handle copying of elements directly
    T(SensorCommLayer::*pFunc)(T) = &SensorCommLayer::CopyType;
    // Call generic parser
    ParseArrayIPC_2D(inputArray, retainElementsArray, retainElementsGroupSizesArray,
        elementsArray, elementsCount, elementGroupSizesArray, elementGroupSizesCount, pFunc);
}

// Parse 2D array into 1D array for IPC. Includes a function variable to handle converting Tin into Tout
// inputArray holds 2D array, other 4 variables contain IPC ready output variables
template <class Tin, class Tout>
void SensorCommLayer::ParseArrayIPC_2D(std::vector<std::vector<Tin>>* inputArray,
    std::vector<Tout>* retainElementsArray, std::vector<int>* retainElementsGroupSizesArray,
    Tout** elementsArray, int* elementsCount, int** elementGroupSizesArray, int* elementGroupSizesCount,
    Tout(SensorCommLayer::*pFunction)(const Tin))
{
    // 1. Traverse inputArray and assign to intermediate outputArray, possibly via pFunction
    for (int i = 0; i < inputArray->size(); i++)
    {
        // 1a. Append all elements of a group
        for (int j = 0; j < inputArray->at(i).size(); j++)
        {
            Tout element = (this->*pFunction)(inputArray->at(i)[j]);
            retainElementsArray->push_back(std::move(element));
        }

        // 1b. Store group frameSize in array
        retainElementsGroupSizesArray->push_back((int)inputArray->at(i).size());
    }

    // 2. Assign data
    *elementsArray = retainElementsArray->data();
    *elementsCount = (int)retainElementsArray->size();
    *elementGroupSizesArray = retainElementsGroupSizesArray->data();
    *elementGroupSizesCount = (int)retainElementsGroupSizesArray->size();
}
