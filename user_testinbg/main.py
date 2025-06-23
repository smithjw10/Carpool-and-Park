import sqlite3
import random
import requests
from datetime import datetime, timedelta
import math
import csv  # Import csv module to handle CSV file writing

# Parameters for customization
START_DATE = "2024-11-01"  # Start date for schedules
END_DATE = "2024-12-31"  # End date for schedules
PICKUP_TIMES = ["07:00", "08:00", "10:00", "11:00"]  # Possible pickup times
DEPARTURE_TIMES = ["13:00", "14:00", "15:00", "17:00", "18:00"]  # Possible departure times
CAMPUS_LATITUDE = 28.06675
CAMPUS_LONGITUDE = -80.623032
MAX_OFFSET_DEGREES = 0.045  # Approx. 5km radius
GENDER_VALUES = ["No Preference", "Women and Non-binary Only", "Same Gender", "Non-binary Only"]
SMOKING_PREFERENCES = ["No Preference", "Vehicle allows smoking", "Vehicle does not allow smoking"]
EATING_PREFERENCES = ["No Preference", "Vehicle allows eating", "Vehicle does not allow eating"]
TEMPERATURE_PREFERENCES = ["No Preference", "Warmer", "Colder"]
DIRECTIONS = ["North", "South", "East", "West"]
CAMPUS_ADDRESS = "150 W University Blvd, Melbourne, FL 32901, USA"
# Mapping directions to angles
direction_to_angle = {
    "North": math.pi / 2,           # 90 degrees
    "South": 3 * math.pi / 2,       # 270 degrees
    "East": 0,                      # 0 degrees
    "West": math.pi,                # 180 degrees
}

# Preference initials mappings
gender_initials = {
    "No Preference": "NP",
    "Women and Non-binary Only": "WNO",
    "Same Gender": "SG",
    "Non-binary Only": "NBO"
}

smoking_initials = {
    "No Preference": "NP",
    "Vehicle allows smoking": "VAS",
    "Vehicle does not allow smoking": "VNAS"
}

eating_initials = {
    "No Preference": "NP",
    "Vehicle allows eating": "VAE",
    "Vehicle does not allow eating": "VNAE"
}

temperature_initials = {
    "No Preference": "NP",
    "Warmer": "W",
    "Colder": "C"
}

db_path = "Carpool.db"

# Database connection
conn = sqlite3.connect(db_path)
cursor = conn.cursor()

# API Key for Google Maps
GOOGLE_MAPS_API_KEY = 'AIzaSyC_2QJAXgsS4PYdDwOvzDi2tb_N5hk2Ikk'  
# Prompt user for inputs
NUM_USERS = int(input("Enter the number of users to create: "))
print_info = input("Do you want to print out information for each user? (yes/no): ").strip().lower()
print_each_user_info = False
confirm_before_insert = False

if print_info == 'yes':
    print_each_user_info = True
    confirm_insert = input("Do you want to see the user's info before submitting? (yes/no): ").strip().lower()
    if confirm_insert == 'yes':
        confirm_before_insert = True

def calculate_offset(direction):
    base_angle = direction_to_angle[direction]
    angle_range = math.pi / 4  # 45 degrees in radians
    theta = (random.uniform(base_angle - angle_range / 2, base_angle + angle_range / 2)) % (2 * math.pi)
    # Generate a random radius with uniform distribution within the circle
    r = MAX_OFFSET_DEGREES * math.sqrt(random.uniform(0, 1))
    # Corrected: Convert polar coordinates to geographic offsets
    delta_lat = r * math.sin(theta)
    delta_lon = r * math.cos(theta)
    return delta_lat, delta_lon

def get_weekdays(start_date, end_date):
    start_dt = datetime.strptime(start_date, "%Y-%m-%d")
    end_dt = datetime.strptime(end_date, "%Y-%m-%d")
    delta = timedelta(days=1)
    weekdays = []
    while start_dt <= end_dt:
        if start_dt.weekday() < 5:  # Monday=0, Sunday=6
            weekdays.append(start_dt)
        start_dt += delta
    return weekdays

def reverse_geocode(lat, lon):
    try:
        url = f'https://maps.googleapis.com/maps/api/geocode/json?latlng={lat},{lon}&key={GOOGLE_MAPS_API_KEY}'
        response = requests.get(url)
        response.raise_for_status()  # Raises HTTPError for bad responses
        data = response.json()
        
        if data.get('status') == 'OK' and data.get('results'):
            return data['results'][0].get('formatted_address', 'Unknown Location')
        else:
            print(f"Geocoding error: {data.get('status')}")
            return 'Unknown Location'
    except requests.exceptions.HTTPError as http_err:
        print(f"HTTP error occurred: {http_err}")  # Print HTTP error
        return 'Unknown Location'
    except Exception as e:
        print(f"An error occurred: {e}")  # General exception
        return 'Unknown Location'

def create_test_users(num_users):
    created_user_ids = []  # List to store UserIDs of created users
    for _ in range(num_users):
        while True:
            user_id = -random.randint(1000, 9999)  # Negative user ID for testing
            try:
                # Assign a random direction
                direction = random.choice(DIRECTIONS)
                
                user_type = random.choice(["rider", "driver"])
                gender = random.choice(["Man", "Woman"])
                driving_distance = random.randint(5, 50)
                belt_count = random.randint(1, 6)
                allow_eat_drink = random.choice(["Yes", "No"])
                allow_smoke_vape = random.choice(["Yes", "No"])
                # Include the direction in the first name
                first_name = f"{direction}Test{abs(user_id)}"
                # Collect preference values
                gender_preference = random.choice(GENDER_VALUES)
                smoking_preference = random.choice(SMOKING_PREFERENCES)
                eating_preference = random.choice(EATING_PREFERENCES)
                temperature_preference = random.choice(TEMPERATURE_PREFERENCES)
                music_preference = "No Preference"
                # Map preferences to initials
                gender_initial = gender_initials[gender_preference]
                smoking_initial = smoking_initials[smoking_preference]
                eating_initial = eating_initials[eating_preference]
                temperature_initial = temperature_initials[temperature_preference]
                # Build the initials string
                preferences_initials = f"{gender_initial}-{smoking_initial}-{eating_initial}-{temperature_initial}"
                # Include the initials in the last name
                last_name = f"LastName{abs(user_id)}-{preferences_initials}"
                email = f"{first_name.lower()}{random.randint(1000,9999)}@test.com"

                pickup_offset_lat, pickup_offset_lon = calculate_offset(direction)
                pickup_lat = CAMPUS_LATITUDE + pickup_offset_lat
                pickup_lon = CAMPUS_LONGITUDE + pickup_offset_lon

                # Reverse geocode the pickup location
                pickup_location = reverse_geocode(pickup_lat, pickup_lon)
                # Reverse geocode the dropoff location (campus)
                dropoff_location = CAMPUS_ADDRESS
                # Assume UserLocation is the same as PickupLocation for this example
                user_location = pickup_location

                if print_each_user_info:
                    print(f"Creating User ID: {user_id}")
                    print(f"First Name: {first_name}")
                    print(f"Last Name: {last_name}")
                    print(f"Email: {email}")
                    print(f"User Type: {user_type}")
                    print(f"Gender: {gender}")
                    print(f"Direction: {direction}")
                    print(f"Driving Distance: {driving_distance} km")
                    print(f"Belt Count: {belt_count}")
                    print(f"Allow Eat/Drink: {allow_eat_drink}")
                    print(f"Allow Smoke/Vape: {allow_smoke_vape}")
                    print("Preferences:")
                    print(f"  Gender Preference: {gender_preference}")
                    print(f"  Smoking Preference: {smoking_preference}")
                    print(f"  Eating Preference: {eating_preference}")
                    print(f"  Temperature Preference: {temperature_preference}")
                    print(f"  Music Preference: {music_preference}")
                    print("Location Information:")
                    print(f"  User Location: {user_location}")
                    print(f"  Pickup Latitude: {pickup_lat}")
                    print(f"  Pickup Longitude: {pickup_lon}")
                    print(f"  Pickup Location: {pickup_location}")
                    print(f"  Dropoff Latitude (Campus): {CAMPUS_LATITUDE}")
                    print(f"  Dropoff Longitude (Campus): {CAMPUS_LONGITUDE}")
                    print(f"  Dropoff Location: {dropoff_location}")

                if confirm_before_insert:
                    proceed = input("Do you want to insert this user into the database? (yes/no): ").strip().lower()
                    if proceed != 'yes':
                        print("User creation skipped.")
                        break  # Skip this user

                # Insert into Users table
                cursor.execute("""
                    INSERT INTO Users (UserID, Email, FirstName, LastName, UserType, Gender, DrivingDistance, BeltCount, AllowEatDrink, AllowSmokeVape, UserLocation, PickupLocation, DropoffLocation)
                    VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ? , ?)
                """, (user_id, email, first_name, last_name, user_type, gender, driving_distance, belt_count, allow_eat_drink, allow_smoke_vape, user_location, pickup_location, dropoff_location))
                conn.commit()

                # Insert into Preferences table
                cursor.execute("""
                    INSERT INTO Preferences (UserID, GenderPreference, EatingPreference, SmokingPreference, TemperaturePreference, MusicPreference)
                    VALUES (?, ?, ?, ?, ?, ?)
                """, (user_id, gender_preference, eating_preference, smoking_preference, temperature_preference, music_preference))
                conn.commit()

                # Insert into Locations table
                cursor.execute("""
                    INSERT INTO Locations (UserID, PickupLongitude, PickupLatitude, DropoffLongitude, DropoffLatitude)
                    VALUES (?, ?, ?, ?, ?)
                """, (user_id, pickup_lon, pickup_lat, CAMPUS_LONGITUDE, CAMPUS_LATITUDE))
                conn.commit()

                # Select one pickup time and one departure time for the user
                pickup_time = random.choice(PICKUP_TIMES)
                departure_time = random.choice(DEPARTURE_TIMES)

                # Get all weekdays within the date range
                weekdays = get_weekdays(START_DATE, END_DATE)

                if print_each_user_info:
                    print("Schedules:")

                for day_dt in weekdays:
                    date_str = day_dt.strftime("%Y-%m-%d")
                    day_name = day_dt.strftime("%A")

                    # Create Arrival schedule (morning)
                    start_time = pickup_time
                    end_time_dt = datetime.strptime(start_time, "%H:%M") + timedelta(minutes=30)
                    end_time = end_time_dt.strftime("%H:%M")
                    start_datetime = f"{date_str} {start_time}:00"
                    end_datetime = f"{date_str} {end_time}:00"
                    cursor.execute("""
                        INSERT INTO Schedules (UserID, Day, Start, End, Text)
                        VALUES (?, ?, ?, ?, ?)
                    """, (user_id, day_name, start_datetime, end_datetime, "Arrival"))

                    if print_each_user_info:
                        # Print arrival schedule
                        print(f"  Arrival on {day_name}, {date_str}: {start_time} - {end_time}")

                    # Create Departure schedule (afternoon)
                    start_time = departure_time
                    end_time_dt = datetime.strptime(start_time, "%H:%M") + timedelta(minutes=60)
                    end_time = end_time_dt.strftime("%H:%M")
                    start_datetime = f"{date_str} {start_time}:00"
                    end_datetime = f"{date_str} {end_time}:00"
                    cursor.execute("""
                        INSERT INTO Schedules (UserID, Day, Start, End, Text)
                        VALUES (?, ?, ?, ?, ?)
                    """, (user_id, day_name, start_datetime, end_datetime, "Departure"))

                    if print_each_user_info:
                        # Print departure schedule
                        print(f"  Departure on {day_name}, {date_str}: {start_time} - {end_time}")

                conn.commit()

                if print_each_user_info:
                    print("-" * 50)  # Separator between users

                # Add the UserID to the list
                created_user_ids.append(user_id)

                break  # Exit loop on successful insertion
            except sqlite3.IntegrityError as e:
                print(f"Error inserting user {user_id}: {e}")
                # Retry with a new UserID if a conflict occurs
                continue

    # After all users are created, write the UserIDs to 'created_users.csv'
    with open('created_users.csv', 'w', newline='') as csvfile:
        writer = csv.writer(csvfile)
        writer.writerow(['UserID'])  # Write header
        for user_id in created_user_ids:
            writer.writerow([user_id])

    print(f"Created {len(created_user_ids)} users. UserIDs saved to 'created_users.csv'.")

# Call the function
create_test_users(NUM_USERS)

# Close the connection
conn.close()
