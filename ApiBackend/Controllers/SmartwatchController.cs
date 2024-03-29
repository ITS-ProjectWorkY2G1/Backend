﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.WatchModels;
using Services.Services;

namespace ApiBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SmartwatchController : ControllerBase
    {
        private readonly ISmartwatchServices _smartwatchService;
        public SmartwatchController(ISmartwatchServices smartwatchService)
        {
            _smartwatchService = smartwatchService;
        }
        [HttpGet("/GetSmartwatches")]
        [Authorize]
        public async Task<List<Smartwatch>> GetSmartwatches(Guid userId, Guid sessionId) =>
            await _smartwatchService.GetSmartwatchAsync(userId, sessionId);
        [HttpGet("/GetAllSmartwatches")]
        [Authorize]
        public async Task<List<Smartwatch>> GetAllSmartwatches() =>
            await _smartwatchService.GetAllSmartwatchesAsync();
        [HttpGet("/GetSessions")]
        [Authorize]
        public async Task<List<Session>> GetSessions(Guid userId) =>
            await _smartwatchService.GetSessionsAsync(userId);
        [HttpGet("/GetAllSessions")]
        [Authorize]
        public async Task<List<Session>> GetAllSessions() =>
            await _smartwatchService.GetAllSessionsAsync();
        [HttpGet("/GetFullSessionsList")]
        [Authorize]
        public async Task<List<FullSessionViewModel>> GetFullSessionsList(Guid userId) =>
            await _smartwatchService.GetFullSessionAsync(userId);
        [HttpGet("/GetSmartwatchById")]
        public async Task<List<Smartwatch>> GetSmartwatchById(Guid Id) =>
            await _smartwatchService.GetSmartwatchById(Id);
    }
}
