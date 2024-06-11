using TaskManager_Simplex_.Models;


namespace TaskManager_Simplex_.Repositories.Interface
{
    public interface ITaskRepo
    {
        Task<IEnumerable<Tasks>> GetAllTasksAsync();
        Task<Tasks> GetTaskByIdAsync(int id);
        Task<Tasks> CreateTaskAsync(Tasks task);
        Task<Tasks> UpdateTaskAsync(Tasks task);
        Task<Tasks> DeleteTaskAsync(int id);

    }
}
