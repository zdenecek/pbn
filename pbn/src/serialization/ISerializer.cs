using System.IO;

namespace pbn.serialization;

public interface ISerializer
{
    /// <summary>
    ///     Serialize the PBN file to a physical file.
    /// </summary>
    void Serialize(PbnFile file, TextWriter outputStream);
}