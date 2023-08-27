using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace pbn;

using System;
using System.Runtime.InteropServices;

public static class DDSTypes
{
    public const int DDS_HANDS = 4;
    public const int DDS_SUITS = 4;
    public const int DDS_STRAINS = 5;
    public const int MAXNOOFBOARDS = 200;
    public const int MAXNOOFTABLES = 40;


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct ddTableDealPBN
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string cards;

        public ddTableDealPBN(string cards)
        {
            this.cards = cards;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ddTableDealsPBN
    {
        public int noOfTables;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAXNOOFTABLES * DDS_STRAINS)] // Adjust the size as needed
        public ddTableDealPBN[] deals;

        public ddTableDealsPBN(string[] cards)
        {
            this.noOfTables = cards.Length;
            this.deals = new ddTableDealPBN[MAXNOOFTABLES * DDS_STRAINS];
            for (int i = 0; i < cards.Length; i++)
            {
                this.deals[i] = new ddTableDealPBN(cards[i]);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ddTableResults
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = DDS_STRAINS * DDS_HANDS)]
        public int[] resTable;

        public ddTableResults()
        {
            this.resTable = new int[DDS_STRAINS * DDS_HANDS];
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ddTablesRes
    {
        public int noOfBoards;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAXNOOFTABLES * DDS_STRAINS)] // Adjust the size as needed
        public ddTableResults[] results;

        public ddTablesRes()
        {
            this.results = new ddTableResults[MAXNOOFTABLES * DDS_STRAINS];
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct parResults
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2 * 16, ArraySubType = UnmanagedType.U1)]
        public char[] parScore;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2 * 128, ArraySubType = UnmanagedType.U1)]
        public char[] parContractsString;

        public parResults()
        {
            this.parScore = new char[2 * 16];
            this.parContractsString = new char[2 * 128];
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct allParResults
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAXNOOFTABLES)]
        public parResults[] presults;

        public allParResults()
        {
            this.presults = new parResults[MAXNOOFTABLES];
        }
    }
}


public static class DDSInterop
{
    private const string DllPath = "dds.dll"; // Update this with the correct DLL name

    [DllImport(DllPath, CallingConvention = CallingConvention.StdCall)]
    public static extern int CalcAllTablesPBN(
        ref DDSTypes.ddTableDealsPBN dealsp,
        int mode,
        int[] trumpFilter,
        ref DDSTypes.ddTablesRes resp,
        ref DDSTypes.allParResults presp);

    public static int vulnerabilityToDDSMode(Vulnerability vul)
    {
        /* mode = 0: par calculation, vulnerability None
           mode = 1: par calculation, vulnerability All
           mode = 2: par calculation, vulnerability NS
           mode = 3: par calculation, vulnerability EW
           mode = -1: no par calculation */
        return vul switch
        {
            Vulnerability.NONE => 0,
            Vulnerability.NS => 2,
            Vulnerability.EW => 3,
            Vulnerability.BOTH => 1,
            _ => throw new ArgumentException($"Unknown vulnerability {vul}")
        };
    }

    public static int suitToDDSStrain(Suit suit)
    {
        return suit switch
        {
            Suit.SPADES => 0,
            Suit.HEARTS => 1,
            Suit.DIAMONDS => 2,
            Suit.CLUBS => 3,
            Suit.NOTRUMP => 4,
            _ => throw new ArgumentException($"Unknown suit {suit}")
        };
    }

    public static int positionToDDSPos(Position pos)
    {
        return pos switch
        {
            Position.NORTH => 0,
            Position.EAST => 1,
            Position.SOUTH => 2,
            Position.WEST => 3,
            _ => throw new ArgumentException($"Unknown position {pos}")
        };
    }
}


public class AnalysisServiceDDS : AnalysisService
{
    public AnalysisTable AnalyzePbn(string pbnDealString, Vulnerability vulnerability)
    {
        var deals = new DDSTypes.ddTableDealsPBN(new string[] { pbnDealString });

        var parResults = new DDSTypes.allParResults();

        var tables = new DDSTypes.ddTablesRes();

        var mode = DDSInterop.vulnerabilityToDDSMode(vulnerability);

        var trumpFilter = new int[DDSTypes.DDS_STRAINS];


        DDSInterop.CalcAllTablesPBN(ref deals, mode, trumpFilter, ref tables, ref parResults);


        return AnalysisTable.BuildAnalysisTable(
            (pos, suit) =>
                tables.results[0]
                    .resTable[
                        DDSTypes.DDS_HANDS *  DDSInterop.suitToDDSStrain(suit)+ DDSInterop.positionToDDSPos(pos) 
                    ]
            );
    }
}