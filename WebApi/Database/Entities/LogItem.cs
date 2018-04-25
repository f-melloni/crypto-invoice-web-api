using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Database.Entities
{
    public class LogItem
    {
        [Key]
        [Required]
        public int Id { get; set; }

        public DateTime Timestamp { get; set; }
        public string Message { get; set; }
        public int Level { get; set; }
    }
}
