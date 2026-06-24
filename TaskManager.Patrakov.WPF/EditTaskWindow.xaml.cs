using System;
using System.Windows;
using System.Windows.Controls;
using TaskManager.Patrakov.Core.Models;

namespace TaskManager.Patrakov.WPF
{
    public partial class EditTaskWindow : Window
    {
        private Task _task;
        private TaskManagerService _service;

        public EditTaskWindow(Task task, TaskManagerService service)
        {
            InitializeComponent();
            _task = task;
            _service = service;
            LoadTaskData();
        }

        private void LoadTaskData()
        {
            TitleTextBox.Text = _task.Title;
            DescriptionTextBox.Text = _task.Description;
            ImportantCheckBox.IsChecked = _task.IsImportant;
            DueDatePicker.SelectedDate = _task.DueDate;

            foreach (ComboBoxItem item in PriorityComboBox.Items)
            {
                string tag = item.Tag?.ToString() ?? "";
                if (tag == _task.Priority.ToString())
                {
                    PriorityComboBox.SelectedItem = item;
                    break;
                }
            }

            foreach (ComboBoxItem item in StatusComboBox.Items)
            {
                string tag = item.Tag?.ToString() ?? "";
                if (tag == _task.Status.ToString())
                {
                    StatusComboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
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

                TaskStatus status = TaskStatus.New;
                var statusItem = StatusComboBox.SelectedItem as ComboBoxItem;
                if (statusItem != null)
                {
                    string tag = statusItem.Tag?.ToString() ?? "New";
                    if (tag == "InProgress")
                        status = TaskStatus.InProgress;
                    else if (tag == "Completed")
                        status = TaskStatus.Completed;
                }

                _task.Title = TitleTextBox.Text;
                _task.Description = DescriptionTextBox.Text;
                _task.Priority = priority;
                _task.DueDate = DueDatePicker.SelectedDate ?? DateTime.Now.AddDays(7);
                _task.Status = status;
                _task.IsImportant = ImportantCheckBox.IsChecked ?? false;

                _service.UpdateTask(_task.Id, _task);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}