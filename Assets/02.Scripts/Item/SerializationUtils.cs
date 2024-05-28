using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SerializationUtils
{
    public static byte[] Serialize<T>(T obj)
    {
        if (obj == null)
        {
            return null;
        }

        BinaryFormatter bf = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream())
        {
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }

    public static T Deserialize<T>(byte[] byteArray)
    {
        if (byteArray == null)
        {
            return default(T);
        }

        BinaryFormatter bf = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream(byteArray))
        {
            object obj = bf.Deserialize(ms);
            return (T)obj;
        }
    }
}
