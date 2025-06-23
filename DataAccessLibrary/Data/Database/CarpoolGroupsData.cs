using DataAccessLibrary.Model;
using DataAccessLibrary.Model.Database_Models;
using DataAccessLibrary.Model.Logic_Models;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static DataAccessLibrary.Model.CarpoolGroupsModel;

namespace DataAccessLibrary.Data.Database
{
    public class CarpoolGroupsData : ICarpoolGroupsData
    {
        private readonly ISQLDataAccess _db;
        private readonly IUsersData _dbUsers;
        private readonly ISchedulesData _dbSchedule;


        public CarpoolGroupsData(ISQLDataAccess db, IUsersData usersData, ISchedulesData schedules)
        {
            _db = db;
            _dbUsers = usersData;
            _dbSchedule = schedules;
        }
        // build a groups from groups that dont exist 
        public async Task<List<RecomendedGroup>> GetRecomendedGroups(int GoalUserID, string Direction)
        {
            List<RecomendedGroup> RecomendedGroups = new();
            // Get all the users 
            List<UserInfoModel> MatchingUsers = new();


            return RecomendedGroups;

        }
        public async Task<List<CarpoolGroupsModel>> GetAllCarpoolGroups()
        {
            string sql = "SELECT * FROM CarpoolGroups";
            var result = await _db.LoadData<CarpoolGroupsModel, dynamic>(sql, new { });
            foreach (var item in result)
            {
                item.Riders = await GetRiders(item.GroupId, item.DriverId);
            }
            return result;
        }
        public async Task<List<CarpoolGroupsModel>> GetCarpoolGroup(int groupId)
        {
            string sql = @"SELECT 
                               GroupID, GroupName,DriverID,Destination,
                                Users.FirstName || "" "" || Users.LastName as DriverName
                            FROM CarpoolGroups
                            JOIN Users on CarpoolGroups.DriverID = Users.UserId
                            WHERE GroupID = @GroupId";
            var result = await _db.LoadData<CarpoolGroupsModel, dynamic>(sql, new { GroupId = groupId });
            foreach (var item in result)
            {
                item.Riders = await GetRiders(item.GroupId, item.DriverId);
            }
            return result;
        }
        public async Task UpdateCarpoolGroup(CarpoolGroupsModel group)
        {
            string sql = @"UPDATE CarpoolGroups 
                           SET GroupName = @GroupName, DriverID = @DriverId, Destination = @Destination, MeetingPoint = @MeetingPoint 
                           WHERE GroupID = @GroupId";
            await _db.SaveData(sql, group);
        }
        public async Task DeleteCarpoolGroup(CarpoolGroupsModel group)
        {
            string sql = @"DELETE FROM CarpoolGroups WHERE GroupID = @GroupId";
            await _db.SaveData(sql, new { group.GroupId });
        }
        public async Task<List<CarpoolGroupsModel>> GetDriverGroups(int driverId)
        {
            string sql = @"SELECT 
                               GroupID, GroupName,DriverID,Destination,
                                Users.FirstName || '""' || Users.LastName as DriverName
                            FROM CarpoolGroups
                            JOIN Users on CarpoolGroups.DriverID = Users.UserId
                            WHERE DriverID = @DriverID";
            var result = await _db.LoadData<CarpoolGroupsModel, dynamic>(sql, new { DriverID = driverId });
            foreach (var item in result)
            {
                item.Riders = await GetRiders(item.GroupId, item.DriverId);
            }
            return result;
        }
        public async Task<List<RiderModel>> GetRiders(int groupId, int driverID)
        {
            string sql = @"SELECT GM.UserId as Id, U.PickupLocation as Location, U.FirstName || "" "" || U.LastName as Name
                            FROM CarpoolGroups G
                            JOIN GroupMembers GM on G.GroupID = GM.GroupID and GM.UserId != @driverID
                            JOIN Users U on GM.UserID = U.UserId
                            WHERE G.GroupID = @groupId";
            return await _db.LoadData<RiderModel, dynamic>(sql, new { groupId, driverID });
        }
        public async Task<List<CarpoolGroupsModel>> GetRiderGroups(int userID)
        {
            string sql = @"select 
                                  CG.GroupID, CG.GroupName,CG.DriverID,CG.Destination,
                                   Users.FirstName || ' ' ||  Users.LastName as DriverName
                            from GroupMembers GM 
                            JOIN CarpoolGroups CG on GM.GroupID = CG.GroupID
                            JOIN Users on CG.DriverID = Users.UserId
                            where GM.UserId = @userID";
            var result = await _db.LoadData<CarpoolGroupsModel, dynamic>(sql, new { userID });
            foreach (var item in result)
            {
                item.Riders = await GetRiders(item.GroupId, item.DriverId);
            }
            return result;

        }
        public async Task CreateNewGroup(CarpoolGroupsModel carpoolGroup)
        {

            string sql = @"INSERT INTO CarpoolGroups (GroupName, DriverID, Destination, CreatorID) 
                   VALUES (@GroupName, @DriverId, @Destination, @CreatorID)";
            await _db.SaveData(sql, carpoolGroup);
            sql = @"select GroupID from CarpoolGroups where GroupName = @groupName";
            var result = await _db.LoadData<string, dynamic>(sql, new { groupName = carpoolGroup.GroupName });
            string GoalGroupID = result.First();
            sql = $@"
                INSERT INTO GroupMembers (GroupID, UserID, JoinDate)
                VALUES (@GroupId, @UserId, CURRENT_TIMESTAMP);";

            await _db.SaveData(sql, new { UserId = carpoolGroup.DriverId, GroupId = GoalGroupID });
        }
        public async Task<int> GetGroupNumber(string GroupName, int CreatorID)
        {
            string sql = @"select GroupID from CarpoolGroups where GroupName = @GroupName and CreatorID = @CreatorID ";
            var data = await _db.LoadData<int, dynamic>(sql, new { GroupName, CreatorID });
            return data.FirstOrDefault();
        }
        public async Task<RecomendedGroup> GetSingleGroup(int GroupID, int RequestingUserID)
        {
            string sqlGroup = @"select * from CarpoolGroups where GroupID = @GroupID";
            List<CarpoolGroupsModel> groupsModels = await _db.LoadData<CarpoolGroupsModel, dynamic>(sqlGroup, new { GroupID });
            CarpoolGroupsModel carpoolGroupsModel = groupsModels.FirstOrDefault();
            UserInfoModel requestingUser = await _dbUsers.GetUserInfoModel(RequestingUserID);
            string sqlMembers = @"select UserID from GroupMembers where GroupID = @GroupID";
            List<int> IDS = await _db.LoadData<int, dynamic>(sqlMembers, new { GroupID });
            List<UserInfoModel> GroupMems = new();
            IDS = IDS.Where(I => I != RequestingUserID).ToList();
            foreach (int userid in IDS)
            {
                GroupMems.Add(await _dbUsers.GetUserInfoModel(userid));
            }
            return new RecomendedGroup
            {
                RequestingUser = requestingUser,
                GroupID = GroupID,
                GroupName = carpoolGroupsModel.GroupName,
                GroupMembers = GroupMems
            };

        }
        public async Task<List<RecomendedGroup>> GetAvailableGroups(int GoalUserID, List<string> Days)
        {
            // Query to get all the matching groups 
            string sql = $@"
                    SELECT s2.GroupID, CG.GroupName, s2.Direction
                    FROM Schedules s1
                    JOIN GroupSchedule s2 on
                        s1.Day = s2.Day 
                        AND s1.Text = s2.Direction
                        AND (s1.Start <= s2.End AND s1.End >= s2.Start)
                    JOIN Users u on s1.UserID = u.UserID
                    LEFT JOIN GroupMembers GM on GM.GroupID = s2.GroupID AND GM.UserID = s1.UserID
                    JOIN CarpoolGroups CG on s2.GroupID = CG.GroupID
                    WHERE s1.UserID = @UserId
                    AND s1.Day IN @Days AND GM.UserID IS NULL;";

            List<(int, string, string)> MatchingScheduleGroupIDs = await _db.LoadData<(int, string, string), dynamic>(sql, new { UserId = GoalUserID, Days });

            // Get the user info for the goal model 
            UserInfoModel GoalUser = await _dbUsers.GetUserInfoModel(GoalUserID);
            List<RecomendedGroup> RecommendedGroups = new List<RecomendedGroup>();
            foreach ((int GroupID, string GroupName, string direction) in MatchingScheduleGroupIDs)
            {
                List<int> GroupMemberIDs = await GetUserIds(GroupID);
                List<UserInfoModel> GroupMembers = new List<UserInfoModel>();
                foreach (int MemberID in GroupMemberIDs)
                {
                    GroupMembers.Add(await _dbUsers.GetUserInfoModel(MemberID));
                }
                RecommendedGroups.Add(new RecomendedGroup(GroupName, GroupID, GroupMembers, GoalUser, direction));
            }
            return RecommendedGroups;
        }
        public async Task<List<int>> GetUserIds(int GroupID)
        {
            string sql = @"select UserID from GroupMembers where GroupID = @groupID";
            return await _db.LoadData<int, dynamic>(sql, new { groupID = GroupID });

        }
        // Get the current groups a user is apart of 
        public async Task<List<(int, string, int, int)>> GetCurrentGroups(int GoalUserID)
        {
            // Query to get all the matching groups 
            string sql = $@"
                    SELECT GM.GroupID, CG.GroupName, CG.CreatorID,GM.UserID
                    FROM GroupMembers GM
                    JOIN CarpoolGroups CG on GM.GroupID = CG.GroupID
                    WHERE GM.UserID = @UserId;";

            return await _db.LoadData<(int, string, int, int), dynamic>(sql, new { UserId = GoalUserID });

        }
        public async Task JoinGroup(int GoalUserID, int GoalGroupID)
        {
            string sql = $@"
                INSERT INTO GroupMembers (GroupID, UserID, JoinDate)
                VALUES (@GroupId, @UserId, CURRENT_TIMESTAMP);";

            await _db.SaveData(sql, new { UserId = GoalUserID, GroupId = GoalGroupID });
        }
        public async Task RemoveGroupMember(int GoalUserID, int GoalGroupID)
        {
            string sql = @" DELETE FROM GroupMembers
                WHERE UserID = @UserId AND GroupID = @GroupId;";

            await _db.SaveData(sql, new { UserId = GoalUserID, GroupId = GoalGroupID });
        }
        // This function will use k clustering and Hierarchical agglomerative clustering (HAC) to form groups of users 
        public async Task<List<RecomendedGroup>> GetRecommendGroups(int GoalUserID, List<string> Days, string TravelDirection, int DistanceWeight, int PreferenceWeight)
        {
            if (!TravelDirection.Equals("arrival") && !TravelDirection.Equals("departure"))
            {
                Console.WriteLine("Error - Invalid Direction " + TravelDirection);
                return null;
            }

            // Build the user info model for the goal user
            UserInfoModel GoalUserModel = await _dbUsers.GetUserInfoModel(GoalUserID);

            // Get a list of UserInfoModel, representing all users that the Goal User can be in a group with
            // For testing purposes, you can generate test users
            List<UserInfoModel> MatchingUserList = GenerateTestUserInfoModels(32);

            // Filter users based on gender preferences
            MatchingUserList = MatchingUserList.Where(user => GenderPreferencesMatch(GoalUserModel, user)).ToList();

            // Include the goal user in the list
            MatchingUserList.Add(GoalUserModel);

            // Initialize clusters (each user is their own cluster)
            List<List<UserInfoModel>> clusters = MatchingUserList.Select(user => new List<UserInfoModel> { user }).ToList();

            // Compute the initial distance matrix
            int n = clusters.Count;
            double[,] distanceMatrix = new double[n, n];

            // Calculate normalized weights
            double totalWeight = DistanceWeight + PreferenceWeight;
            if (totalWeight == 0) totalWeight = 1; // avoid division by zero
            double weightPreference = PreferenceWeight / totalWeight;
            double weightGeographical = DistanceWeight / totalWeight;

            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    double distance = ComputeClusterDistance(clusters[i], clusters[j], weightPreference, weightGeographical);
                    distanceMatrix[i, j] = distance;
                    distanceMatrix[j, i] = distance; // Symmetric
                }
            }

            // HAC Algorithm parameters
            int maxGroupSize = 4;
            int minGroupSize = 2;
            int iteration = 0;
            // HAC clustering loop
            while (true)
            {
                double minDistance = double.MaxValue;
                int minI = -1;
                int minJ = -1;

                n = clusters.Count;

                // Find the pair of clusters with the minimum distance that can be merged
                for (int i = 0; i < n; i++)
                {
                    if (clusters[i].Count >= maxGroupSize)
                        continue; // Cannot merge cluster i further

                    for (int j = i + 1; j < n; j++)
                    {
                        if (clusters[j].Count >= maxGroupSize)
                            continue; // Cannot merge cluster j further

                        // Check if merging exceeds the max group size
                        if (clusters[i].Count + clusters[j].Count > maxGroupSize)
                            continue;

                        double distance = ComputeClusterDistance(clusters[i], clusters[j], weightPreference, weightGeographical);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            minI = i;
                            minJ = j;
                        }
                    }
                }

                // No more clusters can be merged
                if (minI == -1 || minJ == -1)
                    break;

                // Merge clusters[minI] and clusters[minJ]
                clusters[minI].AddRange(clusters[minJ]);
                clusters.RemoveAt(minJ);

                // Update distance matrix
                distanceMatrix = UpdateDistanceMatrix(distanceMatrix, clusters, minI, minJ, weightPreference, weightGeographical);
                n = clusters.Count;
                iteration++;
            }

            // Build recommended groups from clusters of acceptable sizes
            List<RecomendedGroup> GroupList = new List<RecomendedGroup>();
            int count = 0;
            foreach (var cluster in clusters)
            {
                count++;
                if (cluster.Count >= minGroupSize && cluster.Count <= maxGroupSize)
                {
                    var filteredCluster = cluster.Where(user => user.UserID != GoalUserModel.UserID).ToList();
                    RecomendedGroup group = new RecomendedGroup
                    {
                        RequestingUser = GoalUserModel,
                        GroupID = -1, // Assign appropriate GroupID if needed
                        GroupName = $"Recommended Group {count}",
                        GroupMembers = filteredCluster
                    };
                    GroupList.Add(group);
                }
                // Users in smaller clusters are left out
            }

            Console.WriteLine($"The Recommendation Algorithm Found {GroupList.Count} group(s) for you!");
            return GroupList;
        }

        // Haversine formula to calculate the distance between two lat/lon points in meters.
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            // Earth's radius in meters
            double R = 6371e3;
            double lat1Rad = lat1 * Math.PI / 180;
            double lat2Rad = lat2 * Math.PI / 180;
            double deltaLat = (lat2 - lat1) * Math.PI / 180;
            double deltaLon = (lon2 - lon1) * Math.PI / 180;

            double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                       Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                       Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            double distance = R * c;
            return distance;
        }
        // Computes the distance between two clusters using average linkage.
        private double ComputeClusterDistance(List<UserInfoModel> clusterA, List<UserInfoModel> clusterB, double weightPreference, double weightGeographical)
        {
            // Use average linkage: average distance between all pairs of users in the two clusters
            double totalDistance = 0;
            int count = 0;

            foreach (var userA in clusterA)
            {
                foreach (var userB in clusterB)
                {
                    double distance = ComputeUserDistance(userA, userB, weightPreference, weightGeographical);
                    if (distance != double.MaxValue) // Exclude pairs that don't match gender preferences
                    {
                        totalDistance += distance;
                        count++;
                    }
                }
            }

            if (count == 0)
                return double.MaxValue;

            return totalDistance / count;
        }        
        // Computes the distance between two users based on their preference match and geographical distance.
        private double ComputeUserDistance(UserInfoModel userA, UserInfoModel userB, double weightPreference, double weightGeographical)
        {
            // First, check if users match each other's gender preferences
            if (!GenderPreferencesMatch(userA, userB))
            {
                // Return a very high distance if gender preferences do not match
                return double.MaxValue;
            }

            // Calculate preference match score (0 to 1)
            double preferenceScore = CalculatePreferenceMatch(userA, userB);

            // Calculate geographical distance in meters
            double geoDistanceMeters = CalculateDistance(userA.PickupLatitude, userA.PickupLongitude, userB.PickupLatitude, userB.PickupLongitude);

            // Normalize geographical distance (0 to 1)
            double normalizedGeoDistance = geoDistanceMeters / 50000;

            // Combine preference score and geographical distance into overall distance
            double overallDistance = weightPreference * (1.0 - preferenceScore) + weightGeographical * normalizedGeoDistance;

            return overallDistance;
        }

        // Calculates the preference match score between two users (without gender).
        private double CalculatePreferenceMatch(UserInfoModel user1, UserInfoModel user2)
        {
            int matchScore = 0;
            int totalPreferences = 4;

            if (user1.SmokingPreference == user2.AllowSmokeVape || user1.SmokingPreference == "No Preference")
                matchScore++;
            if (user1.EatingPreference == user2.AllowEatDrink || user1.EatingPreference == "No Preference")
                matchScore++;
            if (user1.TemperaturePreference == user2.TemperaturePreference || user1.TemperaturePreference == "No Preference")
                matchScore++;
            if (user1.MusicPreference == user2.MusicPreference || user1.MusicPreference == "No Preference")
                matchScore++;

            return (double)matchScore / totalPreferences;
        }
        // Checks if two users match each other's gender preferences.
        private bool GenderPreferencesMatch(UserInfoModel userA, UserInfoModel userB)
        {
            bool userAMatchesUserBPreference = DoesUserMatchGenderPreference(userA, userB);
            bool userBMatchesUserAPreference = DoesUserMatchGenderPreference(userB, userA);
            return userAMatchesUserBPreference && userBMatchesUserAPreference;
        }

        // Determines if 'targetUser' matches 'user's gender preference.
        private bool DoesUserMatchGenderPreference(UserInfoModel user, UserInfoModel targetUser)
        {

            string userPreference = user.GenderPreference;
            string targetGenderCategory = CategorizeGender(targetUser.Gender);
            if (userPreference == "No Preference")
            {
                return true;
            }
            else if (userPreference == "Same Gender")
            {
                string userGenderCategory = CategorizeGender(user.Gender);
                bool result = userGenderCategory == targetGenderCategory;
                return result;
            }
            else if (userPreference == "Women and Non-binary Only")
            {
                bool result = targetGenderCategory == "Woman" || targetGenderCategory == "Non-binary";
                return result;
            }
            else if (userPreference == "Non-binary Only")
            {
                bool result = targetGenderCategory == "Non-binary";
                return result;
            }
            else
            {
                return false;
            }
        }

        // Categorizes gender into "Man", "Woman", or "Non-binary".
        private string CategorizeGender(string gender)
        {
            if (gender == "Man")
            {
                return "Man";
            }
            else if (gender == "Woman")
            {
                return "Woman";
            }
            else
            {
                // All other genders are categorized as "Non-binary"
                return "Non-binary";
            }
        }
        // Function to print the distance matrix
        private void PrintDistanceMatrix(double[,] matrix, List<UserInfoModel> users)
        {
            int n = users.Count;
            Console.WriteLine("Distance Matrix:");
            Console.Write("          ");
            for (int i = 0; i < n; i++)
            {
                Console.Write($"{users[i].UserID,8}");
            }
            Console.WriteLine();
            for (int i = 0; i < n; i++)
            {
                Console.Write($"{users[i].UserID,8} ");
                for (int j = 0; j < n; j++)
                {
                    double distance = matrix[i, j];
                    if (double.IsPositiveInfinity(distance) || distance == double.MaxValue)
                    {
                        Console.Write($"{"Inf",8}");
                    }
                    else
                    {
                        Console.Write($"{distance,8:F2}");
                    }
                }
                Console.WriteLine();
            }
        }
        public List<UserInfoModel> GenerateTestUserInfoModels(int ListSize)
        {
            List<UserInfoModel> userList = new List<UserInfoModel>();

            // Fixed drop-off location
            double dropoffLatitude = 28.066750;
            double dropoffLongitude = -80.623032;

            // Maximum offset in degrees for pickup locations (approx. 5 km radius)
            double maxOffsetDegrees = 0.045; // Adjust this value to increase or decrease the spread

            Random random = new Random();

            // Define possible values for preferences
            List<string> genderValues = new List<string> { "No Preference", "Women and Non-binary Only", "Same Gender", "Non-binary Only" };
            List<string> carSmokingValues = new List<string> { "No Preference", "Vehicle allows smoking", "Vehicle does not allow smoking" };
            List<string> carEatingValues = new List<string> { "No Preference", "Vehicle allows eating", "Vehicle does not allow eating" };
            List<string> carTempValues = new List<string> { "No Preference", "Warmer", "Colder" };

            List<MusicGenre> musicGenres = new List<MusicGenre>
                {
                    new MusicGenre { Id = "2", Name = "Pop" },
                    new MusicGenre { Id = "3", Name = "Rock" },
                    new MusicGenre { Id = "4", Name = "Hip Hop" },
                    new MusicGenre { Id = "5", Name = "R&B" },
                    new MusicGenre { Id = "6", Name = "Country" },
                    new MusicGenre { Id = "7", Name = "Folk" },
                    new MusicGenre { Id = "8", Name = "Classical" }
                };

            for (int i = 0; i < ListSize; i++)
            {
                UserInfoModel user = new UserInfoModel();

                // Assign incremental UserID starting from 1
                user.UserID = i + 1;

                // Randomly assign "Driver" or "Passenger"
                user.UserType = (random.Next(2) == 0) ? "Driver" : "Passenger";

                // Randomly assign Gender
                user.Gender = (random.Next(2) == 0) ? "Man" : "Woman";

                // Fixed drop-off location
                user.DropoffLocation = "Destination";
                user.DropoffLatitude = dropoffLatitude;
                user.DropoffLongitude = dropoffLongitude;

                // Generate random pickup location within a circle around drop-off
                double radius = maxOffsetDegrees * Math.Sqrt(random.NextDouble());
                double angle = random.NextDouble() * 2 * Math.PI;

                double latOffset = radius * Math.Cos(angle);
                double lonOffset = radius * Math.Sin(angle) / Math.Cos(dropoffLatitude * Math.PI / 180);

                user.PickupLatitude = dropoffLatitude + latOffset;
                user.PickupLongitude = dropoffLongitude + lonOffset;

                user.PickupLocation = $"Pickup Location {user.UserID}";

                // Set other properties with default or dummy values
                user.DrivingDistance = random.Next(5, 50); // Random distance in miles between 5 and 50
                user.BeltCount = random.Next(1, 6); // Random belt count between 1 and 5

                user.AllowEatDrink = (random.Next(2) == 0) ? "Yes" : "No";
                user.AllowSmokeVape = (random.Next(2) == 0) ? "Yes" : "No";

                // Assign random GenderPreference from genderValues
                user.GenderPreference = genderValues[random.Next(genderValues.Count)];

                // Assign random SmokingPreference from carSmokingValues
                user.SmokingPreference = carSmokingValues[random.Next(carSmokingValues.Count)];

                // Assign random EatingPreference from carEatingValues
                user.EatingPreference = carEatingValues[random.Next(carEatingValues.Count)];

                // Assign random TemperaturePreference from carTempValues
                user.TemperaturePreference = carTempValues[random.Next(carTempValues.Count)];

                // Assign random MusicPreference from musicGenres
                bool noMusicPreference = (random.Next(2) == 0);
                if (noMusicPreference)
                {
                    user.MusicPreference = "No Preference";
                }
                else
                {
                    // Randomly select some genres
                    int numGenres = random.Next(1, musicGenres.Count + 1); // At least one genre
                    var selectedGenres = musicGenres.OrderBy(x => random.Next()).Take(numGenres).Select(g => g.Name);
                    user.MusicPreference = string.Join(", ", selectedGenres);
                }

                user.PhonePrivacy = "Share With No One";
                user.AddressPrivacy = "Share With No One";

                user.MakeModel = $"MakeModel{user.UserID}";
                user.VehicleColor = $"Color{user.UserID}";
                user.LicensePlate = $"Plate{user.UserID}";

                user.LicensePicture = null;
                user.CarPicture = null;
                user.ProfilePicture = null;

                user.Rating = random.Next(1, 6); // Random rating between 1 and 5
                // Calculate direction from campus (dropoff location)
                string direction = GetDirection(user.PickupLatitude, user.PickupLongitude, dropoffLatitude, dropoffLongitude);

                // Set first name based on direction
                user.FirstName = direction;

                // Set last name as "TestUser{UserID}"
                user.LastName = $"TestUser{user.UserID}";
                // Add the user to the list
                userList.Add(user);
            }

            return userList;
        }
        class MusicGenre
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
        // Helper function to calculate the direction from campus
        private string GetDirection(double pickupLat, double pickupLon, double dropoffLat, double dropoffLon)
        {
            double deltaLat = pickupLat - dropoffLat;
            double deltaLon = pickupLon - dropoffLon;

            if (Math.Abs(deltaLat) > Math.Abs(deltaLon))
            {
                if (deltaLat > 0)
                    return "North";
                else
                    return "South";
            }
            else
            {
                if (deltaLon > 0)
                    return "East";
                else
                    return "West";
            }
        }
        // Helper function to update the distance matrix after merging clusters
        private double[,] UpdateDistanceMatrix(double[,] oldMatrix, List<List<UserInfoModel>> clusters, int mergedIndex, int removedIndex, double weightPreference, double weightGeographical)
        {
            int n = clusters.Count;
            double[,] newMatrix = new double[n, n];

            int oldIndexI = 0;
            for (int i = 0; i < n; i++)
            {
                if (i == removedIndex)
                    oldIndexI++;
                int oldIndexJ = 0;
                for (int j = 0; j < n; j++)
                {
                    if (j == removedIndex)
                        oldIndexJ++;
                    if (i == j)
                    {
                        newMatrix[i, j] = 0;
                    }
                    else if (i == mergedIndex || j == mergedIndex)
                    {
                        int indexA = mergedIndex < removedIndex ? mergedIndex : mergedIndex + 1;
                        int indexB = i == mergedIndex ? j : i;
                        newMatrix[i, j] = ComputeClusterDistance(clusters[mergedIndex], clusters[indexB], weightPreference, weightGeographical);
                    }
                    else
                    {
                        newMatrix[i, j] = oldMatrix[oldIndexI, oldIndexJ];
                    }
                    oldIndexJ++;
                }
                oldIndexI++;
            }
            return newMatrix;
        }
    }
}