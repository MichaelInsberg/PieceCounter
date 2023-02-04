using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using PieceCounter.Core.Contracts;
#pragma warning disable CS1591

namespace PieceCounter.Core.Tests;

[TestFixture]
public sealed class PieceCountCalculatorTests
{
    private Mock<IPieceCounterSource> _pieceCounterSourceMock;
    private PieceCountCalculator _objectToTest;

    [SetUp]
    public void Setup()
    {
        _pieceCounterSourceMock = new Mock<IPieceCounterSource>();
        _objectToTest = new PieceCountCalculator(_pieceCounterSourceMock.Object);
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
        _ = _pieceCounterSourceMock.Setup(p => p.GetProducedPieces()).Returns((IEnumerable<int>)null);
        var func = () => _ = _objectToTest.CalculatePieceCount();

        //Act & Assert
        _ = func.Should().Throw<InvalidOperationException>();
    }

    [TestCaseSource(nameof(TestData))]
    public void CreatePieceCountCalculator_WithValidInput_ReturnsValidValues(IEnumerable<int> pieces, int expectedResult)
    {
        //Arrange
        _ = _pieceCounterSourceMock.Setup(p => p.GetProducedPieces()).Returns(pieces);
        
        //Act
        var result = _objectToTest.CalculatePieceCount();

        //Act & Assert
        _ = result.Should().Be(expectedResult);
    }

    [TestCaseSource(nameof(TestData))]
    public async Task TaskCreatePieceCountCalculator_WithValidInput_ReturnsValidValuesAsync(IEnumerable<int> pieces, int expectedResult)
    {
        //Arrange
        _ = _pieceCounterSourceMock.Setup(p => p.GetProducedPieces()).Returns(pieces);

        //Act
        var result = await _objectToTest.CalculatePieceCountAsync().ConfigureAwait(false);

        //Act & Assert
        _ = result.Should().Be(expectedResult);
    }


    private static IEnumerable<TestCaseData> TestData()
    {
        yield return new TestCaseData(new List<int>(), 0).SetName("Test 1");
        yield return new TestCaseData(new List<int> { 100 }, 100).SetName("Test 2");
        yield return new TestCaseData(new List<int> { 10, 100 }, 100).SetName("Test 3");
        yield return new TestCaseData(new List<int> { 0, 10, 100 }, 100).SetName("Test 4");
        yield return new TestCaseData(new List<int> { 0, 10, 0 }, 10).SetName("Test 5");
        yield return new TestCaseData(new List<int> { 0, 10, 0, 20 }, 30).SetName("Test 6");
        yield return new TestCaseData(new List<int> { 0, 10, 20, 30, 0, 10, 20 }, 50).SetName("Test 7");
        yield return new TestCaseData(new List<int> { 0, 10, 20, 30, 0, 10, 20, 0, 100 }, 150).SetName("Test 8");
        yield return new TestCaseData(new List<int> { 100, 0, 10, 20, 30, 0, 10, 20, 0, 100 }, 250).SetName("Test 9");
    }
}
