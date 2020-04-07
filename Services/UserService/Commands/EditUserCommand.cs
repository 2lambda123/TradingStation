﻿using DTO;
using DTO.BrokerRequests;
using DTO.RestRequests;
using FluentValidation;
using Kernel;
using Kernel.CustomExceptions;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using UserService.Interfaces;

namespace UserService.Commands
{
    public class EditUserCommand : IEditUserCommand
    {
        private readonly IValidator<UserInfoRequest> userInfoValidator;
        private readonly IValidator<PasswordChangeRequest> passwordChangeValidator;
        private readonly IBus busControl;

        public EditUserCommand
            ([FromServices] IValidator<UserInfoRequest> userInfoValidator, 
            [FromServices] IValidator<PasswordChangeRequest> passwordChangeValidator,
            [FromServices] IBus busControl)
        {
            this.busControl = busControl;
            this.userInfoValidator = userInfoValidator;
            this.passwordChangeValidator = passwordChangeValidator;
        }
        
        public async Task<bool> Execute(EditUserRequest request)
        {
            var passwordRequest = request.PasswordRequest;
            var userInfo = request.UserInfo;
            try
            {
                if (!(string.IsNullOrEmpty(passwordRequest.OldPassword)
                        || string.IsNullOrEmpty(passwordRequest.NewPassword)))
                {
                    passwordChangeValidator.ValidateAndThrow(passwordRequest);
                }
            } catch (Exception e)
            {
                throw new BadRequestException("Unable to edit");
            }

            userInfoValidator.ValidateAndThrow(userInfo);

            string oldPasswordHash = ShaHash.GetPasswordHash(passwordRequest.OldPassword);
            string newPasswordHash = ShaHash.GetPasswordHash(passwordRequest.NewPassword);
            var user = new User
            {
                Id = userInfo.UserId,
                Birthday = userInfo.Birthday,
                FirstName = userInfo.FirstName,
                LastName = userInfo.LastName,
                Email = userInfo.Email
            };

            var passwordHashChangeRequest = new PasswordHashChangeRequest
            {
                OldPasswordHash = oldPasswordHash,
                NewPasswordHash = newPasswordHash
            };

            var internalEditUserInfoRequest = new InternalEditUserInfoRequest
            {
                User = user,
                UserPasswords = passwordHashChangeRequest
            };

            var editUserResult = await EditUser(internalEditUserInfoRequest);
            if (!editUserResult)
            {
                throw new BadRequestException("Unable to edit");
            }

            return editUserResult;
        }

        private async Task<bool> EditUser(InternalEditUserInfoRequest request)
        {
            var uri = new Uri("rabbitmq://localhost/DatabaseService");

            var client = busControl.CreateRequestClient<InternalEditUserInfoRequest>(uri).Create(request);

            var response = await client.GetResponse<OperationResult>();

            return response.Message.IsSuccess;
        }
    }
}
