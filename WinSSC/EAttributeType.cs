using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinSSC
{
    /// <summary>
    /// The data types that may back article attributes. Used for validation and sorting.
    /// </summary>
    public enum EAttributeType
    {
        String,
        Number,
        Date,
        StringArray
    }
}
