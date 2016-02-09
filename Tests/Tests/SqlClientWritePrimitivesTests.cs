﻿/*
    Copyright 2016 Alexander Kuperman

    This file is part of TestDataFramework.

    TestDataFramework is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Foobar is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
*/
using System.Collections.Specialized;
using System.Data.Common;
using System.Text;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.Helpers.Interfaces;
using TestDataFramework.ValueFormatter.Interfaces;
using TestDataFramework.WritePrimitives.Concrete;
using Tests.Mocks;

namespace Tests.Tests
{
    [TestClass]
    public class SqlClientWritePrimitivesTests
    {
        private Mock<DbProviderFactory> dbProviderFactoryMock;
        private SqlClientWritePrimitives primitives;
        private Mock<IValueFormatter> formatterMock;
        private Mock<IRandomSymbolStringGenerator> symbolGeneratorMock;
        private Mock<DbCommand> insertCommandMock;

        private const string ConnectionString = "cn";
        private const string ColumnName = "col1";
        private const string VariableSymbol = "ABCD";

        [TestInitialize]
        public void Initialize()
        {
            XmlConfigurator.Configure();

            this.dbProviderFactoryMock = new Mock<DbProviderFactory>();
            this.formatterMock = new Mock<IValueFormatter>();
            this.symbolGeneratorMock = new Mock<IRandomSymbolStringGenerator>();

            this.primitives = new SqlClientWritePrimitives(SqlClientWritePrimitivesTests.ConnectionString,
                this.dbProviderFactoryMock.Object, this.formatterMock.Object, this.symbolGeneratorMock.Object,
                mustBeInATransaction: false,
                configuration: new NameValueCollection {{"TestDataFramework_DumpSqlInput", "true"}});

            var connectionMock = new Mock<DbConnection>();
            var readerMock = new Mock<DbDataReader>();
            this.insertCommandMock = new Mock<DbCommand>();
            var mockInsertCommand = new MockDbCommand(this.insertCommandMock.Object, readerMock.Object);

            this.dbProviderFactoryMock.Setup(m => m.CreateCommand()).Returns(mockInsertCommand);
            this.dbProviderFactoryMock.Setup(m => m.CreateConnection()).Returns(connectionMock.Object);

            this.symbolGeneratorMock.Setup(m => m.GetRandomString(It.IsAny<int?>())).Returns(SqlClientWritePrimitivesTests.VariableSymbol);
        }

        [TestMethod]
        public void SelectIdentity_Test()
        {
            // Act

            this.primitives.SelectIdentity(SqlClientWritePrimitivesTests.ColumnName);
            this.primitives.Execute();

            // Assert

            string expectedText =
                new StringBuilder($"declare @{SqlClientWritePrimitivesTests.VariableSymbol} bigint;").AppendLine()
                    .AppendLine($"select @{SqlClientWritePrimitivesTests.VariableSymbol} = @@identity;")
                    .AppendLine($"select '{SqlClientWritePrimitivesTests.ColumnName}'")
                    .AppendLine($"select @{SqlClientWritePrimitivesTests.VariableSymbol}")
                    .AppendLine()
                    .ToString();

            this.insertCommandMock.VerifySet(m => m.CommandText = expectedText);
        }

        [TestMethod]
        public void WriteGuid_Test()
        {
            // Act

            this.primitives.WriteGuid(SqlClientWritePrimitivesTests.ColumnName);
            this.primitives.Execute();

            // Assert

            string expectedText =
                new StringBuilder($"declare @{SqlClientWritePrimitivesTests.VariableSymbol} uniqueidentifier;")
                    .AppendLine()
                    .AppendLine($"select @{SqlClientWritePrimitivesTests.VariableSymbol} = NEWID();")
                    .AppendLine($"select '{SqlClientWritePrimitivesTests.ColumnName}'")
                    .AppendLine($"select @{SqlClientWritePrimitivesTests.VariableSymbol}")
                    .AppendLine()
                    .ToString();

            this.insertCommandMock.VerifySet(m => m.CommandText = expectedText);
        }
    }
}
