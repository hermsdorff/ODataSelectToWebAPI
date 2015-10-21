using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    using ODataSelectForWebAPI1;
    using Tests.Fakes;

    [TestClass]
    public class BindingTest
    {
        #region Select

        [TestMethod]
        public void ExpressionTreeWithComplexType()
        {
            // arrange
            const string Query = "$select=Id,Name";
            var parser = new ODataParser();
            var tree = parser.Parse(Query);

            // act
            tree.Bind(typeof(Category));

            // assert
            Assert.AreEqual(typeof(int), tree.Items.Single(i => i.Name.Equals("Id")).ElementType);
            Assert.AreEqual(typeof(string), tree.Items.Single(i => i.Name.Equals("Name")).ElementType);
        }

        [TestMethod]
        public void TypeWithLessPropertiesThenSelectExpression()
        {
            // arrange
            const string Query = "$select=Id,Name,FullName";
            var parser = new ODataParser();
            var tree = parser.Parse(Query);

            // act
            tree.Bind(typeof(Category));

            // assert
            Assert.AreEqual(typeof(int), tree.Items.Single(i => i.Name.Equals("Id")).ElementType);
            Assert.AreEqual(typeof(string), tree.Items.Single(i => i.Name.Equals("Name")).ElementType);
            Assert.AreEqual(typeof(object), tree.Items.Single(i => i.Name.Equals("FullName")).ElementType);
        }

        [TestMethod]
        public void IgnoreSelectInCollectionProperty()
        {
            // arrange
            const string Query = "$select=Id,Name,Products";
            var parser = new ODataParser();
            var tree = parser.Parse(Query);

            // act
            tree.Bind(typeof(Category));

            // assert
            Assert.AreEqual(typeof(int), tree.Items.Single(i => i.Name.Equals("Id")).ElementType);
            Assert.AreEqual(typeof(string), tree.Items.Single(i => i.Name.Equals("Name")).ElementType);
            Assert.AreEqual(null, tree.Items.Single(i => i.Name.Equals("Products")).ElementType);
        }

        [TestMethod]
        public void PropertyIsAComplexObjectWithNoInnerPropertiesSelected()
        {
            // arrange
            const string Query = "$select=Id,Name,Manufacturer";
            var parser = new ODataParser();
            var tree = parser.Parse(Query);

            // act
            tree.Bind(typeof(Product));

            // assert
            Assert.AreEqual(typeof(int), tree.Items.Single(i => i.Name.Equals("Id")).ElementType);
            Assert.AreEqual(typeof(string), tree.Items.Single(i => i.Name.Equals("Name")).ElementType);
            Assert.AreEqual(2, tree.Items.Single(i => i.Name.Equals("Manufacturer")).Items.Count());
            Assert.AreEqual(3, tree.Items.Count());
        }

        [TestMethod]
        public void SelectPropertiesInsideAProperty()
        {
            // arrange
            const string Query = "$select=Id,Name,Manufacturer/Name";
            var parser = new ODataParser();
            var tree = parser.Parse(Query);

            // act
            tree.Bind(typeof(Product));

            // assert
            var manufacturer = tree.Items.Single(i => i.Name.Equals("Manufacturer"));
            Assert.AreEqual(typeof(int), tree.Items.Single(i => i.Name.Equals("Id")).ElementType);
            Assert.AreEqual(typeof(string), tree.Items.Single(i => i.Name.Equals("Name")).ElementType);
            Assert.AreEqual(typeof(string), manufacturer.Items.Single(m => m.Name.Equals("Name")).ElementType);
            Assert.AreEqual(1, tree.Items.Single(i => i.Name.Equals("Manufacturer")).Items.Count());
            Assert.AreEqual(3, tree.Items.Count());
        }

        #endregion

        #region Expand

        [TestMethod]
        public void ExpandACollectionWithoutSelectItsMembers()
        {
            // arrange
            const string Query = "$expand=Products";
            var parser = new ODataParser();
            var tree = parser.Parse(Query);

            // act
            tree.Bind(typeof(Category));

            // assert
            var products = tree.Items.Single(i => i.Name.Equals("Products"));
            var manufacturer = products.Items.Single(i => i.Name.Equals("Manufacturer"));

            Assert.AreEqual(1, tree.Items.Count());
            Assert.AreEqual(3, products.Items.Count());
            Assert.AreEqual(2, manufacturer.Items.Count());
            Assert.IsInstanceOfType(products, typeof(CollectionTree));
            Assert.IsNotInstanceOfType(manufacturer.GetType(), typeof(CollectionTree));
        }

        [TestMethod]
        public void ExpandACollectionOfSystemTypeObjects()
        {
            // arrange
            const string Query = "$expand=Tags";
            var parser = new ODataParser();
            var tree = parser.Parse(Query);

            // act
            tree.Bind(typeof(Category));

            // assert
            Assert.AreEqual(1, tree.Items.Count());
            Assert.IsInstanceOfType(tree.Items.ElementAt(0).Items.ElementAt(0), typeof(SystemTreeLeaf));
        }

        [TestMethod]
        public void ExpandIgnoreNoCollectionProperty()
        {
            // arrange
            const string Query = "$expand=Products,Name";
            var parser = new ODataParser();
            var tree = parser.Parse(Query);

            // act
            tree.Bind(typeof(Category));

            // assert
            Assert.AreEqual(2, tree.Items.Count());
            Assert.AreEqual(typeof(void), tree.Items.Single(i => i.Name.Equals("Name")).ElementType);
            Assert.AreEqual(3, tree.Items.Single(i => i.Name.Equals("Products")).Items.Count());
        }

        [TestMethod]
        public void ExpandInternalCollection()
        {
            // arrange
            const string Query = "$expand=Products/Models";
            var parser = new ODataParser();
            var tree = parser.Parse(Query);

            // act
            tree.Bind(typeof(Category));

            // assert
            Assert.AreEqual(1, tree.Items.Count());
            Assert.AreEqual(1, tree.Items.ElementAt(0).Items.Count());
        }

        [TestMethod]
        public void ExpandMultipleFields()
        {
            // arrange
            const string Query = "$expand=Products,Tags";
            var parser = new ODataParser();
            var tree = parser.Parse(Query);
            
            // act
            tree.Bind(typeof(Category));

            // assert
            Assert.AreEqual(2, tree.Items.Count());
            Assert.AreEqual(3, tree.Items.Single(i=>i.Name.Equals("Products")).Items.Count());
            Assert.AreEqual(1, tree.Items.Single(i=>i.Name.Equals("Tags")).Items.Count());
        }

        #endregion

        #region Select with expand

        [TestMethod]
        public void SelectAndExpandProperty()
        {
            // arrange
            const string Query = "$select=Id,Name&$expand=Products";
            var parser = new ODataParser();
            var tree = parser.Parse(Query);

            // act
            tree.Bind(typeof(Category));

            // assert
            Assert.AreEqual(3, tree.Items.Count());
            Assert.AreEqual(3, tree.Items.Single(i=>i.Name.Equals("Products")).Items.Count());
            Assert.IsInstanceOfType(tree.Items.Single(i=>i.Name.Equals("Products")), typeof(CollectionTree));
        } 

        [TestMethod]
        public void SelectPropertiesOfAnExpandedCollection()
        {
            // arrange
            const string Query = "$select=Products/Id&$expand=Products";
            var parser = new ODataParser();
            var tree = parser.Parse(Query);

            // act
            tree.Bind(typeof(Category));

            // assert
            Assert.AreEqual(1, tree.Items.Count());
            Assert.AreEqual(1, tree.Items.ElementAt(0).Items.Count());
            Assert.AreEqual(typeof(int), tree.Items.ElementAt(0).Items.ElementAt(0).ElementType);
        }

        #endregion
    }
}
