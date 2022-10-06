
--GLOSSARY
--table names
Business
Users
Tips
Friends
Checkins
catagories
Attributes
Store_Hours

--some attribute names
zipcode
business_id
city  (business city)
name   (business name)
user_id
friend_id
numtips
numCheckins

user_id
tipcount  (user)
totallikes (user)

tipdate
tiptext
likes  (tip)

Checkinsdate


--1.
SELECT COUNT(*) 
FROM  Business;
SELECT COUNT(*) 
FROM  Users;
SELECT COUNT(*) 
FROM  Tips;
SELECT COUNT(*) 
FROM  Friends;
SELECT COUNT(*) 
FROM  Checkins;
SELECT COUNT(*) 
FROM  catagories;
SELECT COUNT(*) 
FROM  Attributes;
SELECT COUNT(*) 
FROM  Store_Hours;



--2. Run the following queries on your business table, Checkins table and review table. Make sure to change the attribute names based on your schema. 

SELECT zipcode, COUNT(distinct C.catagory_name)
FROM Business as B, catagories as C
WHERE B.business_id = C.business_id
GROUP BY zipcode
HAVING count(distinct C.catagory_name)>300
ORDER BY zipcode;

SELECT zipcode, COUNT(distinct A.attr_name)
FROM Business as B, Attributes as A
WHERE B.business_id = A.business_id
GROUP BY zipcode
HAVING count(distinct A.attr_name) = 30;


SELECT Users.user_id, count(friend_id)
FROM Users, Friends
WHERE Users.user_id = Friends.user_id AND 
      Users.user_id = 'NxtYkOpXHSy7LWRKJf3z0w'
GROUP BY Users.user_id;


--3. Run the following queries on your business table, Checkins table and tips table. Make sure to change the attribute names based on your schema. 


SELECT business_id, business_name, business_city, numTips, numCheckins
FROM Business 
WHERE business_id ='K8M3OeFCcAnxuxtTc0BQrQ';

SELECT user_id, user_name, tipCount, total_likes
FROM Users
WHERE user_id = 'NxtYkOpXHSy7LWRKJf3z0w';

-----------

SELECT COUNT(*) 
FROM Checkins
WHERE business_id ='K8M3OeFCcAnxuxtTc0BQrQ';

SELECT count(*)
FROM Tips
WHERE  business_id = 'K8M3OeFCcAnxuxtTc0BQrQ';


--4. 
--Type the following statements. Make sure to change the attribute names based on your schema. 

SELECT business_id,business_name, business_city, numCheckins, numtips
FROM Business 
WHERE business_id ='hDD6-yk1yuuRIvfdtHsISg';

INSERT INTO Checkins (business_id, year, month, day, day_time)
VALUES ('hDD6-yk1yuuRIvfdtHsISg','2022','03','31','10:45:07');


--5.
--Type the following statements. Make sure to change the attribute names based on your schema.  

-- same as the above query in part4
SELECT business_id,business_name, business_city, numCheckins, numtips
FROM Business 
WHERE business_id ='hDD6-yk1yuuRIvfdtHsISg';

SELECT user_id, user_name, tipcount, total_likes
FROM Users
WHERE user_id = '3z1EttCePzDn9OZbudD5VA';


INSERT INTO Tips (user_id, business_id, tipdate, tiptime, tipText,likes)  
VALUES ('3z1EttCePzDn9OZbudD5VA','hDD6-yk1yuuRIvfdtHsISg', '2022-03-31', '13:00','EVERYTHING IS AWESOME',0);

UPDATE Tips 
SET likes = likes+1
WHERE user_id = '3z1EttCePzDn9OZbudD5VA' AND 
      business_id = 'hDD6-yk1yuuRIvfdtHsISg' AND 
      tipdate ='2022-03-31' AND
      tiptime = '13:00';

      
