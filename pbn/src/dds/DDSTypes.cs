using System;
using System.Linq;
using System.Runtime.InteropServices;
using pbn.model;

namespace pbn.dds;

public static class DdsTypes
{
    public const int DDS_HANDS = 4;
    public const int DDS_SUITS = 4;
    public const int DDS_STRAINS = 5;
    public const int MAXNOOFBOARDS = 200;
    public const int MAXNOOFTABLES = 40;

    public static int VulnerabilityToDdsMode(Vulnerability vul)
    {
        /* mode = 0: par calculation, vulnerability None
           mode = 1: par calculation, vulnerability All
           mode = 2: par calculation, vulnerability NS
           mode = 3: par calculation, vulnerability EW
           mode = -1: no par calculation */
        return vul switch
        {
            Vulnerability.None => 0,
            Vulnerability.Ns => 2,
            Vulnerability.Ew => 3,
            Vulnerability.Both => 1,
            _ => throw new ArgumentException($"Unknown vulnerability {vul}")
        };
    }

    public static int SuitToDdsStrain(Suit suit)
    {
        return suit switch
        {
            Suit.Spades => 0,
            Suit.Hearts => 1,
            Suit.Diamonds => 2,
            Suit.Clubs => 3,
            Suit.Notrump => 4,
            _ => throw new ArgumentException($"Unknown suit {suit}")
        };
    }

    public static int PositionToDdsPos(Position pos)
    {
        return pos switch
        {
            Position.North => 0,
            Position.East => 1,
            Position.South => 2,
            Position.West => 3,
            _ => throw new ArgumentException($"Unknown position {pos}")
        };
    }


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
            noOfTables = cards.Length;
            deals = new ddTableDealPBN[MAXNOOFTABLES * DDS_STRAINS];
            for (var i = 0; i < cards.Length; i++) deals[i] = new ddTableDealPBN(cards[i]);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ddTableResults
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = DDS_STRAINS * DDS_HANDS)]
        public int[] resTable;

        public ddTableResults()
        {
            resTable = new int[DDS_STRAINS * DDS_HANDS];
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
            results = new ddTableResults[MAXNOOFTABLES * DDS_STRAINS];
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
            parScore = new char[2 * 16];
            parContractsString = new char[2 * 128];
        }
        
        public string ParScoreString => new(parScore);
        public string ParContractsString => new(parContractsString);

        public Contract GetParContract(Position dealer)
        {
            // NS:NS 4Dx\0...\0EW:NS 4Dx\0...
            var str = dealer.IsNs() ? this.ParContractsString[..128]  : this.ParContractsString[128..256];
            var contractStr = string.Concat(str.SkipWhile(c => c != ' ').Skip(1).TakeWhile(c => c != '\0'));

            var declarerLetter = str[3];
            
            var level = int.Parse(contractStr[..1]);
            var suit = SuitHelpers.FromLetter(contractStr.SkipWhile(c => Char.IsDigit(c)).First());
            
            var state = contractStr.Contains("xx") ? ContractDoubleState.Redoubled : contractStr.Contains("x") ? ContractDoubleState.Doubled : ContractDoubleState.NotDoubled;
            
           
            var declarer = PositionHelpers.FromLetter(declarerLetter);
            
            return new Contract(level, suit, declarer, state);
        }

        public int GetParScore(Position dealer)
        {
                // str in format: NS -300\0\0\0\0...\0EW 300\0\0\0\0...\0       
                var str = dealer.IsNs() ? this.ParScoreString[..16]  : this.ParScoreString[16..32];
                str = string.Concat(str.Skip(3).TakeWhile(c => c != '\0'));
                var result =  int.Parse(str);
                if (!dealer.IsNs()) result *= -1;
                return result;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct allParResults
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAXNOOFTABLES)]
        public parResults[] presults;

        public allParResults()
        {
            presults = new parResults[MAXNOOFTABLES];
        }
    }
}