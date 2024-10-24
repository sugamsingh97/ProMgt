using ProMgt.Client.Models.Task;

namespace ProMgt.Client.Infrastructure.HelperFunctions
{
    /// <summary>
    /// Task related utilities
    /// </summary>
    public static class TaskHelper
    {
        /// <summary>
        /// This Takes the list of Task and returns the percentage of tasks completed.
        /// </summary>
        /// <param name="tasks">Pass task list.</param>
        /// <returns></returns>
        public static int GetProgress(List<TaskDisplay> tasks)
        {
            var tasksList = tasks;
            int _totalTasks = tasksList.Count;
            int _completedTasks = 0;

            foreach (var item in tasksList)
            {
                if (item.IsCompleted)
                {
                    _completedTasks++;
                }
            }

            int _progressNumber = (_totalTasks > 0) ? (_completedTasks * 100) / _totalTasks : 0;

            return _progressNumber;
        }

        /// <summary>
        /// This converts List<TaskResponse> to List<TaskDisplay>.
        /// </summary>
        /// <param name="tasks">Pass the List<TaskResponse>.</param>
        /// <returns></returns>
        public static List<TaskDisplay> ConvertToTaskDisplay(List<TaskResponse> tasks)
        {
            List<TaskDisplay> localList = new List<TaskDisplay>();
            foreach (var task in tasks)
            {
                localList.Add(new TaskDisplay
                {
                    Id = task.Id,
                    Name = task.Name,
                    DeadLine = task.DeadLine,
                    IsCompleted = task.IsCompleted,
                    Description = task.Description,
                    DateOfCreation = task.DateOfCreation
                });
            }
            return localList;
        }
    }
}
