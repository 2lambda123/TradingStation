﻿using AuthenticationService.Interfaces;
using System;
using DTO;
using Microsoft.AspNetCore.Mvc;
using MassTransit;
using System.Threading.Tasks;

namespace AuthenticationService.Commands
{
    public class LoginCommand : ILoginCommand
    {
        private readonly ITokensEngine tokensEngine;
        private readonly IBus busControl;

        /// <summary>
        /// Get user ID from UserService.
        /// </summary>
        private async Task<User> GetUserFromUserService(UserEmailPassword data)
        {
            var uri = new Uri("rabbitmq://localhost/UserService");

            var client = busControl.CreateRequestClient<UserEmailPassword>(uri).Create(data);

            var response = await client.GetResponse<User>();

            return response.Message;
        }

        private bool CheckUserCredentials(User user, UserEmailPassword credentials)
        {
            return user.PasswordHash == credentials.Password
                && user.Email == credentials.Email;
        }

        public LoginCommand(
            [FromServices] ITokensEngine tokensEngine,
            [FromServices] IBus busControl)
        {
            this.tokensEngine = tokensEngine;
            this.busControl = busControl;
        }

        public async Task<string> Execute(UserEmailPassword data)
        {
            User user = await GetUserFromUserService(data);

            if (!CheckUserCredentials(user, data))
            {
                throw new Exception("Нет такого юзера, печалька ...");
            }

            return tokensEngine.GetToken(user.Id);
        }
    }
}
