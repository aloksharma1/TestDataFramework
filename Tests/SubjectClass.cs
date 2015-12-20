﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDataFramework;


namespace Tests
{
    public class SubjectClass
    {
        public int Integer { get; set; }

        public long LongInteger { get; set; }

        public short ShortInteger { get; set; }

        public string Text { get; set; }

        [StringLength(10)]
        public string TextWithLength { get; set; }

        public UnresolvableType UnresolvableTypeMember { get; set; }
    }

    public class SecondClass
    {        
    }

    public class UnresolvableType
    {
    }
}
