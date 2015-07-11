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
    public class QueryTest
    {
        [TestMethod]
        public void ExecuteQueryOverACollection()
        {
            // arrange
            const string Query = "$select=Id";
            var parser = new ODataParser();
            var tree = parser.Parse(Query);
            tree.Bind(typeof(Category));
            tree.BuildType();

            var categories = new List<Category>
                {
                    new Category
                        { Id = 1, Name = "C1", Products = new List<Product> { new Product { Id = 1, Name = "P1" } } },
                    new Category
                        { Id = 2, Name = "C2", Products = new List<Product> { new Product { Id = 2, Name = "P2" } } }
                };

            // act
            var selection = DynamicSelection.Select(categories.AsQueryable(), tree.QueryType);

            // assert
            const string expression = "System.Collections.Generic.List`1[Testes.Fakes.Category].Select(t => new {Id:Int32}() {Id = t.Id})";
            Assert.AreEqual(expression, selection.ToString());
        }

        [TestMethod]
        public void ExecuteQueryOverACollectionWithComplexProperty()
        {
            // arrange
            const string Query = "$select=Id,Manufacturer/Name";
            var parser = new ODataParser();
            var tree = parser.Parse(Query);
            tree.Bind(typeof(Product));
            tree.BuildType();

            var products = new List<Product>
                {
                    new Product
                        { Id = 1, Name = "C1", Manufacturer = new Manufacturer{Id = 1, Name="M1"}},
                    new Product
                        { Id = 2, Name = "C2", Manufacturer = new Manufacturer{Id = 1, Name="M2"}}
                };

            // act
            var selection = DynamicSelection.Select(products.AsQueryable(), tree.QueryType);

            // assert
            const string expression = "System.Collections.Generic.List`1[Testes.Fakes.Product].Select(t => new {Id:Int32;Manufacturer:{Name:String}}() {Id = t.Id, Manufacturer = new {Name:String}() {Name = t.Manufacturer.Name}})";
            Assert.AreEqual(expression, selection.ToString());
        }
    }
}
