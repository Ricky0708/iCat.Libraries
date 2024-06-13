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
using Microsoft.Extensions.Hosting;
using iCat.DB.Client.Extensions;
using iCat.DB.Client.Factory.Extensions;
using iCat.DB.Client.Factory.Interfaces;

namespace iCat.DB.Client.Extension.Web.Tests
{
    [TestClass()]
    public class IServiceCollectionExtensionTests
    {
        [TestMethod()]
        public void AddDBClientTest()
        {
            // arrange
            var host = Host.CreateDefaultBuilder().ConfigureServices(service =>
            {
                var dbConnection = Substitute.For<DbConnection>();
                service.AddDBClient(s => new Implements.DBClient(dbConnection));
                var provider = service.BuildServiceProvider();
            }).Build();

            var provider = host.Services;

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
            var host = Host.CreateDefaultBuilder().ConfigureServices(service =>
            {
                var dbConnection = Substitute.For<DbConnection>();
                service.AddDBClient(s => new Implements.DBClient(dbConnection));
                service.AddDBClient(s => new Implements.DBClient(dbConnection));
                var provider = service.BuildServiceProvider();
            }).Build();

            var provider = host.Services;

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
            var host = Host.CreateDefaultBuilder().ConfigureServices(service =>
            {
                var dbConnection = Substitute.For<DbConnection>();

                service.AddDBClients(
                  s => new Implements.DBClient(dbConnection),
                  s => new Implements.DBClient(dbConnection));
            }).Build();

            var provider = host.Services;
            // action


        }

        [TestMethod()]
        public void AddDBClientsTest_Success()
        {
            // arrange
            var host = Host.CreateDefaultBuilder().ConfigureServices(service =>
            {
                var dbConnection = Substitute.For<DbConnection>();
                service.AddDBClients(
                    s => new Implements.DBClient("A", dbConnection),
                    s => new Implements.DBClient("B", dbConnection));

                service.AddDBClients(
                    s => new Implements.DBClient(new DBClientInfo("A", dbConnection)),
                    s => new Implements.DBClient(new DBClientInfo("A", dbConnection)));
                var provider = service.BuildServiceProvider();
            }).Build();

            var provider = host.Services;


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
            var host = Host.CreateDefaultBuilder().ConfigureServices(service =>
            {
                var dbConnection = Substitute.For<DbConnection>();
                service.AddDBFactory(
                       () => new DBClient("A", dbConnection),
                       () => new DBClient("B", dbConnection)
                   );
                var provider = service.BuildServiceProvider();
            }).Build();

            var provider = host.Services;


            // action
            var connectionFactory = provider.GetRequiredService<IConnectionFactory>();
            var unitOfWorkFactory = provider.GetRequiredService<IUnitOfWorkFactory>();

            // assert
            Assert.IsTrue(unitOfWorkFactory.GetUnitOfWork("A") == connectionFactory.GetConnection("A"));
            Assert.IsTrue(unitOfWorkFactory.GetUnitOfWork("A") != connectionFactory.GetConnection("B"));
            Assert.AreEqual(unitOfWorkFactory.GetUnitOfWorks().Count(), 2);
            Assert.AreEqual(connectionFactory.GetConnections().Count(), 2);
        }

        [TestMethod()]
        public void AddDBFactoryTest_A_Success()
        {
            // arrange
            var host = Host.CreateDefaultBuilder().ConfigureServices(service =>
            {
                var dbConnection = Substitute.For<DbConnection>();
                service.AddDBFactory(
                       () => new DBClient("A", dbConnection),
                       () => new DBClient("B", dbConnection)
                   );
                var provider = service.BuildServiceProvider();
            }).Build();

            var provider = host.Services;

            // action
            var unitOfWork = provider.GetRequiredService<IUnitOfWorkFactory>();
            var connection = provider.GetRequiredService<IConnectionFactory>();

            // assert
            Assert.IsTrue(unitOfWork.GetUnitOfWork("A") == connection.GetConnection("A"));
            Assert.IsTrue(unitOfWork.GetUnitOfWork("A") != connection.GetConnection("B"));
            Assert.AreEqual(unitOfWork.GetUnitOfWorks().Count(), 2);
            Assert.AreEqual(connection.GetConnections().Count(), 2);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void AddDBClientFactoryTest_A_Fail()
        {
            // arrange
            var host = Host.CreateDefaultBuilder().ConfigureServices(service =>
            {
                var dbConnection = Substitute.For<DbConnection>();
                service.AddDBFactory(
                       () => new DBClient("A", dbConnection),
                       () => new DBClient("A", dbConnection)
                   );
                var provider = service.BuildServiceProvider();
            }).Build();

            var provider = host.Services;

            // action
            var connectionFactory = provider.GetRequiredService<IConnectionFactory>();
            var unitOfWorkFactory = provider.GetRequiredService<IUnitOfWorkFactory>();

            // assert

        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void AddDBFactoryTest_A_Fail()
        {
            // arrange
            var host = Host.CreateDefaultBuilder().ConfigureServices(service =>
            {
                var dbConnection = Substitute.For<DbConnection>();
                service.AddDBFactory(
                       () => new DBClient("A", dbConnection),
                       () => new DBClient("A", dbConnection)
                   );
                var provider = service.BuildServiceProvider();
            }).Build();

            var provider = host.Services;

            // action
            var factory = provider.GetRequiredService<IUnitOfWorkFactory>();

            // assert

        }

        [TestMethod()]
        public void AddDBClientFactoryTest_B()
        {
            // arrange
            var host = Host.CreateDefaultBuilder().ConfigureServices(service =>
            {
                var connection = Substitute.For<DbConnection>();
                Expression<Func<DBClient>> expr = () => new Implements.DBClient("A", connection);

                Func<IServiceProvider, Expression<Func<DBClient>>[]> func = (s) => new[] { expr };
                service.AddDBFactory(func!);

            }).Build();

            var provider = host.Services;

            // action
            var connectionFactory = provider.GetRequiredService<IConnectionFactory>();
            var unitOfWorkFactory = provider.GetRequiredService<IUnitOfWorkFactory>();


            // assert
            Assert.IsTrue(connectionFactory != null);
            Assert.IsTrue(unitOfWorkFactory != null);
        }

        [TestMethod()]
        public void AddDBFactoryTest_B()
        {
            // arrange
            // arrange
            var host = Host.CreateDefaultBuilder().ConfigureServices(service =>
            {
                var connection = Substitute.For<DbConnection>();
                Expression<Func<DBClient>> expr = () => new Implements.DBClient("A", connection);

                Func<IServiceProvider, Expression<Func<DBClient>>[]> func = (s) => new[] { expr };

                // action
                service.AddDBFactory(func!);

                // assert
                var n = service.BuildServiceProvider().GetService<IConnectionFactory>();
                Assert.IsTrue(n != null);

            }).Build();

            var provider = host.Services;
        }
    }
}