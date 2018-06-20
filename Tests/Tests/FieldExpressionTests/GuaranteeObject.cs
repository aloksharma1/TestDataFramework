﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Populator.Concrete;

namespace Tests.Tests.FieldExpressionTests
{
    public partial class FieldExpressionTests
    {
        [TestMethod]
        public void GuaranteeByFixedQuantity_MixedFunc_Test()
        {
            // Arrange

            var values = new object[] { new ElementType(), (Func<ElementType>)(() => new ElementType()) };
            this.rangeOperableListMock.Setup(m => m.GuaranteeByFixedQuantity(values, 5))
                .Returns(this.rangeOperableListMock.Object);

            // Act

            OperableList<ElementType> result = this.fieldExpression.GuaranteeByFixedQuantity(values, 5);

            // Assert

            this.rangeOperableListMock.Verify(m => m.GuaranteeByFixedQuantity(values, 5));
            Assert.AreEqual(this.rangeOperableListMock.Object, result);
        }

        [TestMethod]
        public void GuaranteeByFixedQuantity_Func_Test()
        {
            // Arrange

            var values = new Func<ElementType>[] { () => new ElementType(), () => new ElementType() };
            this.rangeOperableListMock.Setup(m => m.GuaranteeByFixedQuantity(values, 5))
                .Returns(this.rangeOperableListMock.Object);

            // Act

            OperableList<ElementType> result = this.fieldExpression.GuaranteeByFixedQuantity(values, 5);

            // Assert

            this.rangeOperableListMock.Verify(m => m.GuaranteeByFixedQuantity(values, 5));
            Assert.AreEqual(this.rangeOperableListMock.Object, result);
        }

        [TestMethod]
        public void GuaranteeByFixedQuantity_TListElement_Test()
        {
            // Arrange

            var values = new[] { new ElementType(), new ElementType() };
            this.rangeOperableListMock.Setup(m => m.GuaranteeByFixedQuantity(values, 5))
                .Returns(this.rangeOperableListMock.Object);

            // Act

            OperableList<ElementType> result = this.fieldExpression.GuaranteeByFixedQuantity(values, 5);

            // Assert

            this.rangeOperableListMock.Verify(m => m.GuaranteeByFixedQuantity(values, 5));
            Assert.AreEqual(this.rangeOperableListMock.Object, result);
        }

        [TestMethod]
        public void GuaranteeByPercentageOfTotal_MixedFunc_Test()
        {
            // Arrange

            var values = new object[] { new ElementType(), (Func<ElementType>)(() => new ElementType()) };

            this.rangeOperableListMock.Setup(m => m.GuaranteeByPercentageOfTotal(values, 5))
                .Returns(this.rangeOperableListMock.Object);

            // Act

            OperableList<ElementType> result = this.fieldExpression.GuaranteeByPercentageOfTotal(values, 5);

            // Assert

            this.rangeOperableListMock.Verify(m => m.GuaranteeByPercentageOfTotal(values, 5));
            Assert.AreEqual(this.rangeOperableListMock.Object, result);
        }

        [TestMethod]
        public void GuaranteeByPercentageOfTotal_Func_Test()
        {
            // Arrange

            var values = new Func<ElementType>[] { () => new ElementType(), () => new ElementType() };
            this.rangeOperableListMock.Setup(m => m.GuaranteeByPercentageOfTotal(values, 5))
                .Returns(this.rangeOperableListMock.Object);

            // Act

            OperableList<ElementType> result = this.fieldExpression.GuaranteeByPercentageOfTotal(values, 5);

            // Assert

            this.rangeOperableListMock.Verify(m => m.GuaranteeByPercentageOfTotal(values, 5));
            Assert.AreEqual(this.rangeOperableListMock.Object, result);
        }

        [TestMethod]
        public void GuaranteeByPercentageOfTotal_TListElement_Test()
        {
            // Arrange

            var values = new[] { new ElementType(), new ElementType() };
            this.rangeOperableListMock.Setup(m => m.GuaranteeByPercentageOfTotal(values, 5))
                .Returns(this.rangeOperableListMock.Object);

            // Act

            OperableList<ElementType> result = this.fieldExpression.GuaranteeByPercentageOfTotal(values, 5);

            // Assert

            this.rangeOperableListMock.Verify(m => m.GuaranteeByPercentageOfTotal(values, 5));
            Assert.AreEqual(this.rangeOperableListMock.Object, result);
        }
    }
}