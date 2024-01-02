using iCat.DB.Client.Factory.Interfaces;
using iCat.DB.Client.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;

namespace iCat.Cache.demo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DemoController : ControllerBase
    {
        private readonly IDBClientFactory _factory;

        public DemoController(IDBClientFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        [HttpGet]
        public IActionResult Get()
        {
            using (var unitOfWork = _factory.GetUnitOfWork("MainDB"))
            {
                try
                {
                    unitOfWork.Open();
                    unitOfWork.BeginTransaction();
                    var connection = _factory.GetConnection("MainDB");

                    foreach (var dr in connection.ExecuteReader("SELECT * FROM YourTable", new DbParameter[] { }))
                    {
                        var filed = dr["fieldName"];
                    };

                    unitOfWork.Commit();
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                }
                finally
                {
                    unitOfWork.Close();
                }
            }

            return Ok();
        }

        //[HttpGet]
        //public IActionResult Get()
        //{

        //    using (var unitOfork = _clientFactory.GetUnitOfWork("MainDB"))
        //    {
        //        try
        //        {
        //            unitOfork.Open();
        //            unitOfork.BeginTransaction();

        //            var connection = _clientFactory.GetConnection("MainDB");
        //            foreach (var dr in connection.ExecuteReader("SELECT * FROM YourTable", new DbParameter[] { }))
        //            {
        //                var filed = dr["fieldName"];
        //            };

        //            unitOfork.Commit();
        //        }
        //        catch (Exception)
        //        {
        //            unitOfork.Rollback();
        //        }
        //        finally
        //        {
        //            unitOfork.Close();
        //        }
        //    }

        //    return Ok();
        //}
    }
}
