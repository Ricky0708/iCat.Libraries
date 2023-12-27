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
            var factory = new DBClientFactory(new DefaultConnectionStringProvider(new Dictionary<string, ConnectionData> {
                { "A", new ConnectionData {
                        ConnectionString = "server=192.168.51.233;port=2883;uid=mgplatform@mgplatform#test;pwd=mg@OB123!;DataBase=MgPlatform;max pool size=5000;",
                        DBClientType = typeof(iCat.DB.Client.MySQL.DBClient)
                    }
                },
                { "B", new ConnectionData {
                        ConnectionString = "server=192.168.51.233;port=2883;uid=mgplatform@mgplatform#test;pwd=mg@OB123!;DataBase=MgPlatform;max pool size=5000;",
                        DBClientType = typeof(iCat.DB.Client.MySQL.DBClient)
                    }
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
            var factory = new DBClientFactory(new DefaultConnectionStringProvider(new Dictionary<string, ConnectionData> {
                { "A", new ConnectionData {
                        ConnectionString = "server=192.168.51.233;port=2883;uid=mgplatform@mgplatform#test;pwd=mg@OB123!;DataBase=MgPlatform;max pool size=5000;",
                        DBClientType = typeof(iCat.DB.Client.MySQL.DBClient)
                    }
                },
                { "B", new ConnectionData {
                        ConnectionString = "server=192.168.51.233;port=2883;uid=mgplatform@mgplatform#test;pwd=mg@OB123!;DataBase=MgPlatform;max pool size=5000;",
                        DBClientType = typeof(iCat.DB.Client.MySQL.DBClient)
                    }
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