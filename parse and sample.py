#CptS 451 - Spring 2022
# https://www.psycopg.org/docs/usage.html#query-parameters

#  if psycopg2 is not installed, install it using pip installer :  pip install psycopg2  (or pip3 install psycopg2) 
import json
import psycopg2

def cleanStr4SQL(s):
    return s.replace("'","`").replace("\n"," ")

def int2BoolStr (value):
    if value == 0:
        return 'False'
    else:
        return 'True'

def insert2BusinessTable(conn):
    #reading the JSON file
    with open('./yelpdata/yelp_business.JSON','r') as f:    #TODO: update path for the input file
        line = f.readline()
        count_line = 0

        #connect to yelpdb database on postgres server using psycopg2
        # try:
        #     #TODO: update the database name, username, and password
        #     conn = psycopg2.connect("dbname='milestone2db' user='postgres' host='localhost' password='postgres'")
        # except:
        #     print('Unable to connect to the database!')
        cur = conn.cursor()

        while line:
            data = json.loads(line)
            try:
                cur.execute("INSERT INTO Business (business_id, business_name, business_address, business_city, business_state, zipcode, latitude, longitude, numTips, numCheckins, isOpen, stars)"
                       + " VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)", 
                         (data['business_id'],cleanStr4SQL(data["name"]), cleanStr4SQL(data["address"]), data["city"], data["state"], data["postal_code"], data["latitude"], data["longitude"], 0 , 0 , [False,True][data["is_open"]], data["stars"] ) )              
            except Exception as e:
                print("Insert to businessTABLE failed!",e)

            line = f.readline()
            count_line +=1
     #   conn.commit()

        cur.close()
     #   conn.close()

    print(count_line)
    f.close()

def insert2UserTable(conn):
    #reading the JSON file
    with open('./yelpdata/yelp_user.JSON','r') as f:    
        line = f.readline()
        count_line = 0

        #connect to yelpdb database on postgres server using psycopg2
        # try:
        #     #TODO: update the database name, username, and password
        #     conn = psycopg2.connect("dbname='milestone2db' user='postgres' host='localhost' password='postgres'")
        # except:
        #     print('Unable to connect to the database!')
        cur = conn.cursor()

        while line:
            data = json.loads(line)
            try:
                cur.execute("INSERT INTO Users (user_id, user_name, average_stars, fans, Cool, tipCount, funny, total_likes, useful, yelping_since, user_lat, user_long)"
                       + " VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)", 
                         (data['user_id'],cleanStr4SQL(data["name"]), data["average_stars"], data["fans"], data["cool"], data["tipcount"], data["funny"], 0, data["useful"] , data["yelping_since"], 0, 0) )              
            except Exception as e:
                print("Insert to userTABLE failed!",e)

            line = f.readline()
            count_line +=1
     #   conn.commit()

        cur.close()
    #    conn.close()

    print(count_line)
    f.close()

def insert2FriendsTable(conn):
    #reading the JSON file
    with open('./yelpdata/yelp_user.JSON','r') as f:    
        line = f.readline()
        count_line = 0

        #connect to yelpdb database on postgres server using psycopg2
        # try:
        #     #TODO: update the database name, username, and password
        #     conn = psycopg2.connect("dbname='milestone2db' user='postgres' host='localhost' password='postgres'")
        # except:
        #     print('Unable to connect to the database!')
        cur = conn.cursor()

        while line:
            data = json.loads(line)
            try:
                for x in data["friends"]:
                    cur.execute("INSERT INTO Friends (user_id, friend_id)"
                        + " VALUES (%s, %s)", 
                            (data['user_id'],cleanStr4SQL(x) ))              
            except Exception as e:
                print("Insert to friendsTABLE failed!",e)

            line = f.readline()
            count_line +=1

    #    conn.commit()
        cur.close()
    #    conn.close()

    print(count_line)
    f.close()

def insert2TipsTable(conn):
    #reading the JSON file
    with open('./yelpdata/yelp_tip.JSON','r') as f:    
        line = f.readline()
        count_line = 0

        #connect to yelpdb database on postgres server using psycopg2
        # try:
        #     #TODO: update the database name, username, and password
        #     conn = psycopg2.connect("dbname='milestone2db' user='postgres' host='localhost' password='postgres'")
        # except:
        #     print('Unable to connect to the database!')
        cur = conn.cursor()

        while line:
            data = json.loads(line)
            try:
                cur.execute("INSERT INTO Tips (user_id, business_id, tipDate, tipTime, tipText, likes)"
                    + " VALUES (%s, %s, %s, %s, %s, %s)", 
                        (data['user_id'],data['business_id'], cleanStr4SQL(data["date"][0:10]), cleanStr4SQL(data["date"][11:]), cleanStr4SQL(data["text"]), data["likes"]) )              
            except Exception as e:
                print("Insert to tipsTABLE failed!",e)

            line = f.readline()
            count_line +=1

    #    conn.commit()
        cur.close()
#        conn.close()

    print(count_line)
    f.close()

def insert2CheckinsTable(conn):
    #reading the JSON file
    with open('./yelpdata/yelp_checkin.JSON','r') as f:    
        line = f.readline()
        count_line = 0

        #connect to yelpdb database on postgres server using psycopg2
        # try:
        #     #TODO: update the database name, username, and password
        #     conn = psycopg2.connect("dbname='milestone2db' user='postgres' host='localhost' password='postgres'")
        # except:
        #     print('Unable to connect to the database!')
        cur = conn.cursor()

        while line:
            data = json.loads(line)
            try:    
                for x in data["date"].split(','):
                    x = str(x)

                    cur.execute("INSERT INTO Checkins (business_id, year, month, day, day_time)"
                        + " VALUES (%s, %s, %s, %s, %s)", 
                            (data['business_id'], cleanStr4SQL(x[0:4]), cleanStr4SQL(x[5:7]), cleanStr4SQL(x[8:10]), cleanStr4SQL(x[11:]) ))              
            except Exception as e:
                print("Insert to checkinsTABLE failed!",e)

            line = f.readline()
            count_line +=1
    #    conn.commit()
        cur.close()
    #    conn.close()

    print(count_line)
    f.close()

def insert2StoreHoursTable(conn):
    #reading the JSON file
    with open('./yelpdata/yelp_business.JSON','r') as f:    
        line = f.readline()
        count_line = 0

        #connect to yelpdb database on postgres server using psycopg2
        # try:
        #     #TODO: update the database name, username, and password
        #     conn = psycopg2.connect("dbname='milestone2db' user='postgres' host='localhost' password='postgres'")
        # except:
        #     print('Unable to connect to the database!')
        cur = conn.cursor()

        while line:
            data = json.loads(line)
            try:
                for x in data["hours"].keys():
                    cur.execute("INSERT INTO Store_Hours (business_id, daysofweek, close_state, open_state)"
                        + " VALUES (%s, %s, %s, %s)", 
                            (data['business_id'], cleanStr4SQL(x), cleanStr4SQL(data["hours"][x].split('-')[1]), cleanStr4SQL(data["hours"][x].split('-')[0])) )              
            except Exception as e:
                print("Insert to store_hoursTABLE failed!",e)

            line = f.readline()
            count_line +=1
   #     conn.commit()
        cur.close()
    #    conn.close()

    print(count_line)
    f.close()

def insert2AttributesTable(conn):
    #reading the JSON file
    with open('./yelpdata/yelp_business.JSON','r') as f:    
        line = f.readline()
        count_line = 0

        #connect to yelpdb database on postgres server using psycopg2
        # try:
        #     #TODO: update the database name, username, and password
        #     conn = psycopg2.connect("dbname='milestone2db' user='postgres' host='localhost' password='postgres'")
        # except:
        #     print('Unable to connect to the database!')
        cur = conn.cursor()

        while line:
            data = json.loads(line)
            try:
                for x,y in enumerate(data["attributes"]):
                    if type(data["attributes"][y]) is dict:
                        for a,b in enumerate(data["attributes"][y]):
                            cur.execute("INSERT INTO Attributes (business_id, attr_name, attr_value)"
                            + " VALUES (%s, %s, %s)", 
                                (data['business_id'], cleanStr4SQL(b), cleanStr4SQL(data["attributes"][y][b])) )  
                    else:
                        cur.execute("INSERT INTO Attributes (business_id, attr_name, attr_value)"
                            + " VALUES (%s, %s, %s)", 
                                (data['business_id'], cleanStr4SQL(y), cleanStr4SQL( data["attributes"][y])) )              
            except Exception as e:
                print("Insert to attributesTABLE failed!",e)

            line = f.readline()
            count_line +=1
 #       conn.commit()

        cur.close()
     #   conn.close()

    print(count_line)
    f.close()

def insert2CategoriesTable(conn):
    #reading the JSON file
    with open('./yelpdata/yelp_business.JSON','r') as f:    
        line = f.readline()
        count_line = 0

        #connect to yelpdb database on postgres server using psycopg2
        # try:
        #     #TODO: update the database name, username, and password
        #     conn = psycopg2.connect("dbname='milestone2db' user='postgres' host='localhost' password='postgres'")
        # except:
        #     print('Unable to connect to the database!')
        cur = conn.cursor()

        while line:
            data = json.loads(line)
            try:
                for x in data["categories"].split(', '):
                    cur.execute("INSERT INTO Catagories (business_id, catagory_name)"
                        + " VALUES (%s, %s)", 
                            (data['business_id'], cleanStr4SQL(str(x))) )              
            except Exception as e:
                print("Insert to categoriesTABLE failed!",e)

            line = f.readline()
            count_line +=1
#        conn.commit()

        cur.close()
    #    conn.close()

    print(count_line)
    f.close()

try:
    #TODO: update the database name, username, and password
    conn = psycopg2.connect("dbname='milestone2db' user='postgres' host='localhost' password='bitte'")
except:
    print('Unable to connect to the database!')

insert2BusinessTable(conn)
insert2UserTable(conn)
insert2FriendsTable(conn)
insert2TipsTable(conn)
insert2CheckinsTable(conn)
insert2StoreHoursTable(conn)
insert2AttributesTable(conn)
insert2CategoriesTable(conn)

conn.commit()
conn.close()
