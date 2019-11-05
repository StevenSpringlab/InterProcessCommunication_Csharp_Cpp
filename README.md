# InterProcessCommunication_Csharp_Cpp
Inter Process Communication between C# and C++. 
Has various examples, including templates. 
No 'unsafe' keywords used.

Access hierarchy for external to internal:
YourCsharpCode.cs <-> ExternalIPC.cs <-> NativeMethods.cs <-> SensorInterops.h/.cpp <-> ActualCPPClass.h/.cpp

Besides the basic code required for IPC, example functions are included for various purposes:
- Getting and setting basic data and simple structs from C++ to C#
- Setting an array of structs to C++ from C#
- Getting structs containing 1 or 2 dynamic arrays from C++ to C#. 
  These use templates to deconstruct and reconstruct on each side, which are also provided

Some warnings regarding data transferral over IPC:
1. Passing data over IPC does not ensure it is retained. So make sure not to pass local copies of data. 
Make sure they persist, otherwise they might go out of scope between handling the data in C# and sending it from C++

2. Don't send sequential boolean values over IPC! C# and C++ handle these differently, messing up the marshalling. 
My advice, use int values to avoid any chance of mixing this up, rather than just not sending them after another.

