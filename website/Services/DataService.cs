using Microsoft.EntityFrameworkCore;
using website.Models;

namespace website.Services
{
    public class DataService
    {
        private readonly AppDbContext _context;

        public DataService()
        {
            _context = new();
            _context.Database.EnsureCreated(); // Ensure the database is created
        }

        public async Task<bool> IsUserAllowedAsync(string userId, string userName)
        {
            return await _context.AllowedUsers.AnyAsync(u => u.Id == userId && u.Username == userName);
        }

        public bool IsUserAllowed(string userId, string userName)
        {
            return  _context.AllowedUsers.Any(u => u.Id == userId && u.Username == userName);
        }

        public async Task<List<MusicProject>> LoadMusicProjectsAsync()
        {
            return await _context.MusicProjects.ToListAsync();
        }

        public async Task<List<CodeProject>> LoadCodeProjectsAsync()
        {
            return await _context.CodeProjects.ToListAsync();
        }

        public async Task AddOrUpdateMusicProjectAsync(MusicProject project)
        {
            var existingProject = await _context.MusicProjects.FindAsync(project.Id);
            if (existingProject != null)
            {
                _context.Entry(existingProject).CurrentValues.SetValues(project);
            }
            else
            {
                await _context.MusicProjects.AddAsync(project);
            }
            await _context.SaveChangesAsync();
        }

        public async Task AddOrUpdateCodeProjectAsync(CodeProject project)
        {
            var existingProject = await _context.CodeProjects.FindAsync(project.Id);
            if (existingProject != null)
            {
                _context.Entry(existingProject).CurrentValues.SetValues(project);
            }
            else
            {
                await _context.CodeProjects.AddAsync(project);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<AboutEntry?> LoadAboutEntriesByTypeAsync(AboutType type)
        {
            return await _context.AboutEntries
                .FirstOrDefaultAsync(entry => entry.Type == type);
        }

        public async Task AddOrUpdateAboutEntryAsync(AboutEntry entry)
        {
            var existingEntry = await _context.AboutEntries.FindAsync(entry.Id);
            if (existingEntry != null)
            {
                _context.Entry(existingEntry).CurrentValues.SetValues(entry);
            }
            else
            {
                await _context.AboutEntries.AddAsync(entry);
            }
            await _context.SaveChangesAsync();
        }

        // New method to delete a music project
        public async Task DeleteMusicProjectAsync(string id)
        {
            var projectToDelete = await _context.MusicProjects.FindAsync(id);
            if (projectToDelete != null)
            {
                _context.MusicProjects.Remove(projectToDelete);
                await _context.SaveChangesAsync();
            }
        }

        // New method to delete a code project
        public async Task DeleteCodeProjectAsync(string id)
        {
            var projectToDelete = await _context.CodeProjects.FindAsync(id);
            if (projectToDelete != null)
            {
                _context.CodeProjects.Remove(projectToDelete);
                await _context.SaveChangesAsync();
            }
        }
    }
}