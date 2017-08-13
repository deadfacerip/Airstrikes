using System.Collections.Generic;
using System.Xml.Serialization;
using Rocket.API;

namespace Teyhota.Airstrikes
{
    public class Config : IRocketPluginConfiguration
    {
        public static Config Instance;

        public string Mode;
        public string DisableAutoUpdates;

        public bool AutoAirstrike;

        public string GlobalMessageColor;
        //public string PlayerMessageColor;

        public ushort GroundEffectID;
        public int MinutesBetweenAirstrikes;

        public List<Location> Locations;

        //public float Delay;
        //public float StrikeSpeed;
        //public int StrikeCount;
        //public float DamageIntensity;


        public class Location
        {
            public Location() { }

            internal Location(float strikeSpeed, int strikeCount, float damageIntensity, int range, string name, string position)
            {
                StrikeSpeed = strikeSpeed;
                StrikeCount = strikeCount;
                DamageIntensity = damageIntensity;
                Range = range;
                Name = name;
                Coords = position;
            }
            
            [XmlAttribute]
            public float StrikeSpeed;
            [XmlAttribute]
            public int StrikeCount;
            [XmlAttribute]
            public float DamageIntensity;
            [XmlAttribute]
            public int Range;
            [XmlAttribute]
            public string Name;
            [XmlAttribute]
            public string Coords;
        }
        
        public void LoadDefaults()
        {
            //Mode = "Debug";

            AutoAirstrike = true;

            GlobalMessageColor = "yellow";
            //PlayerMessageColor = "#ffff00";

            GroundEffectID = 136; // 138, 42
            MinutesBetweenAirstrikes = 5;
            
            Locations = new List<Location>()
            {
                new Location(0.2f, 300, 8f, 225, "Seattle", "(-334.8, 38.7, 129.0)"),
                new Location(0.2f, 175, 7f, 100, "Everette", "(740.3, 38.8, 387.4)"),
                new Location(0.2f, 125, 6f, 50, "Kennewick Farms", "(-828.0, 79.2, 245.7)"),
                new Location(0.2f, 300, 6f, 150, "Kent Raceway", "(-253.1, 42.2, -793.0)"),
                new Location(0.3f, 300, 11f, 50, "Scorpion-7", "(853.9, 43.5, 655.2)"),
                new Location(0.2f, 125, 6f, 50, "Clearwater Campground", "(113.7, 38.5, 773.5)"),
                new Location(0.25f, 275, 10f, 55, "Olympia Military Base", "(-688.0, 40.1, -275.0)"),
                new Location(0.2f, 275, 7f, 55, "Tacoma", "(233.3, 40.2, -388.1)")
            };
            
            //Delay = 15f;
            //StrikeSpeed = 0.2f;
            //StrikeCount = 75;
            //DamageIntensity = 6f;
        }
    }
}