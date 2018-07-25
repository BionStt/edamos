using System;
using System.IO;
using System.Runtime.Serialization.Json;

namespace Edamos.Core
{
    public static class Serialization
    {
        public static byte[] SerializeJson(object value)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(value.GetType());
            using (MemoryStream memoryStream = new MemoryStream())
            {
                serializer.WriteObject(memoryStream, value);

                memoryStream.Seek(0, SeekOrigin.Begin);

                return memoryStream.ToArray();
            }
        }

        public static object DeserializeJson(byte[] data, Type type)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(type);
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                object result = serializer.ReadObject(memoryStream);

                return result;
            }
        }
    }
}