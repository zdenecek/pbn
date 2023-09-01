using System.IO;

namespace pbn;

public class PbnSerializer
{
    /// <summary>
    /// Serialize the PBN file to a physical file.
    /// </summary>
    /// <param name="file">The PbnFile to serialize.</param>
    /// <param name="filename">Path to save the file to.</param>
    public void Serialize(PbnFile file, string filename)
    {
        string name = filename;
        if (!filename.EndsWith(".pbn"))
        {
            name += ".pbn";
        }

        using (TextWriter writer = new StreamWriter(name))
        {
            Serialize(file, writer);
        }
    }

    /// <summary>
    /// Serialize the PBN file to a stream.
    /// </summary>
    /// <param name="file">The PbnFile to serialize.</param>
    /// <param name="outputStream">Stream to serialize the file to.</param>
    public void Serialize(PbnFile file, TextWriter outputStream)
    {
        file.Serialize(outputStream);
    }
}