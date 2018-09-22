﻿/*
    Copyright 2016, 2017, 2018 Alexander Kuperman

    This file is part of TestDataFramework.

    TestDataFramework is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    TestDataFramework is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with TestDataFramework.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using TestDataFramework.DeepSetting;
using TestDataFramework.HandledTypeGenerator;
using TestDataFramework.Helpers;
using TestDataFramework.Logger;
using TestDataFramework.TypeGenerator.Interfaces;
using TestDataFramework.ValueGenerator.Interfaces;

namespace TestDataFramework.TypeGenerator.Concrete
{
    public class StandardTypeGenerator : ITypeGenerator
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(StandardTypeGenerator));

        public StandardTypeGenerator(IValueGenerator valueGenerator, IHandledTypeGenerator handledTypeGenerator,
            ITypeGeneratorService typeGeneratorService)
        {
            StandardTypeGenerator.Logger.Debug("Entering constructor");

            this.valueGenerator = valueGenerator;
            this.handledTypeGenerator = handledTypeGenerator;
            this.typeGeneratorService = typeGeneratorService;

            StandardTypeGenerator.Logger.Debug("Exiting constructor");
        }

        #region Protected Methods

        protected virtual void SetProperty(object objectToFill, PropertyInfo targetPropertyInfo,
            ObjectGraphNode objectGraphNode, TypeGeneratorContext context)
        {
            StandardTypeGenerator.Logger.Debug("Entering SetProperty. PropertyInfo: " +
                                               targetPropertyInfo.GetExtendedMemberInfoString());

            object targetPropertyValue = this.valueGenerator.GetValue(targetPropertyInfo, objectGraphNode, context);
            StandardTypeGenerator.Logger.Debug($"targetPropertyValue: {targetPropertyValue}");
            targetPropertyInfo.SetValue(objectToFill, targetPropertyValue);

            StandardTypeGenerator.Logger.Debug("Exiting SetProperty");
        }

        #endregion Protected Methods

        #region Fields

        private readonly IValueGenerator valueGenerator;
        private readonly IHandledTypeGenerator handledTypeGenerator;
        private readonly ITypeGeneratorService typeGeneratorService;

        #endregion Fields

        #region Private methods

        private object ConstructObject(Type forType, ObjectGraphNode objectGraphNode, TypeGeneratorContext context)
        {
            StandardTypeGenerator.Logger.Debug("Entering ConstructObject");

            object handledTypeObject = this.handledTypeGenerator.GetObject(forType, context);

            if (handledTypeObject != null)
                return handledTypeObject;

            if (!context.ComplexTypeRecursionGuard.Push(forType, context.ExplicitPropertySetters,
                objectGraphNode))
            {
                    return Helper.GetDefaultValue(forType);
            }

            bool canBeConstructed = this.InvokeConstructor(forType, out object objectToFillResult, context);

            if (!canBeConstructed)
            {
                context.ComplexTypeRecursionGuard.Pop();
                return objectToFillResult;
            }

            this.FillObject(objectToFillResult, objectGraphNode, context);

            context.ComplexTypeRecursionGuard.Pop();

            StandardTypeGenerator.Logger.Debug("Exiting ConstructObject");
            return objectToFillResult;
        }

        private bool InvokeConstructor(Type forType, out object result, TypeGeneratorContext typeGeneratorContext)
        {
            StandardTypeGenerator.Logger.Debug("Entering StandardTypeGenerator.GetObjectToFill()");

            IOrderedEnumerable<ConstructorInfo> constructors = forType.GetConstructors()
                .OrderBy(constructorInfo => constructorInfo.GetParameters().Length);

            List<object> constructorArguments = null;
            ConstructorInfo resultConstructorInfo = null;
            foreach (ConstructorInfo constructorInfo in constructors)
            {
                ParameterInfo[] parameterInfos = constructorInfo.GetParameters();

                constructorArguments = new List<object>();

                bool parametersFound = true;
                foreach (ParameterInfo parameterInfo in parameterInfos)
                {
                    object argument = this.valueGenerator.GetValue(null, parameterInfo.ParameterType, typeGeneratorContext);

                    if (argument == null)
                    {
                        parametersFound = false;
                        break;
                    }

                    constructorArguments.Add(argument);
                }

                if (!parametersFound) continue;

                resultConstructorInfo = constructorInfo;
                break;
            }

            if (resultConstructorInfo != null)
            {
                result = resultConstructorInfo.Invoke(constructorArguments.ToArray());
                return true;
            }

            StandardTypeGenerator.Logger.Debug("Type has no public constructor. Type: " + forType);

            if (forType.IsValueType)
            {
                result = Activator.CreateInstance(forType);
                return true;
            }

            result = null;
            return false;
        }

        private void FillObject(object objectToFill, ObjectGraphNode objectGraphNode, TypeGeneratorContext context)
        {
            StandardTypeGenerator.Logger.Debug("Entering FillObject<T>");

            PropertyInfo[] targetProperties = StandardTypeGenerator.GetProperties(objectToFill);

            foreach (PropertyInfo targetPropertyInfo in targetProperties)
            {
                ObjectGraphNode propertyObjectGraphNode = objectGraphNode != null
                    ? new ObjectGraphNode(targetPropertyInfo, objectGraphNode)
                    : null;

                IEnumerable<ExplicitPropertySetter> setters =
                    this.typeGeneratorService
                        .GetExplicitlySetPropertySetters(context.ExplicitPropertySetters, propertyObjectGraphNode)
                        .ToList();

                if (setters.Any())
                {
                    StandardTypeGenerator.Logger.Debug($"explicit property setter found");
                    setters.ToList().ForEach(setter => setter.Action(objectToFill));
                }
                else
                {
                    StandardTypeGenerator.Logger.Debug("no explicit property setter found");
                    this.SetProperty(objectToFill, targetPropertyInfo, propertyObjectGraphNode, context);
                }
            }

            StandardTypeGenerator.Logger.Debug("Exiting FillObject<T>");
        }

        private static PropertyInfo[] GetProperties(object objectToFill)
        {
            PropertyInfo[] targetProperties = objectToFill.GetType().GetPropertiesHelper();
            return targetProperties;
        }

        #endregion Private methods

        #region Public methods

        // Main entry point.
        public virtual object GetObject<T>(TypeGeneratorContext context)
        {
            StandardTypeGenerator.Logger.Debug($"Entering GetObject. T: {typeof(T)}");

            object intrinsicValue = this.valueGenerator.GetIntrinsicValue(null, typeof(T), context);

            if (intrinsicValue != null)
                return intrinsicValue;

            var parentObjectGraphNode = new ObjectGraphNode(null, null);

            object result = this.ConstructObject(typeof(T), parentObjectGraphNode, context);

            StandardTypeGenerator.Logger.Debug($"Exiting GetObject. result: {result}");
            return result;
        }

        // A secondary entry point.
        public virtual object GetObject(Type forType, ObjectGraphNode objectGraphNode, TypeGeneratorContext context)
        {
            StandardTypeGenerator.Logger.Debug($"Entering GetObject. forType: {forType}");

            if (objectGraphNode == null)
            {
                context.BlankSetters = true;
                context.BlankSetterCount++;
            }

            object result = this.ConstructObject(forType, objectGraphNode, context);

            if (objectGraphNode == null)
            {
                context.BlankSetterCount--;

                if (context.BlankSetterCount == 0)
                {
                    context.BlankSetters = false;
                }
            }

            StandardTypeGenerator.Logger.Debug($"Exiting GetObject. result: {result}");
            return result;
        }

        #endregion Public methods
    }
}