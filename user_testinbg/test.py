import sqlite3
from datetime import datetime, timedelta

# Parameters
START_DATE = "2024-11-01"
END_DATE = "2024-12-31"

ARRIVAL_START_TIME = "09:00"
ARRIVAL_END_TIME = "10:00"

DEPARTURE_START_TIME = "16:00"
DEPARTURE_END_TIME = "17:00"

db_path = "Carpool.db"

# Connect to database
conn = sqlite3.connect(db_path)
cursor = conn.cursor()

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

# Get list of UserIDs
cursor.execute("SELECT UserID FROM Users")
user_ids = cursor.fetchall()  # This will be a list of tuples [(UserID1,), (UserID2,), ...]

# Convert list of tuples to list of UserIDs
user_ids = [17, 18, 19, 20 ,21, 22,23,24,25,26]

# Get list of weekdays between START_DATE and END_DATE
weekdays = get_weekdays(START_DATE, END_DATE)

for user_id in user_ids:
    for day_dt in weekdays:
        date_str = day_dt.strftime("%Y-%m-%d")
        day_name = day_dt.strftime("%A")
        
        # Arrival schedule
        start_time = ARRIVAL_START_TIME
        end_time = ARRIVAL_END_TIME
        start_datetime = f"{date_str} {start_time}:00"
        end_datetime = f"{date_str} {end_time}:00"
        cursor.execute("""
            INSERT INTO Schedules (UserID, Day, Start, End, Text)
            VALUES (?, ?, ?, ?, ?)
        """, (user_id, day_name, start_datetime, end_datetime, "Arrival"))
        
        # Departure schedule
        start_time = DEPARTURE_START_TIME
        end_time = DEPARTURE_END_TIME
        start_datetime = f"{date_str} {start_time}:00"
        end_datetime = f"{date_str} {end_time}:00"
        cursor.execute("""
            INSERT INTO Schedules (UserID, Day, Start, End, Text)
            VALUES (?, ?, ?, ?, ?)
        """, (user_id, day_name, start_datetime, end_datetime, "Departure"))

conn.commit()
conn.close()
