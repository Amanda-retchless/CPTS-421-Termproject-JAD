CREATE TABLE Business (
    business_id VARCHAR,
    business_name varchar, 
    business_address varchar,
    business_city varchar, 
    business_state varchar, 
    zipcode INTEGER,
    latitude float,
    longitude float,
    numTips INTEGER,
    numCheckins INTEGER,
    isOpen BOOLEAN,
    stars FLOAT,
    PRIMARY KEY (business_id)
);

CREATE TABLE Users (
    user_id VARCHAR,
    user_name VARCHAR,
    average_stars Float,
    fans Integer,
    Cool Integer,
    tipCount Integer,
    funny Integer,
    total_likes Integer,
    useful Integer,
    yelping_since DATE,
    user_lat float,
    user_long float,
    PRIMARY KEY (user_id)
);

CREATE TABLE Friends (
    friend_id VARCHAR,
    user_id VARCHAR, 
    PRIMARY KEY (user_id, friend_id),
    FOREIGN KEY (user_id) REFERENCES Users(user_id)
);

CREATE TABLE Tips (
    user_id VARCHAR,
    business_id VARCHAR,
    tipDate DATE,
    tipTime TIME,
    tipText VARCHAR,
    likes Integer,
    PRIMARY KEY(user_id, business_id, tipDate, tipTime, tipText, likes),
    FOREIGN KEY(user_id) REFERENCES Users(user_id),
    FOREIGN KEY(business_id) REFERENCES Business(business_id)
);

CREATE TABLE Checkins (
    business_id VARCHAR,
    year VARCHAR,
    month VARCHAR,
    day VARCHAR,
    day_time TIME,
    PRIMARY KEY (business_id, year, month, day, day_time),
    FOREIGN KEY (business_id) REFERENCES Business(business_id)
);

CREATE TABLE Store_Hours (
    business_id VARCHAR,
    daysofweek VARCHAR,
    close_state VARCHAR,
    open_state VARCHAR,
    PRIMARY KEY (business_id, daysofweek),
    FOREIGN KEY (business_id) REFERENCES Business(business_id)
);

CREATE TABLE Attributes (
    business_id VARCHAR,
    attr_name VARCHAR,
    attr_value VARCHAR,
    PRIMARY KEY (business_id, attr_name),
    FOREIGN KEY (business_id) REFERENCES Business(business_id)
);

CREATE TABLE catagories (
    business_id VARCHAR,
    catagory_name VARCHAR,
    PRIMARY KEY (business_id, catagory_name),
    FOREIGN KEY (business_id) REFERENCES Business(business_id)
);
