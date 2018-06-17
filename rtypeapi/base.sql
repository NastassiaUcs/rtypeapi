CREATE TABLE request (
    id           INTEGER PRIMARY KEY AUTOINCREMENT,
    request_text VARCHAR,
    ip           VARCHAR,
    body         VARCHAR
);