using CsvHelper.Configuration.Attributes;
using Newtonsoft.Json;

namespace TelegramBotTgLibrary
{
    /// <summary>
    /// Represents data for a plant.
    /// </summary>
    public class PlantData
    {
        // Use JsonProperty and Name attributes to specify the field name in JSON and CSV.
        [JsonProperty("ID")]
        [Name("ID")]
        public string ID { get; set; }

        // Use JsonProperty and Name attributes to specify the field name in JSON and CSV.
        [JsonProperty("Name")]
        [Name("Name")]
        public string Name { get; set; }

        // Use JsonProperty and Name attributes to specify the field name in JSON and CSV.
        [JsonProperty("LatinName")]
        [Name("LatinName")]
        public string LatinName { get; set; }

        // Use JsonProperty and Name attributes to specify the field name in JSON and CSV.
        [JsonProperty("Photo")]
        [Name("Photo")]
        public string Photo { get; set; }

        // Use JsonProperty and Name attributes to specify the field name in JSON and CSV.
        [JsonProperty("LandscapingZone")]
        [Name("LandscapingZone")]
        public string LandscapingZone { get; set; }

        // Use JsonProperty and Name attributes to specify the field name in JSON and CSV.
        [JsonProperty("ProsperityPeriod")]
        [Name("ProsperityPeriod")]
        public string ProsperityPeriod { get; set; }

        // Use JsonProperty and Name attributes to specify the field name in JSON and CSV.
        [JsonProperty("Description")]
        [Name("Description")]
        public string Description { get; set; }

        // Use JsonProperty and Name attributes to specify the field name in JSON and CSV.
        [JsonProperty("LocationPlace")]
        [Name("LocationPlace")]
        public string LocationPlace { get; set; }

        // Use JsonProperty and Name attributes to specify the field name in JSON and CSV.
        [JsonProperty("ViewForm")]
        [Name("ViewForm")]
        public string ViewForm { get; set; }

        // Use JsonProperty and Name attributes to specify the field name in JSON and CSV.
        [JsonProperty("GlobalId")]
        [Name("global_id")]
        public string GlobalId { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PlantData() { }

        /// <summary>
        /// Constructor with parameters.
        /// </summary>
        public PlantData(string id, string name, string latinName, string photo, string landscapingZone, string prosperityPeriod, string description, string locationPlace, string viewForm, string globalId)
        {
            ID = id;
            Name = name;
            LatinName = latinName;
            Photo = photo;
            LandscapingZone = landscapingZone;
            ProsperityPeriod = prosperityPeriod;
            Description = description;
            LocationPlace = locationPlace;
            ViewForm = viewForm;
            GlobalId = globalId;
        }
    }
}
