﻿using System.Collections.Generic;

namespace TestDataFramework.Populator
{
    public abstract class Populatable
    {
        protected internal abstract void Populate();

        internal abstract void AddToReferences(IList<RecordReference> collection);

        public virtual bool IsPopulated { get; protected internal set; }
    }
}
