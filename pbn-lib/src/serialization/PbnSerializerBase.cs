using System.IO;

namespace pbn.serialization;

public abstract class SerializerBase : ISerializer
{
    /// <summary>
    ///     Serialize the PBN file to a stream.
    /// </summary>
    /// <param name="file">The PbnFile to serialize.</param>
    /// <param name="outputStream">The stream to serialize to.</param>
    public abstract void Serialize(PbnFile file, TextWriter outputStream);

    /// <summary>
    ///     Serialize the PBN file to a physical file.
    /// </summary>
    /// <param name="file">The PbnFile to serialize.</param>
    /// <param name="filename">Path to save the file to.</param>
    public void Serialize(PbnFile file, string filename)
    {
        using TextWriter writer = new StreamWriter(filename);
        Serialize(file, writer);
    }
}