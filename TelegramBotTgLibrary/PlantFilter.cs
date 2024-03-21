using System;
using System.Collections.Generic;
using System.Linq;

namespace TelegramBotTgLibrary
{
    /// <summary>
    /// Provides methods to filter plant data.
    /// </summary>
    public class PlantFilter
    {
        /// <summary>
        /// Filters plants by landscaping zone.
        /// </summary>
        /// <param name="plants">The list of plant data to filter.</param>
        /// <param name="filterValue">The value to filter by.</param>
        /// <returns>The filtered list of plant data.</returns>
        public static List<PlantData> FilterByLandscapingZone(List<PlantData> plants, string filterValue)
        {
            return plants.Where(p => p.LandscapingZone.Contains(filterValue, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        /// <summary>
        /// Filters plants by location place.
        /// </summary>
        /// <param name="plants">The list of plant data to filter.</param>
        /// <param name="filterValue">The value to filter by.</param>
        /// <returns>The filtered list of plant data.</returns>
        public static List<PlantData> FilterByLocationPlace(List<PlantData> plants, string filterValue)
        {
            return plants.Where(p => p.LocationPlace.Contains(filterValue, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        /// <summary>
        /// Filters plants by prosperity period.
        /// </summary>
        /// <param name="plants">The list of plant data to filter.</param>
        /// <param name="filterValue">The value to filter by.</param>
        /// <returns>The filtered list of plant data.</returns>
        public static List<PlantData> FilterByProsperityPeriod(List<PlantData> plants, string filterValue)
        {
            return plants.Where(p => p.ProsperityPeriod.Contains(filterValue, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        /// <summary>
        /// Filters plants by both landscaping zone and prosperity period.
        /// </summary>
        /// <param name="plants">The list of plant data to filter.</param>
        /// <param name="landscapingZone">The value to filter the landscaping zone by.</param>
        /// <param name="prosperityPeriod">The value to filter the prosperity period by.</param>
        /// <returns>The filtered list of plant data.</returns>
        public static List<PlantData> FilterByLandscapingZoneAndProsperityPeriod(List<PlantData> plants, string landscapingZone, string prosperityPeriod)
        {
            return plants.Where(p => p.LandscapingZone.Contains(landscapingZone, StringComparison.OrdinalIgnoreCase) && 
                                     p.ProsperityPeriod.Contains(prosperityPeriod, StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }
}
