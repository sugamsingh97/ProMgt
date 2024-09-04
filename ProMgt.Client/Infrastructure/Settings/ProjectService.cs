namespace ProMgt.Client.Infrastructure.Settings
{
    public class ProjectService
    {
        private bool _isCompleted;
        public Func<Task> OnTaskCompletedChanged;
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
    }
}
