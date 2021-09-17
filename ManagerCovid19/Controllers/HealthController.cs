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
using System.Security.Claims;

namespace ManagerCovid19.Controllers
{
    public class HealthController : Controller
    {
        private readonly ManagerCovid19Context _context;

        public HealthController(ManagerCovid19Context context)
        {
            _context = context;
        }

        // GET: HealthRegistrations
        public async Task<IActionResult> Index()
        {
            var managerCovid19Context = _context.HealthRegistration.Include(h => h.Member);
            return View(await managerCovid19Context.ToListAsync());
        }

        // GET: HealthRegistrations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var healthRegistration = await _context.HealthRegistration
                .Include(h => h.Member)
                .FirstOrDefaultAsync(m => m.HealthRegistrationID == id);
            if (healthRegistration == null)
            {
                return NotFound();
            }

            return View(healthRegistration);
        }

        [Authorize]
        public IActionResult Create()
        {
            ViewData["MemberRegistrationNumber"] = new SelectList(_context.Member, "MemberRegistrationNumber", "MemberRegistrationNumber");
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(bool HowRUFeeling, string? Symptoms)
        {
            HealthRegistration register = new HealthRegistration();
            register.MemberRegistrationNumber = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier)
                .Select(c => c.Value).SingleOrDefault();
            register.RegisterDateTime = DateTime.Now;
            register.HowRUFeeling = HowRUFeeling;
            register.Symptoms = Symptoms;
            if (ModelState.IsValid)
            {
                _context.Add(register);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        // GET: HealthRegistrations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var healthRegistration = await _context.HealthRegistration.FindAsync(id);
            if (healthRegistration == null)
            {
                return NotFound();
            }
            ViewData["MemberRegistrationNumber"] = new SelectList(_context.Member, "MemberRegistrationNumber", "MemberRegistrationNumber", healthRegistration.MemberRegistrationNumber);
            return View(healthRegistration);
        }

        // POST: HealthRegistrations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HealthRegistrationID,MemberRegistrationNumber,RegisterDateTime,HowRUFeeling,Symptoms")] HealthRegistration healthRegistration)
        {
            if (id != healthRegistration.HealthRegistrationID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(healthRegistration);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HealthRegistrationExists(healthRegistration.HealthRegistrationID))
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
            ViewData["MemberRegistrationNumber"] = new SelectList(_context.Member, "MemberRegistrationNumber", "MemberRegistrationNumber", healthRegistration.MemberRegistrationNumber);
            return View(healthRegistration);
        }

        // GET: HealthRegistrations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var healthRegistration = await _context.HealthRegistration
                .Include(h => h.Member)
                .FirstOrDefaultAsync(m => m.HealthRegistrationID == id);
            if (healthRegistration == null)
            {
                return NotFound();
            }

            return View(healthRegistration);
        }

        // POST: HealthRegistrations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var healthRegistration = await _context.HealthRegistration.FindAsync(id);
            _context.HealthRegistration.Remove(healthRegistration);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HealthRegistrationExists(int id)
        {
            return _context.HealthRegistration.Any(e => e.HealthRegistrationID == id);
        }
    }
}
