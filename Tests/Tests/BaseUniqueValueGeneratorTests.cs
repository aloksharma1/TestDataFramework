﻿using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class BaseUniqueValueGeneratorTests
    {
        [TestMethod]
        public void DeferValue_Test()
        {
            throw new NotImplementedException();
            
            /*
            // Arange

            const int initialCount = 5;

            PropertyInfo keyPropertyInfo = typeof(ClassWithStringAutoPrimaryKey).GetProperty("Key");
            this.propertyValueAccumulatorMock.Setup(m => m.GetValue(It.Is<PropertyInfo>(pi => pi == keyPropertyInfo), initialCount)).Returns(1);

            int i = 0;

            var delegateArray = new DeferredValueGetterDelegate<ulong>[2];

            this.deferredValueGeneratorMock.Setup(
                m => m.AddDelegate(It.IsAny<PropertyInfo>(), It.IsAny<DeferredValueGetterDelegate<ulong>>()))
                .Callback<PropertyInfo, DeferredValueGetterDelegate<ulong>>((pi, d) => delegateArray[i++] = d);

            // Act

            this.generator.DeferValue(keyPropertyInfo);
            this.generator.DeferValue(keyPropertyInfo);
            
            // Assert

            Assert.AreEqual(1, delegateArray[0](initialCount));
            Assert.AreEqual(1, delegateArray[1](initialCount));
            */
        }

        [TestMethod]
        public void GetValue_Test()
        {
            throw new NotImplementedException();
        }
    }
}