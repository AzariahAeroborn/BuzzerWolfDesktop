using Microsoft.EntityFrameworkCore;
using System;

namespace BuzzerWolf.Models
{
    [PrimaryKey(nameof(EntityId),nameof(DataTable))]
    public class Sync
    {
        public int EntityId { get; set; }
        public int? Season { get; set; }
        public string DataTable { get; set; }

        public DateTimeOffset LastSync { get; set; }
        public DateTimeOffset NextAutoSync { get; set; }
    }
}
