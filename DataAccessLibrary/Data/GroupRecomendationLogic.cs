using DataAccessLibrary.Data.Database;
using DataAccessLibrary.Model.Logic_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data
{
    public class GroupRecomendationLogic
    {
        private readonly IUsersData _dbUsers;
        public List<UserScheduleEntry> UsersToGroup { get; set; }
        public List<TimeWindow> TimeSlots {  get; set; }
        public GroupRecomendationLogic(List<UserScheduleEntry> usersToGroup, Week timeSlots , IUsersData usersData) { 
            UsersToGroup = usersToGroup;
            TimeSlots = GenerateTimeSlots(timeSlots);
            _dbUsers = usersData;
        }
        public async Task<List<RecomendedGroup>> GetRecomendedGroupsAsync()
        {
            List<RecomendedGroup> GroupList = new();

            foreach (TimeWindow timeWindow in this.TimeSlots)
            {
                // Get the list of user info models
                List<int> UserIds = UsersToGroup
                     .Where(e => e.Start >= timeWindow.Start && e.End <= timeWindow.End)
                     .Select(e => e.UserID)
                     .ToList();
                List<UserInfoModel> usersToGroup = await _dbUsers.GetListUserInfoModel(UserIds);
                // Run algo on that list of users to get the best groups, add them to the running list of groups, this will then be displayed in a hierarchical display
                List<RecomendedGroup> recomendedGroups = await HACGrouping(usersToGroup, timeWindow);
                GroupList.AddRange(recomendedGroups);
            }
            return await CompressGroupsAsync(GroupList);
        }
        public async Task<List<RecomendedGroup>> HACGrouping(List<UserInfoModel> UsersToGroup, TimeWindow window)
        {
            List<RecomendedGroup> CreatedGroups = new();
            List<List<UserInfoModel>> clusters = UsersToGroup.Select(user => new List<UserInfoModel> { user }).ToList();
            // Compute the initial distance matrix
            double[,] distanceMatrix = CreateDistanceMatrix(clusters);
            // HAC Algorithm parameters
            int maxGroupSize = 5;
            int minGroupSize = 2;
            int iteration = 0;
            // HAC clustering loop
            while (true)
            {
                double minDistance = double.MaxValue;
                int minI = -1;
                int minJ = -1;

                int n = clusters.Count;

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

                        double distance = ComputeClusterDistance(clusters[i], clusters[j]);
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
                {
                    Console.WriteLine("Iteration before break " + iteration);
                    break;
                }


                // Merge clusters[minI] and clusters[minJ]
                clusters[minI].AddRange(clusters[minJ]);
                clusters.RemoveAt(minJ);

                // Update distance matrix
                distanceMatrix = UpdateDistanceMatrix(distanceMatrix, clusters, minI, minJ);
                n = clusters.Count;
                iteration++;
            }
            // Build recommended groups from clusters of acceptable sizes
            List<RecomendedGroup> GroupList = new List<RecomendedGroup>();
            int count = 0;
            foreach (var cluster in clusters)
            {
                count++;
                string displayNames = string.Join(", ", cluster.Select(member => $"{member.FirstName} {member.LastName}"));
                string direction = "arrival";
                if (window.Start.Hour >= 13)
                {
                    direction = "departure";
                }
                RecomendedGroup group = new RecomendedGroup
                {
                    GroupName = $"Recommended Group {displayNames}",
                    GroupMembers = cluster,
                    StartWindow = window.Start,
                    EndWindow = window.End,
                    Direction = direction,                    
                };
                

                GroupList.Add(group);
                // Users in smaller clusters are left out
            }
            Console.WriteLine($"The Recommendation Algorithm Found {GroupList.Count} group(s) for you!");

            return GroupList;
        }
        private double[,] UpdateDistanceMatrix(double[,] oldMatrix, List<List<UserInfoModel>> clusters, int mergedIndex, int removedIndex)
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
                        newMatrix[i, j] = ComputeClusterDistance(clusters[mergedIndex], clusters[indexB]);
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
        public double[,] CreateDistanceMatrix(List<List<UserInfoModel>> clusters)
        {
            int n = clusters.Count;
            double[,] distanceMatrix = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    double distance = ComputeClusterDistance(clusters[i], clusters[j]);
                    distanceMatrix[i, j] = distance;
                    distanceMatrix[j, i] = distance; // Symmetric
                }
            }
            return distanceMatrix;
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
        private double ComputeClusterDistance(List<UserInfoModel> clusterA, List<UserInfoModel> clusterB)
        {
            // Use average linkage: average distance between all pairs of users in the two clusters
            double totalDistance = 0;
            int count = 0;

            foreach (var userA in clusterA)
            {
                foreach (var userB in clusterB)
                {
                    double distance = ComputeUserDistance(userA, userB);
                    if (distance != double.MaxValue)
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
        private double ComputeUserDistance(UserInfoModel userA, UserInfoModel userB)
        {

            double genderMulti = 1;
            // First, check if users match each other's gender preferences
            if (!GenderPreferencesMatch(userA, userB))
            {
                // Return a very high distance if gender preferences do not match
                genderMulti = 2.0;
            }

            // Calculate preference match score (0 to 1)
            double preferenceScore = CalculatePreferenceMatch(userA, userB);

            // Calculate geographical distance in meters
            double geoDistanceMeters = CalculateDistance(userA.PickupLatitude, userA.PickupLongitude, userB.PickupLatitude, userB.PickupLongitude);

            // Normalize geographical distance (0 to 1)
            double normalizedGeoDistance = geoDistanceMeters / 50000;

            // Combine preference score and geographical distance into overall distance
            double overallDistance = genderMulti * ((preferenceScore * .2) + (normalizedGeoDistance * 1.1));

            return overallDistance;
        }
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
        private List<TimeWindow> GenerateTimeSlots(Week week)
        {
            DateTime currentDay = week.StartDate;
            DateTime endTime = week.EndDate.AddDays(1); // Include the last day fully
            List<TimeWindow> timeSlots = new();

            while (currentDay < endTime)
            {
                // Create arrival time slots (7 AM to 12 PM)
                DateTime arrivalStart = currentDay.AddHours(7);
                DateTime arrivalEnd = currentDay.AddHours(12);
                while (arrivalStart < arrivalEnd)
                {
                    timeSlots.Add(new TimeWindow(arrivalStart, arrivalStart.AddHours(1), "arrival"));
                    arrivalStart = arrivalStart.AddHours(1);
                }

                // Create departure time slots (1 PM to 7 PM)
                DateTime departureStart = currentDay.AddHours(13);
                DateTime departureEnd = currentDay.AddHours(19);
                while (departureStart < departureEnd)
                {
                    timeSlots.Add(new TimeWindow(departureStart, departureStart.AddHours(1), "departure"));
                    departureStart = departureStart.AddHours(1);
                }

                // Move to the next day
                currentDay = currentDay.AddDays(1);
            }

            return timeSlots;
        }
        public async Task<List<RecomendedGroup>> CompressGroupsAsync(List<RecomendedGroup> groups)
        {
            var compressedGroups = new List<RecomendedGroup>();

            // Group by members and direction to detect recurring patterns
            var groupedByMembers = groups
                .GroupBy(g => (string.Join(",", g.GroupMembers.Select(m => m.UserID)), g.Direction))
                .ToList();

            foreach (var group in groupedByMembers)
            {
                var groupedTimes = group
                .Select(g => (g.StartWindow, g.EndWindow)) // Creates a list of tuples
                .OrderBy(g => g.StartWindow)
                .ToList();

                // Extract repeating pattern
                var recurringPattern = ExtractRecurringPattern(groupedTimes);
                var firstGroup = group.First();

                var compressedGroup = new RecomendedGroup
                {
                    GroupName = firstGroup.GroupName,
                    GroupMembers = firstGroup.GroupMembers,
                    Direction = firstGroup.Direction,
                    StartWindow = groupedTimes.First().StartWindow,
                    EndWindow = groupedTimes.Last().EndWindow,
                    RecurringPattern = recurringPattern,
                    IsRecurring = !string.IsNullOrEmpty(recurringPattern) ? "Yes"  : "No",
                    ActiveTimeSlots = groupedTimes.Select(g => g.StartWindow).ToList()
                };

                compressedGroups.Add(compressedGroup);
            }

            return compressedGroups;
        }

        private string ExtractRecurringPattern(List<(DateTime StartWindow, DateTime EndWindow)> groupedTimes)
        {
            var days = groupedTimes
                .GroupBy(t => t.StartWindow.DayOfWeek)
                .Select(g => g.Key.ToString())
                .OrderBy(d => d)
                .ToList();

            if (days.Count == 0)
                return string.Empty;

            var timeRange = $"{groupedTimes.First().StartWindow:hh:mm tt}-{groupedTimes.Last().EndWindow:hh:mm tt}";
            return $"{string.Join(", ", days)}: {timeRange}";
        }
    }
}
