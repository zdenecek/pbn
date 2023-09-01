

namespace pbn.manipulators;

public class PbnBoardAnalyzer
{
    public enum AnalysisType
    {
        /// <summary>
        /// Analysis made in format of one made with Deep Finesse.
        /// Contains following tags:
        ///  <list>
        /// <item> Minimax </item>
        /// <item> Ability </item>
        /// </list>
        /// </summary>
        DeepFinesseAnalysis,
        
        /// <summary>
        /// Analysis in format of one made with Bridge Composer. Generated file usually is version 2.1.
        /// Contains following tags:
        /// <list>
        /// <item> OptimumResultTable </item>
        /// <item> OptimumScore </item>
        /// <item> DoubleDummyTricks </item>
        /// </list>
        /// Does not contain information about minimax contract, only score, so it cannot be used to generate
        /// <see cref="DeepFinesseAnalysis"/> like syntax.
        /// See bridgecomposer.pbn file for example.
        /// Note that BridgeComposer generated file is usually bloated.
        /// </summary>
        BridgeComposeAnalysis
    }
    
    public void AddAnalyses(PbnFile file)
    {
        foreach (var board in file.Boards)
        {
        }
    }


    public bool HasAnalysis(PbnFile.BoardContext context)
    {
        
    }

}