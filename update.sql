-- reset business
UPDATE business
SET numCheckins = 0, numTips = 0;

-- update business
UPDATE business
SET numCheckins = t1.ccnt, numTips = t2.tcnt
FROM
(
    SELECT business_id, COUNT(Checkins.business_id) as ccnt
    FROM Checkins
    GROUP BY Checkins.business_id
) as t1
,
(
    Select Tips.business_id, COUNT(Tips.user_id) AS tcnt
    FROM Tips 
    GROUP BY Tips.business_id
) as t2
WHERE business.business_id = t1.business_id and business.business_id = t2.business_id;

-- reset users
UPDATE Users
SET total_likes = 0, tipCount = 0;

-- update users
UPDATE Users
SET  tipCount = t1.tcnt, total_likes = lcnt
FROM  
(
    Select Tips.user_id, COUNT(Tips.user_id) AS tcnt, SUM(Tips.likes) as lcnt
    FROM Tips 
    GROUP BY Tips.user_id
) as t1
WHERE t1.user_id = Users.user_id;
