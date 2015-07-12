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
            const string expression = "System.Collections.Generic.List`1[Testes.Fakes.Company].Select(t => new {Name:String;Owner:{Contact:{Email:String};Name:String}}() {Name = t.Name, Owner = new {Contact:{Email:String};Name:String}() {Name = t.Owner.Name, Contact = new {Email:String}() {Email = t.Owner.Contact.Email}}})";
            Assert.AreEqual(expression, selection.ToString());
        }
    }
}
