using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace DataAccessLibrary.Model.Logic_Models
{
    public class UserInfoModel
    {
        public long UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserType { get; set; }
        public string PickupLocation { get; set; }
        public string DropoffLocation { get; set; }
        public long DrivingDistance { get; set; }
        public string Gender { get; set; } = "Non-binary";
        public long BeltCount { get; set; }
        public string AllowEatDrink { get; set; } = "No Preference";
        public string AllowSmokeVape { get; set; } = "No Preference";
        public string GenderPreference { get; set; } = "No Preference";
        public string EatingPreference { get; set; } = "No Preference";
        public string SmokingPreference { get; set; } = "No Preference";
        public string TemperaturePreference { get; set; } = "No Preference";
        public string MusicPreference { get; set; } = "No Preference";
        public double PickupLatitude { get; set; }
        public double PickupLongitude { get; set; }
        public double DropoffLatitude { get; set; }
        public double DropoffLongitude { get; set; }
        public string PhonePrivacy { get; set; } = "Share With No One";
        public string AddressPrivacy { get; set; } = "Share With No One";
        public string MakeModel { get; set; }
        public string VehicleColor { get; set; }
        public string LicensePlate { get; set; }
        public byte[] LicensePicture { get; set; }
        public byte[] CarPicture { get; set; }
        public byte[] ProfilePicture { get; set; }
        public long Rating { get; set; } = -1;

        public UserInfoModel()
        {

        }
        // Constructor that Dapper will use
        public UserInfoModel(long userID, string firstName, string lastName, string userType, string pickupLocation, string dropoffLocation, long drivingDistance, string gender, long beltCount, string allowEatDrink, string allowSmokeVape, string genderPreference, string eatingPreference, string smokingPreference, string temperaturePreference, string musicPreference, double pickupLatitude, double pickupLongitude, double dropoffLongitude, double dropoffLatitude, string phonePrivacy, string addressPrivacy, string makeModel, string vehicleColor, string licensePlate, byte[] licensePicture, byte[] carPicture, byte[] profilePicture, long rating)
        {
            UserID = userID;
            FirstName = firstName;
            LastName = lastName;
            UserType = userType;
            PickupLocation = pickupLocation;
            DropoffLocation = dropoffLocation;
            DrivingDistance = drivingDistance;
            Gender = gender;
            BeltCount = beltCount;
            AllowEatDrink = allowEatDrink;
            AllowSmokeVape = allowSmokeVape;
            GenderPreference = genderPreference;
            EatingPreference = eatingPreference;
            SmokingPreference = smokingPreference;
            TemperaturePreference = temperaturePreference;
            MusicPreference = musicPreference;
            PickupLatitude = pickupLatitude;
            PickupLongitude = pickupLongitude;
            DropoffLatitude = dropoffLatitude;
            DropoffLongitude = dropoffLongitude;
            PhonePrivacy = phonePrivacy;
            AddressPrivacy = addressPrivacy;
            MakeModel = makeModel;
            VehicleColor = vehicleColor;
            LicensePlate = licensePlate;
            LicensePicture = licensePicture;
            CarPicture = carPicture;
            ProfilePicture = profilePicture;
            Rating = rating;
        }

        // Optional: Override ToString for easy display of user info
        public override string ToString()
        {
            return $"UserID: {UserID}, Name: {FirstName} {LastName}, UserType: {UserType}, Pickup: {PickupLocation} (Lat: {PickupLatitude}, Lon: {PickupLongitude}), Dropoff: {DropoffLocation} (Lat: {DropoffLatitude}, Lon: {DropoffLongitude}), DrivingDistance: {DrivingDistance}, Gender: {Gender}, BeltCount: {BeltCount}, AllowEatDrink: {AllowEatDrink}, AllowSmokeVape: {AllowSmokeVape}, Preferences: [Gender: {GenderPreference}, Eating: {EatingPreference}, Smoking: {SmokingPreference}, Temperature: {TemperaturePreference}, Music: {MusicPreference}]";
        }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            UserInfoModel other = (UserInfoModel)obj;
            return UserID == other.UserID;
        }

        public override int GetHashCode()
        {
            return UserID.GetHashCode();
        }
    }
}