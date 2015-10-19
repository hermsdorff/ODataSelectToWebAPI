using Effort;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

using ODataSelectForWebAPI1;
using Tests.Fakes;
using Newtonsoft.Json;

namespace Tests
{
    using Tests.Map;

    [TestClass]
    public class QueryEntity
    {
        [TestMethod]
        public void ExecuteQueryOverAnEntityCollection()
        {
            // arrange
            DbConnection conn = DbConnectionFactory.CreateTransient();
            var context = new TestDataContext(conn);

            const string Query = "$select=Id,Status";
            var parser = new ODataParser();
            var tree = parser.Parse(Query);
            tree.Bind(typeof(Category));
            tree.BuildType();

            context.Categories.Add(
                new Category { Id = 1, Name = "C1", Status = EnumStatus.OK, Products = new List<Product> { new Product { Id = 1, Name = "P1" } } });
            context.Categories.Add(
                new Category { Id = 2, Name = "C2", Status = EnumStatus.Error, Products = new List<Product> { new Product { Id = 2, Name = "P2" } } });
            
            context.SaveChanges();

            var categories = context.Categories;

            // act
            var selection = DynamicSelection.Select(categories.AsQueryable(), tree.QueryType.Value);
            var json = JsonConvert.SerializeObject(selection);

            // assert
            const string expectedResult = "[{\"Id\":1,\"Status\":1},{\"Id\":2,\"Status\":2}]";
            Assert.AreEqual(expectedResult, json);
        }

        [TestMethod]
        public void ExecuteQueryOverAnEntityCollectionWithComplexProperty()
        {
            // arrange
            DbConnection conn = DbConnectionFactory.CreateTransient();
            var context = new TestDataContext(conn);

            const string Query = "$select=Name,Owner/Name,Owner/Contact/Email";
            var parser = new ODataParser();
            var tree = parser.Parse(Query);
            tree.Bind(typeof(Company));
            tree.BuildType();

            context.Companies.Add(new Company{ Name = "C1", Owner = new Person { Name = "P1", Contact = new Contact{ Email = "user@email.com"}}});
            context.SaveChanges();
            
            var companies = context.Companies;

            // act
            var selection = DynamicSelection.Select(companies.AsQueryable(), tree.QueryType.Value);
            var json = JsonConvert.SerializeObject(selection);
            
            // assert
            const string expectedResult = "[{\"Name\":\"C1\",\"Owner\":{\"Contact\":{\"Email\":\"user@email.com\"},\"Name\":\"P1\"}}]";
            Assert.AreEqual(expectedResult, json);
        }

        [TestMethod]
        public void ExecuteQueryOverAnEntityCollectionWithInnerCollection()
        {
            // arrange
            DbConnection conn = DbConnectionFactory.CreateTransient();
            var context = new TestDataContext(conn);

            const string Query = "$select=Name,Models/Name,Models/Id&$expand=Models";
            var parser = new ODataParser();
            var tree = parser.Parse(Query);
            tree.Bind(typeof(Product));
            tree.BuildType();
            tree = parser.Parse(Query);
            tree.Bind(typeof(Product));
            tree.BuildType();

            context.Products.Add(
                new Product
                        {
                            Name = "P1",
                            Models =
                                new List<Model>
                                    {
                                        new Model { Id = 1, Name = "M1" }, 
                                        new Model { Id = 2, Name = "M2" }
                                    }
                        });
            context.SaveChanges();
            var products = context.Products;

            // act
            var selection = DynamicSelection.Select(products.AsQueryable(), tree.QueryType.Value);
            var json = JsonConvert.SerializeObject(selection);

            // assert
            const string expectedResult =
                "[{\"Models\":[{\"Id\":1,\"Name\":\"M1\"},{\"Id\":2,\"Name\":\"M2\"}],\"Name\":\"P1\"}]";
            Assert.AreEqual(expectedResult, json);
        }

        [TestMethod]
        public void ExecuteQueryExpandingACollectionInsideNonCollectionProperty()
        {
            // arrange
            DbConnection conn = DbConnectionFactory.CreateTransient();
            var context = new TestDataContext(conn);

            var company = new Company
                { Id = 0, Name = "C1", Owner = new Person { Id = 0, Name = "P1", Addresses = new List<Address>() } };
            var address = new Address { City = "Ct1", Person = company.Owner, Street = "St1" };
            company.Owner.Addresses.Add(address);

            context.Companies.Add(company);
            context.SaveChanges();
            
            var companies = context.Companies.AsQueryable();

            const string Query = "$expand=Owner/Addresses&$select=Name,Owner/Addresses/City";
            //const string Query = "$select=Name";
            var parser = new ODataParser();
            var tree = parser.Parse(Query);
            tree.Bind(typeof(Company));
            tree.BuildType();

            // act
            var selection = DynamicSelection.Select(companies, tree.QueryType.Value);
            var json = JsonConvert.SerializeObject(selection);

            // assert
            const string expectedResult =
                "[{\"Owner\":{\"Addresses\":[{\"City\":\"Ct1\"}]},\"Name\":\"C1\"}]";
            Assert.AreEqual(expectedResult, json);
        }
    }
}
