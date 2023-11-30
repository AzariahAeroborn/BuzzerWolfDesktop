using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace BuzzerWolf.Models
{
    [PrimaryKey(nameof(TeamId),nameof(DataTable))]
    public class Sync
    {
        public int TeamId { get; set; }
        public string DataTable { get; set; }

        public DateTimeOffset LastSync { get; set; }
        public DateTimeOffset NextAutoSync { get; set; }

        public static List<Sync> InitializeFor(int teamId)
        {
            return new List<Sync>
            {
            };
        }
    }
}
