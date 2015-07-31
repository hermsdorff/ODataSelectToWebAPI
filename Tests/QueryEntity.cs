using Effort;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace Tests
{
    [TestClass]
    class QueryEntity
    {
        [TestMethod]
        public void ExecuteQueryOverACollection()
        {
            // arrange
            DbConnection conn = DbConnectionFactory.CreateTransient();
            var context = new TestContext(conn);

            // act
            // assert
        }
    }
}
