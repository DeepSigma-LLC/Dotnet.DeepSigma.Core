using DeepSigma.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSigma.Core.Serialization;

/// <summary>
/// Provides methods for serializing and deserializing collections of objects to and from binary format.
/// </summary>
public static class BinarySerializer
{
    /// <summary>
    /// Serializes a collection of objects to a byte array using a specified, custom writer method for each object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="items"></param>
    /// <param name="object_writer_method"></param>
    /// <param name="file_version"></param>
    /// <returns></returns>
    public static byte[] Serialize<T>(IEnumerable<T> items, Action<BinaryWriter, T> object_writer_method, int file_version)
    {
        using MemoryStream stream = new();
        using BinaryWriter writer = new(stream);

        writer.Write(file_version);

        uint number_of_items = (uint)items.Count();
        writer.Write(number_of_items);

        foreach (T item in items)
        {
            object_writer_method(writer, item);
        }
        writer.Flush();
        stream.Position = 0;
        return stream.ToArray();
    }

    /// <summary>
    /// Deserializes a byte array into a collection of objects using a specified, custom parser method for each object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="object_parser_method"></param>
    /// <returns></returns>
    public static IEnumerable<T> Deserialize<T>(byte[] data, Func<BinaryReader, T> object_parser_method)
    {
        using MemoryStream stream = new(data);
        using BinaryReader reader = new(stream);

        int file_version = reader.ReadInt32();
        uint number_of_items = reader.ReadUInt32();

        List<T> items = [];
        for (uint i = 0; i < number_of_items; i++)
        {
            T? item = object_parser_method(reader);
            if (item != null)
            {
                items.Add(item);
            }
        }
        return items;
    }
}