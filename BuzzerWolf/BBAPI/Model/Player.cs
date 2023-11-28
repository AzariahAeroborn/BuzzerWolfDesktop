using System.Linq;
using System.Xml.Linq;

namespace BuzzerWolf.BBAPI.Model
{
    public class Player
    {
        public Player(XElement playerInfo)
        {
            Id = int.Parse(playerInfo.Attribute("id")!.Value);
            FirstName = playerInfo.Descendants("firstName").First().Value;
            LastName = playerInfo.Descendants("lastName").First().Value;
            Nationality = new Country(playerInfo.Descendants("nationality").First());
            Age = int.Parse(playerInfo.Descendants("age").First().Value);
            HeightInches = int.Parse(playerInfo.Descendants("height").First().Value);
            DMI = int.Parse(playerInfo.Descendants("dmi").First().Value);
            Jersey = int.TryParse(playerInfo.Attribute("jersey")?.Value, out var jersey) ? jersey : null;
            Salary = int.Parse(playerInfo.Descendants("salary").First().Value);
            BestPosition = playerInfo.Descendants("bestPosition").First().Value;
            GameShape = int.Parse(playerInfo.Descendants("gameShape").First().Value);
            Potential = int.Parse(playerInfo.Descendants("potential").First().Value);

            if (playerInfo.Descendants("jumpShot").FirstOrDefault() != null)
            {
                Skills = new PlayerSkills(playerInfo);
            }
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public Country Nationality { get; set; }
        public int Age { get; set; }
        public int HeightInches { get; set; }
        public int DMI { get; set; }
        public int? Jersey { get; set; }
        public int Salary { get; set; }
        public string BestPosition { get; set; }
        public int GameShape { get; set; }
        public int Potential { get; set; }
        public PlayerSkills? Skills { get; set; }
    }

    public class PlayerSkills
    {
        public PlayerSkills(XElement playerInfo)
        {
            JumpShot = int.Parse(playerInfo.Descendants("jumpShot").First().Value);
            JumpRange = int.Parse(playerInfo.Descendants("range").First().Value);
            OutsideDefense = int.Parse(playerInfo.Descendants("outsideDef").First().Value);
            Handling = int.Parse(playerInfo.Descendants("handling").First().Value);
            Driving = int.Parse(playerInfo.Descendants("driving").First().Value);
            Passing = int.Parse(playerInfo.Descendants("passing").First().Value);
            InsideShot = int.Parse(playerInfo.Descendants("insideShot").First().Value);
            InsideDefense = int.Parse(playerInfo.Descendants("insideDef").First().Value);
            Rebounding = int.Parse(playerInfo.Descendants("rebound").First().Value);
            ShotBlocking = int.Parse(playerInfo.Descendants("block").First().Value);
            Stamina = int.Parse(playerInfo.Descendants("stamina").First().Value);
            FreeThrow = int.Parse(playerInfo.Descendants("freeThrow").First().Value);
            Experience = int.Parse(playerInfo.Descendants("experience").First().Value);
        }

        public int JumpShot { get; set; }
        public int JumpRange { get; set; }
        public int OutsideDefense { get; set; }
        public int Handling { get; set; }
        public int Driving { get; set; }
        public int Passing { get; set; }
        public int InsideShot { get; set; }
        public int InsideDefense { get; set; }
        public int Rebounding { get; set; }
        public int ShotBlocking { get; set; }
        public int Stamina { get; set; }
        public int FreeThrow { get; set; }
        public int Experience { get; set; }
    }
}
