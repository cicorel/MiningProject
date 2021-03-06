﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MiningProject.DataContexts;
using MiningProject.Models;

namespace MiningProject.Controllers
{
    public class HistoriesController : Controller
    {
        private readonly TruckLocationDbContext _context;

        public HistoriesController(TruckLocationDbContext context)
        {
            _context = context;
        }

        // GET: Histories
        public async Task<IActionResult> Index(string locationString)
        {
            var historyList = from m in _context.Histories.Include(history => history.Truck).Include(history => history.Location) select m;
            if (!String.IsNullOrEmpty(locationString))
            {
                historyList = historyList.Where(s => s.Location.LocationName.Contains(locationString));
            }
            return View(await historyList.ToListAsync());
        }

        // GET: Histories/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var history = await _context.Histories
                .FirstOrDefaultAsync(m => m.ID == id);
            if (history == null)
            {
                return NotFound();
            }

            return View(history);
        }

        // GET: Histories/Create
        public IActionResult Create()
        {
            var locationList = _context.Locations.ToList();
            ViewData["Locations"] = new SelectList(locationList, "ID", "LocationName");
            var truckList = _context.Trucks.ToList();
            ViewData["Trucks"] = new SelectList(truckList, "ID", "PlateNumber");
            return View();
        }

        // POST: Histories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,ArrivalTime,DepartureTime,TruckID,LocationID")] History history)
        {
            if (ModelState.IsValid)
            {
                _context.Add(history);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(history);
        }

        // GET: Histories/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var history = await _context.Histories.FindAsync(id);
            if (history == null)
            {
                return NotFound();
            }
            return View(history);
        }

        // POST: Histories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("ID,ArrivalTime,DepartureTime")] History history)
        {
            if (id != history.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(history);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HistoryExists(history.ID))
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
            return View(history);
        }

        // GET: Histories/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var history = await _context.Histories
                .FirstOrDefaultAsync(m => m.ID == id);
            if (history == null)
            {
                return NotFound();
            }

            return View(history);
        }

        // POST: Histories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var history = await _context.Histories.FindAsync(id);
            _context.Histories.Remove(history);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HistoryExists(long id)
        {
            return _context.Histories.Any(e => e.ID == id);
        }
    }
}
