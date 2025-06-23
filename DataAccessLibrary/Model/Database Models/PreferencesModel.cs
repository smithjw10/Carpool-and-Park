using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Model
{
    public class PreferencesModel
    {
        public int UserId { get; set; }
        public string GenderPreference { get; set; } = "No Preference";
        public string EatingPreference { get; set; } = "No Preference";
        public string SmokingPreference { get; set; } = "No Preference";
        public string TemperaturePreference { get; set; } = "No Preference";
        public string MusicPreference { get; set; } = "No Preference";

        public PreferencesModel() { }

        public PreferencesModel(int userId, string genderPreference, string eatingPreference, string smokingPreference, string temperaturePreference , string musicPreference)
        {
            UserId = userId;
            GenderPreference = genderPreference;
            EatingPreference = eatingPreference;
            SmokingPreference = smokingPreference;
            TemperaturePreference = temperaturePreference;
            MusicPreference = musicPreference;
        }
        public PreferencesModel(int userId)
        {
            UserId = userId;
        }
    }
}
