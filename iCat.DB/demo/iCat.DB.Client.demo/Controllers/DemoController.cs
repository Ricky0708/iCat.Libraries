using iCat.DB.Client.Factory.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;

namespace iCat.Cache.demo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DemoController : ControllerBase
    {
        private readonly IDBClientFactory _clientFactory;

        public DemoController(IDBClientFactory clientFactory)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        }

        [HttpGet]
        public IActionResult Get()
        {

            using (var unitOfork = _clientFactory.GetUnitOfWork("MainDB"))
            {
                try
                {
                    unitOfork.Open();
                    unitOfork.BeginTransaction();

                    var connection = _clientFactory.GetConnection("MainDB");
                    foreach (var dr in connection.ExecuteReader("SELECT * FROM YourTable", new DbParameter[] { }))
                    {
                        var filed = dr["fieldName"];
                    };

                    unitOfork.Commit();
                }
                catch (Exception)
                {
                    unitOfork.Rollback();
                }
                finally
                {
                    unitOfork.Close();
                }
            }

            return Ok();
        }
    }
}
