using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TaskManager.Patrakov.Core.Models;

namespace TaskManager.Patrakov.WPF
{
    public partial class MainWindow : Window
    {
        private TaskManagerService _service;
        private List<Task> _currentTasks;

        public MainWindow()
        {
            InitializeComponent();
            InitializeService();
            LoadTasks();
            UpdateStatistics();
        }

        private void InitializeService()
        {
            _service = new TaskManagerService("tasks.json");
            try
            {
                _service.LoadFromFile();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void LoadTasks()
        {
            _currentTasks = _service.GetAllTasks();
            TasksListView.ItemsSource = _currentTasks;
            UpdateStatistics();
        }

        private void RefreshTaskList()
        {
            TasksListView.ItemsSource = null;
            TasksListView.ItemsSource = _currentTasks;
            UpdateStatistics();
        }

        private void UpdateStatistics()
        {
            var stats = _service.GetStatistics();
            StatisticsText.Text = $"Всего: {stats.total} | Завершено: {stats.completed} | " +
                                 $"Просрочено: {stats.overdue}";
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
                {
                    MessageBox.Show("Введите название задачи!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    TitleTextBox.Focus();
                    return;
                }

                Priority priority = Priority.Medium;
                var priorityItem = PriorityComboBox.SelectedItem as ComboBoxItem;
                if (priorityItem != null)
                {
                    string tag = priorityItem.Tag?.ToString() ?? "Medium";
                    if (tag == "Low")
                        priority = Priority.Low;
                    else if (tag == "High")
                        priority = Priority.High;
                }

                var task = new Task(
                    TitleTextBox.Text,
                    DescriptionTextBox.Text,
                    priority,
                    DueDatePicker.SelectedDate ?? DateTime.Now.AddDays(7)
                );

                _service.AddTask(task);
                _service.SaveToFile();

                LoadTasks();
                ClearInputFields();

                MessageBox.Show("Задача добавлена!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedTask = TasksListView.SelectedItem as Task;
            if (selectedTask == null)
            {
                MessageBox.Show("Выберите задачу для редактирования!", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var editWindow = new EditTaskWindow(selectedTask, _service);
            editWindow.Owner = this;

            if (editWindow.ShowDialog() == true)
            {
                _service.SaveToFile();
                LoadTasks();
                UpdateStatistics();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedTask = TasksListView.SelectedItem as Task;
            if (selectedTask == null)
            {
                MessageBox.Show("Выберите задачу для удаления!", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show($"Удалить задачу '{selectedTask.Title}'?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _service.DeleteTask(selectedTask.Id);
                _service.SaveToFile();
                LoadTasks();
                UpdateStatistics();
            }
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (SearchTextBox == null) return; 

            var filterItem = FilterComboBox.SelectedItem as ComboBoxItem;
            string filter = filterItem?.Tag?.ToString() ?? "All";
            string searchTerm = SearchTextBox.Text;

            List<Task> tasks = _service.GetAllTasks();

            if (filter == "New")
                tasks = tasks.Where(t => t.Status == TaskStatus.New).ToList();
            else if (filter == "InProgress")
                tasks = tasks.Where(t => t.Status == TaskStatus.InProgress).ToList();
            else if (filter == "Completed")
                tasks = tasks.Where(t => t.Status == TaskStatus.Completed).ToList();
            else if (filter == "Important")
                tasks = tasks.Where(t => t.IsImportant).ToList();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string searchLower = searchTerm.ToLower();
                tasks = tasks.Where(t =>
                    t.Title.ToLower().Contains(searchLower) ||
                    t.Description.ToLower().Contains(searchLower)
                ).ToList();
            }

            _currentTasks = tasks;
            RefreshTaskList();
        }

        private void TasksListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            EditButton_Click(sender, null);
        }

        private void SortPriorityButton_Click(object sender, RoutedEventArgs e)
        {
            _currentTasks = _service.SortByPriority();
            RefreshTaskList();
        }

        private void SortDueDateButton_Click(object sender, RoutedEventArgs e)
        {
            _currentTasks = _service.SortByDueDate();
            RefreshTaskList();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadTasks();
            ClearInputFields();
        }

        private void ClearInputFields()
        {
            TitleTextBox.Clear();
            DescriptionTextBox.Clear();
            PriorityComboBox.SelectedIndex = 1;
            DueDatePicker.SelectedDate = DateTime.Now;
        }
    }
}