//Part of WinSSC, © Edward Tippetts and other WinSSC contributors 2015 - https://github.com/Theodus2009/WinSSC
//Licenced under GNU Lesser GPL v3 - see COPYING.txt and COPYING LESSER.txt for detailsusing System;
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
