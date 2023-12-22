using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Localization.WebApplicationTest.Models
{
    public class TestModel
    {
        [MaxLength(3, ErrorMessage = "Error.MaxLength")]
        [Required(ErrorMessage = "Error.Required")]
        public required string Name { get; set; }

    }
}
