import sqlite3
import csv  # Import csv module to read CSV file

# Database connection
db_path = "Carpool.db"
conn = sqlite3.connect(db_path)
cursor = conn.cursor()

# Read UserIDs from 'created_users.csv'
def read_user_ids_from_csv(filename):
    user_ids = []
    try:
        with open(filename, 'r') as csvfile:
            reader = csv.reader(csvfile)
            next(reader)  # Skip header
            for row in reader:
                if row:
                    user_ids.append(int(row[0]))
        return user_ids
    except FileNotFoundError:
        print(f"File {filename} not found.")
        return []
    except Exception as e:
        print(f"An error occurred while reading {filename}: {e}")
        return []

def delete_users(user_ids):
    try:
        # Begin transaction
        conn.execute("BEGIN")
        
        # Convert list to tuple for SQL IN clause
        user_ids_tuple = tuple(user_ids)
        
        if not user_ids_tuple:
            print("No UserIDs provided to delete.")
            return

        # Delete from Schedules table
        cursor.execute(f"""
            DELETE FROM Schedules WHERE UserID IN ({','.join(['?']*len(user_ids_tuple))})
        """, user_ids_tuple)

        # Delete from Preferences table
        cursor.execute(f"""
            DELETE FROM Preferences WHERE UserID IN ({','.join(['?']*len(user_ids_tuple))})
        """, user_ids_tuple)

        # Delete from Locations table
        cursor.execute(f"""
            DELETE FROM Locations WHERE UserID IN ({','.join(['?']*len(user_ids_tuple))})
        """, user_ids_tuple)

        # Delete from Users table
        cursor.execute(f"""
            DELETE FROM Users WHERE UserID IN ({','.join(['?']*len(user_ids_tuple))})
        """, user_ids_tuple)

        # Commit transaction
        conn.commit()
        print(f"Deleted data for UserIDs: {user_ids} ({len(user_ids)})")
    except Exception as e:
        # Rollback transaction on error
        conn.rollback()
        print(f"An error occurred: {e}")

# Read UserIDs from 'created_users.csv'
user_ids_to_remove = read_user_ids_from_csv('created_users.csv')

# Call the function with the list of UserIDs
delete_users(user_ids_to_remove)

# Close the connection
conn.close()
