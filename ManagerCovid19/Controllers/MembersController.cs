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
using ManagerCovid19;

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
        public async Task<IActionResult> Index(string search, int? page)
        {
            int pageSize = 50;
            ViewBag.Page = page;

            ViewBag.filter = search;

            var members = from m in _context.Member
                          select m;

            if (!String.IsNullOrEmpty(search)) members = members.Where(m => m.Name.Contains(search)
                                                                         || m.MemberRegistrationNumber.Contains(search)
                                                                         || m.City.Contains(search)
                                                                         || m.State.Contains(search));

            /*ViewBag.sort = String.IsNullOrEmpty(sort) ? "" : sort;
            ViewBag.order = String.IsNullOrEmpty(order) ? "" : order;
            

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
*/

            return View(PaginatedList<Member>.Create(members, page ?? 1, pageSize));
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
            if (Utils.fBrowserIsMobile(Request)) return Forbid();
            return View();
        }

        // POST: Members/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MemberRegistrationNumber,Name,Sector,Password,BirthDate,City,State")] Member member)
        {
            if (Utils.fBrowserIsMobile(Request)) return Forbid();

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
            if (Utils.fBrowserIsMobile(Request)) return Forbid();

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
            if (Utils.fBrowserIsMobile(Request)) return Forbid();

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
            if (Utils.fBrowserIsMobile(Request)) return Forbid();

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
            if (Utils.fBrowserIsMobile(Request)) return Forbid();

            var member = await _context.Member.FindAsync(id);
            _context.Member.Remove(member);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Import()
        {
            if (Utils.fBrowserIsMobile(Request)) return Forbid();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Import(IFormFile file)
        {
            if (Utils.fBrowserIsMobile(Request)) return Forbid();

            if (file == null) return RedirectToAction(nameof(Index));
            var path = Path.GetTempFileName();
            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                file.CopyTo(fileStream);
            }

            string csvData = System.IO.File.ReadAllText(path);

            foreach (string row in csvData.Split("\r\n"))
            {
                if (!string.IsNullOrEmpty(row))
                {
                    string[] values = row.Split(",");
                    Member m = new Member();

                    m.MemberRegistrationNumber = values[0];
                    m.Name = values[1];
                    m.Sector = (Member.Sectors) int.Parse(values[2]);
                    m.Password = values[3];
                    m.BirthDate = Convert.ToDateTime(values[4]);
                    m.City = values[5];
                    m.State= values[6];

                    _context.Add(m);
                }
            }

            await _context.SaveChangesAsync();

            /*string conn = String.Empty;
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
            ViewData["debug"] = xtable.Rows[0][0];*/
            return RedirectToAction(nameof(Index));
        }

        private bool MemberExists(string id)
        {
            return _context.Member.Any(e => e.MemberRegistrationNumber == id);
        }
    }
}
