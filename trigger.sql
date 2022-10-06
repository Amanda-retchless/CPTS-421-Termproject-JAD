/*
 A
*/
-- Updating total_likes and tip count in user
CREATE OR REPLACE FUNCTION tipUpdate() RETURNS trigger AS '
BEGIN
    UPDATE Business
    SET numtips = Business.numtips + 1
    WHERE Business.business_id = NEW.business_id;
    UPDATE Users
    SET tipCount = Users.tipCount + 1
    WHERE Users.user_id = NEW.user_id;
    RETURN NEW;
END
' LANGUAGE plpgsql;

-- trigger
CREATE TRIGGER newTip
AFTER INSERT ON Tips
FOR EACH ROW
WHEN (NEW.user_id IS NOT NULL and NEW.business_id IS NOT NULL)
EXECUTE PROCEDURE tipUpdate();

-- test
/*INSERT INTO Tips VALUES ('djQLJTLA4Tx7TpzYCKIqJQ', '5KheTjYPu1HcQzQFtm4_vw', '2014-03-23', '21:32:49', 'Test Text', 20); -- TODO insert data
DELETE FROM Tips WHERE user_id = 'djQLJTLA4Tx7TpzYCKIqJQ' and business_id = '5KheTjYPu1HcQzQFtm4_vw' and tipText = 'Test Text';
SELECT * 
FROM Users 
WHERE user_id = 'djQLJTLA4Tx7TpzYCKIqJQ';
SELECT * 
FROM Business 
WHERE business_id = '5KheTjYPu1HcQzQFtm4_vw';*/

-- clean
--DROP TRIGGER newTip on Tips;

/*
 B
*/
-- Updating the checkin count
CREATE OR REPLACE FUNCTION checkinUpdate() RETURNS trigger AS '
BEGIN
    UPDATE Business
    SET numCheckins = Business.numCheckins + 1
    WHERE Business.business_id = NEW.business_id;
    RETURN NEW;
END
' LANGUAGE plpgsql;

-- trigger
CREATE TRIGGER newCheckin
AFTER INSERT ON Checkins
FOR EACH ROW
WHEN (NEW.business_id IS NOT NULL)
EXECUTE PROCEDURE checkinUpdate();

-- test
/*INSERT INTO Checkins VALUES ('5KheTjYPu1HcQzQFtm4_vw', '2022', '03', '25', '07:05:00'); 
DELETE FROM Checkins WHERE business_id = '5KheTjYPu1HcQzQFtm4_vw' and year = '2022' and month = '03';
SELECT * 
FROM Business 
WHERE business_id = '5KheTjYPu1HcQzQFtm4_vw';*/

-- clean
--DROP TRIGGER newCheckin on Checkins;

/*
 C
*/
-- total_likes
CREATE OR REPLACE FUNCTION likesUpdate() RETURNS trigger AS '
BEGIN
    UPDATE Users
    SET total_likes = Users.total_likes + NEW.likes - OLD.likes
    WHERE Users.user_id = NEW.user_id;
    RETURN NEW;
END
' LANGUAGE plpgsql;

-- trigger
CREATE TRIGGER newLike
AFTER Update ON Tips
FOR EACH ROW 
WHEN (NEW.user_id IS NOT NULL and NEW.likes - OLD.likes != 0)
EXECUTE PROCEDURE likesUpdate();

-- test
/*Update Tips
SET likes = likes + 1
Where Tips.user_id = 'jRyO2V1pA4CdVVqCIOPc1Q' and Tips.business_id = '5KheTjYPu1HcQzQFtm4_vw';
SELECT *
FROM Users
WHere Users.user_id = 'jRyO2V1pA4CdVVqCIOPc1Q';*/

-- clean
--DROP TRIGGER newLike on Tips;
