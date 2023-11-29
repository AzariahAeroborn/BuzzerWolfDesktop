using System;
using System.Linq;
using System.Xml.Linq;

namespace BuzzerWolf.BBAPI.Model
{
    public class TeamInfo
    {
        public TeamInfo(XElement bbapiResponse)
        {
            Id = int.Parse(bbapiResponse.Descendants("team").First().Attribute("id")!.Value);
            TeamName = bbapiResponse.Descendants("teamName").First().Value;
            ShortName = bbapiResponse.Descendants("shortName").First().Value;
            Owner = new TeamOwner(bbapiResponse.Descendants("owner").First());
            CreateDate = DateTime.Parse(bbapiResponse.Descendants("createDate").First().Value);

            var lastLogin = bbapiResponse.Descendants("lastLoginDate").FirstOrDefault();
            if (lastLogin != null)
            {
                LastLoginDate = DateTime.Parse(lastLogin.Value);
            }

            League = new League(bbapiResponse.Descendants("league").First());
            Country = new Country(bbapiResponse.Descendants("country").First());
            Rival = new TeamRival(bbapiResponse.Descendants("rival").First());
        }

        public int Id { get; set; }
        public string TeamName { get; set; }
        public string ShortName { get; set; }
        public TeamOwner Owner { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public League League { get; set; }
        public Country Country { get; set; }
        public TeamRival Rival { get; set; }
    }

    public class TeamRival
    {
        public TeamRival(XElement rivalInfo)
        {
            Id = int.Parse(rivalInfo.Attribute("id")!.Value);
            TeamName = rivalInfo.Value;
        }

        public int Id { get; set; }
        public string TeamName { get; set; }
    }

    public class TeamOwner
    {
        public TeamOwner(XElement ownerInfo)
        {
            Name = ownerInfo.Value;
            IsSupporter = ownerInfo.Attribute("supporter")?.Value == "1";
        }

        public string Name { get; set; }
        public bool IsSupporter { get; set; }
    }
}
