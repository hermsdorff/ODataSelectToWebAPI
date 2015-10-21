using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    using ODataSelectForWebAPI1;

    using Tests.Fakes;

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
            Assert.AreEqual(tree.QueryType.Key, typeof(int).Name);
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
            Assert.AreEqual(tree.QueryType.Key, typeof(int).Name);
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
            Assert.AreEqual("{Id:Int32;Name:String}", tree.QueryType.Key);
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
            Assert.AreEqual(3, tree.QueryType.Value.GetFields().Length);

            Assert.AreEqual(typeof(Int32), tree.QueryType.Value.GetField("Id").FieldType);
            Assert.AreEqual(typeof(String), tree.QueryType.Value.GetField("Name").FieldType);

            var manufacturer = tree.QueryType.Value.GetField("Manufacturer").FieldType;
            Assert.AreEqual(2, manufacturer.GetFields().Length);
            Assert.AreEqual(typeof(Int32), manufacturer.GetField("Id").FieldType);
            Assert.AreEqual(typeof(String), manufacturer.GetField("Name").FieldType);
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
            Assert.IsNotNull(tree.QueryType.Value.GetField("Models").FieldType.GetInterface("System.Collections.IEnumerable"));

            var modelType = tree.QueryType.Value.GetField("Models").FieldType.GetGenericArguments();
            Assert.AreEqual(1, modelType.Length);

            Assert.AreEqual(2, modelType[0].GetFields().Length);
            Assert.AreEqual(typeof(Int32), modelType[0].GetField("Id").FieldType);
            Assert.AreEqual(typeof(string), modelType[0].GetField("Name").FieldType);
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
            Assert.IsNotNull(tree.QueryType.Value.GetField("Models").FieldType.GetInterface("System.Collections.IEnumerable"));
            
            var genericType = tree.QueryType.Value.GetField("Models").FieldType.GetGenericArguments();
            Assert.AreEqual(1, genericType.Length);
            
            Assert.AreEqual(2, genericType[0].GetFields().Length);
            Assert.AreEqual(typeof(Int32), genericType[0].GetField("Id").FieldType);
            Assert.AreEqual(typeof(string), genericType[0].GetField("Name").FieldType);
        }

        [TestMethod]
        public void BuildTypeWithCircularReference()
        {
            // arrange
            const string Query = "$select=Name,Parent";
            var parser = new ODataParser();
            var tree = parser.Parse(Query);
            tree.Bind(typeof(CircularReference));

            // act
            tree.BuildType();

            // assert
            Assert.AreEqual(typeof(string), tree.QueryType.Value.GetField("Name").FieldType);
            
            var parent = tree.QueryType.Value.GetField("Parent").FieldType;
            Assert.AreEqual(2, parent.GetFields().Length);
            Assert.AreEqual(typeof(string), parent.GetField("Name").FieldType);

            var innerParent = parent.GetField("Parent").FieldType;
            Assert.AreEqual(1, innerParent.GetFields().Length);
            Assert.AreEqual(typeof(string), innerParent.GetField("Name").FieldType);
        }
    }
}