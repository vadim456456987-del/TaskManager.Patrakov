using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;

namespace TaskManager.Patrakov.Core.Models
{
    public class TaskManagerService  
    {
        private List<Task> _tasks;
        private readonly string _filePath;

        
        public TaskManagerService()
        {
            _tasks = new List<Task>();
            _filePath = "tasks.json";
        }

        
        public TaskManagerService(string filePath)
        {
            _tasks = new List<Task>();
            _filePath = filePath;
        }

        

        public void AddTask(Task task)
        {
            if (string.IsNullOrWhiteSpace(task.Title))
                throw new ArgumentException("Название задачи не может быть пустым");

            _tasks.Add(task);
        }

        public List<Task> GetAllTasks()
        {
            return _tasks.ToList();
        }

        public Task GetTaskById(Guid id)
        {
            return _tasks.FirstOrDefault(t => t.Id == id);
        }

        public bool UpdateTask(Guid id, Task updatedTask)
        {
            var existingTask = GetTaskById(id);
            if (existingTask == null)
                return false;

            existingTask.Title = updatedTask.Title;
            existingTask.Description = updatedTask.Description;
            existingTask.Priority = updatedTask.Priority;
            existingTask.DueDate = updatedTask.DueDate;
            existingTask.Status = updatedTask.Status;
            existingTask.IsImportant = updatedTask.IsImportant;

            return true;
        }

        public bool DeleteTask(Guid id)
        {
            var task = GetTaskById(id);
            if (task == null)
                return false;

            return _tasks.Remove(task);
        }

        public List<Task> FilterByStatus(TaskStatus status)
        {
            return _tasks.Where(t => t.Status == status).ToList();
        }

        public List<Task> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAllTasks();

            searchTerm = searchTerm.ToLower();
            return _tasks.Where(t =>
                t.Title.ToLower().Contains(searchTerm) ||
                t.Description.ToLower().Contains(searchTerm)
            ).ToList();
        }

        

        public List<Task> SortByPriority()
        {
            return _tasks.OrderBy(t => t.Priority).ToList();
        }

        public List<Task> SortByDueDate()
        {
            return _tasks.OrderBy(t => t.DueDate).ToList();
        }

        public (int total, int completed, int overdue, int important) GetStatistics()
        {
            int total = _tasks.Count;
            int completed = _tasks.Count(t => t.Status == TaskStatus.Completed);
            int overdue = _tasks.Count(t => t.IsOverdue());
            int important = _tasks.Count(t => t.IsImportant);

            return (total, completed, overdue, important);
        }

        public List<Task> GetImportantTasks()
        {
            return _tasks.Where(t => t.IsImportant).ToList();
        }

        

        public void SaveToFile()
        {
            try
            {
                string json = JsonConvert.SerializeObject(_tasks, Formatting.Indented);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при сохранении файла: {ex.Message}");
            }
        }

        public void LoadFromFile()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    string json = File.ReadAllText(_filePath);
                    _tasks = JsonConvert.DeserializeObject<List<Task>>(json) ?? new List<Task>();
                }
                else
                {
                    _tasks = new List<Task>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при загрузке файла: {ex.Message}");
            }
        }

        public void ClearAll()
        {
            _tasks.Clear();
        }
    }
}