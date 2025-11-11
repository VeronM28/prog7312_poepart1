using System;
using System.Collections.Generic;

namespace prog7212_poe_part1_V1.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; }
        public string Location { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Priority { get; set; } // For priority queue
    }

    public class EventSearch
    {
        public string Category { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string SearchTerm { get; set; }
    }

    public class UserPreference
    {
        public string Category { get; set; }
        public int SearchCount { get; set; }
        public DateTime LastSearched { get; set; }
    }
}