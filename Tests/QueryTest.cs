using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    using System;

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
            var selection = DynamicSelection.Select(categories.AsQueryable(), tree.QueryType.Value);

            // assert
            var elementType = selection.AsQueryable().ElementType.GetFields();
            
            Assert.AreEqual(2, elementType.Length);
            Assert.AreEqual(typeof(Int32), selection.AsQueryable().ElementType.GetField("Id").FieldType);
            Assert.AreEqual(typeof(EnumStatus), selection.AsQueryable().ElementType.GetField("Status").FieldType);
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
            var selection = DynamicSelection.Select(companies.AsQueryable(), tree.QueryType.Value);

            // assert
            var elementType = selection.ElementType.GetFields();
            Assert.AreEqual(2, elementType.Length);

            Assert.AreEqual(typeof(string), selection.ElementType.GetField("Name").FieldType);
            
            var owner = selection.AsQueryable().ElementType.GetField("Owner").FieldType;
            Assert.AreEqual(2, owner.GetFields().Length);
            Assert.AreEqual(typeof(string), owner.GetField("Name").FieldType);

            var contact = owner.GetField("Contact").FieldType;
            Assert.AreEqual(1, contact.GetFields().Length);
            Assert.AreEqual(typeof(string), contact.GetField("Email").FieldType);
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
            var selection = DynamicSelection.Select(companies.AsQueryable(), tree.QueryType.Value);

            // assert
            var elementType = selection.ElementType;
            Assert.AreEqual(2, elementType.GetFields().Length);
            Assert.AreEqual(typeof(string), elementType.GetField("Name").FieldType);

            var models = elementType.GetField("Models").FieldType;
            Assert.IsNotNull(models.GetInterface("System.Collections.IEnumerable"));
            
            var genericArgument = models.GetGenericArguments();
            Assert.AreEqual(1, genericArgument.Length);
            Assert.AreEqual(2, genericArgument[0].GetFields().Length);
            Assert.AreEqual(typeof(string), genericArgument[0].GetField("Name").FieldType);
            Assert.AreEqual(typeof(Int32), genericArgument[0].GetField("Id").FieldType);
        }
    }
}
