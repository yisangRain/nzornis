import sqlite3

DB_NAME = "NZORNIS"

def initialize_database():
    # Connect to the database (or create it if it doesn't exist)
    conn = sqlite3.connect(DB_NAME)
    cursor = conn.cursor()

    # Define SQL commands to create tables
    create_table_1 = """
    CREATE TABLE IF NOT EXISTS table1 (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        name TEXT NOT NULL,
        age INTEGER NOT NULL
    );
    """

    create_table_2 = """
    CREATE TABLE IF NOT EXISTS table2 (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        address TEXT NOT NULL,
        phone_number TEXT NOT NULL
    );
    """

    create_table_3 = """
    CREATE TABLE IF NOT EXISTS table3 (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        product_name TEXT NOT NULL,
        price REAL NOT NULL
    );
    """

    # Execute the SQL commands to create the tables
    cursor.execute(create_table_1)
    cursor.execute(create_table_2)
    cursor.execute(create_table_3)

    # Commit the changes
    conn.commit()

    # Close the connection
    conn.close()


