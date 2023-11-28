﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace BuzzerWolf.BBAPI.Model
{
    public class SeasonList
    {
        public SeasonList(XElement bbapiResponse)
        {
            Seasons = new List<Season>();
            foreach (var seasonInfo in bbapiResponse.Descendants("season"))
            {
                Seasons.Add(new Season(seasonInfo));
            }
        }

        public List<Season> Seasons { get; set; }
    }

    public class Season
    {
        public Season(XElement seasonInfo)
        {
            Id = int.Parse(seasonInfo.Attribute("id")!.Value);
            Start = DateTime.Parse(seasonInfo.Descendants("start").First().Value);
            var finish = seasonInfo.Descendants("finish").FirstOrDefault();
            Finish = (finish != null) ? DateTime.Parse(finish.Value) : null;
        }

        public int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime? Finish { get; set; }
    }
}
