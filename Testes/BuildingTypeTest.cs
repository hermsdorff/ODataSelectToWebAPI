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
        public void BuildTypeFromSystemTypes()
        {
            // arrange
            var tree = new ExpressionTree("Id");
            tree.Bind(typeof(int));

            // act
            tree.BuildType();

            // assert
            Assert.AreEqual(tree.QueryType, typeof(int));
        }

        [TestMethod]
        public void BuildTypeUsingSystemTreeLeaf()
        {
            // arrange
            var tree = new SystemTreeLeaf();
            tree.Bind(typeof(int));

            // act
            tree.BuildType();

            // assert
            Assert.AreEqual(tree.QueryType, typeof(int));
        }

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
            Assert.AreEqual("{Id:Int32;Name:String}", tree.QueryType.Name);
        }

        [TestMethod]
        public void BuildTypeFromMultiLevelSelect()
        {
            // arrange
            const string Query = "$select=Id,Name,Manufacturer";
            var parser = new ODataParser();
            var tree = parser.Parse(Query);
            tree.Bind(typeof(Product));

            // act
            tree.BuildType();

            // assert
            Assert.AreEqual("{Id:Int32;Manufacturer:{Id:Int32;Name:String};Name:String}", tree.QueryType.Name);
        }

        [TestMethod]
        public void BuildTypeWithExpandedCollection()
        {
            // arrange
            const string Query = "$expand=Models";
            var parser = new ODataParser();
            var tree = parser.Parse(Query);
            tree.Bind(typeof(Product));

            // act
            tree.BuildType();

            // assert   
            Assert.AreEqual("{Models:List`1<{Id:Int32;Name:String}>}", tree.QueryType.Name);
        }

        [TestMethod]
        public void BuildTypeWithExpandedCollectionWithIgnoredProperty()
        {
            // arrange
            const string Query = "$expand=Models,Manufacturer";
            var parser = new ODataParser();
            var tree = parser.Parse(Query);
            tree.Bind(typeof(Product));

            // act
            tree.BuildType();

            // assert
            Assert.AreEqual("{Models:List`1<{Id:Int32;Name:String}>}", tree.QueryType.Name);
        }
    }
}
