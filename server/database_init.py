"""
Simple database for the NZOrnis.
Notes:
    - No security measures have been implemented for the user data. No hashing.
    - For development purposes only. Not to be used for any production (live) application.
"""
import sqlite3
import datetime
from time import mktime

DB_NAME = "NZORNIS"
GRID_SPLIT = 5 #how many cells in both xy 
#[(top left), (bottom right)] lat = y, lon = x
BOUNDARY = [(-43.230, 172.138), (-43.905, 173.161) ]


def initialise_database(database_name):
    # Connect to the database (or create it if it doesn't exist)
    conn = sqlite3.connect(database_name)
    cursor = conn.cursor()

    # Query to initialise tables
    query_tables = """
        CREATE TABLE IF NOT EXISTS user (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            first_name TEXT NOT NULL,
            last_name TEXT NOT NULL,
            email TEXT NOT NULL,
            password TEXT NOT NULL
        );
        CREATE TABLE IF NOT EXISTS sighting (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            title TEXT NOT NULL,
            description TEXT,
            user INTEGER NOT NULL,
            time INTEGER NOT NULL,
            longitude REAL NOT NULL,
            latitude REAL NOT NULL,
            path TEXT NOT NULL,
            FOREIGN KEY(user) REFERENCES user(id)
        );
        CREATE TABLE IF NOT EXISTS comment (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            user INTEGER NOT NULL,
            time INTEGER NOT NULL,
            comment TEXT NOT NULL,
            sighting_id INTEGER NOT NULL,
            FOREIGN KEY(user) REFERENCES user(id),
            FOREIGN KEY(sighting_id) REFERENCES sighting(id)
        );
        CREATE TABLE IF NOT EXISTS cell (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            code INTEGER NOT NULL,
            sighting_id INTEGER NOT NULL,
            FOREIGN KEY(sighting_id) REFERENCES sighting(id)
            FOREIGN KEY(code) REFERENCES grid(id)
        );
        CREATE TABLE IF NOT EXISTS grid (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            min_latitude REAL NOT NULL,
            min_longitude REAL NOT NULL,
            max_latitude REAL NOT NULL,
            max_longitude REAL NOT NULL
        );
    """
    # NB: time to be saved as UNIX integer as SQLite does not support datetime
    # REAL is float in SQLite

    cursor.execute(query_tables)

    # Commit the changes
    conn.commit()

    # Close the connection
    conn.close()


def insert_test_accounts(database_name):
    """
    Insert test accounts.
    Run only when test accounts are required.
    """
    # Connect to the database (or create it if it doesn't exist)
    conn = sqlite3.connect(database_name)
    cursor = conn.cursor()

    # Create 5 test accounts
    query = """
        INSERT INTO user (first_name, last_name, email, password) 
        VALUES ('April', 'Adams', 'test1@test.com', 'test'),
            ('Ben', 'Begins', 'test2@test.com', 'test'),
            ('Chris', 'Chen', 'test3@test.com', 'test'),
            ('Dan', 'Dao', 'test4@test.com', 'test'),
            ('Elise', 'Evans', 'test5@test.com', 'test');
    """

    cursor.execute(query)
    conn.commit()
    conn.close()


def insert_test_grid(database_name, gps_bounds):
    """
    Grid table populator
    Range limited to the greater Christchurch area for development
    """
    # query builder
    query = "INSERT INTO grid (min_latitude, min_longitude, max_latitude, max_longitude) VALUES "

    lat_inc = round((gps_bounds[1][0] - gps_bounds[0][0]) / 5, 5)
    lon_inc = round((gps_bounds[1][1] - gps_bounds[0][1]) / 5, 5)

    for i in range(GRID_SPLIT):
        min_lon = gps_bounds[0][1] + (lon_inc * i)
        max_lon = min_lon + lon_inc
        for j in range(GRID_SPLIT):
            min_lat = gps_bounds[0][0] + (lat_inc * j)
            max_lat = min_lat + lat_inc
            query += f"({min_lat}, {min_lon}, {max_lat}, {max_lon}), "

    query = query.rstrip(", ")
    query += ";"
    print(query) 

    conn = sqlite3.connect(database_name)
    cursor = conn.cursor()

    cursor.execute(query)
    conn.commit()
    conn.close()

    
def insert_test_sightings(database_name):
    """
    Insert mock sightings for test and development purposes
    """
    # UNIX timestamp
    current_time = int(mktime(datetime.datetime.now().timetuple()))

    coords = [(-43.52048, 172.58301), (-43.52134, 172.58395), (-43.52016, 172.58252), (-43.52048, 172.58301), (-43.51950, 172.56664)]

    query_1 = f"INSERT INTO sighting (title, description, user, time, longitude, latitude, path) \
    VALUES ('HIT Lab', 'Workspace', 1, {current_time}, {coords[0][0]}, {coords[0][1]}, 'server/testAssets/bird_1.jpg'), \
        ('Rata foyer', 'By Nuts & Bolts', 2, {current_time - 120}, {coords[1][0]}, {coords[1][1]}, 'server/testAssets/bird_2.jpg'), \
        ('Fire station', 'Outside the fire station', 3, {current_time - 600}, {coords[2][0]}, {coords[2][1]}, 'server/testAssets/blob.mp4'), \
        ('Another HIT Lab', 'Overlap', 4, {current_time - 20}, {coords[3][0]}, {coords[3][1]}, 'server/testAssets/blob.mp4'), \
        ('Far away', 'somewhere', '1', {current_time- 1200}, {coords[4][0]}, {coords[4][1]}, 'server/testAssets/blob.mp4');"
    
    print(query_1)
     
    conn = sqlite3.connect(database_name)
    cursor = conn.cursor()

    cursor.execute(query_1)
    conn.commit()
    print(query_1)

    grid_ids = []

    # update the cell table
    query_2 = "INSERT INTO cell (code, sighting_id) \
        VALUES "
    
    # Uncomment below for generating query to query for the grid id per test sighting entry
    # All default tests belong to grid 13
    # i = 1
    # for lat, lon in coords:
    #     c_query = f"SELECT id FROM grid WHERE \
    #         min_latitude >= {lat} \
    #         AND min_longitude <= {lon} \
    #         AND max_latitude < {lat} \
    #         AND max_longitude > {lon};"
    #     print(c_query)
    #     cursor.execute(c_query)
    #     grid_ids.append((i, cursor.fetchone()[0]))
    # for s, g in grid_ids:
    #     query_2 += f"({g}, {s}), "

    # comment out below if using dynamic generation
    for i in range(1,6):
        query_2 += f"(13, {i}),"
    
    query_2 = query_2.rstrip(", ")
    query_2 += ";"

    print(query_2)
    cursor.execute(query_2)
    conn.commit()
    conn.close()


def initialise():
    initialise_database(DB_NAME)
    insert_test_accounts(DB_NAME)
    insert_test_grid(DB_NAME, BOUNDARY)
    insert_test_sightings(DB_NAME)
    print("Database initialisation successful.")

