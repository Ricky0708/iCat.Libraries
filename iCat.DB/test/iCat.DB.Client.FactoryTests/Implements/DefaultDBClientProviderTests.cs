using Microsoft.VisualStudio.TestTools.UnitTesting;
using iCat.DB.Client.Factory.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using iCat.DB.Client.Implements;
using System.Data.Common;
using iCat.DB.Client.Models;

namespace iCat.DB.Client.Factory.Implements.Tests
{
    [TestClass()]
    public class DefaultDBClientProviderTests
    {
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void NewDefaultDBClientProviderTest_ConnectionParameter()
        {
            // arrange
            var connection = Substitute.For<DbConnection>();

            // action
            var provider = new DefaultDBClientProvider(() => new DBClient(connection));

            // assert
        }

        [TestMethod()]
        public void NewDefaultDBClientProviderTest_StringParameter()
        {
            // arrange
            var connection = Substitute.For<DbConnection>();

            // action
            var provider = new DefaultDBClientProvider(() => new DBClient("TestDBA", connection));

            // assert
            Assert.IsTrue(provider.GetDBClientCreator("TestDBA") != null);
        }

        [TestMethod()]
        public void NewDefaultDBClientProviderTest_ClientInfoParameter()
        {
            // arrange
            var connection = Substitute.For<DbConnection>();

            // action
            var provider = new DefaultDBClientProvider(() => new DBClient(new DBClientInfo("TestDBA", connection)));

            // assert
            Assert.IsTrue(provider.GetDBClientCreator("TestDBA") != null);
        }
    }
}