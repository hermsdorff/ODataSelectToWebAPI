using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Testes
{
    using RuntimeSelectExpand;

    [TestClass]
    public class QueryParseTest
    {
        [TestMethod]
        public void SelectQuery()
        {
            // arrange
            const string Query = "$select=Id,Name,Age&$anotherParameter";
            var parser = new ODataParser();

            // act
            var expressionTree = parser.Parse(Query);

            // assert
            Assert.AreEqual(3, expressionTree.Items.Count());
        }

        [TestMethod]
        public void MultiLevelSelectQuery()
        {
            // arrange
            const string Query = "$select=Id,Name,Address/City,Address/ZipCode";
            var parser = new ODataParser();

            // act
            var expressionTree = parser.Parse(Query);

            // assert
            Assert.AreEqual(3, expressionTree.Items.Count());
            Assert.AreEqual(2, expressionTree.Items.Single(i => i.Name.Equals("Address")).Items.Count());
        }

        [TestMethod]
        public void ExpandQuery()
        {
            // arrange
            const string Query = "$expand=Addresses,PhoneNumbers,Jobs";
            var parser = new ODataParser();

            // act
            var expressionTree = parser.Parse(Query);

            // assert
            Assert.AreEqual(3, expressionTree.Items.Count());
            Assert.IsInstanceOfType(expressionTree.Items.ElementAt(0), typeof(CollectionTree));
            Assert.IsInstanceOfType(expressionTree.Items.ElementAt(1), typeof(CollectionTree));
            Assert.IsInstanceOfType(expressionTree.Items.ElementAt(2), typeof(CollectionTree));
        }

        [TestMethod]
        public void MultiLevelExpandQuery()
        {
            // arrange
            const string Query = "$expand=Companies/Addresses,Companies/PhoneNumbers,Products";
            var parser = new ODataParser();

            // act
            var expressionTree = parser.Parse(Query);

            // assert
            Assert.AreEqual(2, expressionTree.Items.Count());
            Assert.AreEqual(2, expressionTree.Items.Single(i => i.Name.Equals("Companies")).Items.Count());
            Assert.IsInstanceOfType(expressionTree.Items.ElementAt(0), typeof(CollectionTree));
            Assert.IsInstanceOfType(
                expressionTree.Items.Single(i => i.Name.Equals("Companies")).Items.ElementAt(0), typeof(CollectionTree));
            Assert.IsInstanceOfType(
                expressionTree.Items.Single(i => i.Name.Equals("Companies")).Items.ElementAt(1), typeof(CollectionTree));
        }

        [TestMethod]
        public void ExpandAndSelectQuery()
        {
            // arrange
            const string Query = "$expand=Addresses&$select=Name,Addresses/City,Addresses/ZipCode";
            var parser = new ODataParser();

            // act
            var expressionTree = parser.Parse(Query);

            // assert
            Assert.AreEqual(2, expressionTree.Items.Count());
            Assert.IsInstanceOfType(
                expressionTree.Items.Single(i => i.Name.Equals("Addresses")), typeof(CollectionTree));
            Assert.IsNotInstanceOfType(
                expressionTree.Items.Single(i => !i.Name.Equals("Addresses")), typeof(CollectionTree));
            Assert.AreEqual(2, expressionTree.Items.Single(i => i.Name.Equals("Addresses")).Items.Count());
            Assert.IsNotInstanceOfType(
                expressionTree.Items.Single(i => i.Name.Equals("Addresses")).Items.ElementAt(0), 
                typeof(CollectionTree));
            Assert.IsNotInstanceOfType(
                expressionTree.Items.Single(i => i.Name.Equals("Addresses")).Items.ElementAt(1), 
                typeof(CollectionTree));
        }
    }
}