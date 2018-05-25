using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models.FileModels
{
    public class FileData
    {
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
    }
}
