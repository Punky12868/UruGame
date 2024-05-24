using System.Collections.Generic;

public class NullOrCero
{
    public static bool isListNullOrCero<T>(List<T> value)
    {
        if (value == null)
        {
            return true;
        }
        else if (value.Count <= 0)
        {
            return true;
        }
        return false;
    }

    public static bool isArrayNullOrCero<T>(T[] value)
    {
        if (value == null)
        {
            return true;
        }
        else if (value.Length <= 0)
        {
            return true;
        }
        return false;
    }
}
