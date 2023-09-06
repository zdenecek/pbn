using System.IO;

namespace pbn;

public interface ISerializer
{
    /// <summary>
    ///     Serialize the PBN file to a physical file.
    /// </summary>
    void Serialize(PbnFile file, TextWriter outputStream);
}