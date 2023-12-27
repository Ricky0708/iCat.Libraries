using Microsoft.VisualStudio.TestTools.UnitTesting;
using iCat.DB.Client.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iCat.DB.Client.Interfaces;
using iCat.DB.Client.Models;

namespace iCat.DB.Client.Implements.Tests
{
    [TestClass()]
    public class DBClientFactoryTests
    {
        [TestMethod()]
        public void GetUnitOfWorkTest()
        {
            // arrange
            var factory = new DBClientFactory(new DefaultConnectionStringProvider(new List<ConnectionData> {
                new ConnectionData{
                    Category = "A",
                    ConnectionString = "server=192.168.51.233;port=2883;uid=mgplatform@mgplatform#test;pwd=mg@OB123!;DataBase=MgPlatform;max pool size=5000;",
                    DBClientType = typeof(iCat.DB.Client.MySQL.DBClient)
                }
            }));

            // action
            var a = factory.GetUnitOfWork("A");
            var b = factory.GetConnection("A");

            // assert
            Assert.IsTrue(a.Equals(b));
        }
    }
}