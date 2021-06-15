using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using ProjectManagementSystem.Models;
using ProjectManagementSystem.Repositories.Interfaces;
using ProjectManagementSystem.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using static ProjectManagementSystem.Helpers.Enums.Enums;

namespace ProjectManagementSystem.Controllers
{
    public class TaskController : Controller
    {
        private readonly ITaskRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;


        public TaskController(ITaskRepository repository, IUserRepository userRepository, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userRepository = userRepository;
            _mapper = mapper;
            _userManager = userManager;

            SetStatusAndDevelopers();
        }

        [Authorize(Roles = "Administrator, ProjectManager, Developer")]
        public ActionResult Tasks(int id)
        {
            var tasks = new List<Task>();

            var loggedInUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInURoleName = _userManager.Users.FirstOrDefault(x => x.Id == loggedInUser).RoleName;

            if (loggedInURoleName == Role.Administrator.ToString() || loggedInURoleName == Role.ProjectManager.ToString())
            {
                tasks = _repository.GetTasks(id).ToList();
            }
            else if (loggedInURoleName == Role.Developer.ToString())
            {
                tasks = _repository.GetTasksForUser(loggedInUser, id).ToList();
            }

            return View(tasks);
        }

        [Authorize(Roles = "Administrator, ProjectManager")]
        public ActionResult Create(int id)
        {
            var taskViewModel = CreateTaskViewModel(id);

            return View(taskViewModel);
        }

        [Authorize(Roles = "Administrator, ProjectManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TaskViewModel task)
        {
            if (ModelState.IsValid)
            {

                var newTask = new Task();

                newTask.Status = task.Status ?? Status.New.ToString();

                newTask.Progress = 0;
                newTask.Deadline = task.Deadline;
                newTask.Description = task.Description;

                if (task.AssigneeId != null)
                {
                    newTask.AssigneeId = task.AssigneeId;
                }

                newTask.ProjectCode = task.ProjectCode;

                _repository.CreateTask(newTask);

                return RedirectToAction("Tasks", new RouteValueDictionary(
                   new { controller = "Task", action = "Tasks", id = task.ProjectCode }));
            }
            return View(task);
        }

        [Authorize(Roles = "Administrator, ProjectManager, Developer")]
        [HttpGet]
        public ActionResult Edit(int id)
        {

            var task = _repository.GetTaskById(id);

            var taskViewModel = _mapper.Map<TaskViewModel>(task);

            return View(taskViewModel);
        }

        [Authorize(Roles = "Administrator, ProjectManager, Developer")]
        [HttpPost]
        public ActionResult Edit(TaskViewModel task)
        {
            if (ModelState.IsValid)
            {
                var newTask = _mapper.Map<Task>(task);

                _repository.UpdateTask(newTask);

                return RedirectToAction("Tasks", new RouteValueDictionary(
                   new { controller = "Task", action = "Tasks", id = task.ProjectCode }));

            }
            else
            {
                return View(task);
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var task = _repository.GetTaskById(id);
            return View(task);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var pc = _repository.GetTaskById(id).ProjectCode;
            _repository.DeleteTask(id);


            return RedirectToAction("Tasks", new RouteValueDictionary(
                new { controller = "Task", action = "Tasks", id = pc }));
        }


        public ActionResult Details(int id)
        {
            var task = _repository.GetTaskById(id);
            return View(task);
        }

        private TaskViewModel CreateTaskViewModel(int id)
        {
            var vm = new TaskViewModel();

            vm.ProjectCode = id;

            return vm;
        }

        private void SetStatusAndDevelopers()
        {
            var developers = _userRepository.GetAllQueryable().Where(u => u.RoleName.Equals(Role.Developer.ToString()));

            TaskViewModel.Developers = new List<SelectListItem>();

            foreach (var dev in developers)
            {
                TaskViewModel.Developers.Add(new SelectListItem(text: dev.Name + " " + dev.Surname, value: dev.Id));

            }

            TaskViewModel.Statuses = new List<SelectListItem>
            {
                new SelectListItem{Text = "New", Value = Status.New.ToString()},
                new SelectListItem{Text = "In Progress", Value = Status.InProgress.ToString()},
                new SelectListItem{Text = "Finished", Value = Status.Finished.ToString()},

            };
        }
    }
}
