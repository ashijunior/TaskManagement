using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManager_Simplex_.Models
{
    public class Tasks
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
        public int AssignedUserId { get; set; } // Foreign key
    }

}
