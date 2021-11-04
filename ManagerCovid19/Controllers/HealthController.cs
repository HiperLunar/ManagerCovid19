﻿using System;
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

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var managerCovid19Context = _context.HealthRegistration;
            var RN = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            return View(await managerCovid19Context.Where(model => model.MemberRegistrationNumber == RN).ToArrayAsync());
        }

        // GET: Health/Details/5
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

        public IActionResult Overview(DateTime fromDate, DateTime toDate)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Filter(DateTime fromDate, DateTime toDate)
        {
            var response = new
            {
                registerByDate = await _context.HealthRegistration
                .Where(m => DateTime.Compare(m.RegisterDateTime, fromDate) >= 0 && DateTime.Compare(m.RegisterDateTime, toDate) <= 0)
                .GroupBy(m => m.RegisterDateTime.Date)
                .Select(m => new
                {
                    Date = m.Key,
                    Count = m.Count()
                })
                .ToListAsync(),

                notWellByDate = await _context.HealthRegistration
                .Where(m => DateTime.Compare(m.RegisterDateTime, fromDate) >= 0 && DateTime.Compare(m.RegisterDateTime, toDate) <= 0 && !m.HowRUFeeling)
                .GroupBy(m => m.RegisterDateTime.Date)
                .Select(m => new
                {
                    Date = m.Key,
                    Count = m.Count()
                })
                .ToListAsync(),

                registerBySectorByDate = _context.HealthRegistration
                .Include(m => m.Member)
                .Where(m => DateTime.Compare(m.RegisterDateTime, fromDate) >= 0 && DateTime.Compare(m.RegisterDateTime, toDate) <= 0)
                .AsEnumerable()
                .GroupBy(m => m.RegisterDateTime.Date)
                .Select(m => new
                {
                    Key = m.Key,
                    Administrador = m.Sum(n => n.Member.Sector == Member.Sectors.Administrador ? 1 : 0),
                    AdministradorNotWell = m.Sum(n => n.Member.Sector == Member.Sectors.Administrador && !n.HowRUFeeling ? 1 : 0),
                    Aluno = m.Sum(n => n.Member.Sector == Member.Sectors.Aluno ? 1 : 0),
                    AlunoNotWell = m.Sum(n => n.Member.Sector == Member.Sectors.Aluno && !n.HowRUFeeling ? 1 : 0),
                    Funcionario = m.Sum(n => n.Member.Sector == Member.Sectors.Funcionario ? 1 : 0),
                    FuncionarioNotWell = m.Sum(n => n.Member.Sector == Member.Sectors.Funcionario && !n.HowRUFeeling ? 1 : 0),
                    Professor = m.Sum(n => n.Member.Sector == Member.Sectors.Professor ? 1 : 0),
                    ProfessorNotWell = m.Sum(n => n.Member.Sector == Member.Sectors.Professor && !n.HowRUFeeling ? 1 : 0)
                }),
                lastDays = _context.HealthRegistration
                .Include(m => m.Member)
                .Where(m => DateTime.Compare(m.RegisterDateTime, DateTime.Today.AddDays(-5)) > 0 && DateTime.Compare(m.RegisterDateTime, DateTime.Today) < 0 && !m.HowRUFeeling)
                .AsEnumerable()
                .GroupBy(m => m.RegisterDateTime.Date)
                .Select(m => new
                {
                    Count = m.Count(),
                    Administrador = m.Sum(n => n.Member.Sector == Member.Sectors.Administrador ? 1 : 0),
                    Aluno = m.Sum(n => n.Member.Sector == Member.Sectors.Aluno ? 1 : 0),
                    Funcionario = m.Sum(n => n.Member.Sector == Member.Sectors.Funcionario ? 1 : 0),
                    Professor = m.Sum(n => n.Member.Sector == Member.Sectors.Professor ? 1 : 0),
                }).ToList()
            };
            return Json(response);
        }

        // GET: Health/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["MemberRegistrationNumber"] = new SelectList(_context.Member, "MemberRegistrationNumber", "MemberRegistrationNumber");
            return View();
        }

        // POST: Health/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("HealthRegistrationID,HowRUFeeling,FaltaDeAr,Cansaco,Febre,Calafrios,Tosse,DorDeGarganta,DorDeCabeca,DorNoPeito,PerdaDeOlfato,PerdaPaladar,Diarreia,Coriza,Espirros")] HealthRegistration healthRegistration)
        {
            healthRegistration.RegisterDateTime = DateTime.Now;
            healthRegistration.MemberRegistrationNumber = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier)
                .Select(c => c.Value).SingleOrDefault();
            if (ModelState.IsValid)
            {
                _context.Add(healthRegistration);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Success));
            }
            ViewData["MemberRegistrationNumber"] = new SelectList(_context.Member, "MemberRegistrationNumber", "MemberRegistrationNumber", healthRegistration.MemberRegistrationNumber);
            return View(healthRegistration);
        }

        public IActionResult Success()
        {
            return View();
        }

        // GET: Health/Edit/5
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
            ViewData["hidden"] = healthRegistration.HowRUFeeling ? "" : "style=\"display: none;\"";
            return View(healthRegistration);
        }

        // POST: Health/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HealthRegistrationID,MemberRegistrationNumber,RegisterDateTime,HowRUFeeling,FaltaDeAr,Cansaco,Febre,Calafrios,Tosse,DorDeGarganta,DorDeCabeca,DorNoPeito,PerdaDeOlfato,PerdaPaladar,Diarreia,Coriza,Espirros")] HealthRegistration healthRegistration)
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

        // GET: Health/Delete/5
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

        // POST: Health/Delete/5
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
