using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectManagementSystem.Models;
using ProjectManagementSystem.Repositories.Interfaces;
using ProjectManagementSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using static ProjectManagementSystem.Helpers.Enums.Enums;

namespace ProjectManagementSystem.Controllers
{
    public class ProjectController : Controller
    {
        private readonly IProjectRepository _repository;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;


        public ProjectController(IProjectRepository projectRepository, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _repository = projectRepository;
            _mapper = mapper;
            _userManager = userManager;
        }

        [Authorize(Roles = "Administrator, ProjectManager, Developer")]
        public ActionResult Projects()
        {
            var projects = new List<Project>();

            var loggedInUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInURoleName = _userManager.Users.FirstOrDefault(x => x.Id == loggedInUser).RoleName;

            if (loggedInURoleName == Role.Administrator.ToString())
            {
                projects = _repository.GetAllProjects().ToList();
            }
            else if (loggedInURoleName == Role.ProjectManager.ToString())
            {
                projects = _repository.GetProjectsForProjectManager(loggedInUser).ToList();
            }
            else if (loggedInURoleName == Role.Developer.ToString())
            {
                projects = _repository.GetProjectsForDeveloper(loggedInUser).ToList();
            }

            return View(projects);
        }


        public ActionResult Details(int id)
        {
            var project = _repository.GetProjectById(id);
            return View(project);
        }


        [Authorize(Roles = "Administrator, ProjectManager")]
        public ActionResult Create()
        {
            var projectViewModel = new ProjectViewModel();
            AddProjManagersToProjectViewModel();
            return View(projectViewModel);
        }

        [Authorize(Roles = "Administrator, ProjectManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProjectViewModel project)
        {
            if (ModelState.IsValid)
            {
                Console.WriteLine(project.ProjectName);

                var newProj = new Project();

                newProj.ProjectName = project.ProjectName;
                newProj.ProjectManagerId = project.ProjectManagerId;


                _repository.CreateProject(newProj);

                return RedirectToAction("Projects");
            }

            return View();
        }


        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var project = _repository.GetProjectById(id);

            var projectViewModel = _mapper.Map<ProjectViewModel>(project);

            return View(projectViewModel);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public ActionResult Edit(ProjectViewModel project)
        {
            if (ModelState.IsValid)
            {
                var newProject = _mapper.Map<Project>(project);

                _repository.UpdateProject(newProject);

                return RedirectToAction("Projects");
            }
            else
            {
                return View(project);
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var project = _repository.GetProjectById(id);
            return View(project);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _repository.DeleteProject(id);

            return RedirectToAction("Projects");
        }

        private void AddProjManagersToProjectViewModel()
        {
            var projectManagers = _userManager.Users.Where(u => u.RoleName.Equals(Role.ProjectManager.ToString())).ToList();

            ProjectViewModel.ProjectManagers = new List<SelectListItem>();

            foreach (var pm in projectManagers)
            {
                ProjectViewModel.ProjectManagers.Add(new SelectListItem(text: pm.Name + " " + pm.Surname, value: pm.Id));
            }
        }

    }
}
