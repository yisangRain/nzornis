#####################################################################
# Database 
#####################################################################

import sqlite3
from sqlite3 import Error

def create_connection(db_file):
    # Creates connection to the SQLite database
    conn = None

    try:
        conn = sqlite3.connect(db_file)
        return conn
    
    except Error as e:
        print(e)


def query_database(connection, query):
    """
    Query the database using the given connection and query string
    """
    cursor = connection.cursor()

    cursor.execute(query)

    return cursor.fetchall()


def insert_database(connection, query):
    """
    Insert into the database using the given connection and query string
    """
    cursor = connection.cursor()

    cursor.execute(query)
    connection.commit()

    id = cursor.lastrowid

    if id != None:
        return id
    
    return ValueError


def setup_database(connection):
    # function to setup database if it's not ready

    cursor = connection.cursor()

    test_query = "SELECT * FROM AR LIMIT 1;"

    try:
        query_database(connection, test_query)
        print("Using existing database.")
    
    except Error as e:

        setup_query = "CREATE TABLE AR ( \
            AR_ID integer PRIMARY KEY AUTOINCREMENT, \
            user integer, \
            ar_name text, \
            position text, \
            filename text, \
            region integer, \
            geojson text, \
            time text, \
            description text, \
            status text \
            );"
        
        cursor.execute(setup_query)
        connection.commit()

        print("Database setup successful.")

        
