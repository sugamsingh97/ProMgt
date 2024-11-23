using CsvHelper.TypeConversion;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using ProMgt.Client.Components.Dialogs;
using ProMgt.Client.Models.Assignments;
using ProMgt.Client.Models.Project;
using ProMgt.Client.Models.Section;
using ProMgt.Client.Models.Task;
using ProMgt.Client.Models.User;
using System;
using System.ComponentModel;
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
        private readonly AuthenticationStateProvider _autStateProvider;
        public ProjectService(
            IDialogService dialogService,
            HttpClient httpClient,
            NavigationManager navigationManager,
            AuthenticationStateProvider autStateProvider
            )
        {
            _dialogService = dialogService;
            _httpClient = httpClient;
            _navigationManager = navigationManager;
            _autStateProvider = autStateProvider;
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
            var authState = await _autStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var IsAuthenticated = user.Identity.IsAuthenticated;

            if (IsAuthenticated == false)
            {
                _navigationManager.NavigateTo("/signin");
                return;
            }

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

        // This invokes an event when task asignee is updated.

        public event Func<Task> OnTaskAssignementChanged;

        public async Task TaskAsigneeIsUpdated()
        {
            if (OnTaskAssignementChanged != null)
            {
                await OnTaskAssignementChanged.Invoke();
            }
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

        #region Users
        public async Task<List<UserResponse>> GetUserList()
        {
            try
            {


                var users = await _httpClient.GetFromJsonAsync<List<UserResponse>>("api/project/getAppUserList");


                if (users != null)
                {
                    return users;
                }

                else
                {
                    return new List<UserResponse>();
                    // Return an empty list if users is null
                }
            }

            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request error: {ex.Message}");
            }
            catch (NotSupportedException ex)
            {
                Console.WriteLine($"Content type error: {ex.Message}");
            }
            return new List<UserResponse>();
        }

        //Add contact
        public event Func<Task> ContactChanges;

        public async Task AddNewContact(UserResponse user)
        {
            var parameters = new DialogParameters { ["User"] = user };
            var dialog = await _dialogService.ShowAsync<AddContactDialog>("User Details", parameters);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                var addedUser = (UserResponse)result.Data;
                await HandleAddUser(addedUser);
            }
        }

        private async Task HandleAddUser(UserResponse user)
        {
            try
            {
                ContactResponse newContact = new ContactResponse
                {
                    ContactUserId = user.UserId
                };
                var response = await _httpClient.PostAsJsonAsync<ContactResponse>("api/contact", newContact);
                if (ContactChanges != null)
                {
                    await ContactChanges.Invoke();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<ContactDisplay>> GetUserContactList()
        {
            List<ContactDisplay> contactList = new();
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<ContactDisplay>>("api/contact/getallcontact");

                contactList = (response ?? new List<ContactDisplay>())
                                .ToList();
                return contactList;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request error: {ex.Message}");
            }
            catch (NotSupportedException ex)
            {
                Console.WriteLine($"Content type error: {ex.Message}");
            }
            return contactList;
        }

        public async Task<bool> DeleteContact(int _contactId)
        {
            try
            {

                var response = await _httpClient.DeleteAsync($"api/contact/{_contactId}");


                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    if (ContactChanges != null)
                    {
                        await ContactChanges.Invoke();
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

        #endregion

        #region MemberAssignments
        public event Func<Task> OnNewMemberAdded;

        /// <summary>
        /// This assignes member to a project
        /// </summary>
        /// <param name="ProjectId"></param>
        public async void AddMember(int ProjectId)
        {
            errorMessage = string.Empty;
            var options = new DialogOptions
            {
                CloseOnEscapeKey = true,
                CloseButton = true,
                MaxWidth = MaxWidth.Small,
                FullWidth = true
            };

            var DialogResponse = await _dialogService.ShowAsync<AddMemberDialog>("Add Member", options);

            var result = await DialogResponse.Result;

            ProjectAssignedInputModel newMemberAssignment = new();

            if (result != null && !result.Canceled)
            {
                newMemberAssignment = result.Data as ProjectAssignedInputModel ?? new();
                // adding the ProjectId to the object received with asignee Id.

                newMemberAssignment.ProjectId = ProjectId;
            }

            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/projectassignment", newMemberAssignment);

                if (response.IsSuccessStatusCode)
                {
                    if (OnNewMemberAdded != null)
                    {
                        await OnNewMemberAdded.Invoke();
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
        /// This Gets users for a perticular project
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <returns></returns>
        public async Task<List<UserResponse>> GetProjectMembers(int ProjectId)
        {
            List<UserResponse> Members = new();
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<UserResponse>>($"/api/projectassignment/getmembers/{ProjectId}");
                if (response != null) 
                { 
                    Members = response; 
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

            return Members;
        }

        public async Task<bool> DeleteAssignment(string _asigneeId)
        {
            try
            {
                //var response = await _httpClient.DeleteAsync($"/api/taskassignment/{_asigneeId}");

                var response = await _httpClient.DeleteAsync($"api/taskassignment/asignee/{_asigneeId}");

                if (response.StatusCode == HttpStatusCode.NoContent)
                {                    
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

        #endregion
    }
}
