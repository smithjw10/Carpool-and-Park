using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Model
{
    public class UsersModel
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string UserType { get; set; } = "rider";
        public string UserLocation { get; set; }
        public string PickupLocation { get; set; }
        public string DropoffLocation { get; set; }
        public int DrivingDistance { get; set; } = 30;
        public string PhonePrivacy { get; set; } = "Share With No One";
        public string Gender { get; set; } = "Other";
        public byte[] ProfilePicture { get; set; }
        public string AddressPrivacy { get; set; } = "Share With No One";
        public int? BeltCount { get; set; } 
        public string MakeModel { get; set; }
        public string VehicleColor { get; set; }
        public string LicensePlate { get; set; }
        public byte[] LicensePicture { get; set; }
        public byte[] CarPicture { get; set; }
        public string AllowEatDrink { get; set; } = "false";
        public string AllowSmokeVape { get; set; } = "false";
        public double Rating { get; set; }
        public UsersModel() { }

        public UsersModel(int userId, string email, string firstName, string lastName, string phone, string userType, string userLocation, string pickupLocation, string dropoffLocation, int drivingDistance, double rating)
        {
            UserId = userId;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            Phone = phone;
            UserType = userType;
            UserLocation = userLocation;
            PickupLocation = pickupLocation;
            DropoffLocation = dropoffLocation;
            DrivingDistance = drivingDistance;
            Rating = rating;
        }

        public UsersModel(string email, string firstName, string lastName, string phone, string userType, string userLocation, string pickupLocation, string dropoffLocation, int drivingDistance, double rating)
        {

            Email = email;
            FirstName = firstName;
            LastName = lastName;
            Phone = phone;
            UserType = userType;
            UserLocation = userLocation;
            PickupLocation = pickupLocation;
            DropoffLocation = dropoffLocation;
            DrivingDistance = drivingDistance;
            Rating = rating;
        }
        public UsersModel(int userId, string email, string firstName, string lastName, string phone, string userType, string userLocation, string pickupLocation, string dropoffLocation, int drivingDistance, string phonePrivacy, string gender, byte[] profilePicture, string addressPrivacy, int? beltCount, string makeModel, string vehicleColor, string licensePlate, byte[] licensePicture, byte[] carPicture, string allowEatDrink, string allowSmokeVape, double rating)
        {
            UserId = userId;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            Phone = phone;
            UserType = userType;
            UserLocation = userLocation;
            PickupLocation = pickupLocation;
            DropoffLocation = dropoffLocation;
            DrivingDistance = drivingDistance;
            PhonePrivacy = phonePrivacy;
            Gender = gender;
            ProfilePicture = profilePicture;
            AddressPrivacy = addressPrivacy;
            BeltCount = beltCount;
            MakeModel = makeModel;
            VehicleColor = vehicleColor;
            LicensePlate = licensePlate;
            LicensePicture = licensePicture;
            CarPicture = carPicture;
            AllowEatDrink = allowEatDrink;
            AllowSmokeVape = allowSmokeVape;
            Rating = rating;
        }
        public override string ToString()
        {
            return $"Email: {Email}, FirstName: {FirstName}, LastName: {LastName}, Phone: {Phone}, UserType: {UserType}, UserLocation: {UserLocation}, PickupLocation: {PickupLocation}, DropoffLocation: {DropoffLocation}, DrivingDistance: {DrivingDistance} miles";
        }
    }

}
