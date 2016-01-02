﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.Helpers.Interfaces;
using TestDataFramework.Randomizer;

namespace Tests.Tests
{
    [TestClass]
    public class StandardRandomizerTests
    {
        private StandardRandomizer randomizer;
        private Mock<Random> randomMock;
        private Mock<IRandomSymbolStringGenerator> stringGeneratorMock;
        private readonly DateTime now = DateTime.Now;

        private const int Integer = 5;

        [TestInitialize]
        public void Initialize()
        {
            this.randomMock = new Mock<Random>();
            this.stringGeneratorMock = new Mock<IRandomSymbolStringGenerator>();
            this.randomizer = new StandardRandomizer(this.randomMock.Object, this.stringGeneratorMock.Object, () => this.now);
        }

        [TestMethod]
        public void RandomizeInteger_Test()
        {
            // Arrange

            this.randomMock.Setup(m => m.Next(It.Is<int>(i => i == int.MaxValue))).Returns(StandardRandomizerTests.Integer).Verifiable();

            // Act

            int result = this.randomizer.RandomizeInteger(null);

            // Assert

            this.randomMock.Verify();
            Assert.AreEqual(StandardRandomizerTests.Integer, result);
        }

        [TestMethod]
        public void RandomizeInteger_WithMax_Test()
        {
            // Arrange

            this.randomMock.Setup(m => m.Next(It.IsAny<int>())).Returns(StandardRandomizerTests.Integer);
            const int maximum = 7;

            // Act

            int result = this.randomizer.RandomizeInteger(maximum);

            // Assert

            Assert.AreEqual(StandardRandomizerTests.Integer, result);
            this.randomMock.Verify(m => m.Next(), Times.Never());
            this.randomMock.Verify(m => m.Next(It.Is<int>(max => max == maximum)), Times.Once());
        }

        [TestMethod]
        public void RandomizeLongInteger_Word0Max_Test()
        {
            // Arrange

            this.randomMock.Setup(m => m.Next(0x10000)).Returns(StandardRandomizerTests.Integer).Verifiable();

            // Act

            long result = this.randomizer.RandomizeLongInteger(0x10000);

            // Assert

            this.randomMock.Verify();
            Assert.AreEqual((long)StandardRandomizerTests.Integer, result);
        }


        [TestMethod]
        public void RandomizeLongInteger_Test()
        {
            // Arrange

            long maxValue = long.MaxValue - new Random().Next() - 1;
            long expected = maxValue - new Random().Next();

            if ((maxValue & 0xffff) == 0)
            {
                maxValue--;
            }

            this.randomMock.Setup(m => m.Next((int)((maxValue >> (16 * 0)) & 0xffff)))
                .Returns((int)((expected >> (16 * 0)) & 0xffff)).Verifiable();

            this.randomMock.Setup(m => m.Next((int)((maxValue >> (16 * 1)) & 0xffff) + 1))
                .Returns((int)((expected >> (16 * 1)) & 0xffff)).Verifiable();

            this.randomMock.Setup(m => m.Next((int)((maxValue >> (16 * 2)) & 0xffff) + 1))
                .Returns((int)((expected >> (16 * 2)) & 0xffff)).Verifiable();

            this.randomMock.Setup(m => m.Next((int)((maxValue >> (16 * 3)) & 0xffff) + 1))
                .Returns((int)((expected >> (16 * 3)) & 0xffff)).Verifiable();

            // Act

            long result = this.randomizer.RandomizeLongInteger(maxValue);

            // Assert

            this.randomMock.Verify();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RandomizeLongInteger_DefaultMax_Test()
        {
            // Arrange

            this.randomMock.Setup(m => m.Next(It.IsAny<int>())).Returns(1);

            // Act

            long result = this.randomizer.RandomizeLongInteger(null);

            // Assert

            this.randomMock.Verify(m => m.Next(0xffff), Times.Once);
            this.randomMock.Verify(m => m.Next(0x10000), Times.Exactly(2));
            this.randomMock.Verify(m => m.Next(0x8000), Times.Once);

            const long expected = 0x0001000100010001;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RandomizeLongInteger_MaxReturnBoundaryCondition_Test()
        {
            // Arrange

            this.randomMock.Setup(m => m.Next(0xffff)).Returns(0xffff);
            this.randomMock.Setup(m => m.Next(0x10000)).Returns(0xffff);
            this.randomMock.Setup(m => m.Next(0x8000)).Returns(0x7fff);

            // Act

            long result = this.randomizer.RandomizeLongInteger(null);

            // Assert

            Assert.AreEqual(long.MaxValue, result);
        }

        [TestMethod]
        public void RandomizeShortInteger_Test()
        {
            // Arrange

            const int max = 7;
            this.randomMock.Setup(m => m.Next(It.Is<int>(i => i == max))).Returns(StandardRandomizerTests.Integer).Verifiable();

            // Act

            short result = this.randomizer.RandomizeShortInteger(max);

            // Assert

            this.randomMock.Verify();
            Assert.AreEqual((short)StandardRandomizerTests.Integer, result);
        }

        [TestMethod]
        public void RandomizeShortInteger_DefaultMax_Test()
        {
            // Arrange

            this.randomMock.Setup(m => m.Next(It.Is<int>(i => i == short.MaxValue))).Returns(StandardRandomizerTests.Integer).Verifiable();

            // Act

            short result = this.randomizer.RandomizeShortInteger(null);

            // Assert

            this.randomMock.Verify();
            Assert.AreEqual((short)StandardRandomizerTests.Integer, result);
        }

        [TestMethod]
        public void RandomizeString_Test()
        {
            // Arrange

            const string returnValue = "ABCD";
            this.stringGeneratorMock.Setup(m => m.GetRandomString(It.Is<int?>(i => i == 5))).Returns(returnValue);

            // Act

            string result = this.randomizer.RandomizeString(5);

            // Assert

            Assert.AreEqual(returnValue, result);
        }

        [TestMethod]
        public void RandomizeCharacter_Test()
        {
            // 26 letters in the alphabet
            for (int code = 0; code < 26; code++)
            {
                this.randomMock.Setup(m => m.Next(It.Is<int>(i => i == 26))).Returns(code);

                char result = this.randomizer.RandomizeCharacter();

                // 65 is ASCII code of "A".
                Assert.AreEqual((char)(code + 65), result);
            }
        }

        [TestMethod]
        public void RandomizeDecimal_DefaultPrecision_Test()
        {
            // Arrange

            const decimal expected = 12345.12m;

            this.randomMock.Setup(m => m.Next()).Returns(12345);
            this.randomMock.Setup(m => m.NextDouble()).Returns(0.12345d);

            // Act

            decimal result = this.randomizer.RandomizeDecimal(null);

            // Assert

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RandomizeDecimal_Test()
        {
            // Arrange

            const decimal expected = 12345.1234m;

            this.randomMock.Setup(m => m.Next()).Returns(12345);
            this.randomMock.Setup(m => m.NextDouble()).Returns(0.12345d);

            // Act

            decimal result = this.randomizer.RandomizeDecimal(4);

            // Assert

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RandomizeDouble_DefaultPrecision_Test()
        {
            // Arrange

            const double expected = 12345.12d;

            this.randomMock.Setup(m => m.Next()).Returns(12345);
            this.randomMock.Setup(m => m.NextDouble()).Returns(0.12345d);

            // Act

            double result = this.randomizer.RandomizeDouble(null);

            // Assert

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RandomizeDouble_Test()
        {
            // Arrange

            const double expected = 12345.1234d;

            this.randomMock.Setup(m => m.Next()).Returns(12345);
            this.randomMock.Setup(m => m.NextDouble()).Returns(0.12345d);

            // Act

            double result = this.randomizer.RandomizeDouble(4);

            // Assert

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RandomizeBoolean_ReturnsTrue_Test()
        {
            // Arrange

            this.randomMock.Setup(m => m.Next(2)).Returns(1).Verifiable();

            // Act

            bool result = this.randomizer.RandomizeBoolean();

            // Assert

            this.randomMock.Verify();
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void RandomizeBoolean_ReturnsFalse_Test()
        {
            // Arrange

            this.randomMock.Setup(m => m.Next(2)).Returns(0).Verifiable();

            // Act

            bool result = this.randomizer.RandomizeBoolean();

            // Assert

            this.randomMock.Verify();
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void RandomizeDateTime_Past_Test()
        {
            // Arrange

            int ticks = new Random().Next();

            this.randomMock.Setup(m => m.Next()).Returns(ticks);

            DateTime expected = this.now.AddTicks(-ticks);

            // Act

            DateTime explicitPastResult = this.randomizer.RandomizeDateTime(PastOrFuture.Past);
            DateTime implicitPastResult = this.randomizer.RandomizeDateTime(null);

            // Assert

            Assert.AreEqual(expected, explicitPastResult);
            Assert.AreEqual(expected, implicitPastResult);
        }

        [TestMethod]
        public void RandomizeDateTime_Future_Test()
        {
            // Arrange

            int ticks = new Random().Next();

            this.randomMock.Setup(m => m.Next()).Returns(ticks);

            DateTime expected = this.now.AddTicks(ticks);

            // Act

            DateTime result = this.randomizer.RandomizeDateTime(PastOrFuture.Future);

            // Assert

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RandomizeByte_Test()
        {
            // Arrange

            const byte expected = 5;
            this.randomMock.Setup(m => m.NextBytes(It.IsAny<byte[]>())).Callback<byte[]>(a => a[0] = expected);

            // Act

            byte result = this.randomizer.RandomizeByte();

            // Assert

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RandomizeEmailAddress_Test()
        {
            // Arrange

            this.stringGeneratorMock.Setup(m => m.GetRandomString(It.IsAny<int?>())).Returns("abcde");

            // Act

            string result = this.randomizer.RandomizeEmailAddress();

            // Assert

            Assert.AreEqual("abcde@domain.com", result);
        }
    }
}