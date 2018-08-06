﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete.FieldExpression;
using TestDataFramework.Populator.Concrete.OperableList;
using Tests.TestModels;

namespace Tests.Tests.FieldExpressionTests.FieldExpression
{
    [TestClass]
    public class FieldExpressionTests
    {
        private TestContext testContext;

        [TestInitialize]
        public void Initialize()
        {
            this.testContext = new TestContext();
        }

        [TestMethod]
        public void OperableList_Test()
        {
            OperableListEx<ElementType> actual =
                this.testContext.FieldExpression.OperableList;

            Assert.IsNotNull(actual);
            Assert.AreEqual(this.testContext.OperableListMock.Object, actual);
        }

        [TestMethod]
        public void RecordObjects_Test()
        {
            // Arrange
            var element = new ElementType();
            this.testContext.OperableListMock.SetupGet(m => m.RecordObjects).Returns(new[] {element});

            // Act

            IEnumerable<ElementType> result = this.testContext.FieldExpression.RecordObjects;

            // Assert

            Assert.IsNotNull(result);

            ElementType[] resultArray = result.ToArray();

            Assert.AreEqual(1, resultArray.Length);
            Assert.AreEqual(element, resultArray[0]);
        }

        [TestMethod]
        public void Make_Test()
        {
            // Arrange

            var expected = new ElementType[0];
            this.testContext.OperableListMock.Setup(m => m.Make()).Returns(expected);

            // Act

            IEnumerable<ElementType> actual = this.testContext.FieldExpression.Make();

            // Assert

            this.testContext.OperableListMock.Verify(m => m.Make());

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void BindAndMake()
        {
            // Arrange

            var expected = new ElementType[0];
            this.testContext.OperableListMock.Setup(m => m.BindAndMake()).Returns(expected);

            // Act

            IEnumerable<ElementType> actual = this.testContext.FieldExpression.BindAndMake();

            // Assert

            this.testContext.OperableListMock.Verify(m => m.BindAndMake());

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SetRange_RangeFactory_Test()
        {
            // Act

            var rangeFactory = (Func<IEnumerable<ElementType.PropertyType>>)(() => new[]
            {
                new ElementType.PropertyType(), new ElementType.PropertyType(), new ElementType.PropertyType(),
            });

            FieldExpression<ElementType, ElementType.PropertyType>
                actual = this.testContext.FieldExpression.SetRange(m => m.AProperty, rangeFactory);

            // Assert

            Assert.IsNotNull(actual);
            Assert.AreEqual(this.testContext.FieldExpression, actual);

            this.testContext.OperableListMock.Verify(m => m.AddRange(n => n.AProperty, rangeFactory));
        }

        [TestMethod]
        public void SetRange_Range_Test()
        {
            // Act

            var range = new[]
            {
                new ElementType.PropertyType(), new ElementType.PropertyType(), new ElementType.PropertyType(),
            };

            FieldExpression<ElementType, ElementType.PropertyType>
                actual = this.testContext.FieldExpression.SetRange(m => m.AProperty, range);

            // Assert

            Assert.IsNotNull(actual);
            Assert.AreEqual(this.testContext.FieldExpression, actual);

            this.testContext.OperableListMock.Verify(m => m.AddRange(n => n.AProperty, range));
        }
    }
}
