using System.Runtime.InteropServices;

namespace pbn.dds;

public static class DdsInterop
{
    private const string DllPath = "dds.dll"; // Update this with the correct DLL name

    [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
    public static extern int CalcAllTablesPBN(
        ref DdsTypes.ddTableDealsPBN dealsp,
        int mode,
        int[] trumpFilter,
        ref DdsTypes.ddTablesRes resp,
        ref DdsTypes.allParResults presp);

}