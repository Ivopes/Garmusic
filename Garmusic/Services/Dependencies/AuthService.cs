using Garmusic.Interfaces.Repositories;
using Garmusic.Interfaces.Services;
using Garmusic.Models;
using Garmusic.Models.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Garmusic.Services.Dependencies
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        public AuthService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public async Task<string> GetDropboxJwtAsync(int accountId)
        {
            return await _authRepository.GetDropboxJwtAsync(accountId);
        }

        public string GetDropboxKeys()
        {
            return _authRepository.GetDropboxKeys();
        }

        public async Task<string> LoginAsync(Account account)
        {
            return await _authRepository.LoginAsync(account);
        }

        public async Task<string> LoginWatchAsync(Account account)
        {
            return await _authRepository.LoginWatchAsync(account);
        }

        public async Task<string> RegisterAsync(Account account)
        {
            return await _authRepository.RegisterAsync(account);
        }

        public async Task RegisterDropboxAsync(int accountId, DropboxJson json)
        {
            await _authRepository.RegisterDropboxAsync(accountId, json);
        }

        public async Task SignOutDbx(int aID)
        {
            await _authRepository.SignOutDbx(aID);
        }
    }
}
