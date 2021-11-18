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
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

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

        public async Task<IActionResult> GetRegisters(int month)
        {
            var RN = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            var health = await _context.HealthRegistration
                .Where(h => h.RegisterDateTime.Date.Month == month && h.MemberRegistrationNumber == RN)
                .ToListAsync();
            return Json(health);
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

        [Authorize(Roles = "Admin")]
        public IActionResult Overview(DateTime fromDate, DateTime toDate)
        {
            if (Utils.fBrowserIsMobile(Request)) return Forbid();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Filter(DateTime fromDate, DateTime toDate, int? minimum)
        {
            var response = new
            {
                lastDays = _context.HealthRegistration
                .Include(m => m.Member)
                .Where(m => DateTime.Compare(m.RegisterDateTime, DateTime.Today.AddDays(-5)) > 0 && DateTime.Compare(m.RegisterDateTime, DateTime.Today) < 0 && !m.HowRUFeeling)
                .AsEnumerable()
                .GroupBy(m => m.HealthRegistrationID)
                .Select(m => new
                {
                    Count = m.Count(),
                    Administrador = m.Sum(n => n.Member.Sector == Member.Sectors.Administrador ? 1 : 0),
                    Aluno = m.Sum(n => n.Member.Sector == Member.Sectors.Aluno ? 1 : 0),
                    Funcionario = m.Sum(n => n.Member.Sector == Member.Sectors.Funcionario ? 1 : 0),
                    Professor = m.Sum(n => n.Member.Sector == Member.Sectors.Professor ? 1 : 0),
                }),

                membersMinimum = _context.Member
                .Where(m => m.HealthRegistrations
                    .Where(h => DateTime.Compare(h.RegisterDateTime, fromDate) >= 0 && DateTime.Compare(h.RegisterDateTime, toDate) <= 0)
                    .Count() >= minimum)
                .ToList(),
                
                /*_context.HealthRegistration
                .Include(m => m.Member)
                .Where(m => DateTime.Compare(m.RegisterDateTime, fromDate) >= 0 && DateTime.Compare(m.RegisterDateTime, toDate) <= 0)
                .AsEnumerable()
                .GroupBy(m => m.MemberRegistrationNumber)
                .Where(m => m.Count() >= minimum)
                .Select(m => m.Key),*/

                charts = new List<object>
                {
                    new {
                        columns = new List<object> {
                            new { type = "date", name = "Datas" },
                            new { type = "number", name = "Quantidade" }
                        },
                        data = await _context.HealthRegistration
                            .Where(m => DateTime.Compare(m.RegisterDateTime, fromDate) >= 0 && DateTime.Compare(m.RegisterDateTime, toDate) <= 0)
                            .GroupBy(m => m.RegisterDateTime.Date)
                            .Select(m => new List<object>{ m.Key, m.Count() })
                            .ToListAsync(),
                        options = new {
                            title = "Quantidade diária de relatórios"
                        }
                    },
                    new
                    {
                        columns = new List<object>
                        {
                            new { type = "date", name = "Datas"  },
                            new { type = "number", name = "Quantidade de registros \"Não estou me sentindo bem\""  }
                        },
                        data = await _context.HealthRegistration
                            .Where(m => DateTime.Compare(m.RegisterDateTime, fromDate) >= 0 && DateTime.Compare(m.RegisterDateTime, toDate) <= 0 && !m.HowRUFeeling)
                            .GroupBy(m => m.RegisterDateTime.Date)
                            .Select(m => new List<object>{ m.Key, m.Count() })
                            .ToListAsync(),
                        options = new
                        {
                            title = "Quantidade diária de registros de saúde \"Não muito bem\""
                        }
                    },
                    new
                    {
                        columns = new List<object>
                        {
                            new { type = "date", name = "Datas"  },
                            new { type = "number", name = "Administrador"  },
                            new { type = "number", name = "Aluno"  },
                            new { type = "number", name = "Funcionário"  },
                            new { type = "number", name = "Professor"  }
                        },
                        data = _context.HealthRegistration
                            .Include(m => m.Member)
                            .Where(m => DateTime.Compare(m.RegisterDateTime, fromDate) >= 0 && DateTime.Compare(m.RegisterDateTime, toDate) <= 0)
                            .AsEnumerable()
                            .GroupBy(m => m.RegisterDateTime.Date)
                            .Select(m => new List<object>
                            {
                                m.Key,
                                m.Sum(n => n.Member.Sector == Member.Sectors.Administrador ? 1 : 0),
                                m.Sum(n => n.Member.Sector == Member.Sectors.Aluno ? 1 : 0),
                                m.Sum(n => n.Member.Sector == Member.Sectors.Funcionario ? 1 : 0),
                                m.Sum(n => n.Member.Sector == Member.Sectors.Professor ? 1 : 0)
                            }),
                        options = new
                        {
                            title = "Quantidade diária de registros de saúde por setor"
                        }
                    },
                    new
                    {
                        columns = new List<object>
                        {
                            new { type = "date", name = "Datas"  },
                            new { type = "number", name = "Administrador"  },
                            new { type = "number", name = "Aluno"  },
                            new { type = "number", name = "Funcionário"  },
                            new { type = "number", name = "Professor"  }
                        },
                        data = _context.HealthRegistration
                            .Include(m => m.Member)
                            .Where(m => DateTime.Compare(m.RegisterDateTime, fromDate) >= 0 && DateTime.Compare(m.RegisterDateTime, toDate) <= 0 && !m.HowRUFeeling)
                            .AsEnumerable()
                            .GroupBy(m => m.RegisterDateTime.Date)
                            .Select(m => new List<object>
                            {
                                m.Key,
                                m.Sum(n => n.Member.Sector == Member.Sectors.Administrador ? 1 : 0),
                                m.Sum(n => n.Member.Sector == Member.Sectors.Aluno ? 1 : 0),
                                m.Sum(n => n.Member.Sector == Member.Sectors.Funcionario ? 1 : 0),
                                m.Sum(n => n.Member.Sector == Member.Sectors.Professor ? 1 : 0)
                            }),
                        options = new
                        {
                            title = "Quantidade diária de registros de saúde \"Não muito bem\" por setor"
                        }
                    }
                }
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
            if (healthRegistration.HowRUFeeling)
            {
                healthRegistration.FaltaDeAr = false;
                healthRegistration.Cansaco = false;
                healthRegistration.Calafrios= false;
                healthRegistration.Coriza = false;
                healthRegistration.Diarreia= false;
                healthRegistration.DorDeCabeca = false;
                healthRegistration.DorDeGarganta = false;
                healthRegistration.DorNoPeito = false;
                healthRegistration.Espirros = false;
                healthRegistration.FaltaDeAr = false;
                healthRegistration.Febre = false;
                healthRegistration.PerdaDeOlfato = false;
                healthRegistration.PerdaPaladar = false;
                healthRegistration.Tosse = false;
            }
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
