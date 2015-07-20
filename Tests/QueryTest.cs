using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    using ODataSelectForWebAPI1;
    using Tests.Fakes;

    [TestClass]
    public class QueryTest
    {
        [TestMethod]
        public void ExecuteQueryOverACollection()
        {
            // arrange
            const string Query = "$select=Id,Status";
            var parser = new ODataParser();
            var tree = parser.Parse(Query);
            tree.Bind(typeof(Category));
            tree.BuildType();

            var categories = new List<Category>
                {
                    new Category
                        { Id = 1, Name = "C1", Status = EnumStatus.OK, Products = new List<Product> { new Product { Id = 1, Name = "P1" } } },
                    new Category
                        { Id = 2, Name = "C2", Status = EnumStatus.Error, Products = new List<Product> { new Product { Id = 2, Name = "P2" } } }
                };

            // act
            var selection = DynamicSelection.Select(categories.AsQueryable(), tree.QueryType);

            // assert
            const string expression = "System.Collections.Generic.List`1[Tests.Fakes.Category].Select(t0 => new {Id:Int32;Status:EnumStatus}() {Id = t0.Id, Status = t0.Status})";
            Assert.AreEqual(expression, selection.ToString());
        }

        [TestMethod]
        public void ExecuteQueryOverACollectionWithComplexProperty()
        {
            // arrange
            const string Query = "$select=Name,Owner/Name,Owner/Contact/Email";
            var parser = new ODataParser();
            var tree = parser.Parse(Query);
            tree.Bind(typeof(Company));
            tree.BuildType();

            var companies = new List<Company>
                {
                    new Company
                        { Name = "C1", Owner = new Person { Name = "P1", Contact = new Contact{ Email = "user@email.com"}}}
                };

            // act
            var selection = DynamicSelection.Select(companies.AsQueryable(), tree.QueryType);

            // assert
            const string expression = "System.Collections.Generic.List`1[Tests.Fakes.Company].Select(t0 => new {Name:String;Owner:{Contact:{Email:String};Name:String}}() {Name = t0.Name, Owner = new {Contact:{Email:String};Name:String}() {Name = t0.Owner.Name, Contact = new {Email:String}() {Email = t0.Owner.Contact.Email}}})";
            Assert.AreEqual(expression, selection.ToString());
        }

        [TestMethod]
        public void ExecuteQueryOverACollectionWithInnerCollection()
        {
            // arrange
            const string Query = "$select=Name,Models/Name,Models/Id&$expand=Models";
            var parser = new ODataParser();
            var tree = parser.Parse(Query);
            tree.Bind(typeof(Product));
            tree.BuildType();
            tree = parser.Parse(Query);
            tree.Bind(typeof(Product));
            tree.BuildType();

            var companies = new List<Product>
                {
                    new Product
                        {
                            Name = "P1",
                            Models =
                                new List<Model>
                                    {
                                        new Model { Id = 1, Name = "M1" }, 
                                        new Model { Id = 2, Name = "M2" }
                                    }
                        }
                };

            // act
            var selection = DynamicSelection.Select(companies.AsQueryable(), tree.QueryType);

            // assert
            const string expression =
                "System.Collections.Generic.List`1[Tests.Fakes.Product].Select(t0 => new {Name = t0.Name, Models = new List`1(t0.Models.Select(t1 => new {Id:Int32;Name:String}() {Id = t1.Id, Name = t1.Name}))})";
            Assert.AreEqual(expression, selection.ToString());
        }
    }
}
