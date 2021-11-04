using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ManagerCovid19.Data;
using ManagerCovid19.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data.OleDb;

namespace ManagerCovid19.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MembersController : Controller
    {
        private readonly ManagerCovid19Context _context;

        public MembersController(ManagerCovid19Context context)
        {
            _context = context;
        }

        // GET: Members
        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 10;
            ViewBag.Page = page;

            /*ViewBag.sort = String.IsNullOrEmpty(sort) ? "" : sort;
            ViewBag.order = String.IsNullOrEmpty(order) ? "" : order;
            var members = from m in _context.Member
                          select m;

            if (!String.IsNullOrEmpty(search)) members = members.Where(m => m.Name.Contains(search)
                                                                         || m.MemberRegistrationNumber.Contains(search)
                                                                         || m.City.Contains(search)
                                                                         || m.State.Contains(search));

            switch (sort)
            {
                case "MemberRegistrationNumber":
                    if (order == "D") members = members.OrderByDescending(s => s.MemberRegistrationNumber);
                    else members = members.OrderBy(s => s.MemberRegistrationNumber);
                        break;
                case "Name":
                    if (order == "D") members = members.OrderByDescending(s => s.Name);
                    else members = members.OrderBy(s => s.Name);
                    break;
                case "Sector":
                    if (order == "D") members = members.OrderByDescending(s => s.Sector);
                    else members = members.OrderBy(s => s.Sector);
                    break;
                case "BirthDate":
                    if (order == "D") members = members.OrderByDescending(s => s.BirthDate);
                    else members = members.OrderBy(s => s.BirthDate);
                    break;
                case "City":
                    if (order == "D") members = members.OrderByDescending(s => s.City);
                    else members = members.OrderBy(s => s.City);
                    break;
                case "State":
                    if (order == "D") members = members.OrderByDescending(s => s.State);
                    else members = members.OrderBy(s => s.State);
                    break;
            }

            int pageSize = 10;
            page ??= 1;*/

            return View(await _context.Member.Skip((page??1-1)*pageSize).Take(pageSize).ToListAsync());
        }

        // GET: Members/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Member
                .Include(m => m.HealthRegistrations)
                .FirstOrDefaultAsync(m => m.MemberRegistrationNumber == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // GET: Members/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Members/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MemberRegistrationNumber,Name,Sector,Password,BirthDate,City,State")] Member member)
        {
            if (ModelState.IsValid)
            {
                _context.Add(member);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        // GET: Members/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Member.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            return View(member);
        }

        // POST: Members/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MemberRegistrationNumber,Name,Sector,Password,BirthDate,City,State")] Member member)
        {
            if (id != member.MemberRegistrationNumber)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(member);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MemberExists(member.MemberRegistrationNumber))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        // GET: Members/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Member
                .FirstOrDefaultAsync(m => m.MemberRegistrationNumber == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // POST: Members/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var member = await _context.Member.FindAsync(id);
            _context.Member.Remove(member);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Import()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Import(IFormFile file)
        {
            var path = Path.GetTempFileName();
            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                file.CopyTo(fileStream);
            }
            string conn = String.Empty;
            DataTable xtable = new DataTable();
            if (file.FileName.EndsWith(".xlsx")) conn = @"provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Extended Properties='Excel 8.0;HRD=Yes;IMEX=1';";
            else conn = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties='Excel 12.0;HDR=NO';";
            using (OleDbConnection con = new OleDbConnection(conn))
            {
                //try
                //{
                    con.Open();
                    OleDbDataAdapter oleAdpt = new OleDbDataAdapter("select * from [Sheet1$]", con);
                    oleAdpt.Fill(xtable);
                    con.Close();
                //} catch { }
            }
            ViewData["debug"] = xtable.Rows[0][0];
            return View("Import");
        }

        private bool MemberExists(string id)
        {
            return _context.Member.Any(e => e.MemberRegistrationNumber == id);
        }
    }
}
