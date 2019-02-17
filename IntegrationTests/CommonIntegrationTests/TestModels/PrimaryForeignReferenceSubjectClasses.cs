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
using TestDataFramework;

namespace IntegrationTests.CommonIntegrationTests.TestModels
{
    public class PrimaryTable
    {
        [PrimaryKey]
        public int Key1 { get; set; }

        [PrimaryKey]
        public int Key2 { get; set; }

        public string Text { get; set; }

        public int Integer { get; set; }
    }

    public class PrimaryTableB
    {
        [PrimaryKey]
        public int Key1 { get; set; }
    }

    public class ForeignTable
    {
        [PrimaryKey(PrimaryKeyAttribute.KeyTypeEnum.Auto)]
        public int Key { get; set; }

        [ForeignKey(typeof(PrimaryTable), nameof(PrimaryTable.Key1))]
        public int ForeignKeyA1 { get; set; }

        [ForeignKey(typeof(PrimaryTable), nameof(PrimaryTable.Key2))]
        public int ForeignKeyA2 { get; set; }

        [ForeignKey(typeof(PrimaryTableB), nameof(PrimaryTableB.Key1))]
        public int ForeignKeyB { get; set; }

        public string Text { get; set; }

        public int Integer { get; set; }
    }

    public class Tester
    {
    }

    [Table("TestDataFramework", "dbo", "ManualKeyPrimaryTable")]
    public class ManualKeyPrimaryTableClass
    {
        [PrimaryKey(PrimaryKeyAttribute.KeyTypeEnum.Auto)]
        public int Tester { get; set; }

        [PrimaryKey(PrimaryKeyAttribute.KeyTypeEnum.Manual)]
        [StringLength(20)]
        public string Key1 { get; set; }

        [PrimaryKey(PrimaryKeyAttribute.KeyTypeEnum.Manual)]
        public int Key2 { get; set; }

        public string AString { get; set; }

        [Precision(2)] [Max(int.MaxValue)]
        public decimal ADecimal { get; set; }

        [Precision(3)]
        public float AFloat { get; set; }
    }

    [Table("TestDataFramework", "dbo", "ManualKeyForeignTable")]
    public class ManualKeyForeignTable
    {
        [PrimaryKey]
        public Guid UserId { get; set; }

        [ForeignKey(typeof(ManualKeyPrimaryTableClass), "Tester")]
        public int? FirstForeignKey { get; set; }

        [StringLength(20)]
        [PrimaryKey]
        [ForeignKey("ManualKeyPrimaryTable", "Key1")]
        public string ForeignKey1 { get; set; }

        [ForeignKey(typeof(ManualKeyPrimaryTableClass), "Key2")]
        public int? ForeignKey2 { get; set; }

        public short AShort { get; set; }

        public long ALong { get; set; }

        public Guid AGuid { get; set; }
    }

    public class UnresolvedKeyTable
    {
        [ForeignKey("DoesntExist", "DoesntExistEither")]
        public int? DoesntExist { get; set; }
    }

    public class KeyNoneTable
    {
        public string Text { get; set; }
    }

    #region For referntial integrity tests

    public class TypeMismatchPrimaryTable
    {
        [PrimaryKey]
        public string Key { get; set; }
    }

    public class TypeMismatchForeignTable
    {
        [ForeignKey(typeof(TypeMismatchPrimaryTable), "Key")]
        public int ForeignKey { get; set; }
    }

    public class TableTypeMismatchPrimaryTable
    {
        [PrimaryKey]
        public int Key { get; set; }
    }

    public class TableTypeMismatchForeignTable
    {
        [ForeignKey(typeof(object), "Key")]
        public int ForeignKey { get; set; }
    }

    public class PropertyNameMismatchPrimaryTable
    {
        [PrimaryKey]
        public int Key { get; set; }
    }

    public class PropertyNameMismatchForeignTable
    {
        [ForeignKey(typeof(PropertyNameMismatchPrimaryTable), "KeyX")]
        public int ForeignKey { get; set; }
    }

    #endregion For referntial integrity tests
}