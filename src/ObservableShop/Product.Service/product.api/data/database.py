import os
import psycopg2

def connect():
    endpoint = os.getenv("DB_HOST")
    port = os.getenv("DB_HOST_PORT")
    user = os.getenv("DB_USER")
    password = os.getenv("DB_PASS")
    database = os.getenv("DB_DATABASE")

    conn = psycopg2.connect(
        database=database, user=user, password=password, host=endpoint, port= port
    )
    conn.autocommit = True
    cursor = conn.cursor()
    return cursor, conn;

def isProductAvailable(product_id):
    try:
        cursor, conn = connect()
        query = "select stock from products where product_id = %s"        
        cursor.execute(query, (product_id,))
        row = cursor.fetchone()
        if row:
            return row[0] > 0
        return False
    except (Exception, psycopg2.Error) as error:
        print("Error while fetching data from PostgreSQL", error)
    finally:
        if conn:
            cursor.close()
            conn.close()
            print("PostgreSQL connection is closed")

def getPrice(product_id):
    try:
        cursor, conn = connect()
        query = "select price from products where product_id = %s"        
        cursor.execute(query, (product_id,))
        row = cursor.fetchone()
        if row:
            return row[0].replace("$", "")
        return -1
    except (Exception, psycopg2.Error) as error:
        print("Error while fetching data from PostgreSQL", error)
    finally:
        if conn:
            cursor.close()
            conn.close()
            print("PostgreSQL connection is closed")

def update_price(product_id, new_price):
    try:
        cursor, conn = connect()
        query = "update products set price = %s where product_id = %s"        
        cursor.execute(query, (new_price, product_id))
        updated_rows = cursor.rowcount        
        conn.commit()
        return updated_rows == 1
    except (Exception, psycopg2.Error) as error:
        print("Error while updating data in PostgreSQL", error)
    finally:
        if conn:
            cursor.close()
            conn.close()
            print("PostgreSQL connection is closed")

