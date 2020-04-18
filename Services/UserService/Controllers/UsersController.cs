using DTO;
using DTO.RestRequests;
using IDeleteUserUserService.Interfaces;
using Kernel.CustomExceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using UserService.Interfaces;
using UserService.Utils;

namespace UserService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<UserConfirmation> userManager;
        public UsersController(UserManager<UserConfirmation> userManager)
        {
            this.userManager = userManager;
        }
        [Route("create")]
        [HttpPost]
        public async Task<bool> CreateUser([FromServices] ICreateUserCommand command, [FromBody] CreateUserRequest request)
        {
            var result = await command.Execute(request);

            var userConfirm = new UserConfirmation
            {
                Id = result.Id.ToString(),
                Email = result.Email
            };

            // ��������� ������ ��� ������������
            var code = await userManager.GenerateEmailConfirmationTokenAsync(userConfirm);
            var callbackUrl = Url.Action(
                "ConfirmEmail",
                "Account",
                new { userId = userConfirm.Id, code = code },
                protocol: HttpContext.Request.Scheme);
            var emailSender = new EmailSender();
            await emailSender.SendEmailAsync(userConfirm.Email, "Confirm your account",
                $"����������� �����������, ������� �� ������: <a href='{callbackUrl}'>link</a>");

            return true;
            //return Content("��� ���������� ����������� ��������� ����������� ����� � ��������� �� ������, ��������� � ������");
        }

        [Route("edit")]
        [HttpPut]
        public async Task<bool> EditUser([FromServices] IEditUserCommand command, [FromBody] EditUserRequest request)
        {
            return await command.Execute(request);
        }

        [Route("delete")]
        [HttpDelete]
        public async Task<bool> DeleteUser([FromServices] IDeleteUserCommand command, [FromBody] DeleteUserRequest request)
        {
            return await command.Execute(request);
        }

        [Route("get")]
        [HttpGet]
        public async Task<User> GetUser([FromServices] IGetUserByIdCommand command, [FromHeader] Guid userId)
        {
            return await command.Execute(userId);
        }
    }
}
