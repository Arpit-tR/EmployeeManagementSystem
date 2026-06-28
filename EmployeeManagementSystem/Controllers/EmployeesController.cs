
using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
namespace EmployeeManagementSystem.Controllers
{
    public class EmployeesController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;



        // GET: EMPLOYEES
        public async Task<IActionResult> Index(string searchString)
        {
            var employees = _context.Employees
                                    .Include(e => e.Department)
                                    .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                employees = employees.Where(e =>
                    e.Name.Contains(searchString) ||
                    e.Email.Contains(searchString) ||
                    e.Department.DepartmentName.Contains(searchString));
            }

            return View(await employees.ToListAsync());
        }









        // GET: EMPLOYEES/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
            .Include(e => e.Department)
            .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: EMPLOYEES/Create
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Departments,
                                                      "DepartmentId",
                                                      "DepartmentName");
            return View();
        }



        // POST: EMPLOYEES/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,Name,Email,Mobile,Address,Gender,JoiningDate,DepartmentId")] Employee employee)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(employee);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Employee added successfully.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while saving the employee.");
            }

            ViewData["DepartmentId"] = new SelectList(
                _context.Departments,
                "DepartmentId",
                "DepartmentName",
                employee.DepartmentId);

            return View(employee);
        }

        // GET: EMPLOYEES/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            ViewData["DepartmentId"] = new SelectList(
                _context.Departments,
                "DepartmentId",
                "DepartmentName",
                employee.DepartmentId);

            return View(employee);
        }

        // POST: EMPLOYEES/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
         [Bind("EmployeeId,Name,Email,Mobile,Address,Gender,JoiningDate,DepartmentId")] Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Employee updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(employee.EmployeeId))
                {
                    return NotFound();
                }

                ModelState.AddModelError("", "The employee record no longer exists.");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred while updating the employee.");
            }

            ViewData["DepartmentId"] = new SelectList(
                _context.Departments,
                "DepartmentId",
                "DepartmentName",
                employee.DepartmentId);

            return View(employee);
        }
        // GET: EMPLOYEES/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: EMPLOYEES/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(id);

                if (employee == null)
                {
                    return NotFound();
                }

                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Employee deleted successfully.";
            }
            catch (Exception)
            {
                TempData["Error"] = "Error while deleting employee.";
            }

            return RedirectToAction(nameof(Index));
        }




        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
        // PDF Report Action
        public IActionResult GeneratePdf()
        {
            try
            {
                var employees = _context.Employees
                    .Include(e => e.Department)
                    .ToList();

                var pdf = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(30);

                        page.Header()
                            .Text("Employee Report")
                            .FontSize(20)
                            .Bold()
                            .AlignCenter();

                        page.Content().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Cell().Text("Name").Bold();
                            table.Cell().Text("Email").Bold();
                            table.Cell().Text("Mobile").Bold();
                            table.Cell().Text("Department").Bold();

                            foreach (var emp in employees)
                            {
                                table.Cell().Text(emp.Name ?? "");
                                table.Cell().Text(emp.Email ?? "");
                                table.Cell().Text(emp.Mobile ?? "");
                                table.Cell().Text(emp.Department?.DepartmentName ?? "");
                            }
                        });
                    });
                });

                var pdfBytes = pdf.GeneratePdf();

                return File(pdfBytes,
                    "application/pdf",
                    "EmployeeReport.pdf");
            }
            catch (Exception)
            {
                TempData["Error"] = "Error while generating PDF report.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}

