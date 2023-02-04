using System.Collections.Generic;

namespace PieceCounter.Core.Contracts;

/// <summary>
/// The IPieceCounterSource interface
/// </summary>
public interface IPieceCounterSource
{
    /// <summary>
    /// Gets the produced pieces.
    /// </summary>
    /// <returns>An enumerable with the produced pieces</returns>
    IEnumerable<int> GetProducedPieces();

}
