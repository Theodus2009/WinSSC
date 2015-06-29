using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WinSSC;
using System.Collections.Generic;

namespace WinSSC
{
    [TestClass]
    public class AttributeSetTests
    {
        [TestMethod]
        public void ParseAttributeSetPositive()
        {
            string input = @"#A comment line
                             Title:String
                             Index:Number
                             Tags:StringArray
                             Date:Date";
            IList<AttributeDef> output = AttributeSet.ParseAttributeDefs(input);
            Assert.AreEqual(4, output.Count);
            Assert.AreEqual("Title", output[0].Name);
            Assert.AreEqual("Index", output[1].Name);
            Assert.AreEqual("Tags", output[2].Name);
            Assert.AreEqual("Date", output[3].Name);
            Assert.AreEqual(EAttributeType.String, output[0].DataType);
            Assert.AreEqual(EAttributeType.Number, output[1].DataType);
            Assert.AreEqual(EAttributeType.StringArray, output[2].DataType);
            Assert.AreEqual(EAttributeType.Date, output[3].DataType);
        }

        [TestMethod]
        public void ParseAttributeSetSoftFailOnType()
        {
            string input = @"Title:String
                             #Next line should soft-fail
                             Index:Numbers
                             Tags:StringArray";
            IList<AttributeDef> output = AttributeSet.ParseAttributeDefs(input);
            Assert.AreEqual(2, output.Count);
            Assert.AreEqual("Title", output[0].Name);
            Assert.AreEqual("Tags", output[1].Name);
            Assert.AreEqual(EAttributeType.String, output[0].DataType);
            Assert.AreEqual(EAttributeType.StringArray, output[1].DataType);
        }

        [TestMethod]
        public void ParseAttributeSetSoftFailOnLineLength()
        {
            string input = @":String
                             Index:Number:
                             Tags:StringArray";
            IList<AttributeDef> output = AttributeSet.ParseAttributeDefs(input);
            Assert.AreEqual(1, output.Count);
            Assert.AreEqual("Tags", output[0].Name);
            Assert.AreEqual(EAttributeType.StringArray, output[0].DataType);
        }
    }
}
