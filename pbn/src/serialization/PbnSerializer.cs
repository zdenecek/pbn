using System.IO;

namespace pbn;

public class PbnSerializer : SerializerBase
{
    /// <summary>
    ///     Serialize the PBN file to a stream.
    /// </summary>
    /// <param name="file">The PbnFile to serialize.</param>
    /// <param name="outputStream">Stream to serialize the file to.</param>
    public override void Serialize(PbnFile file, TextWriter outputStream)
    {
        file.Serialize(outputStream);
    }
}