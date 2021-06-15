using ProjectManagementSystem.Models;
using System.Collections.Generic;


namespace ProjectManagementSystem.Repositories.Interfaces
{
    public interface IProjectRepository
    {
        IEnumerable<Project> GetAllProjects();
        IEnumerable<Project> GetProjectsForProjectManager(string pmId);
        public IEnumerable<Project> GetProjectsForDeveloper(string devId);
        Project GetProjectById(int id);
        void CreateProject(Project project);
        void UpdateProject(Project project);
        void DeleteProject(int id);
    }
}
