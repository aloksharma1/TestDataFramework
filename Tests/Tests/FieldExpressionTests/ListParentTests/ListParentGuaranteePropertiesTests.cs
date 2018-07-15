﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.DeepSetting;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete.FieldExpression;
using Tests.TestModels;

namespace Tests.Tests.FieldExpressionTests.ListParentTests
{
    public partial class ListParentFieldExpressionTests
    {
        [TestMethod]
        public void GuaranteePropertiesByFixedQuantity_TProperty_Test()
        {
            var values = new[] { new TestModels.ElementType.PropertyType(), };

            Func<FieldExpression<TestModels.ElementType, TestModels.ElementType.PropertyType>> action = () => this.listParentFieldExpression.GuaranteePropertiesByFixedQuantity(values, 5);

            this.GuaranteePropertiesByFixedQuantity_Test(action, values);
        }

        [TestMethod]
        public void GuaranteePropertiesByFixedQuantity_Func_Test()
        {
            var values = new[] { new TestModels.ElementType.PropertyType() };

            IEnumerable<Func<TestModels.ElementType.PropertyType>> funcs =
                values.Select<TestModels.ElementType.PropertyType, Func<TestModels.ElementType.PropertyType>>(value => () => value);

            Func<FieldExpression<TestModels.ElementType, TestModels.ElementType.PropertyType>> action = () => this.listParentFieldExpression.GuaranteePropertiesByFixedQuantity(funcs, 5);

            this.GuaranteePropertiesByFixedQuantity_Test(action, values);
        }

        [TestMethod]
        public void GuaranteePropertiesByFixedQuantity_Mixed_TypeAndFunc_Test()
        {
            var values = new[] { new TestModels.ElementType.PropertyType(), new TestModels.ElementType.PropertyType() };
            var objects = new object[] { values[0], (Func<TestModels.ElementType.PropertyType>)(() => values[1]) };

            Func<FieldExpression<TestModels.ElementType, TestModels.ElementType.PropertyType>> action = () => this.listParentFieldExpression.GuaranteePropertiesByFixedQuantity(objects, 5);

            this.GuaranteePropertiesByFixedQuantity_Test(action, values);
        }

        private void GuaranteePropertiesByFixedQuantity_Test(Func<FieldExpression<TestModels.ElementType, TestModels.ElementType.PropertyType>> action,
            TestModels.ElementType.PropertyType[] values)
        {
            void AssertFrequency(GuaranteedValues guaranteedValues)
            {
                Assert.AreEqual(5, guaranteedValues.TotalFrequency);
                Assert.IsNull(guaranteedValues.FrequencyPercentage);
            }

            this.GuaranteeProperties_Test(action, values, AssertFrequency);
        }

        [TestMethod]
        public void GuaranteePropertiesByPercentageOfTotal_TProperty_Test()
        {
            var values = new[] { new TestModels.ElementType.PropertyType(), };

            Func<FieldExpression<TestModels.ElementType, TestModels.ElementType.PropertyType>> action = () => this.listParentFieldExpression.GuaranteePropertiesByPercentageOfTotal(values, 5);

            this.GuaranteePropertiesByPercentageOfTotal_Test(action, values);
        }

        [TestMethod]
        public void GuaranteePropertiesByPercentageOfTotal_Func_Test()
        {
            var values = new[] { new TestModels.ElementType.PropertyType() };

            IEnumerable<Func<TestModels.ElementType.PropertyType>> funcs =
                values.Select<TestModels.ElementType.PropertyType, Func<TestModels.ElementType.PropertyType>>(value => () => value);

            Func<FieldExpression<TestModels.ElementType, TestModels.ElementType.PropertyType>> action = () => this.listParentFieldExpression.GuaranteePropertiesByPercentageOfTotal(funcs, 5);

            this.GuaranteePropertiesByPercentageOfTotal_Test(action, values);
        }

        [TestMethod]
        public void GuaranteePropertiesByPercentageOfTotal_Mixed_TypeAndFunc_Test()
        {
            var values = new[] { new TestModels.ElementType.PropertyType(), new TestModels.ElementType.PropertyType() };
            var objects = new object[] { values[0], (Func<TestModels.ElementType.PropertyType>)(() => values[1]) };

            Func<FieldExpression<TestModels.ElementType, TestModels.ElementType.PropertyType>> action = () => this.listParentFieldExpression.GuaranteePropertiesByPercentageOfTotal(objects, 5);

            this.GuaranteePropertiesByPercentageOfTotal_Test(action, values);
        }

        private void GuaranteePropertiesByPercentageOfTotal_Test(Func<FieldExpression<TestModels.ElementType, TestModels.ElementType.PropertyType>> action,
            TestModels.ElementType.PropertyType[] values)
        {
            void AssertFrequency(GuaranteedValues guaranteedValues)
            {
                Assert.AreEqual(5, guaranteedValues.FrequencyPercentage);
                Assert.IsNull(guaranteedValues.TotalFrequency);
            }

            this.GuaranteeProperties_Test(action, values, AssertFrequency);
        }

        private void GuaranteeProperties_Test(Func<FieldExpression<TestModels.ElementType, TestModels.ElementType.PropertyType>> action,
            TestModels.ElementType.PropertyType[] values, Action<GuaranteedValues> assertFrequency)
        {
            var objectGraph = new List<PropertyInfo> {typeof(TestModels.ElementType).GetProperty(nameof(TestModels.ElementType.AProperty))};

            this.objectGraphServiceMock.Setup(m => m.GetObjectGraph(this.expression)).Returns(objectGraph);

            // Act

            FieldExpression<TestModels.ElementType, TestModels.ElementType.PropertyType> resultFieldExpression = action();

            // Assert

            GuaranteedValues guaranteedValues = this.listParentOperableListMock.Object.GuaranteedPropertySetters.Single();

            assertFrequency(guaranteedValues);

            for (int i = 0; i < values.Length; i++)
            {
                var propertySettersEnumerable = guaranteedValues.Values as IEnumerable<ExplicitPropertySetter>;
                Assert.IsNotNull(propertySettersEnumerable);
                List<ExplicitPropertySetter> propertySetters = propertySettersEnumerable.ToList();

                Assert.AreEqual(objectGraph, propertySetters[i].PropertyChain);

                var element = new TestModels.ElementType();
                propertySetters[i].Action(element);
                Assert.AreEqual(values[i], element.AProperty);
            }
        }

        [TestMethod]
        public void GuaranteePropertiesByFixedQuantity_Func_Default_FixedQuantity_Test()
        {
            // Arrange

            var guaranteedValues = new List<Func<TestModels.ElementType.PropertyType>>();

            const int count = 5;

            for (int i = 0; i < count; i++)
            {
                var elementPropertyValue = new TestModels.ElementType.PropertyType();
                guaranteedValues.Add(() => elementPropertyValue);
            }

            // Act

            ListParentFieldExpression<TestModels.ElementType, TestModels.ElementType.PropertyType> result =
                this.listParentFieldExpression.GuaranteePropertiesByFixedQuantity(guaranteedValues);

            // Assert

            Assert.AreEqual(count, result.OperableList.GuaranteedPropertySetters.Single().TotalFrequency);
        }

        [TestMethod]
        public void GuaranteePropertiesByFixedQuantity_TProperty_Default_FixedQuantity_Test()
        {
            // Arrange

            var guaranteedValues = new List<TestModels.ElementType.PropertyType>();

            const int count = 5;

            for (int i = 0; i < count; i++)
            {
                var elementPropertyValue = new TestModels.ElementType.PropertyType();
                guaranteedValues.Add(elementPropertyValue);
            }

            // Act

            ListParentFieldExpression<TestModels.ElementType, TestModels.ElementType.PropertyType> result = this.listParentFieldExpression.GuaranteePropertiesByFixedQuantity(guaranteedValues);

            // Assert

            Assert.AreEqual(count, result.OperableList.GuaranteedPropertySetters.Single().TotalFrequency);
        }

        [TestMethod]
        public void GuaranteePropertiesByFixedQuantity_Mixed_FuncAndTProperty_Default_FixedQuantity_Test()
        {
            // Arrange

            var guaranteedValues = new List<object>();

            const int count = 5;

            for (int i = 0; i < count; i++)
            {
                object elementPropertyValue = i % 2 == 0
                    ? new TestModels.ElementType.PropertyType()
                    : (object)(Func<TestModels.ElementType.PropertyType>)(() => new TestModels.ElementType.PropertyType());

                guaranteedValues.Add(elementPropertyValue);
            }

            // Act

            ListParentFieldExpression<TestModels.ElementType, TestModels.ElementType.PropertyType> result = this.listParentFieldExpression.GuaranteePropertiesByFixedQuantity(guaranteedValues);

            // Assert

            Assert.AreEqual(count, result.OperableList.GuaranteedPropertySetters.Single().TotalFrequency);
        }

        [TestMethod]
        public void GuaranteePropertiesByPercentageOfTotal_Func_Default_FixedQuantity_Test()
        {
            // Act

            ListParentFieldExpression<TestModels.ElementType, TestModels.ElementType.PropertyType> result =
                this.listParentFieldExpression.GuaranteePropertiesByPercentageOfTotal(new List<Func<TestModels.ElementType.PropertyType>>());

            // Assert

            Assert.AreEqual(10, result.OperableList.GuaranteedPropertySetters.Single().FrequencyPercentage);
        }

        [TestMethod]
        public void GuaranteePropertiesByPercentageOfTotal_TProperty_Default_FixedQuantity_Test()
        {
            // Act

            ListParentFieldExpression<TestModels.ElementType, TestModels.ElementType.PropertyType> result =
                this.listParentFieldExpression.GuaranteePropertiesByPercentageOfTotal(new List<TestModels.ElementType.PropertyType>());

            // Assert

            Assert.AreEqual(10, result.OperableList.GuaranteedPropertySetters.Single().FrequencyPercentage);
        }

        [TestMethod]
        public void GuaranteePropertiesByPercentageOfTotal_Mixed_FuncAndTProperty_Default_FixedQuantity_Test()
        {
            // Arrange

            var guaranteedValues = new List<object>();

            for (int i = 0; i < 5; i++)
            {
                object elementPropertyValue = i % 2 == 0
                    ? new TestModels.ElementType.PropertyType()
                    : (object)(Func<TestModels.ElementType.PropertyType>)(() => new TestModels.ElementType.PropertyType());

                guaranteedValues.Add(elementPropertyValue);
            }

            // Act

            ListParentFieldExpression<TestModels.ElementType, TestModels.ElementType.PropertyType> result =
                this.listParentFieldExpression.GuaranteePropertiesByPercentageOfTotal(guaranteedValues);

            // Assert

            Assert.AreEqual(10, result.OperableList.GuaranteedPropertySetters.Single().FrequencyPercentage);
        }
    }
}