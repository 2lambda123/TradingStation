﻿using System;

using Microsoft.EntityFrameworkCore;

using NUnit.Framework;
using Moq;

using DTO;
using DataBaseService.Database;
using DataBaseService.Database.Models;
using DataBaseService.Repositories;
using DataBaseService.Repositories.Interfaces;
using DataBaseService.Mappers.Interfaces;


namespace DatabaseServiceTests.Repositories
{
    public class UserRepositoryCreateTests
    {
        private IUserRepository repository;
        private Mock<IUserMapper> mapper;
        private DbContextOptions<TPlatformDbContext> dbOptionsCreateUser;
        private DbContextOptions<TPlatformDbContext> dbOptionsCreateCredential;

        #region BIO
        private Guid userId = Guid.NewGuid();
        private Guid credentialId = Guid.NewGuid();

        private string firstName = "Adam";
        private string lastName = "Yablokov";
        private string email = "adam.ya@eden.org";
        private DateTime birth = DateTime.MinValue;
        private string passwordHash = "passwordHash";

        private User user;
        private DbUser dbUser;
        private UserCredential credential;
        private DbUserCredential dbCredential;
        #endregion

        [SetUp]
        public void Initialize()
        {
            dbOptionsCreateUser = new DbContextOptionsBuilder<TPlatformDbContext>()
                .UseInMemoryDatabase(databaseName: "Create_new_user_test")
                .Options;

            dbOptionsCreateCredential = new DbContextOptionsBuilder<TPlatformDbContext>()
                .UseInMemoryDatabase(databaseName: "Create_new_credential_test")
                .Options;

            user = new User
            {
                Id = userId,
                FirstName = firstName,
                LastName = lastName,
                Birthday = birth,
                Email = email
            };

            dbUser = new DbUser
            {
                Id = userId,
                FirstName = firstName,
                LastName = lastName,
                Birthday = birth
            };

            credential = new UserCredential()
            {
                Id = credentialId,
                UserId = userId,
                Email = email,
                PasswordHash = passwordHash
            };

            dbCredential = new DbUserCredential()
            {
                Id = credentialId,
                UserId = userId,
                Email = email,
                PasswordHash = passwordHash
            };

            mapper = new Mock<IUserMapper>();

            mapper
                .Setup(m => m.MapToDbUserCredential(credential))
                .Returns(dbCredential);

            mapper
                .Setup(m => m.MapToDbUser(user))
                .Returns(dbUser);
        }

        [Test]
        public void CreateUserTest()
        {
            using var dbContext = new TPlatformDbContext(dbOptionsCreateUser);
            repository = new UserRepository(mapper.Object, dbContext);
            repository.CreateUser(user, email);

            Assert.AreEqual(1, dbContext.Users.CountAsync().Result);
            Assert.AreEqual(
                dbUser,
                dbContext
                .Users
                .FirstOrDefaultAsync()
                .Result);
        }

        [Test]
        public void CreateUserCredential()
        {
            using var dbContext = new TPlatformDbContext(dbOptionsCreateCredential);
            repository = new UserRepository(mapper.Object, dbContext);
            repository.CreateUserCredential(credential);

            Assert.AreEqual(1, dbContext.UsersCredentials.CountAsync().Result);
            Assert.AreEqual(
                dbCredential,
                dbContext
                .UsersCredentials
                .FirstOrDefaultAsync()
                .Result);
        }
    }
}