using TaskManager_Simplex_.Context;
using TaskManager_Simplex_.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace TaskManager_Simplex_.Repositories.Implementation
{
    public class TaskRepo : ITaskRepo
    {
        private readonly AppDbContext _context;

        public TaskRepo(AppDbContext context)
        {
            _context = context;
        }

        //Get All Tasks
        public async Task<IEnumerable<Models.Tasks>> GetAllTasksAsync()
        {
            return await _context.Tasks.ToListAsync();
        }

        //Get Tasks By Id
        public async Task<Models.Tasks> GetTaskByIdAsync(int id)
        {
            return await _context.Tasks.FindAsync(id);
        }

        //Create New Tasks
        public async Task<Models.Tasks> CreateTaskAsync(Models.Tasks task)
        {
            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();
            return task;
        }

        //Update Tasks By Id
        public async Task<Models.Tasks> UpdateTaskAsync(Models.Tasks task)
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
            return task;
        }

        //Delete Task By Id
        public async Task<Models.Tasks> DeleteTaskAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
            }
            return task;
        }

    }
}
