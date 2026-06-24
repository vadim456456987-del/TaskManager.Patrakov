using System;
using TaskManager.Patrakov.Core.Models;

namespace TaskManager.Patrakov.Tests
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== ТЕСТИРОВАНИЕ TASK MANAGER SERVICE ===\n");

            TestAddTask();
            TestFilterAndSearch();
            TestUpdateAndDelete();
            TestSaveAndLoad();
            TestStatistics();

            Console.WriteLine("\nВсе тесты завершены! Нажмите любую клавишу...");
            Console.ReadKey();
        }

        static void TestAddTask()
        {
            Console.WriteLine("1. Тест добавления задач:");
            var service = new TaskManagerService();

            var task1 = new Task("Купить продукты", "Молоко, хлеб, яйца", Priority.High, DateTime.Now.AddDays(1));
            var task2 = new Task("Сдать отчет", "Отчет по практике", Priority.Medium, DateTime.Now.AddDays(3));

            service.AddTask(task1);
            service.AddTask(task2);

            Console.WriteLine($"   Добавлено задач: {service.GetAllTasks().Count}");
            Console.WriteLine($"   Ожидалось: 2");
            Console.WriteLine($"   Результат: {(service.GetAllTasks().Count == 2 ? "✓ ПРОЙДЕН" : "✗ НЕ ПРОЙДЕН")}\n");
        }

        static void TestFilterAndSearch()
        {
            Console.WriteLine("2. Тест фильтрации и поиска:");
            var service = new TaskManagerService();

            service.AddTask(new Task("Задача 1", "Описание 1", Priority.Low, DateTime.Now));
            service.AddTask(new Task("Задача 2", "Описание 2", Priority.Medium, DateTime.Now));
            service.AddTask(new Task("Важная задача", "Срочно!", Priority.High, DateTime.Now));

            var searchResults = service.Search("Важная");
            Console.WriteLine($"   Поиск 'Важная': найдено {searchResults.Count} задач");
            Console.WriteLine($"   Ожидалось: 1");
            Console.WriteLine($"   Результат: {(searchResults.Count == 1 ? "✓ ПРОЙДЕН" : "✗ НЕ ПРОЙДЕН")}");

            var sortedTasks = service.SortByPriority();
            Console.WriteLine($"   Сортировка по приоритету: {sortedTasks.Count} задач\n");
        }

        static void TestUpdateAndDelete()
        {
            Console.WriteLine("3. Тест обновления и удаления:");
            var service = new TaskManagerService();

            var task = new Task("Тестовая задача", "Описание", Priority.Medium, DateTime.Now);
            service.AddTask(task);

            task.Title = "Обновленная задача";
            bool updated = service.UpdateTask(task.Id, task);
            Console.WriteLine($"   Обновление задачи: {(updated ? "✓" : "✗")}");

            bool deleted = service.DeleteTask(task.Id);
            Console.WriteLine($"   Удаление задачи: {(deleted ? "✓" : "✗")}");
            Console.WriteLine($"   Осталось задач: {service.GetAllTasks().Count}\n");
        }

        static void TestSaveAndLoad()
        {
            Console.WriteLine("4. Тест сохранения и загрузки:");
            var service1 = new TaskManagerService("test_tasks.json");

            service1.AddTask(new Task("Тест 1", "Описание 1", Priority.Low, DateTime.Now));
            service1.AddTask(new Task("Тест 2", "Описание 2", Priority.High, DateTime.Now.AddDays(5)));
            service1.SaveToFile();

            var service2 = new TaskManagerService("test_tasks.json");
            service2.LoadFromFile();

            Console.WriteLine($"   Сохранено задач: {service1.GetAllTasks().Count}");
            Console.WriteLine($"   Загружено задач: {service2.GetAllTasks().Count}");
            Console.WriteLine($"   Результат: {(service1.GetAllTasks().Count == service2.GetAllTasks().Count ? "✓ ПРОЙДЕН" : "✗ НЕ ПРОЙДЕН")}\n");
        }

        static void TestStatistics()
        {
            Console.WriteLine("5. Тест статистики:");
            var service = new TaskManagerService();

            service.AddTask(new Task("Задача 1", "", Priority.Medium, DateTime.Now.AddDays(-1)));
            service.AddTask(new Task("Задача 2", "", Priority.Medium, DateTime.Now.AddDays(1)));
            service.AddTask(new Task("Задача 3", "", Priority.Medium, DateTime.Now.AddDays(2)) { Status = TaskStatus.Completed });
            service.AddTask(new Task("Задача 4", "", Priority.Medium, DateTime.Now.AddDays(3)) { IsImportant = true });

            var stats = service.GetStatistics();

            
            int total = stats.total;
            int completed = stats.completed;
            int overdue = stats.overdue;
            int important = stats.important;

            Console.WriteLine($"   Всего задач: {total}");
            Console.WriteLine($"   Завершено: {completed}");
            Console.WriteLine($"   Просрочено: {overdue}");
            Console.WriteLine($"   Важных: {important}\n");
        }
    }
}