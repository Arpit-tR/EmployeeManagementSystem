using Microsoft.AspNetCore.Mvc;
using EmployeeManagementSystem.Data;

namespace EmployeeManagementSystem.Controllers
{
    public class HomeController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

                public IActionResult Index()
        {
            ViewBag.TotalEmployees = _context.Employees.Count();
            ViewBag.TotalDepartments = _context.Departments.Count();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}