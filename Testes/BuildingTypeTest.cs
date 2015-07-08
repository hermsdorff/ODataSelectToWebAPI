using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Testes
{
    using RuntimeSelectExpand;

    using Testes.Fakes;

    [TestClass]
    public class BuildingTypeTest
    {
        [TestMethod]
        public void BuildTypeFromSelect()
        {
            // arrange
            const string Query = "$select=Id,Name";
            var parser = new ODataParser();
            var tree = parser.Parse(Query);
            tree.Bind(typeof(Category));

            // act
            tree.BuildType();

            // assert
            Assert.AreEqual("Id:int;Name:string", tree.QueryType.Name);
        }
    }
}
