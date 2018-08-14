CREATE TABLE request (
    id           INTEGER PRIMARY KEY AUTOINCREMENT,
    request_text VARCHAR,
    ip           VARCHAR,
    body         VARCHAR,
    date         VARCHAR
);

CREATE TABLE names (        
    ip           VARCHAR,
    name         VARCHAR,
    date datetime default current_timestamp
);