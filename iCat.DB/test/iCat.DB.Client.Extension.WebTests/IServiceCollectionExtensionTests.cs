using Microsoft.VisualStudio.TestTools.UnitTesting;
using iCat.DB.Client.Extension.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
using Microsoft.Extensions.DependencyInjection.Extensions;
using iCat.DB.Client.Interfaces;
using iCat.DB.Client.Implements;
using iCat.DB.Client.Models;
using System.Linq.Expressions;
using iCat.DB.Client.Factory.Interfaces;
using iCat.DB.Client.Factory.Implements;
using Microsoft.AspNetCore.Builder;

namespace iCat.DB.Client.Extension.Web.Tests
{
    [TestClass()]
    public class IServiceCollectionExtensionTests
    {
        [TestMethod()]
        public void AddDBClientTest()
        {
            // arrange
            var service = WebApplication.CreateBuilder().Services;
            var dbConnection = Substitute.For<DbConnection>();
            service.AddDBClient(s => new Implements.DBClient(dbConnection));
            var provider = service.BuildServiceProvider();

            // action
            var unitOfWork = provider.GetRequiredService<IUnitOfWork>();
            var connection = provider.GetRequiredService<IConnection>();

            // assert
            Assert.IsTrue(unitOfWork == connection);
            Assert.IsTrue(unitOfWork != null);
            Assert.IsTrue(connection != null);
        }

        [TestMethod()]
        public void AddDBClientTest_Count1()
        {
            // arrange
            var service = WebApplication.CreateBuilder().Services;
            var dbConnection = Substitute.For<DbConnection>();
            service.AddDBClient(s => new Implements.DBClient(dbConnection));
            service.AddDBClient(s => new Implements.DBClient(dbConnection));
            var provider = service.BuildServiceProvider();

            // action
            var unitOfWorks = provider.GetRequiredService<IEnumerable<IUnitOfWork>>();
            var connections = provider.GetRequiredService<IEnumerable<IConnection>>();

            // assert
            Assert.AreEqual(unitOfWorks.ToList().Count(), 1);
            Assert.AreEqual(connections.ToList().Count(), 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void AddDBClientsTest_Fail()
        {
            // arrange
            var service = WebApplication.CreateBuilder().Services;
            var dbConnection = Substitute.For<DbConnection>();

            // action
            service.AddDBClients(
              s => new Implements.DBClient(dbConnection),
              s => new Implements.DBClient(dbConnection));

        }

        [TestMethod()]
        public void AddDBClientsTest_Success()
        {
            // arrange
            var service = WebApplication.CreateBuilder().Services;
            var dbConnection = Substitute.For<DbConnection>();
            service.AddDBClients(
                s => new Implements.DBClient("A", dbConnection),
                s => new Implements.DBClient("B", dbConnection));

            service.AddDBClients(
                s => new Implements.DBClient(new DBClientInfo("A", dbConnection)),
                s => new Implements.DBClient(new DBClientInfo("A", dbConnection)));
            var provider = service.BuildServiceProvider();

            // action
            var unitOfWorks = provider.GetRequiredService<IEnumerable<IUnitOfWork>>();
            var connections = provider.GetRequiredService<IEnumerable<IConnection>>();

            // assert
            Assert.AreEqual(unitOfWorks.Count(), 4);
            Assert.AreEqual(connections.Count(), 4);
            Assert.IsTrue(unitOfWorks.ToList()[0] == connections.ToList()[0]);
            Assert.IsTrue(unitOfWorks.ToList()[0] != connections.ToList()[1]);

        }

        [TestMethod()]
        public void AddDBClientFactoryTest_A_Success()
        {
            // arrange
            var service = WebApplication.CreateBuilder().Services;
            var dbConnection = Substitute.For<DbConnection>();
            service.AddDBClientFactory(
                   () => new DBClient("A", dbConnection),
                   () => new DBClient("B", dbConnection)
               );
            var provider = service.BuildServiceProvider();

            // action
            var factory = provider.GetRequiredService<IDBClientFactory>();

            // assert
            Assert.IsTrue(factory.GetUnitOfWork("A") == factory.GetConnection("A"));
            Assert.IsTrue(factory.GetUnitOfWork("A") != factory.GetConnection("B"));
            Assert.AreEqual(factory.GetUnitOfWorks().Count(), 2);
            Assert.AreEqual(factory.GetConnections().Count(), 2);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void AddDBClientFactoryTest_A_Fail()
        {
            // arrange
            var service = WebApplication.CreateBuilder().Services;
            var dbConnection = Substitute.For<DbConnection>();
            service.AddDBClientFactory(
                   () => new DBClient("A", dbConnection),
                   () => new DBClient("A", dbConnection)
               );
            var provider = service.BuildServiceProvider();

            // action
            var factory = provider.GetRequiredService<IDBClientFactory>();

            // assert

        }

        [TestMethod()]
        public void AddDBClientFactoryTest_B()
        {
            // arrange
            var service = WebApplication.CreateBuilder().Services;
            var connection = Substitute.For<DbConnection>();
            Expression<Func<DBClient>> expr = () => new Implements.DBClient("A", connection);

            Func<IServiceProvider, Expression<Func<DBClient>>[]> func = (s) => new[] { expr };

            // action
            service.AddDBClientFactory(func);

            // assert
            var n = service.BuildServiceProvider().GetService<IDBClientFactory>();
            Assert.IsTrue(n != null);

        }
    }
}