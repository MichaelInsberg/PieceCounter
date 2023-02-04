using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using PieceCounter.Core.Contracts;
#pragma warning disable CS1591

namespace PieceCounter.Core.Tests;

[TestFixture]
public sealed class PieceCountCalculatorTests
{
    private Mock<IPieceCounterSource> pieceCounterSourceMock;
    private PieceCountCalculator objectToTest;

    [SetUp]
    public void Setup()
    {
        pieceCounterSourceMock = new Mock<IPieceCounterSource>();
        objectToTest = new PieceCountCalculator(pieceCounterSourceMock.Object);
    }

    [Test]
    public void CreatePieceCountCalculator_WithParameterNull_ShouldThrow()
    {
        //Arrange
        var func = () => _ = new PieceCountCalculator(null);

        //Act & Assert
        _ = func.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void CreatePieceCountCalculator_WithPieceCounterSourceReturnsNull_ShouldThrow()
    {
        //Arrange
        _ = pieceCounterSourceMock.Setup(p => p.GetProducedPieces()).Returns((IEnumerable<int>)null);
        var func = () => _ = objectToTest.CalculatePieceCount();

        //Act & Assert
        _ = func.Should().Throw<InvalidOperationException>();
    }

    [TestCaseSource(nameof(TestData))]
    public void CreatePieceCountCalculator_WithValidInput_ReturnsValidValues(IEnumerable<int> pieces, int expectedResult)
    {
        //Arrange
        _ = pieceCounterSourceMock.Setup(p => p.GetProducedPieces()).Returns(pieces);
        //Act
        var result = objectToTest.CalculatePieceCount();

        //Act & Assert
        _ = result.Should().Be(expectedResult);
    }

    private static IEnumerable<TestCaseData> TestData()
    {
        yield return new TestCaseData(new List<int>(), 0).SetName("Empty list");
        yield return new TestCaseData(new List<int> { 100 }, 100).SetName("List with only on value");
        yield return new TestCaseData(new List<int> { 10, 100 }, 100).SetName("List with 2 values");
        yield return new TestCaseData(new List<int> { 0, 10, 100 }, 100).SetName("List with 3 values start with zero");
        yield return new TestCaseData(new List<int> { 0, 10, 0 }, 10).SetName("List with 3 values start and ends with zero");
        yield return new TestCaseData(new List<int> { 0, 10, 0, 20 }, 30).SetName("List with 3 values start and ends with zero");
        yield return new TestCaseData(new List<int> { 0, 10, 20, 30, 0, 10, 20 }, 50).SetName("List with 3 values start and ends with zero");
        yield return new TestCaseData(new List<int> { 0, 10, 20, 30, 0, 10, 20, 0, 100 }, 150).SetName("List with 3 values start and ends with zero");
    }
}
