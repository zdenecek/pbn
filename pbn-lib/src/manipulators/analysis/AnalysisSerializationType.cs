namespace pbn.manipulators.analysis;

public enum AnalysisSerializationType
{
        
    /// <summary>
    ///     Analysis made in format of one made with Deep Finesse.
    ///     Contains following tags:
    ///     <list>
    ///         <item> Minimax </item>
    ///         <item> Ability </item>
    ///     </list>
    /// </summary>
    AbilityAnalysis,

    /// <summary>
    ///     Analysis in format of one made with Bridge Composer. Generated file usually is version 2.1.
    ///     Contains following tags:
    ///     <list>
    ///         <item> OptimumResultTable </item>
    ///         <item> OptimumScore </item>
    ///         <item> DoubleDummyTricks </item>
    ///     </list>
    ///     Does not contain information about minimax contract, only score, so it cannot be used to generate
    ///     <see cref="AbilityAnalysis" /> like syntax.
    ///     See bridgecomposer.pbn file for example.
    ///     Note that BridgeComposer generated file is usually bloated.
    /// </summary>
    OptimumResultTableAnalysis,
        
    /// <summary>
    ///     Analysis in format of one made with PsBridge (legacy). 
    ///     An escaped line in the following format
    ///     %%!R 75974684697597468469
    ///     Does not contain information about minimax contract, only score, so it cannot be used to generate
    ///     <see cref="AbilityAnalysis" /> like syntax.
    ///     See psbridge.pbn file for example.
    /// </summary>
    PsBridgeAnalysis,
}