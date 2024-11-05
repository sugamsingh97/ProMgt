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

        public Func<Task> OnTaskCompletedChanged;
        public Func<Task> OnDrawerOpenChange;
        public event Func<Task> OnFieldChanges;
        public event Func<Task> OnProjectNameUpdated;
        public Func<Task> OnNewProjectAddedChanged;

        public bool IsCompleted()
        {
            return _isCompleted;
        }
        public async void ToggleTaskCompleted()
        {
            _isCompleted = !_isCompleted;           

            if (OnTaskCompletedChanged != null)
            {
                await OnTaskCompletedChanged.Invoke();
            }
        }

        private bool _isDrawerOpen = false;

        public bool IsDrawerOpen()
        {
            return _isDrawerOpen;
        }

        public async void ToggleDrawer()
        {
            _isDrawerOpen = !_isDrawerOpen;

            if (OnDrawerOpenChange != null)
            {
                await OnDrawerOpenChange.Invoke();
            }

        }

        // New Field updated
        public async Task TriggerFieldChanges()
        {
            if (OnFieldChanges != null)
            {
                await OnFieldChanges.Invoke();
            }
        }

        // Update Project List

        public async Task TriggerProjectNameUpdated()
        {
            if (OnProjectNameUpdated != null)
            {
                await OnProjectNameUpdated.Invoke();
            }
        }

        //project added

        public string errorMessage { get; set; } = string.Empty;

        /// <summary>
        /// This creates new project.
        /// </summary>
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

        // Open Task info drawer
        public Func<int, int, Task> OnTaskInfoDrawerOpen;
        public Func<Task> OnTaskInfoDrawerClose;
        public async Task OpenTaskInfoDrawer(int projectId, int taskId)
        {
            if (OnTaskInfoDrawerOpen != null)
            {
                await OnTaskInfoDrawerOpen.Invoke( projectId, taskId);
            }
        }

        public async Task CloseTaskInfoDrawer()
        {
            if (OnTaskInfoDrawerClose != null)
            {
                await OnTaskInfoDrawerClose.Invoke();
            }
        }

        //New task added
        public Func<Task> OnNewTaskAddedChanged;

        /// <summary>
        /// This creates new task.
        /// </summary>
        /// <param name="ProjectId"></param>
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

        /// <summary>
        /// This is where a new section is created
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

        // Refresh the page
        public Func<Task> TaskInforUpdated;

        public async Task TaskInfoIsUpdated()
        {
            if (TaskInforUpdated != null)
            {
                await TaskInforUpdated.Invoke();
            }
        }

        // Info changed
        public Func<Task> TaskFieldUpdated;

        public async Task TaskFieldIsUpdated()
        {
            if (TaskFieldUpdated != null)
            {
                await TaskFieldUpdated.Invoke();
            }
        }

        // Delete Project

        public Func<Task> OnProjectDeleted;

        public async Task<bool>  DeleteProject(int ProjectId, string ProjectName)
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

        public Func<Task> OnTaskDeleted;

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
    }
}
