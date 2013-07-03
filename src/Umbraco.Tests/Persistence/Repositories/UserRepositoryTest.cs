﻿using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Umbraco.Core.Models.Membership;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Querying;
using Umbraco.Core.Persistence.Repositories;
using Umbraco.Core.Persistence.UnitOfWork;
using Umbraco.Tests.TestHelpers;
using Umbraco.Tests.TestHelpers.Entities;

namespace Umbraco.Tests.Persistence.Repositories
{
    [TestFixture]
    public class UserRepositoryTest : BaseDatabaseFactoryTest
    {
        [SetUp]
        public override void Initialize()
        {
            base.Initialize();
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
        }

        [Test]
        public void Can_Instantiate_Repository()
        {
            // Arrange
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();

            // Act
            var repository = RepositoryResolver.Current.ResolveByType<IUserRepository>(unitOfWork);

            // Assert
            Assert.That(repository, Is.Not.Null);
        }

        [Test]
        public void Can_Perform_Add_On_UserRepository()
        {
            // Arrange
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();
            var repository = RepositoryResolver.Current.ResolveByType<IUserRepository>(unitOfWork);

            var user = MockedUser.CreateUser(CreateAndCommitUserType());

            // Act
            repository.AddOrUpdate(user);
            unitOfWork.Commit();

            // Assert
            Assert.That(user.HasIdentity, Is.True);
        }

        [Test]
        public void Can_Perform_Multiple_Adds_On_UserRepository()
        {
            // Arrange
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();
            var repository = RepositoryResolver.Current.ResolveByType<IUserRepository>(unitOfWork);

            var user1 = MockedUser.CreateUser(CreateAndCommitUserType(), "1");
            var use2 = MockedUser.CreateUser(CreateAndCommitUserType(), "2");

            // Act
            repository.AddOrUpdate(user1);
            unitOfWork.Commit();
            repository.AddOrUpdate(use2);
            unitOfWork.Commit();

            // Assert
            Assert.That(user1.HasIdentity, Is.True);
            Assert.That(use2.HasIdentity, Is.True);
        }

        [Test]
        public void Can_Verify_Fresh_Entity_Is_Not_Dirty()
        {
            // Arrange
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();
            var repository = RepositoryResolver.Current.ResolveByType<IUserRepository>(unitOfWork);
            var user = MockedUser.CreateUser(CreateAndCommitUserType());
            repository.AddOrUpdate(user);
            unitOfWork.Commit();

            // Act
            var resolved = repository.Get((int)user.Id);
            bool dirty = ((User)resolved).IsDirty();

            // Assert
            Assert.That(dirty, Is.False);
        }

        [Test]
        public void Can_Perform_Update_On_UserRepository()
        {
            // Arrange
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();
            var repository = RepositoryResolver.Current.ResolveByType<IUserRepository>(unitOfWork);
            var user = MockedUser.CreateUser(CreateAndCommitUserType());
            repository.AddOrUpdate(user);
            unitOfWork.Commit();

            // Act
            var resolved = repository.Get((int)user.Id);
            
            resolved.Name = "New Name";
            resolved.Permissions = "ZYX";
            resolved.Language = "fr";
            resolved.IsApproved = false;
            resolved.Password = "new";
            resolved.NoConsole = true;
            resolved.StartContentId = 10;
            resolved.StartMediaId = 11;
            resolved.DefaultToLiveEditing = true;
            resolved.Email = "new@new.com";
            resolved.Username = "newName";
            resolved.RemoveAllowedSection("content");

            repository.AddOrUpdate(resolved);
            unitOfWork.Commit();
            var updatedItem = repository.Get((int)user.Id);

            // Assert
            Assert.That(updatedItem.Id, Is.EqualTo(resolved.Id));
            Assert.That(updatedItem.Name, Is.EqualTo(resolved.Name));
            Assert.That(updatedItem.Permissions, Is.EqualTo(resolved.Permissions));
            Assert.That(updatedItem.Language, Is.EqualTo(resolved.Language));
            Assert.That(updatedItem.IsApproved, Is.EqualTo(resolved.IsApproved));
            Assert.That(updatedItem.Password, Is.EqualTo(resolved.Password));
            Assert.That(updatedItem.NoConsole, Is.EqualTo(resolved.NoConsole));
            Assert.That(updatedItem.StartContentId, Is.EqualTo(resolved.StartContentId));
            Assert.That(updatedItem.StartMediaId, Is.EqualTo(resolved.StartMediaId));
            Assert.That(updatedItem.DefaultToLiveEditing, Is.EqualTo(resolved.DefaultToLiveEditing));
            Assert.That(updatedItem.Email, Is.EqualTo(resolved.Email));
            Assert.That(updatedItem.Username, Is.EqualTo(resolved.Username));
            Assert.That(updatedItem.AllowedSections.Count(), Is.EqualTo(1));
            Assert.IsTrue(updatedItem.AllowedSections.Contains("media"));
        }

        [Test]
        public void Can_Perform_Delete_On_UserRepository()
        {
            // Arrange
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();
            var repository = RepositoryResolver.Current.ResolveByType<IUserRepository>(unitOfWork);

            var user = MockedUser.CreateUser(CreateAndCommitUserType());

            // Act
            repository.AddOrUpdate(user);
            unitOfWork.Commit();
            var id = user.Id;

            var repository2 = RepositoryResolver.Current.ResolveByType<IUserRepository>(unitOfWork);
            repository2.Delete(user);
            unitOfWork.Commit();

            var resolved = repository2.Get((int)id);

            // Assert
            Assert.That(resolved, Is.Null);
        }

        [Test]
        public void Can_Perform_Get_On_UserRepository()
        {
            // Arrange
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();
            var repository = RepositoryResolver.Current.ResolveByType<IUserRepository>(unitOfWork);
            var user = MockedUser.CreateUser(CreateAndCommitUserType());
            repository.AddOrUpdate(user);
            unitOfWork.Commit();

            // Act
            var updatedItem = repository.Get((int)user.Id);

            // Assert
            Assert.That(updatedItem.Id, Is.EqualTo(user.Id));
            Assert.That(updatedItem.Name, Is.EqualTo(user.Name));
            Assert.That(updatedItem.Permissions, Is.EqualTo(user.Permissions));
            Assert.That(updatedItem.Language, Is.EqualTo(user.Language));
            Assert.That(updatedItem.IsApproved, Is.EqualTo(user.IsApproved));
            Assert.That(updatedItem.Password, Is.EqualTo(user.Password));
            Assert.That(updatedItem.NoConsole, Is.EqualTo(user.NoConsole));
            Assert.That(updatedItem.StartContentId, Is.EqualTo(user.StartContentId));
            Assert.That(updatedItem.StartMediaId, Is.EqualTo(user.StartMediaId));
            Assert.That(updatedItem.DefaultToLiveEditing, Is.EqualTo(user.DefaultToLiveEditing));
            Assert.That(updatedItem.Email, Is.EqualTo(user.Email));
            Assert.That(updatedItem.Username, Is.EqualTo(user.Username));
            Assert.That(updatedItem.AllowedSections.Count(), Is.EqualTo(2));
            Assert.IsTrue(updatedItem.AllowedSections.Contains("media"));
            Assert.IsTrue(updatedItem.AllowedSections.Contains("content"));
        }

        [Test]
        public void Can_Perform_GetByQuery_On_UserRepository()
        {
            // Arrange
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();
            var repository = RepositoryResolver.Current.ResolveByType<IUserRepository>(unitOfWork);
            CreateAndCommitMultipleUsers(repository, unitOfWork);

            // Act
            var query = Query<IUser>.Builder.Where(x => x.Username == "TestUser1");
            var result = repository.GetByQuery(query);

            // Assert
            Assert.That(result.Count(), Is.GreaterThanOrEqualTo(1));
        }

        [Test]
        public void Can_Perform_GetAll_By_Param_Ids_On_UserRepository()
        {
            // Arrange
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();
            var repository = RepositoryResolver.Current.ResolveByType<IUserRepository>(unitOfWork);
            var users = CreateAndCommitMultipleUsers(repository, unitOfWork);

            // Act
            var result = repository.GetAll((int)users[0].Id, (int)users[1].Id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Any(), Is.True);
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public void Can_Perform_GetAll_On_UserRepository()
        {
            // Arrange
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();
            var repository = RepositoryResolver.Current.ResolveByType<IUserRepository>(unitOfWork);
            CreateAndCommitMultipleUsers(repository, unitOfWork);

            // Act
            var result = repository.GetAll();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Any(), Is.True);
            Assert.That(result.Count(), Is.GreaterThanOrEqualTo(3));
        }

        [Test]
        public void Can_Perform_Exists_On_UserRepository()
        {
            // Arrange
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();
            var repository = RepositoryResolver.Current.ResolveByType<IUserRepository>(unitOfWork);
            var users = CreateAndCommitMultipleUsers(repository, unitOfWork);

            // Act
            var exists = repository.Exists((int)users[0].Id);

            // Assert
            Assert.That(exists, Is.True);
        }

        [Test]
        public void Can_Perform_Count_On_UserRepository()
        {
            // Arrange
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();
            var repository = RepositoryResolver.Current.ResolveByType<IUserRepository>(unitOfWork);
            var users = CreateAndCommitMultipleUsers(repository, unitOfWork);

            // Act
            var query = Query<IUser>.Builder.Where(x => x.Username == "TestUser1" || x.Username == "TestUser2");
            var result = repository.Count(query);

            // Assert
            Assert.That(result, Is.GreaterThanOrEqualTo(2));
        }

        [Test]
        public void Can_Remove_Section_For_User()
        {
            // Arrange
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();
            var repository = RepositoryResolver.Current.ResolveByType<IUserRepository>(unitOfWork);
            var users = CreateAndCommitMultipleUsers(repository, unitOfWork);

            // Act

            //add and remove a few times, this tests the internal collection
            users[0].RemoveAllowedSection("content");
            users[0].RemoveAllowedSection("content");
            users[0].AddAllowedSection("content");
            users[0].RemoveAllowedSection("content");
            
            users[1].RemoveAllowedSection("media");
            users[1].RemoveAllowedSection("media");

            repository.AddOrUpdate(users[0]);
            repository.AddOrUpdate(users[1]);
            unitOfWork.Commit();

            // Assert
            var result = repository.GetAll((int) users[0].Id, (int) users[1].Id).ToArray();
            Assert.AreEqual(1, result[0].AllowedSections.Count());
            Assert.AreEqual("media", result[0].AllowedSections.First());
            Assert.AreEqual(1, result[1].AllowedSections.Count());
            Assert.AreEqual("content", result[1].AllowedSections.First());
        }

        [Test]
        public void Can_Add_Section_For_User()
        {
            // Arrange
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();
            var repository = RepositoryResolver.Current.ResolveByType<IUserRepository>(unitOfWork);
            var users = CreateAndCommitMultipleUsers(repository, unitOfWork);

            // Act
            
            //add and remove a few times, this tests the internal collection
            users[0].AddAllowedSection("settings");
            users[0].AddAllowedSection("settings");
            users[0].RemoveAllowedSection("settings");
            users[0].AddAllowedSection("settings");

            users[1].AddAllowedSection("developer");

            //add the same even though it's already there
            users[2].AddAllowedSection("content");

            repository.AddOrUpdate(users[0]);
            repository.AddOrUpdate(users[1]);
            unitOfWork.Commit();

            // Assert
            var result = repository.GetAll((int)users[0].Id, (int)users[1].Id, (int)users[2].Id).ToArray();
            Assert.AreEqual(3, result[0].AllowedSections.Count());
            Assert.IsTrue(result[0].AllowedSections.Contains("content"));
            Assert.IsTrue(result[0].AllowedSections.Contains("media"));
            Assert.IsTrue(result[0].AllowedSections.Contains("settings"));
            Assert.AreEqual(3, result[1].AllowedSections.Count());
            Assert.IsTrue(result[1].AllowedSections.Contains("content"));
            Assert.IsTrue(result[1].AllowedSections.Contains("media"));
            Assert.IsTrue(result[1].AllowedSections.Contains("developer"));
            Assert.AreEqual(2, result[2].AllowedSections.Count());
            Assert.IsTrue(result[1].AllowedSections.Contains("content"));
            Assert.IsTrue(result[1].AllowedSections.Contains("media"));
        }

        private IUser[] CreateAndCommitMultipleUsers(IUserRepository repository, IUnitOfWork unitOfWork)
        {
            var user1 = MockedUser.CreateUser(CreateAndCommitUserType(), "1");
            var user2 = MockedUser.CreateUser(CreateAndCommitUserType(), "2");
            var user3 = MockedUser.CreateUser(CreateAndCommitUserType(), "3");
            repository.AddOrUpdate(user1);
            repository.AddOrUpdate(user2);
            repository.AddOrUpdate(user3);
            unitOfWork.Commit();
            return new IUser[] {user1, user2, user3};
        }

        private IUserType CreateAndCommitUserType()
        {
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();
            var repository = RepositoryResolver.Current.ResolveByType<IUserTypeRepository>(unitOfWork);
            var userType = MockedUserType.CreateUserType();
            repository.AddOrUpdate(userType);
            unitOfWork.Commit();
            return userType;
        }
    }
}