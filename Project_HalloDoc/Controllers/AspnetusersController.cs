using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_HalloDoc.DataContext;
using Project_HalloDoc.DataModels;

namespace Project_HalloDoc.Controllers
{
    public class AspnetusersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AspnetusersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Aspnetusers
        public async Task<IActionResult> Index() => _context.Aspnetusers != null ?
                          View(await _context.Aspnetusers.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Aspnetusers'  is null.");

        // GET: Aspnetusers/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Aspnetusers == null)
            {
                return NotFound();
            }

            var aspnetuser = await _context.Aspnetusers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aspnetuser == null)
            {
                return NotFound();
            }

            return View(aspnetuser);
        }

        // GET: Aspnetusers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Aspnetusers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Username,Passwordhash,Securitystamp,Email,Emailconfirmed,Phonenumber,Phonenumberconfirmed,Twofactorenabled,Lockoutenddateutc,Lockoutenabled,Accessfailedcount,Ip,Corepasswordhash,Hashversion,Modifieddate")] Aspnetuser aspnetuser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(aspnetuser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(aspnetuser);
        }

        // GET: Aspnetusers/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Aspnetusers == null)
            {
                return NotFound();
            }

            var aspnetuser = await _context.Aspnetusers.FindAsync(id);
            if (aspnetuser == null)
            {
                return NotFound();
            }
            return View(aspnetuser);
        }

        // POST: Aspnetusers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Username,Passwordhash,Securitystamp,Email,Emailconfirmed,Phonenumber,Phonenumberconfirmed,Twofactorenabled,Lockoutenddateutc,Lockoutenabled,Accessfailedcount,Ip,Corepasswordhash,Hashversion,Modifieddate")] Aspnetuser aspnetuser)
        {
            if (id != aspnetuser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(aspnetuser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AspnetuserExists(aspnetuser.Id))
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
            return View(aspnetuser);
        }

        // GET: Aspnetusers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Aspnetusers == null)
            {
                return NotFound();
            }

            var aspnetuser = await _context.Aspnetusers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aspnetuser == null)
            {
                return NotFound();
            }

            return View(aspnetuser);
        }

        // POST: Aspnetusers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Aspnetusers == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Aspnetusers'  is null.");
            }
            var aspnetuser = await _context.Aspnetusers.FindAsync(id);
            if (aspnetuser != null)
            {
                _context.Aspnetusers.Remove(aspnetuser);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Validate(Aspnetuser Email, Aspnetuser Passwordhash)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("b1c1_patient_request", "Home");
            }
            return View();
            //var x = _context.Aspnetusers.FirstOrDefault(m => m.Email == obj.Email);
            //if (x != null && x.Passwordhash == obj.Passwordhash)
            //{
            //    return RedirectToAction("b1c1_patient_request", "Home");
            //}
            //else
            //{
            //    return RedirectToAction("b2_registered_user", "Home");
            //}
        }

        public IActionResult patient_login(LoginModel loginModel)
        {
            if (_loginService.Login(loginModel))
            {

                return RedirectToAction("submit_request", "Patient");
            }
            TempData["Email"] = "Enter Valid Email";

            return RedirectToAction("patient_login", "Patient");
        }

        private bool AspnetuserExists(string id)
        {
            return (_context.Aspnetusers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
