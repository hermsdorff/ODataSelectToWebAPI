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
            const string Query = "$select=Id,Products/Name&$expand=Products";
            var parser = new ODataParser();
            var tree = parser.Parse(Query);
            tree.Bind(typeof(Category));
            tree.BuildType();

            var categories = new List<Category>
                {
                    new Category
                        { Id = 1, Name = "C1", Products = new List<Product> { new Product { Id = 1, Name = "P1" } } }
                };

            // act
            var selection = DynamicSelection.Select(categories.AsQueryable(), tree.QueryType);

            // assert
        }
    }
}
