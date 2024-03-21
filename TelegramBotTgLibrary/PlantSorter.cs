using System.Collections.Generic;

namespace TelegramBotTgLibrary
{
    /// <summary>
    /// Provides methods to sort plant data.
    /// </summary>
    public class PlantSorter
    {
        /// <summary>
        /// Sorts plant data by Latin name in ascending order.
        /// </summary>
        /// <param name="plants">The list of plant data to sort.</param>
        public static void SortByLatinNameAscending(List<PlantData> plants)
        {
            plants.Sort((p1, p2) => string.Compare(p1.LatinName, p2.LatinName));
        }

        /// <summary>
        /// Sorts plant data by Latin name in descending order.
        /// </summary>
        /// <param name="plants">The list of plant data to sort.</param>
        public static void SortByLatinNameDescending(List<PlantData> plants)
        {
            plants.Sort((p1, p2) => string.Compare(p2.LatinName, p1.LatinName));
        }
    }
}