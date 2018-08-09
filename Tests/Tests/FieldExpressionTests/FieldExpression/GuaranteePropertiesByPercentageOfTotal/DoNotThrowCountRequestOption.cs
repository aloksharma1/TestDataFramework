﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Populator.Concrete.FieldExpression;
using Tests.TestModels;

namespace Tests.Tests.FieldExpressionTests.FieldExpression.GuaranteePropertiesByPercentageOfTotal
{
    [TestClass]
    public class DoNotThrow
    {
        private TestContext testContext;

        [TestInitialize]
        public void Initialize()
        {
            this.testContext = new TestContext();
        }

        [TestMethod]
        public void PropertyFuncs_DefaultQuantity_Test()
        {
            // Act

            FieldExpression<ElementType, ElementType.PropertyType>
                returnResult = this.testContext.FieldExpression.GuaranteePropertiesByPercentageOfTotal(
                    new Func<ElementType.PropertyType>[]
                        {() => new ElementType.PropertyType(), () => new ElementType.PropertyType()},
                    ValueCountRequestOption.DoNotThrow);

            // Assert

            this.testContext.AssertPercentage(returnResult, 10, ValueCountRequestOption.DoNotThrow);
        }

        [TestMethod]
        public void PropertyFuncs_ExplicitQuantity_Test()
        {
            // Act

            FieldExpression<ElementType, ElementType.PropertyType>
                returnResult = this.testContext.FieldExpression.GuaranteePropertiesByPercentageOfTotal(
                    new Func<ElementType.PropertyType>[]
                        {() => new ElementType.PropertyType(), () => new ElementType.PropertyType()},
                    5, ValueCountRequestOption.DoNotThrow);

            // Assert

            this.testContext.AssertPercentage(returnResult, 5, ValueCountRequestOption.DoNotThrow);
        }

        [TestMethod]
        public void PropertiesAndFuncs_DefaultQuantity_Test()
        {
            // Act

            FieldExpression<ElementType, ElementType.PropertyType>
                returnResult = this.testContext.FieldExpression.GuaranteePropertiesByPercentageOfTotal(
                    new object[]
                    {
                        new ElementType.PropertyType(),
                        (Func<ElementType.PropertyType>) (() => new ElementType.PropertyType())
                    }, ValueCountRequestOption.DoNotThrow);

            // Assert

            this.testContext.AssertPercentage(returnResult, 10, ValueCountRequestOption.DoNotThrow);
        }

        [TestMethod]
        public void PropertiesAndFuncs_ExplicitQuantity_Test()
        {
            // Act

            FieldExpression<ElementType, ElementType.PropertyType>
                returnResult = this.testContext.FieldExpression.GuaranteePropertiesByPercentageOfTotal(
                    new object[]
                    {
                        new ElementType.PropertyType(),
                        (Func<ElementType.PropertyType>) (() => new ElementType.PropertyType())
                    }, 5, ValueCountRequestOption.DoNotThrow);

            // Assert

            this.testContext.AssertPercentage(returnResult, 5, ValueCountRequestOption.DoNotThrow);
        }

        [TestMethod]
        public void Properties_DefaultQuantity_Test()
        {
            // Act

            FieldExpression<ElementType, ElementType.PropertyType>
                returnResult = this.testContext.FieldExpression.GuaranteePropertiesByPercentageOfTotal(
                    new []
                    {
                        new ElementType.PropertyType(),
                        new ElementType.PropertyType(),
                    }, ValueCountRequestOption.DoNotThrow);

            // Assert

            this.testContext.AssertPercentage(returnResult, 10, ValueCountRequestOption.DoNotThrow);
        }

        [TestMethod]
        public void Properties_ExplicitQuantity_Test()
        {
            // Act

            FieldExpression<ElementType, ElementType.PropertyType>
                returnResult = this.testContext.FieldExpression.GuaranteePropertiesByPercentageOfTotal(
                    new []
                    {
                        new ElementType.PropertyType(),
                        new ElementType.PropertyType(),
                    }, 5, ValueCountRequestOption.DoNotThrow);

            // Assert

            this.testContext.AssertPercentage(returnResult, 5, ValueCountRequestOption.DoNotThrow);
        }
    }
}