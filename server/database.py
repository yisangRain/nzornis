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


def check_status(connection, user, ar):
    """
    Checks recorded status of the uploaded video
    user and ar_id must match
    Returns status as [string] | None
    """ 
    q = f"SELECT status, filename FROM AR WHERE user={user} AND ar_id={ar};"

    cursor = connection.cursor()
    cursor.execute(q)

    return cursor.fetchone()


def change_status(connection, user, ar, status):
    """
    Change the recorded status of the uploaded video
    user and ar_id must match
    Returns status as [string] | None
    """ 
    q = f"UPDATE AR SET status='{status}' WHERE user={user} AND ar_id={ar};"

    cursor = connection.cursor()
    cursor.execute(q)


def delete_database(connection, id):
    """
    Deletes from AR table based on the given ar_id
    Returns True if deleted successfully
    False otherwise
    """
    # Delete operation
    query = f"DELETE FROM AR WHERE ar_id={id};"
    cursor = connection.cursor()
    cursor.execute(query)
    connection.commit()

    # confirm deletion
    c_query = f"SELECT * FROM AR WHERE ar_id={id};"
    cursor.execute(c_query)
    if len(cursor.fetchall()) == 0:
        return True

    return False


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

        
