using Microsoft.AspNetCore.Components;
using MudBlazor;
using ProMgt.Client.Components.Dialogs;
using ProMgt.Client.Models.Project;
using ProMgt.Client.Models.Section;
using ProMgt.Client.Models.Task;
using System.Net;
using System.Net.Http.Json;



namespace ProMgt.Client.Infrastructure.Settings
{
    public class ProjectService
    {
        private bool _isCompleted;
        private bool _isDrawerOpen = false;
        public string errorMessage { get; set; } = string.Empty;
        private readonly IDialogService _dialogService;
        private readonly HttpClient _httpClient;
        private readonly NavigationManager _navigationManager;
        private readonly ISnackbar _snackbar;

        public ProjectService(
            IDialogService dialogService, 
            HttpClient httpClient, 
            NavigationManager navigationManager, 
            ISnackbar snackbar)
        {
            _dialogService = dialogService;
            _httpClient = httpClient;
            _navigationManager = navigationManager;
            _snackbar = snackbar;
        }

        public event Func<Task> TaskInforUpdated;
        public event Func<Task> OnTaskCompletedChanged;
        public event Func<Task> OnDrawerOpenChange;
        public event Func<Task> OnFieldChanges;
        public event Func<Task> OnProjectNameUpdated;
        public event Func<Task> OnNewProjectAddedChanged;
        public event Func<int, int, Task> OnTaskInfoDrawerOpen;
        public event Func<Task> OnTaskInfoDrawerClose;
        public event Func<Task> OnNewTaskAddedChanged;
        public event Func<Task> TaskFieldUpdated;
        public event Func<Task> OnProjectDeleted;
        public event Func<Task> OnTaskDeleted;  

        #region Project
        // This creates a new project.
        public async void CreateNewProject()
        {
            errorMessage = string.Empty;
            var options = new DialogOptions
            {
                CloseOnEscapeKey = true,
                CloseButton = true,
                MaxWidth = MaxWidth.Small,
                FullWidth = true
            };

            var dialogResponse = await _dialogService.ShowAsync<CreateProjectDialog>("Create new Project", options);

            var result = await dialogResponse.Result;
            ProjectCreateModel dialogResponseProject = new();


            if (result != null && !result.Canceled)
            {

                dialogResponseProject = result.Data as ProjectCreateModel ?? new ProjectCreateModel();

            }

            ProjectInputModel _projectInputModel = new()
            {
                Name = dialogResponseProject.Name,
                Description = dialogResponseProject.Description,
                DeadLine = dialogResponseProject.DeadLine

            };
            try
            {
                var response = await _httpClient.PostAsJsonAsync<ProjectInputModel>("api/project", _projectInputModel);
                if (response.IsSuccessStatusCode)
                {
                    var createdProjectResponse = await response.Content.ReadFromJsonAsync<ProjectResponse>();

                    /*
                     * When a Project is successfully created
                     * 3 Sections are created.
                     */
                    string[] sectionNames = { "TO DO", "DOING", "DONE" };

                    foreach (var sectionName in sectionNames)
                    {
                        await CreateSection(new SectionInputModel
                        {
                            ProjectId = createdProjectResponse.Id,
                            Name = sectionName
                        });
                    }

                    if (OnNewProjectAddedChanged != null)
                    {
                        await OnNewProjectAddedChanged.Invoke();
                    }
                }
                else
                {
                    errorMessage = $"Error: {response.ReasonPhrase}";
                }

            }
            catch (HttpRequestException httpEx)
            {
                errorMessage = $"Request error: {httpEx.Message}";
            }
            catch (Exception ex)
            {
                errorMessage = $"An unexpected error occurred: {ex.Message}";
            }
        }

        // This invokes event when Project name is updated.
        public async Task TriggerProjectNameUpdated()
        {
            if (OnProjectNameUpdated != null)
            {
                await OnProjectNameUpdated.Invoke();
            }
        }

        // This invokes an event when a project is deleted.
        public async Task<bool> DeleteProject(int ProjectId, string ProjectName)
        {
            errorMessage = string.Empty;
            var options = new DialogOptions
            {
                CloseOnEscapeKey = true,
                CloseButton = true,
                MaxWidth = MaxWidth.Small,
                FullWidth = true
            };

            var DialogResponse = await _dialogService.ShowAsync<ConfirmProjectDelete>($"Are you sure you want to delete \"{ProjectName}\" project?", options);

            var result = await DialogResponse.Result;           

            if (result != null && !result.Canceled)
            {                              

                try
                {
                   
                    var response = await _httpClient.DeleteAsync($"api/project/{ProjectId}");
                    

                    if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        if (OnProjectDeleted != null)
                        {
                            await OnProjectDeleted.Invoke();
                        }
                       return true;
                    }
                    else
                        return false;
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Request error: {ex.Message}");
                    return false;
                }
                catch (NotSupportedException ex)
                {
                    Console.WriteLine($"Content type error: {ex.Message}");
                    return false;

                }
               
            }
            else
                return false;
            
        }
        #endregion

        #region Task
        // This creates a new task.
        public async void CreateNewTask(int ProjectId)
        {
            errorMessage = string.Empty;
            var options = new DialogOptions
            {
                CloseOnEscapeKey = true,
                CloseButton = true,
                MaxWidth = MaxWidth.Small,
                FullWidth = true
            };

            var DialogResponse = await _dialogService.ShowAsync<AddTaskDialog>("Add task dialog", options);

            var result = await DialogResponse.Result;

            TaskInputModel newTask = new();

            if (result != null && !result.Canceled)
            {

                newTask = result.Data as TaskInputModel ?? new();
                newTask.ProjectId = ProjectId;
            }

            try
            {
                var defaultSection = await _httpClient.GetFromJsonAsync<SectionResponse>($"api/section/projectdefaultsection/{ProjectId}");

                newTask.SectionId = defaultSection.Id;

                var response = await _httpClient.PostAsJsonAsync<TaskInputModel>("/api/task/createtask", newTask);
                if (response.IsSuccessStatusCode)
                {

                    if (OnNewTaskAddedChanged != null)
                    {
                        await OnNewTaskAddedChanged.Invoke();
                    }
                }
                else
                {
                    errorMessage = $"Error: {response.ReasonPhrase}";
                }
            }
            catch (HttpRequestException httpEx)
            {
                errorMessage = $"Request error: {httpEx.Message}";
            }
            catch (Exception ex)
            {
                errorMessage = $"An unexpected error occurred: {ex.Message}";
            }
        }

        // This invokes an event when task status is changed.
        public async void ToggleTaskCompleted()
        {
            _isCompleted = !_isCompleted;

            if (OnTaskCompletedChanged != null)
            {
                await OnTaskCompletedChanged.Invoke();
            }
        }

        // This invokes an event when task info is updated.
        public async Task TaskInfoIsUpdated()
        {
            if (TaskInforUpdated != null)
            {
                await TaskInforUpdated.Invoke();
            }
        }

        // This invokes an event when a field is updated.
        public async Task TaskFieldIsUpdated()
        {
            if (TaskFieldUpdated != null)
            {
                await TaskFieldUpdated.Invoke();
            }
        }

        // This invokes an event after field changes.
        public async Task TriggerFieldChanges()
        {
            if (OnFieldChanges != null)
            {
                await OnFieldChanges.Invoke();
            }
        }

        // Checking if drawer is open.
        public bool IsDrawerOpen()
        {
            return _isDrawerOpen;
        }

        // This opens fields drawer.
        public async void ToggleDrawer()
        {
            _isDrawerOpen = !_isDrawerOpen;

            if (OnDrawerOpenChange != null)
            {
                await OnDrawerOpenChange.Invoke();
            }

        }

        // This opens task info drawer.
        public async Task OpenTaskInfoDrawer(int projectId, int taskId)
        {
            if (OnTaskInfoDrawerOpen != null)
            {
                await OnTaskInfoDrawerOpen.Invoke(projectId, taskId);
            }
        }

        // This closes task info drawer.
        public async Task CloseTaskInfoDrawer()
        {
            if (OnTaskInfoDrawerClose != null)
            {
                await OnTaskInfoDrawerClose.Invoke();
            }
        }

        // This envokes an event when a task is deleted.
        public async Task<bool> DeleteTask(int TaskId, string TaskName)
        {
            errorMessage = string.Empty;
            var options = new DialogOptions
            {
                CloseOnEscapeKey = true,
                CloseButton = true,
                MaxWidth = MaxWidth.Small,
                FullWidth = true
            };

            var DialogResponse = await _dialogService.ShowAsync<ConfirmTaskDelete>($"Are you sure you want to delete \"{TaskName}\" Task?", options);

            var result = await DialogResponse.Result;

            if (result != null && !result.Canceled)
            {

                try
                {

                    var response = await _httpClient.DeleteAsync($"api/task/{TaskId}");


                    if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        if (OnTaskDeleted != null)
                        {
                            await OnTaskDeleted.Invoke();
                        }
                        return true;
                    }
                    else
                        return false;
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Request error: {ex.Message}");
                    return false;
                }
                catch (NotSupportedException ex)
                {
                    Console.WriteLine($"Content type error: {ex.Message}");
                    return false;

                }

            }
            else
                return false;

        }
        #endregion

        #region Section
        /// <summary>
        /// This creates a new section.
        /// </summary>
        /// <param name="newSection"></param>
        /// <returns></returns>
        public async Task CreateSection(SectionInputModel newSection)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync<SectionInputModel>("api/section/createsection", newSection);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("New SectionCreated");
                }
                else
                {
                    errorMessage = $"Error: {response.ReasonPhrase}";
                }

            }
            catch (HttpRequestException httpEx)
            {
                errorMessage = $"Request error: {httpEx.Message}";
            }
            catch (Exception ex)
            {
                errorMessage = $"An unexpected error occurred: {ex.Message}";
            }
        }
        #endregion
    }
}
