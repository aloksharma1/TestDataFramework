﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using TestDataFramework.Helpers;
using TestDataFramework.Populator;
using TestDataFramework.RepositoryOperations.Model;
using TestDataFramework.WritePrimitives;

namespace TestDataFramework.RepositoryOperations.Operations.InsertRecord
{
    public class InsertRecord : AbstractRepositoryOperation
    {
        #region Private Fields

        private static readonly ILog Logger = LogManager.GetLogger(typeof(InsertRecord));
        private readonly InsertRecordService service;

        private readonly List<ColumnSymbol> primaryKeyValues = new List<ColumnSymbol>();

        #endregion Private Fields

        public InsertRecord(InsertRecordService service, RecordReference recordReference, IEnumerable<AbstractRepositoryOperation> peers)
        {
            InsertRecord.Logger.Debug("Entering constructor");

            this.Peers = peers;
            this.RecordReference = recordReference;
            this.service = service;

            InsertRecord.Logger.Debug("Exiting constructor");
        }

        #region Public methods

        public override void Write(CircularReferenceBreaker breaker, IWritePrimitives writer, Counter order, AbstractRepositoryOperation[] orderedOperations)
        {
            InsertRecord.Logger.Debug("Entering Write:" + this.DumpObject());

            if (breaker.IsVisited<IWritePrimitives, Counter, AbstractRepositoryOperation[]>(this.Write))
            {
                InsertRecord.Logger.Debug("Write already visited. Exiting");
                return;
            }

            if (this.IsWriteDone)
            {
                InsertRecord.Logger.Debug("Write already done. Exiting");
                return;
            }

            breaker.Push<IWritePrimitives, Counter, AbstractRepositoryOperation[]>(this.Write);

            IEnumerable<InsertRecord> primaryKeyOperations = this.service.GetPrimaryKeyOperations(this.Peers).ToList();

            this.service.WritePrimaryKeyOperations(writer, primaryKeyOperations, breaker, order, orderedOperations);

            Columns columnData = this.service.GetColumnData(primaryKeyOperations);

            this.Order = order.Value++;
            orderedOperations[this.Order] = this;

            string tableName = Helper.GetTableName(this.RecordReference.RecordType);

            this.service.WritePrimitives(writer, tableName, columnData.AllColumns, this.primaryKeyValues);

            this.service.CopyForeignKeyColumns(columnData.ForeignKeyColumns);

            this.IsWriteDone = true;

            breaker.Pop();

            InsertRecord.Logger.Debug("Exiting Write");
        }

        public override void Read(Counter readStreamPointer, object[] data)
        {
            InsertRecord.Logger.Debug("Entering Read");

            if (this.service.KeyType == PrimaryKeyAttribute.KeyTypeEnum.Auto)
            {
                PropertyAttribute<PrimaryKeyAttribute> primaryKeyPropertyAttribute =
                    this.RecordReference.RecordType.GetPropertyAttributes<PrimaryKeyAttribute>().First();

                object value = data[readStreamPointer.Value++];
                object result = Convert.ChangeType(value, primaryKeyPropertyAttribute.PropertyInfo.PropertyType);

                primaryKeyPropertyAttribute.PropertyInfo.SetValue(this.RecordReference.RecordObject, result);
            }

            InsertRecord.Logger.Debug("Exiting Read");
        }

        public virtual IEnumerable<ColumnSymbol> GetPrimaryKeySymbols()
        {
            return this.primaryKeyValues;
        }

        #endregion Public methods

        #region Write related private methods

        private string DumpObject()
        {
            return Helper.DumpObject(this.RecordReference.RecordObject);
        }

        #endregion Write related private methods
    }
}
