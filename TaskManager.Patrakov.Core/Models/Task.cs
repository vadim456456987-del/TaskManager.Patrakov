using System;

namespace TaskManager.Patrakov.Core.Models
{
    public class Task
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Priority Priority { get; set; }
        public DateTime DueDate { get; set; }
        public TaskStatus Status { get; set; }
        public bool IsImportant { get; set; }  
        public DateTime CreatedAt { get; set; }

        
        public Task()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
            Status = TaskStatus.New;
            Priority = Priority.Medium;
            IsImportant = false;
        }

        
        public Task(string title, string description, Priority priority, DateTime dueDate)
        {
            Id = Guid.NewGuid();
            Title = title;
            Description = description;
            Priority = priority;
            DueDate = dueDate;
            Status = TaskStatus.New;
            IsImportant = false;
            CreatedAt = DateTime.Now;
        }

        
        public override string ToString()
        {
            string importantMark = IsImportant ? " ⭐" : "";
            return $"{Title}{importantMark} - {Status} (До: {DueDate:dd.MM.yyyy})";
        }

        
        public bool IsOverdue()
        {
            return Status != TaskStatus.Completed && DueDate < DateTime.Now;
        }
    }
}