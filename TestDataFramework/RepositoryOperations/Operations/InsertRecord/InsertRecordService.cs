﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;
using TestDataFramework.Helpers;
using TestDataFramework.Populator;
using TestDataFramework.RepositoryOperations.Model;
using TestDataFramework.WritePrimitives;

namespace TestDataFramework.RepositoryOperations.Operations.InsertRecord
{
    public class InsertRecordService
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(InsertRecordService));
        private readonly RecordReference recordReference;

        private readonly PrimaryKeyAttribute.KeyTypeEnum keyType;

        public InsertRecordService(RecordReference recordReference)
        {
            this.recordReference = recordReference;
            this.keyType = this.DetermineKeyType();
        }

        private PrimaryKeyAttribute.KeyTypeEnum DetermineKeyType()
        {
            IEnumerable<PrimaryKeyAttribute> pkAttributes =
                this.recordReference.RecordType.GetUniqueAttributes<PrimaryKeyAttribute>().ToList();

            if (!pkAttributes.Any())
            {
                return PrimaryKeyAttribute.KeyTypeEnum.None;
            }

            if (pkAttributes.Count() > 1)
            {
                return PrimaryKeyAttribute.KeyTypeEnum.Manual;
            }

            PrimaryKeyAttribute.KeyTypeEnum result = pkAttributes.First().KeyType;
            return result;
        }

        public virtual IEnumerable<InsertRecord> GetPrimaryKeyOperations(IEnumerable<AbstractRepositoryOperation> peers)
        {
            InsertRecordService.Logger.Debug("Entering GetPrimaryKeyOperations");

            IEnumerable<InsertRecord> result = peers.Where(
                peer =>
                {
                    var pkRecord = peer as InsertRecord;

                    bool peerResult = pkRecord != null
                                  && this.recordReference.PrimaryKeyReferences.Any(
                                      primaryKeyReference => primaryKeyReference == pkRecord.RecordReference);

                    return peerResult;

                }).Cast<InsertRecord>().ToList();

            InsertRecordService.Logger.Debug("Exiting GetPrimaryKeyOperations");

            return result;
        }

        public virtual void WritePrimaryKeyOperations(IWritePrimitives writer, IEnumerable<InsertRecord> primaryKeyOperations,
            CircularReferenceBreaker breaker, CurrentOrder currentOrder, AbstractRepositoryOperation[] orderedOperations)
        {
            InsertRecordService.Logger.Debug("Entering WriteHigherPriorityOperations");

            primaryKeyOperations.ToList().ForEach(o => o.Write(breaker, writer, currentOrder, orderedOperations));

            InsertRecordService.Logger.Debug("Exiting WriteHigherPriorityOperations");
        }

        public virtual Columns GetColumnData(IEnumerable<InsertRecord> primaryKeyOperations)
        {
            InsertRecordService.Logger.Debug("Entering GetColumnData");

            // ReSharper disable once UseObjectOrCollectionInitializer
            var result = new Columns();

            result.RegularColumns = this.GetRegularColumns();
            result.ForeignKeyColumns = this.GetForeignKeyColumns(primaryKeyOperations);

            InsertRecordService.Logger.Debug("Exiting GetColumnData");
            return result;
        }

        private IEnumerable<Column> GetForeignKeyColumns(IEnumerable<InsertRecord> primaryKeyOperations)
        {
            InsertRecordService.Logger.Debug("Entering GetForeignKeyVariables");

            IEnumerable<ColumnSymbol> symbolList = primaryKeyOperations.SelectMany(o => o.GetPrimaryKeySymbols());

            IEnumerable<PropertyAttribute<ForeignKeyAttribute>> propertyAttributes = this.recordReference.RecordType.GetPropertyAttributes<ForeignKeyAttribute>();

            IEnumerable<Column> result = symbolList.Select(sl =>
            {
                PropertyAttribute<ForeignKeyAttribute> matchingPropertyAttribute =
                    propertyAttributes.First(
                        pa =>
                            sl.TableType == pa.Attribute.PrimaryTableType &&
                            sl.ColumnName == pa.Attribute.PrimaryKeyName);

                return new Column
                {
                    Name = Helper.GetColunName(matchingPropertyAttribute.PropertyInfo),
                    Value = sl.Value
                };

            });

            InsertRecordService.Logger.Debug("Exiting GetForeignKeyVariables");

            return result;
        }

        private IEnumerable<Column> GetRegularColumns()
        {
            InsertRecordService.Logger.Debug("Entering GetRegularColumnData");

            IEnumerable<Column> result =
                this.recordReference.RecordType.GetProperties()
                    .Where(
                        p =>
                            p.GetSingleAttribute<ForeignKeyAttribute>() == null &&
                            (p.GetSingleAttribute<PrimaryKeyAttribute>() == null ||
                             this.keyType == PrimaryKeyAttribute.KeyTypeEnum.Manual))
                    .Select(
                        p =>
                            new Column
                            {
                                Name = Helper.GetColunName(p),
                                Value = p.GetValue(this.recordReference.RecordObject)
                            });

            InsertRecordService.Logger.Debug("Exiting GetRegularColumnData");

            return result.ToList();
        }

        public virtual void WritePrimitives(IWritePrimitives writer, IEnumerable<Column> columns, List<ColumnSymbol> primaryKeyValues)
        {
            InsertRecordService.Logger.Debug("Entering WritePrimitives");

            writer.Insert(columns);
            this.HandlePrimaryKeyValues(writer, primaryKeyValues);

            InsertRecordService.Logger.Debug("Exiting WritePrimitives");
        }

        private void HandlePrimaryKeyValues(IWritePrimitives writer, List<ColumnSymbol> primaryKeyValues)
        {
            InsertRecordService.Logger.Debug("Entering HandlePrimaryKeyValues");

            switch (this.keyType)
            {
                case PrimaryKeyAttribute.KeyTypeEnum.Auto:
                    InsertRecordService.Logger.Debug("Taking KeyTypeEnum.Auto branch");

                    string primaryKeyColumnName = this.GetPrimaryKeyColumnName();
                    object identityVariable = writer.SelectIdentity();

                    primaryKeyValues.Add(new ColumnSymbol
                    {
                        ColumnName = primaryKeyColumnName,
                        Value = identityVariable,
                        TableType = this.recordReference.RecordType
                    });

                    break;

                case PrimaryKeyAttribute.KeyTypeEnum.Manual:
                    InsertRecordService.Logger.Debug("Taking KeyTypeEnum.Auto Manual branch");

                    primaryKeyValues.AddRange(this.GetPrimaryKeyValues());

                    break;

                default:
                    InsertRecordService.Logger.Debug("No special key type handling fo this branch");
                    break;
            }

            InsertRecordService.Logger.Debug("Exiting HandlePrimaryKeyValues");
        }

        private IEnumerable<ColumnSymbol> GetPrimaryKeyValues()
        {
            InsertRecordService.Logger.Debug("Entering GetPrimaryKeyValues");

            IEnumerable<PropertyAttribute<PrimaryKeyAttribute>> pkPropertyAttributes =
                this.recordReference.RecordType.GetPropertyAttributes<PrimaryKeyAttribute>();

            IEnumerable<ColumnSymbol> result = pkPropertyAttributes.Select(pa => new ColumnSymbol
            {
                ColumnName = Helper.GetColunName(pa.PropertyInfo),
                TableType = this.recordReference.RecordType,
                Value = pa.PropertyInfo.GetValue(this.recordReference.RecordObject)
            });

            InsertRecordService.Logger.Debug("Exiting GetPrimaryKeyValues");

            return result;
        }

        private string GetPrimaryKeyColumnName()
        {
            InsertRecordService.Logger.Debug("Entering GetPrimaryKeyColumnName");

            PropertyAttribute<PrimaryKeyAttribute> pkPropertyAttribute =
                this.recordReference.RecordType.GetPropertyAttributes<PrimaryKeyAttribute>().First();

            InsertRecordService.Logger.Debug("Exiting GetPrimaryKeyColumnName");

            return Helper.GetColunName(pkPropertyAttribute.PropertyInfo);
        }

        public virtual void CopyForeignKeyColumns(IEnumerable<Column> foreignKeyColumns)
        {
            foreignKeyColumns.ToList().ForEach(c =>
            {
                if (c.Value.IsSpecialType())
                {
                    return;
                }

                PropertyInfo targetProperty =
                    this.recordReference.RecordType.GetProperties().First(p => Helper.GetColunName(p).Equals(c.Name));

                targetProperty.SetValue(this.recordReference.RecordObject, c.Value);
            });
        }
    }
}