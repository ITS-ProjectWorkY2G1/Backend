﻿using Microsoft.EntityFrameworkCore;
using Models;
using Models.Database;
using Models.WatchModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class SmartwatchServices : ISmartwatchServices
    {
        private readonly WatchContext _context;
        public SmartwatchServices(WatchContext context)
        {
            _context = context;
        }

        public async Task<List<Session>> GetSessionsAsync(Guid userId)
        {
            var userData = await _context.Smartwatch.Where(x => x.UserId == userId).ToListAsync();
            List<Guid> userSessionIds = userData.Select(x => x.SessionId).Distinct().ToList();
            return await _context.Session.Where(x => userSessionIds.Any(y => y == x.SessionId)).ToListAsync();
        }
        public async Task<List<Smartwatch>> GetSmartwatchAsync(Guid userId, Guid SessionId) =>
            await _context.Smartwatch.Where(x => x.UserId == userId && x.SessionId == SessionId).ToListAsync();

        public async Task<List<FullSessionViewModel>> GetFullSessionAsync(Guid userId)
        {
            var FullSessionList = new List<FullSessionViewModel>();

            var userData = await _context.Smartwatch.Where(x => x.UserId == userId).ToListAsync();
            List<Guid> userSessionIds = userData.Select(x => x.SessionId).Distinct().ToList();
            List<Session> SessionList = await _context.Session.Where(x => userSessionIds.Any(y => y == x.SessionId)).ToListAsync();

            foreach(var s in SessionList)
            {
                var newSession = new FullSessionViewModel()
                {
                    SessionDistance = s.SessionDistance,
                    SessionId = s.SessionId,
                    AvgHeartRate = s.AvgHeartRate,
                    PoolLaps = s.PoolLaps,
                    SessionTime = s.SessionTime,
                    PoolLength = s.PoolLength
                };

                newSession.Smartwatches = await _context.Smartwatch.Where(x => x.UserId == userId && x.SessionId == s.SessionId).ToListAsync();
                FullSessionList.Add(newSession);
            }

            return FullSessionList;
        }

        public async Task<List<Smartwatch>> GetAllSmartwatchesAsync()=>
            await _context.Smartwatch.ToListAsync();
        public async Task<List<Session>> GetAllSessionsAsync() =>
            await _context.Session.ToListAsync();
        public async Task<List<Smartwatch>> GetSmartwatchById(Guid Id)=>        
            await _context.Smartwatch.Where(x => x.Id == Id).ToListAsync();
    }
}
