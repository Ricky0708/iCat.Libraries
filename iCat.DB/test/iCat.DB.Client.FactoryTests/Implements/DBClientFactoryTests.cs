using Microsoft.VisualStudio.TestTools.UnitTesting;
using iCat.DB.Client.Factory.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iCat.DB.Client.Factory.Models;
using iCat.DB.Client.Factory.Interfaces;
using iCat.DB.Client.Implements;
namespace iCat.DB.Client.Factory.Implements.Tests
{
    [TestClass()]
    public class DBClientFactoryTests
    {
        [TestMethod()]
        public void GetUnitOfWorkTest()
        {
            var n = typeof(MySQL.DBClient).BaseType;

            // arrange
            var factory = new DBClientFactory(new DefaultConnectionStringProvider(new List<ConnectionCreator> {
                //new ConnectionCreator
                //{
                //    ConnectionGenerator = () => new MSSQL.DBClient("A", "server=192.168.51.233;port=2883;uid=mgplatform@mgplatform#test;pwd=mg@OB123!;DataBase=MgPlatform;max pool size=5000;")
                //},
                new ConnectionCreator
                {
                    ConnectionGenerator = () => new MySQL.DBClient("B", "server=192.168.51.233;port=2883;uid=mgplatform@mgplatform#test;pwd=mg@OB123!;DataBase=MgPlatform;max pool size=5000;")
                }
            }));

            // action
            var a = factory.GetUnitOfWork("B");
            var b = factory.GetConnection("B");

            // assert
            Assert.IsTrue(a.Equals(b));
        }

        [TestMethod()]
        public void RemoveUnitOfWorkTest()
        {
            // arrange
            var factory = new DBClientFactory(new DefaultConnectionStringProvider(new List<ConnectionCreator> {
                new ConnectionCreator
                {
                    ConnectionGenerator = () => new MSSQL.DBClient("A", "server=192.168.51.233;port=2883;uid=mgplatform@mgplatform#test;pwd=mg@OB123!;DataBase=MgPlatform;max pool size=5000;")
                },
                new ConnectionCreator
                {
                    ConnectionGenerator = () => new MySQL.DBClient("B", "server=192.168.51.233;port=2883;uid=mgplatform@mgplatform#test;pwd=mg@OB123!;DataBase=MgPlatform;max pool size=5000;")
                }
            }));

            // action
            var a = factory.GetUnitOfWork("B");
            using (a)
            {

            }
            var b = factory.GetConnection("B");
            // assert
            Assert.IsTrue(!a.Equals(b));
        }
    }
}