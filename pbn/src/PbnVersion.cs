namespace pbn;

/// <summary>
///    The version of the PBN file.
/// </summary>
public enum PbnVersion
{
    Unknown = -1,
    NotSpecified = 0,
    V10 = 100,
    V20 = 200,
    V21 = 210
}

public static class PbnVersionHelper
{
    /// <summary>
    /// Parses the PBN version from a string. Returns <see cref="PbnVersion.Unknown"/> if the version is not recognized.
    /// </summary>
    public static PbnVersion FromString(string versionString)
    {
        if (versionString == "1.0")
        {
            return PbnVersion.V10;
        }
        else if (versionString == "2.0")
        {
            return PbnVersion.V20;
        }
        else if (versionString == "2.1")
        {
            return PbnVersion.V21;
        }
        else
        {
            return PbnVersion.Unknown;
        }
    }
}