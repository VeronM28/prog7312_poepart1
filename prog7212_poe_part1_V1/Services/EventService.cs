using prog7212_poe_part1_V1.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace prog7212_poe_part1_V1.Services
{
    public class EventService
    {
        // Sorted Dictionary for organizing events by date
        private SortedDictionary<DateTime, List<Event>> eventsByDate;

        // Dictionary for quick event lookup by ID
        private Dictionary<int, Event> eventsById;

        // Sets for unique categories and locations
        private HashSet<string> categories;
        private HashSet<string> locations;

        // Stack for recent searches
        private Stack<EventSearch> recentSearches;

        // Queue for event notifications
        private Queue<Event> eventNotifications;

        // Priority Queue for recommended events
        private List<Event> priorityEvents;

        // User preferences for recommendations
        private Dictionary<string, UserPreference> userPreferences;

        public EventService()
        {
            eventsByDate = new SortedDictionary<DateTime, List<Event>>();
            eventsById = new Dictionary<int, Event>();
            categories = new HashSet<string>();
            locations = new HashSet<string>();
            recentSearches = new Stack<EventSearch>();
            eventNotifications = new Queue<Event>();
            priorityEvents = new List<Event>();
            userPreferences = new Dictionary<string, UserPreference>();

            InitializeSampleData();
        }

        private void InitializeSampleData()
        {
            var sampleEvents = new List<Event>
            {
                new Event { Id = 1, Title = "Community Music Festival", Description = "Annual local music festival featuring various artists", Date = DateTime.Now.AddDays(5), Category = "Music", Location = "City Park", Price = 25.00m, Priority = 2 },
                new Event { Id = 2, Title = "Tech Conference 2024", Description = "Latest trends in technology and innovation", Date = DateTime.Now.AddDays(10), Category = "Technology", Location = "Convention Center", Price = 150.00m, Priority = 1 },
                new Event { Id = 3, Title = "Farmers Market", Description = "Fresh local produce and handmade goods", Date = DateTime.Now.AddDays(2), Category = "Food", Location = "Town Square", Price = 0.00m, Priority = 3 },
                new Event { Id = 4, Title = "Art Exhibition", Description = "Local artists showcase their work", Date = DateTime.Now.AddDays(7), Category = "Art", Location = "Art Gallery", Price = 10.00m, Priority = 2 },
                new Event { Id = 5, Title = "Charity Run", Description = "5K run for local charity", Date = DateTime.Now.AddDays(14), Category = "Sports", Location = "River Trail", Price = 15.00m, Priority = 1 }
            };

            foreach (var eventItem in sampleEvents)
            {
                AddEvent(eventItem);
            }
        }

        public void AddEvent(Event eventItem)
        {
            // Add to events by date (Sorted Dictionary)
            if (!eventsByDate.ContainsKey(eventItem.Date.Date))
            {
                eventsByDate[eventItem.Date.Date] = new List<Event>();
            }
            eventsByDate[eventItem.Date.Date].Add(eventItem);

            // Add to events by ID (Dictionary)
            eventsById[eventItem.Id] = eventItem;

            // Add to sets for unique values
            categories.Add(eventItem.Category);
            locations.Add(eventItem.Location);

            // Add to priority events list
            priorityEvents.Add(eventItem);
            priorityEvents = priorityEvents.OrderBy(e => e.Priority).ThenBy(e => e.Date).ToList();
        }

        public List<Event> SearchEvents(EventSearch search)
        {
            // Push search to recent searches stack
            recentSearches.Push(search);

            // Update user preferences for recommendations
            UpdateUserPreferences(search);

            var results = GetAllEvents().AsQueryable();

            if (!string.IsNullOrEmpty(search.Category))
                results = results.Where(e => e.Category == search.Category);

            if (search.StartDate.HasValue)
                results = results.Where(e => e.Date >= search.StartDate.Value);

            if (search.EndDate.HasValue)
                results = results.Where(e => e.Date <= search.EndDate.Value);

            if (!string.IsNullOrEmpty(search.SearchTerm))
                results = results.Where(e => e.Title.Contains(search.SearchTerm) ||
                                           e.Description.Contains(search.SearchTerm));

            return results.ToList();
        }

        private void UpdateUserPreferences(EventSearch search)
        {
            if (!string.IsNullOrEmpty(search.Category))
            {
                if (userPreferences.ContainsKey(search.Category))
                {
                    userPreferences[search.Category].SearchCount++;
                    userPreferences[search.Category].LastSearched = DateTime.Now;
                }
                else
                {
                    userPreferences[search.Category] = new UserPreference
                    {
                        Category = search.Category,
                        SearchCount = 1,
                        LastSearched = DateTime.Now
                    };
                }
            }
        }

        public List<Event> GetRecommendedEvents()
        {
            var recommendations = new List<Event>();
            var allEvents = GetAllEvents();

            // Get top categories from user preferences
            var topCategories = userPreferences
                .OrderByDescending(up => up.Value.SearchCount)
                .ThenByDescending(up => up.Value.LastSearched)
                .Take(3)
                .Select(up => up.Key);

            foreach (var category in topCategories)
            {
                var categoryEvents = allEvents
                    .Where(e => e.Category == category && e.Date >= DateTime.Now)
                    .OrderBy(e => e.Date)
                    .Take(2);

                recommendations.AddRange(categoryEvents);
            }

            // If no preferences, show high priority events
            if (!recommendations.Any())
            {
                recommendations = priorityEvents
                    .Where(e => e.Date >= DateTime.Now)
                    .Take(4)
                    .ToList();
            }

            return recommendations.Distinct().ToList();
        }

        public void AddEventNotification(Event eventItem)
        {
            eventNotifications.Enqueue(eventItem);
        }

        public Event GetNextNotification()
        {
            return eventNotifications.Count > 0 ? eventNotifications.Dequeue() : null;
        }

        public Stack<EventSearch> GetRecentSearches()
        {
            return new Stack<EventSearch>(recentSearches.Reverse());
        }

        public HashSet<string> GetCategories()
        {
            return categories;
        }

        public HashSet<string> GetLocations()
        {
            return locations;
        }

        public List<Event> GetAllEvents()
        {
            return eventsByDate.Values.SelectMany(list => list).ToList();
        }

        public List<Event> GetUpcomingEvents()
        {
            return GetAllEvents()
                .Where(e => e.Date >= DateTime.Now)
                .OrderBy(e => e.Date)
                .ToList();
        }
    }
}