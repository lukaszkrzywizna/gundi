﻿using Gundi;

namespace AnotherAssembly;

public class BaseException : Exception
{
    public BaseException(Type unionType, string expectedCase, string actualCase)
        : base($"Wrong {unionType.Name} cast. Expected: {expectedCase}, Actual: {actualCase}")
    {
    }
}

public class MyException : BaseException
{
    public MyException(Type unionType, string expectedCase, string actualCase)
        : base(unionType, expectedCase, actualCase)
    {
    }
}

//[Union(CustomCastException = typeof(MyException))]
public partial record MyAssemblyUnion
{
    static partial void Cases(int a, string b);
}