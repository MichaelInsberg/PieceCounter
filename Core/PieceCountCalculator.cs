using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PieceCounter.Core.Contracts;

namespace PieceCounter.Core;

/// <summary>
/// The PieceCountCalculator class
/// </summary>
public sealed class PieceCountCalculator
{
    private readonly IPieceCounterSource _pieceCounterSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="PieceCountCalculator"/> class.
    /// </summary>
    /// <param name="pieceCounterSource">The piece counter source.</param>
    public PieceCountCalculator(IPieceCounterSource pieceCounterSource)
    {
        this._pieceCounterSource = pieceCounterSource ?? throw new ArgumentNullException(nameof(pieceCounterSource));
    }

    /// <summary>
    /// Calculates the piece count.
    /// </summary>
    /// <returns>The produced piece count</returns>
    public int CalculatePieceCount()
    {
        return CalculateInternal();
    }

    /// <summary>
    /// Calculates the piece count asynchronous.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task of the produced piece count</returns>
    public Task<int> CalculatePieceCountAsync(CancellationToken cancellationToken = default)
    {
        return Task<int>.Factory.StartNew(() => CalculateInternal(cancellationToken), cancellationToken);

    }

    private int CalculateInternal(CancellationToken cancellationToken = default)
    {
        var pieces = _pieceCounterSource.GetProducedPieces();
        if (pieces == null)
        {
            throw new InvalidOperationException("The piece counter source returned null");
        }

        var pieceList = pieces.ToList();

        switch (pieceList.Count)
        {
            case 0:
                return 0;
            case 1:
                return pieceList[0];
        }

        var highestValues = new List<int> { 0 };

        foreach (int piece in pieceList)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return 0;
            }
            bool mustAddItem = piece == 0 && highestValues[^1] != 0;
            if (mustAddItem)
            {
                highestValues.Add(0);
            }

            highestValues[^1] = piece;
        }

        return highestValues.Sum();
    }
}
