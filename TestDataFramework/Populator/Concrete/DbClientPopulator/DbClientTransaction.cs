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
using System.Data.Common;
using System.Transactions;
using TestDataFramework.Exceptions;
using TestDataFramework.Populator.Interfaces;

namespace TestDataFramework.Populator.Concrete.DbClientPopulator
{
    public class DbClientTransaction : IDbClientTransaction
    {
        internal DbTransaction DbTransaction { get; set; }

        public DbClientTransaction(DbClientTransactionOptions options)
        {
            this.Options = options;
        }

        public DbClientTransactionOptions Options { get; }

        public virtual void Commit()
        {
            if (this.DbTransaction == null)
            {
                throw new TransactionException(Messages.NoTransaction);
            }

            this.DbTransaction.Commit();
        }

        public virtual void Rollback()
        {
            if (this.DbTransaction == null)
            {
                throw new TransactionException(Messages.NoTransaction);
            }

            this.DbTransaction.Rollback();
        }

        public virtual void Dispose()
        {
            if (this.DbTransaction == null)
            {
                throw new TransactionException(Messages.NoTransaction);
            }

            this.DbTransaction.Dispose();

            this.OnDisposed?.Invoke();
        }

        internal Action OnDisposed { get; set; }
    }
}
