#####################################################################
# Database
#####################################################################

import sqlite3
from sqlite3 import Error
from enum import Enum
import json
from collections import namedtuple

"""
DAOs
"""
New_Sighting = namedtuple('NewSighting', ['title', 'desc', 'user', 'time', 'lon','lat', 'path'])

class Entity(Enum):
    COMMENT = "comment"
    SIGHTING = "sighting"
    USER = "user"


class ConversionStatus(Enum):
    RAW_IMAGE = 0
    RAW_VIDEO = 1
    CONVERTING = 2
    READY = 3


class User:
    def __init__(self, id: int, fname: str, lname: str, email: str, password: str):
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
    

    def check_email_exists(self, conn, email: str):
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
    def __init__ (self, user: int, time: int, comment: str, sighting: int):
        self.user = user
        self.time = time
        self.comment = comment
        self.sighting = sighting

    
    def get_comments(self, conn, entity: Entity, target_id):
        """
        Returns comments based on the given entity type
        Entity (enum) -> comment | sighting | user
        Returns empty list if none retrieved
        """
        q = "SELECT * FROM comment WHERE "

        if entity == Entity.COMMENT:
            q += f"id={target_id};"
        
        elif entity == Entity.SIGHTING:
            q += f"sighting_id={target_id};"
        
        elif entity == Entity.USER:
            q += f"user={target_id};"
          
        cursor = conn.cursor()
        cursor.execute(q)

        return cursor.fetchall()
    

    def add_comment(self, conn):
        """
        Inserts new comment into the comment table
        returns comment id
        Requires fully initialised self, minus the comment id
        """
        q = f"INSERT INTO comment (user, time, comment, sighting_id) \
            VALUES ({self.user}, {self.time}, {self.comment}, {self.sighting});"
        
        cursor = conn.cursor()

        cursor.execute(q)
        conn.commit()

        check = f"SELECT id FROM comment WHERE user={self.user} AND time={self.time} \
            AND sighting_id={self.sighting};"
        
        cursor.execute(check)

        return cursor.fetchone()[0]


    def delete_comment(self, conn, target_id: int):
        """
        Deletes a row in the comment table based on the target id
        Uses self.id if target_id is None
        """
        q = f"DELETE FROM comment WHERE id={target_id};"
        
        cursor = conn.cursor()

        cursor.execute(q)
        conn.commit()

        check = f"SELECT * FROM comment WHERE id = {target_id};"
        cursor.execute(check)

        return cursor.fetchone() is None



    def edit_comment(self, target_id: int, content: str | None):
        """
        Edits a row in the comment table based on the target id with the content
        Uses self.id and/or self.comment if targets_id and/or content are None
        """
        raise NotImplementedError
    

class LatLon:
    def __init__ (self, lat: float, lon: float):
        self.lat = lat
        self.lon = lon


class Sighting:
    
    def __init__(self, info: New_Sighting = None):
        self.title = None
        self.desc = None
        self.user = None
        self.time = None
        self.latlon = None
        self.path = None

        if info != None:
            self.title = info.title
            self.desc = info.desc
            self.user = info.user
            self.time = info.time
            self.latlon = LatLon(info.lat, info.lon)
            self.path = info.path
        

    
    def check_init(self):
        """
        Checks if all columns (except id) are not None
        """
        check = [self.title != None, self.desc != None, self.user != None, 
                 self.time != None, self.latlon != None, self.path != None]

        return all(check)

    
    def get_by_id(self, conn, entity: Entity, target_id: int):
        """
        Queries for sighting based on the target sighting id or user id.
        """
        q = "SELECT * FROM sighting WHERE "
        if entity == Entity.SIGHTING:
            q += f"id={target_id};"
        elif entity == Entity.USER:
            q += f"user={target_id};"
        else:
            raise NotImplementedError("Given entity type is not implemented for this function")

        cursor = conn.cursor()
        cursor.execute(q)

        return cursor.fetchall()
    

    def delete(self, conn, target_id: int):
        """
        Deletes target row within the sightings table
        Uses self values if target_id is None
        Returns True if success
        """
        q = f"DELETE FROM sighting WHERE id={target_id};"

        cursor = conn.cursor()
        cursor.execute(q)
        conn.commit()

        check = f"SELECT * FROM sighting WHERE id={target_id};"
        cursor.execute(check)

        return cursor.fetchone() is None


    def edit(self, conn, target_id: int, title = None | str, desc = None | str):
        """
        Edits title and/or description of the target sighting
        Uses self.id if target_id is None.
        Edits nothing if no title or desc is given
        Returns True upon success
        """
        if title==None & desc==None:
            raise ValueError("title and description cannot both be empty.")
        q = "UPDATE sighting SET "
        if title != None:
            q += f"title={title}, "
        if desc != None:
            q += f"description={desc} "
        q.rstrip(", ")

        q += f"WHERE id={target_id};"

        cursor = conn.cursor()
        cursor.execute(q)
        conn.commit()

        check = f"SELECT * FROM sighting WHERE id={target_id};"
        cursor.execute(check)
        result = cursor.fetchall()

        resultArray = []
        resultArray.append(len(result) == 1)

        if title != None:
            resultArray.append(resultArray[0][1] == title)
        
        if desc != None:
            resultArray.append(resultArray[0][2] == desc)
        
        return all(resultArray)
        


    def new_string(self):
        """
        Returns string to be placed inside the VALUES SQL for inserting new row into the sighting table
        """
        return f'"{self.title}", "{self.desc}", {self.user}, {self.time}, \
            {self.latlon.lon}, {self.latlon.lat}, "{self.path}"'


    def new(self, conn):
        """
        Inserts new row into the sighting and cell table
        Returns the id of the created row
        """
        if self.check_init() is False:
            raise AssertionError("sighting: Not all column fields are initialised. Please check.")

        q = f"INSERT INTO sighting (title, description, user, time, longitude, latitude, path) \
            VALUES ({self.new_string()});"

        cursor = conn.cursor()
        cursor.execute(q)

        check = f"SELECT id FROM sighting WHERE user={self.user} AND time={self.time};"

        cursor.execute(check)
        s = cursor.fetchone()[0]

        status = None
        if self.path.endswith(".mp4"):
            status = ConversionStatus.RAW_VIDEO
        else:
            status = ConversionStatus.RAW_IMAGE

        g = Grid().getGridCode(conn, self.latlon)
        Cell().new(conn, g, s, status)

        conn.commit()

        return s
    

    def toJson(self, results):
        """Converts given list of db results into json"""
        json_data = {}
        for i in range(results):
            item = results[i]
            data = {'id': item[0],
                    'title': item[1],
                    'description': item[2],
                    'user': item[3],
                    'time': item[4],
                    'longitude': item[5],
                    'latitude': item[6],
                    'type': item[-1]}
            json_data[i] = data
        return json.dumps(json_data).encode()


class Grid:

    def __init__(self):
        pass

    def getGridCode(self, conn, latlon: LatLon):
        q = f"SELECT id FROM grid WHERE min_latitude >= {latlon.lat} AND min_longitude <= {latlon.lon}\
             AND max_latitude < {latlon.lat} AND max_longitude > {latlon.lon};"
        
        cursor = conn.cursor()
        cursor.execute(q)

        return cursor.fetchone()[0]





class Cell:
    def __init__ (self, code = None | int, sighting = None | int):
        self.code = code
        self.sighting = sighting
   
    def check_status(self, conn, sighting_id: int):
        """
        return status of the given sighting
        """
        q = f"SELECT status FROM cell WHERE sighting_id={sighting_id};"

        cursor = conn.cursor()
        cursor.execute(q)

        return ConversionStatus(cursor.fetchone()[0])
    

    def update_status(self, conn, sighting_id: int, status: ConversionStatus):
        """
        update status of the given sighting
        """
        q = f"UPDATE cell SET status={status.value} WHERE sighting_id={sighting_id};"

        cursor = conn.cursor()
        cursor.execute(q)

    

    def get_all_by_grid(self, conn, code: int):
        """
        Query the cell table by the grid code
        """
        q = f"SELECT * FROM sighting at s LEFT JOIN (SELECT * FROM cell WHERE code={code}) as c \
            on s.id = c.sighting_id;"
        
        cursor = conn.cursor()
        cursor.execute(q)

        return cursor.fetchall()
    

    def delete(self, conn, sighting: int):
        """
        delete cell entry based on the sighting id
        """
        q = f"DELETE FROM cell WHERE sighting_id={sighting};"

        cursor = conn.cursor()
        cursor.execute(q)
        conn.commit()

        check = f"SELECT id FROM cell WHERE sighting_id={sighting};"
        cursor.execute(check)

        return cursor.fetchone() is None
    

    def new(self, conn, code:int, sighting:int, status: ConversionStatus):
        """
        Insert new entry into the cell table.
        return the cell id of the created
        """
        q = f"INSERT INTO cell (code, sighting_id, status) \
            VALUES ({code}, {sighting}, {status.value});"
        
        cursor = conn.cursor()
        cursor.execute(q)
        conn.commit()

        check = f"SELECT id FROM cell WHERE code={code} AND sighting_id={sighting};"
        cursor.execute(check)

        return cursor.fetchone()[0]


class Connection:
    """
    Database handler class
    """

    def __init__(self):
        pass

    def create_connection (self, dbname):
        try: 
            return sqlite3.connect(dbname)
        except Error as e:
            return e
        
    def magic(self, conn, q):
        """
        Magic query for dev purposes.
        """
        cursor = conn.cursor()
        cursor.execute(q)
        conn.commit()
        
