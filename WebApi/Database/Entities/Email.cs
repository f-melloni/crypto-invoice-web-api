using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Database.Entities
{
    public class Email
    {
        [Key]
        [Required]
        public int Id { get; set; }
        public string Name { get; set; }

    }
}
