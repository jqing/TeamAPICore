using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamAPICore.Models
{
    public class PersistedDocument
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public string URL { get; set; }
        [JsonIgnore]
        public string Hash { get; set; }

    }
}
