using DataAccessLibrary.Model;
using DataAccessLibrary.Model.Logic_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data.Database
{
    public class GroupRecomendationData : IGroupRecommendationData
    {
        private readonly ISQLDataAccess _db;
        private readonly IUsersData _dbUsers;
        public GroupRecomendationData(ISQLDataAccess db, IUsersData usersData)
        {
            _db = db;
            _dbUsers = usersData;
        }
        public async Task<TripModel> GetTripWithMems(int TripID)
        {
            string query = @"select  CT.ID, CT.DriverID, CT.GroupID, CT.Start, CT.End, CGT.Direction, CT.Status
                                from CarpoolTrips CT
                                JOIN CarpoolGroupTable CGT on CT.GroupID = CGT.ID
                                WHERE CT.ID = @TripID";
            List<TripModel> tripModels = await _db.LoadData<TripModel, dynamic>(query, new { TripID });
            if (tripModels.Any())
            {
                TripModel Trip = tripModels.First();
                string UserQueryConfirmed = @"select UserID from CarpoolTripMembers where TripID = @TripID and Status = @Status";
                List<int> ConfiremdUserIDs = await _db.LoadData<int, dynamic>(UserQueryConfirmed, new { TripID , Status = "Confirmed"});
                string UserQueryPending = @"select UserID from CarpoolTripMembers where TripID = @TripID and Status = @Status";
                List<int> PendingUserIDs = await _db.LoadData<int, dynamic>(UserQueryPending, new { TripID, Status = "Pending" });
                string UserQueryDeclined= @"select UserID from CarpoolTripMembers where TripID = @TripID and Status = @Status";
                List<int> DeclinedUserIDs = await _db.LoadData<int, dynamic>(UserQueryDeclined, new { TripID, Status = "Declined" });
                Trip.ConfirmedUsers = await _dbUsers.GetListUserInfoModel(ConfiremdUserIDs);
                Trip.PendingUsers = await _dbUsers.GetListUserInfoModel(PendingUserIDs);
                Trip.DeclinedUsers = await _dbUsers.GetListUserInfoModel(DeclinedUserIDs);


                return Trip;
            }
            else
            {
                return null;
            }

        }

        public async Task<List<RecomendedGroup>> GetRecommendedGroupsForTimePeriod(Week week)
        {
            // SQL query to select groups within the specified time period
            string sql = @"
                        SELECT 
                            GR.GroupID,
                            GR.GroupName,
                            GR.Start AS StartWindow,
                            GR.End AS EndWindow,
                            GR.Direction,
                            GR.RecurringPattern,
                            GR.IsRecurring,
                            GR.ActiveTimeSlots AS ActiveTimeSlotsSerialized
                        FROM GroupRecomendation GR
                        WHERE Start <= @EndDate AND End >= @StartDate";

            var parameters = new { StartDate = week.StartDate, EndDate = week.EndDate };

            // Load groups from the database
            var groups = await _db.LoadData<RecomendedGroup, dynamic>(sql, parameters);

            if (!groups.Any())
            {
                return new List<RecomendedGroup>();
            }

            // Deserialize ActiveTimeSlots and collect GroupIDs
            foreach (var group in groups)
            {
                if (!string.IsNullOrEmpty(group.ActiveTimeSlotsSerialized))
                {
                    group.ActiveTimeSlots = group.ActiveTimeSlotsSerialized.Split(',')
                        .Select(s => DateTime.Parse(s))
                        .ToList();
                }
            }
            return await GetGroupMembersForList(groups);

        }
        public async Task<List<RecomendedGroup>> GetGroupMembersForList(List<RecomendedGroup> memberLessGroups)
        {

            for (int i = 0; i < memberLessGroups.Count; i++)
            {
                memberLessGroups[i] = await GetGroupMembers(memberLessGroups[i]);
            }
            return memberLessGroups;
        }
        public async Task<RecomendedGroup> GetGroupMembers(RecomendedGroup memberLessGroup)
        {
            // SQL query to get all group members for the collected GroupIDs
            string sqlMembers = @"
                        SELECT  UserID
                        FROM GroupRecomendationMembership
                        WHERE GroupID = @groupID";
            List<int> memberIDs = await _db.LoadData<int, dynamic>(sqlMembers, new { groupID = memberLessGroup.GroupID });
            memberLessGroup.GroupMembers = await _dbUsers.GetListUserInfoModel(memberIDs);
            return memberLessGroup;
        }
        public async Task DeleteRecommendedGroups(List<RecomendedGroup> groupsToDelete)
        {
            // Loop through each group in the provided list
            foreach (var group in groupsToDelete)
            {
                // Delete from the GroupRecomendationMembership table first
                string deleteMembershipSql = @"
            DELETE FROM GroupRecomendationMembership
            WHERE GroupID = @GroupID";

                await _db.SaveData(deleteMembershipSql, new { GroupID = group.GroupID });

                // Delete from the GroupRecomendation table
                string deleteGroupSql = @"
            DELETE FROM GroupRecomendation
            WHERE GroupID = @GroupID";

                await _db.SaveData(deleteGroupSql, new { GroupID = group.GroupID });
            }
        }
        public async Task InsertRecommendedGroups(RecomendedGroup recomendedGroup)
        {
            string sql = @"
                            INSERT INTO GroupRecomendation 
                            (GroupName, CreateDate, Start, End, Direction, RecurringPattern, IsRecurring, ActiveTimeSlots) 
                            VALUES 
                            (@GroupName, CURRENT_TIMESTAMP, @StartWindow, @EndWindow, @Direction, @RecurringPattern, @IsRecurring, @ActiveTimeSlots)";

            // Serialize ActiveTimeSlots to a string (e.g., JSON) for storage
            string activeTimeSlotsSerialized = string.Join(",", recomendedGroup.ActiveTimeSlots.Select(ts => ts.ToString("o")));
            var parameters = new
            {
                GroupName = recomendedGroup.GroupName,
                StartWindow = recomendedGroup.StartWindow.ToString("o"), // ISO 8601 format for consistency
                EndWindow = recomendedGroup.EndWindow.ToString("o"),
                Direction = recomendedGroup.Direction,
                RecurringPattern = recomendedGroup.RecurringPattern,
                IsRecurring = recomendedGroup.IsRecurring.Equals("Yes") ? "Yes" : "No",
                ActiveTimeSlots = activeTimeSlotsSerialized
            };
            int GroupID = await _db.SaveDataAndGetLastId(sql, parameters);
            foreach (var i in recomendedGroup.GroupMembers)
            {
                string sqlGroupMem = @"
                            INSERT INTO GroupRecomendationMembership 
                            (GroupID, UserID) 
                            VALUES 
                            (@Groupid, @Userid)";
                await _db.SaveData(sqlGroupMem, new { Userid = i.UserID, Groupid = GroupID });
            }

        }
        public async Task InsertRecommendedGroups(List<RecomendedGroup> recomendedGroups)
        {
            foreach (var i in recomendedGroups)
            {
                await InsertRecommendedGroups(i);
            }
        }
        public async Task<List<RecomendedGroup>> GetUsersUpcomingRecommendations(int userID)
        {
            var today = DateTime.Today;
            var startDate = today.AddDays(((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7);
            var endDate = startDate.AddDays(5); // Friday after Monday
            string sql = @"SELECT DISTINCT
                                GR.GroupID,
                                GR.GroupName,
                                GR.Start AS StartWindow,
                                GR.End AS EndWindow,
                                GR.Direction,
                                GR.RecurringPattern,
                                GR.IsRecurring,
                                GR.ActiveTimeSlots AS ActiveTimeSlotsSerialized,
	                            GRm.UserID
                            FROM GroupRecomendation GR
                                LEFT JOIN GroupRecomendationMembership GRM ON GR.GroupID = GRM.GroupID
                                LEFT JOIN MapRecommendToReal MRR ON GR.GroupID = MRR.RecommendGroupID
                                LEFT JOIN CarpoolGroupMemberTable CGMT ON MRR.CreatedGroupID = CGMT.GroupID
                            WHERE GRM.UserID = @Userid AND Start <= @EndDate   AND NOT EXISTS ( SELECT 1
                                                                                                FROM CarpoolGroupMemberTable CGMT_Sub
                                                                                                    JOIN MapRecommendToReal MRR_Sub ON CGMT_Sub.GroupID = MRR_Sub.CreatedGroupID
                                                                                                WHERE MRR_Sub.RecommendGroupID = GR.GroupID AND CGMT_Sub.UserID = GRM.UserID);";
            // add back if needed to show only the next week and not before  AND End >= @StartDate;
            var parameters = new { StartDate = startDate, EndDate = endDate, Userid = userID };
            var groups = await _db.LoadData<RecomendedGroup, dynamic>(sql, parameters);
            // Deserialize ActiveTimeSlots and collect GroupIDs
            foreach (var group in groups)
            {
                if (!string.IsNullOrEmpty(group.ActiveTimeSlotsSerialized))
                {
                    group.ActiveTimeSlots = group.ActiveTimeSlotsSerialized.Split(',')
                        .Select(s => DateTime.Parse(s))
                        .ToList();
                }
            }
            groups = await GetGroupMembersForList(groups);
            return groups;

        }
        public async Task AcceptGroupRec(RecomendedGroup group, int UserID)
        {
            string sql = @"select CreatedGroupID from MapRecommendToReal where RecommendGroupID = @RecGroupID"; 
            var groupIDs = await _db.LoadData<int, dynamic>(sql, new {RecGroupID = group.GroupID});
            if(groupIDs == null || groupIDs.Count() == 0)
            {
                // Group doesn't exist yet 
                await CreateRealGroupFromRec(group, UserID);
            }
            else
            {
                // Group Already Exists just add user to the trips and to the group 
                int GroupID = groupIDs.FirstOrDefault();
                await AddMemberToGroup(UserID, GroupID);
                await AddMemberAsPending(UserID, GroupID);
            }


        }
        public async Task CreateRealGroupFromRec(RecomendedGroup group, int UserID)
        {
            // Insert group into the groups table 
            string sql = @"INSERT INTO CarpoolGroupTable (Name, Direction, CreateDate) Values (@name, @direction, CURRENT_TIMESTAMP)";
            var parameters = new { name = group.GroupName, direction = group.Direction };
            int GroupId = await _db.SaveDataAndGetLastId(sql, parameters);
            // Add Group mem that intiaed to that group 
            await AddMemberToGroup(UserID, GroupId);
            // Create Trips for each time slots  
            foreach (DateTime TripStart in group.ActiveTimeSlots)
            {
                string TripSQL = @"INSERT INTO CarpoolTrips (GroupID, Start, End, Status) Values (@GroupId, @TripStart, @TripEnd, @Status)";
                var TripParameters = new { GroupId, TripStart, TripEnd = TripStart.AddHours(1) , Status = "Not Started"};
                int TripID = await _db.SaveDataAndGetLastId(TripSQL, TripParameters);
            }
            // Add Group Creator as pending invite 
            await AddMemberAsPending(UserID, GroupId);
            string sqlInsertMapping = @"INSERT INTO MapRecommendToReal VALUES (@RecGroupID, @NewGroupID);";
            await _db.SaveData(sqlInsertMapping, new {RecGroupID = group.GroupID, NewGroupID = GroupId});
        }
        public async Task AddMemberToGroup(int UserID, int GroupID)
        {
            string sql = @"INSERT INTO CarpoolGroupMemberTable (GroupID, UserID) Values (@GroupID, @UserID)";
            var parameters = new { GroupID, UserID};
            await _db.SaveData(sql, parameters);
        }
        // Adds the member as pending for each trip 
        public async Task AddMemberAsPending(int UserID, int GroupID)
        {
            string sqlGetTripIds = @"select ID from CarpoolTrips where GroupID = @GroupID";
            List<int> TripIDs = await _db.LoadData<int, dynamic>(sqlGetTripIds, new { GroupID });
            foreach (int TripID in TripIDs)
            {
                string sqlInsert = "INSERT INTO CarpoolTripMembers (TripID, UserID, Status) Values (@TripID, @UserID, @Status)";
                var para = new { TripID, UserID, Status = "Confirmed" };
                await _db.SaveData(sqlInsert, para);
            }
        }
        public async Task DeclineGroupRec(RecomendedGroup group, int UserID)
        {
            string query = @"
            DELETE FROM GroupRecomendationMembership
            WHERE GroupID = @GroupID AND UserID = @UserID";
            await _db.SaveData(query, new { GroupID = group.GroupID, UserID = UserID });
        }
        public async Task<List<TripModel>> GetTripModelsForHomePage(int UserID)
        {
            List<TripModel> tripModels = new List<TripModel>();
            string query = @"select  CT.ID, CT.DriverID, CT.GroupID, CT.Start, CT.End, CGT.Direction, CT.Status, CTM.Status as UserTripStatus
                                from CarpoolTrips CT
                                JOIN CarpoolTripMembers CTM on CT.ID = CTM.TripID
                                JOIN CarpoolGroupTable CGT on CT.GroupID = CGT.ID
                                WHERE CTM.UserId = @UserID";
            tripModels = await _db.LoadData<TripModel,dynamic>(query, new { UserID = UserID });
            return tripModels;
        }
        public async Task ConfirmTripAsRider(TripModel trip, int userID)
        {
            string query = @"
                            UPDATE CarpoolTripMembers
                            SET Status = 'Confirmed'
                            WHERE TripID = @TripID AND UserID = @UserID;";
            await _db.SaveData(query, new { TripID = trip.ID, UserID = userID });
        }
        public async Task ConfirmTripAsDriver(TripModel trip, int userID)
        {

            await ConfirmTripAsRider(trip, userID);

            if (trip.DriverID == null)
            {
               
                string query = @"
                                UPDATE CarpoolTrips
                                SET DriverID = @UserID
                                WHERE ID = @TripID;";
                await _db.SaveData(query, new { TripID = trip.ID, UserID = userID });
            }
            else
            {
                int LeastDrivenID = await LeastMilesDriver(trip.DriverID.Value, userID);
                if(LeastDrivenID == userID)
                {
                    string query = @"
                                UPDATE CarpoolTrips
                                SET DriverID = @UserID
                                WHERE ID = @TripID;";
                    await _db.SaveData(query, new { TripID = trip.ID, UserID = userID });
                }
            }


        }
        public async Task SetUserStatusToDeclineAsync(int tripId, int userId)
        {
            string query = @"
                        UPDATE CarpoolTripMembers
                        SET Status = 'Declined'
                        WHERE TripID = @TripID AND UserID = @UserID;";

            await _db.SaveData(query, new { TripID = tripId, UserID = userId });
        }
        public async Task RemoveUserFromTripAsync(int tripId, int userId)
        {
            string query = @"
                DELETE FROM CarpoolTripMembers
                WHERE TripID = @TripID AND UserID = @UserID;";

            await _db.SaveData(query, new { TripID = tripId, UserID = userId });
        }
        public async Task SetUserStatusToDeclineAndSetDriverNull(int tripId, int userId)
        {
            string query = @"
                        UPDATE CarpoolTripMembers
                        SET Status = 'Declined'
                        WHERE TripID = @TripID AND UserID = @UserID;";
            await _db.SaveData(query, new { TripID = tripId, UserID = userId });
            string query2 = @"
                        UPDATE CarpoolTrip
                        SET DriverID = NULL
                        WHERE TripID = @TripID AND DriverID = @UserID;";
            await _db.SaveData(query2, new { TripID = tripId, UserID = userId });
        }
        public async Task<int> LeastMilesDriver(int userID1, int userID2)
        {
            string sql = @"WITH UserMiles AS (
                                SELECT 
                                    UserID, 
                                    COALESCE(MilesDriven, 0) AS MilesDriven
                                FROM 
                                    TripStatistics
                                WHERE 
                                    UserID IN (@userID1, @userID2)
                            )
                            SELECT 
                                CASE 
                                    WHEN COUNT(*) < 2 THEN -1 -- If data for one or both users doesn't exist
                                    WHEN MIN(MilesDriven) = MAX(MilesDriven) THEN -1 -- If miles are equal
                                    ELSE (SELECT UserID FROM UserMiles ORDER BY MilesDriven ASC LIMIT 1) 
                                END AS Result
                            FROM 
                                UserMiles;";
            List<int> x = await _db.LoadData<int,dynamic>(sql, new { userID1, userID2 });
            if (x.Count > 0)
            {
                return x.First();
            } 
            else 
            { 
                return -1; 
            }
        }
        public async Task StartTrip(int TripID)
        {
            string query = @"
                            UPDATE CarpoolTrips
                            SET Status = 'In Progress'
                            WHERE ID = @TripID;";
            await _db.SaveData(query, new { TripID });
        }
        public async Task EndTrip(int TripID)
        {
            string query = @"
                            UPDATE CarpoolTrips
                            SET Status = 'Completed'
                            WHERE ID = @TripID;";
            await _db.SaveData(query, new { TripID });
            string query2 = @"DELETE FROM GroupMemberLocations WHERE TripID = @TripID";
            await _db.SaveData(query2, new { TripID });

        }
    }
}
