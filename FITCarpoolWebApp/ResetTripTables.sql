-- Delete data and reset auto-increment for table1
DELETE FROM CarpoolGroupTable;
DELETE FROM CarpoolGroupMemberTable;
DELETE FROM CarpoolTripMembers;
DELETE FROM CarpoolTrips;
DELETE FROM MapRecommendToReal;


UPDATE sqlite_sequence SET seq = 0 WHERE name = 'CarpoolGroupTable';
UPDATE sqlite_sequence SET seq = 0 WHERE name = 'CarpoolTrips';