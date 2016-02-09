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
using System;
using TestDataFramework;

namespace Tests.TestModels
{
    public class PrimaryTable
    {
        [PrimaryKey(KeyType = PrimaryKeyAttribute.KeyTypeEnum.Auto)]
        public int Key { get; set; }

        public string Text { get; set; }

        public int Integer { get; set; }
    }

    public class ForeignTable
    {
        [PrimaryKey(KeyType = PrimaryKeyAttribute.KeyTypeEnum.Auto)]
        public int Key { get; set; }

        [ForeignKey(primaryTable: typeof (PrimaryTable), primaryKeyName: "Key")]
        public int ForeignKey { get; set; }

        public string Text { get; set; }

        public int Integer { get; set; }
    }
    
    public class ManualKeyPrimaryTable
    {
        [PrimaryKey]
        [StringLength(20)]
        public string Key1 { get; set; }

        [PrimaryKey]
        public int Key2 { get; set; }

        public string AString { get; set; }

        [Precision(2)]
        public decimal ADecimal { get; set; }

        [Precision(3)]
        public float AFloat { get; set; }
    }

    public class ManualKeyForeignTable
    {
        [PrimaryKey]
        public Guid UserId { get; set; }

        [StringLength(20)]
        [PrimaryKey]
        [ForeignKey(typeof(ManualKeyPrimaryTable), "Key1")]
        public string ForeignKey1 { get; set; }

        [ForeignKey(typeof(ManualKeyPrimaryTable), "Key2")]
        public int ForeignKey2 { get; set; }

        public short AShort { get; set; }

        public long ALong { get; set; }
    }

    public class TertiaryManualKeyForeignTable
    {
        [PrimaryKey(PrimaryKeyAttribute.KeyTypeEnum.Auto)]
        public int Pk { get; set; }

        [ForeignKey(typeof(ManualKeyForeignTable), "UserId")]
        public Guid FkManualKeyForeignTable { get; set; }

        [ForeignKey(typeof(ManualKeyForeignTable), "ForeignKey1")]
        [StringLength(20)]
        public string FkStringForeignKey { get; set; }

        public int AnInt { get; set; }
    }

    public class ForeignToAutoPrimaryTable
    {
        [PrimaryKey]
        [ForeignKey(typeof(TertiaryManualKeyForeignTable), "Pk")]
        public int ForignKey { get; set; }
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
