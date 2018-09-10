﻿using SelectelSharpCore.Requests;
using System;
using System.Threading.Tasks;

namespace SelectelSharpCore
{
    public class SelectelClient
    {
        public string StorageUrl { get; private set; }
        public string AuthToken { get; private set; }
        public long ExpireAuthToken { get; private set; }

        public async Task AuthorizeAsync(string user, string key)
        {
            var result = await ExecuteAsync(new AuthRequest(user, key));

            StorageUrl = result.StorageUrl;
            AuthToken = result.AuthToken;
            ExpireAuthToken = result.ExpireAuthToken;
        }

        public async Task<T> ExecuteAsync<T>(BaseRequest<T> request)
        {
            if (!request.AllowAnonymously)
            {
                CheckTokenNotNull();
            }

            await request.Execute(StorageUrl, AuthToken);
            return request.Result;
        }

        private void CheckTokenNotNull()
        {
            if (string.IsNullOrEmpty(AuthToken))
            {
                throw new Exception("You should first authorize this client. Call AuthorizeAsync method.");
            }
        }
    }
}