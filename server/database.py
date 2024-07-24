#####################################################################
# Database
#####################################################################

import sqlite3
from sqlite3 import Error
from enum import Enum

"""
DAOs
"""
class Entity(Enum):
    COMMENT = "comment"
    SIGHTING = "sighting"
    USER = "user"


class User:
    def __init__(self, id, fname, lname, email, password):
        self.id = id
        self.fname = fname
        self.lname = lname
        self.email = email
        self.password = password


    def new_string(self):
        """
        Returns string to be placed inside the VALUES SQL for inserting new row into the 
        user table
        """
        return f"{self.fname}, {self.lname}, {self.email}, {self.password}"
    

    def check_email_exists(self, conn, email):
        """
        Checks if email is registered
        Returns True if registered
        """
        
        q = f"SELECT id FROM user WHERE email={email};"

        cursor = conn.cursor()
        cursor.execute(q)

        return len(cursor.fetchall()) > 0
    

    def create_user (self, conn):
        """
        Inserts new user into the User table and returns user id
        param: user --> class User()
        """
        q = f"INSERT INTO user (first_name, last_name, email, password) \
            VALUES ({self.new_string()});"
        
        check = f"SELECT id FROM user WHERE email = {self.email};"
        
        cursor = conn.cursor()
        cursor.execute(q)
        conn.commit()

        cursor.execute(check)

        return cursor.fetchone()[0]
    

    def delete_user (self, conn, target_id=None):
        """
        Deletes a row from the user table by given optional param target_id 
        or by self.id if target_id is None
        Returns True if successful
        """
        if target_id == None:
            target_id = self.id

        q = f"DELETE FROM user WHERE id={target_id};"

        cursor = conn.cursor()
        cursor.execute(q)
        conn.commit()

        check = f"SELECT * FROM user WHERE id={target_id};"
        cursor.execute(check)

        return len(cursor.fetchall()) > 0
    

    def edit_user(self):
        """Not implemented"""
        pass
        

class Comment:
    def __init__ (self, id, user, time, comment, sighting):
        self.id = id
        self.user = user
        self.time = time
        self.comment = comment
        self.sighting = sighting

    
    def get_comments(self, conn, entity, target_id = None):
        """
        Returns comments based on the given entity type
        Entity (enum) -> comment | sighting | user
        Returns empty list if none retrieved
        """
        if entity != Entity.COMMENT & target_id == None:
            raise NotImplementedError("Error: id required for non-comment targets.")
        
        q = "SELECT * FROM comment WHERE "

        if entity == Entity.COMMENT:
            if target_id == None:
                target_id = self.id
            q += f"id={target_id};"
        
        elif entity == Entity.SIGHTING:
            q += f"sighting_id={target_id};"
        
        elif entity == Entity.USER:
            q += f"user={target_id};"
        
        else:
            raise NotImplementedError(f"Error: {entity.value} not implemented.")
            
        cursor = conn.cursor()
        cursor.execute(q)

        return cursor.fetchall()
    

    def add_comment(self):
        """
        Inserts new comment into the comment table
        returns comment id
        """
        pass


    def delete_comment(self, target_id = None):
        """
        Deletes a row in the comment table based on the target id
        Uses self.id if target_id is None
        """
        pass


    def edit_comment(self, target_id = None, content = None):
        """
        Edits a row in the comment table based on the target id with the content
        Uses self.id and/or self.comment if targets_id and/or content are None
        """
        pass
    

class LatLon:
    def __init__ (self, lat, lon):
        self.lat = lat
        self.lon = lon


class Sighting:
    def __init__ (self):
        self.id = None
        self.title = None
        self.desc = None
        self.user = None
        self.time = None
        self.latlon = None
        self.path = None

    
    def get_by_id(self, entity, target_id = None):
        """
        Queries for sighting based on the target sighting id or user id.
        Uses self values if target_id is None
        """
        pass


    def delete(self, target_id = None):
        """
        Deletes target row within the sightings table
        Uses self values if target_id is None
        Returns True if success
        """
        pass


    def edit(self, title = None, desc = None, target_id = None):
        """
        Edits title and/or description of the target sighting
        Uses self.id if target_id is None.
        Edits nothing if no title or desc is given
        Returns True upon success
        """
        pass


    def new(self):
        """
        Inserts new row into the sighting and cell table
        """
        pass


class Grid:
    def __init__ (self, id, min_latlon, max_latlon):
        self.id = id
        self.min_latlon = min_latlon
        self.max_latlon = max_latlon


class Cell:
    def __init__ (self, id, code, sighting):
        self.id = id
        self.code = code
        self.sighting = sighting


class Connection:
    """
    Database handler class
    """
    def __init__ (self, dbname):
        self.dbname = dbname
    

    def create_connection (self):
        try: 
            return sqlite3.connect(self.dbname)
        except Error as e:
            return e
        


    













def get_filename(conn, id, ar_id):
    """
    Return filename if the ar_id and user matches
    """
    cursor = conn.cursor()
    query = f"SELECT filename FROM AR WHERE user='{id}' AND ar_id='{ar_id}';"

    cursor.execute(query)

    return cursor.fetchone()


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

        
