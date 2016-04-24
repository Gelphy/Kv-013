﻿using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

using FluentAssertions;

using GitHubExtension.Security.DAL.Identity;
using GitHubExtension.Security.DAL.Infrastructure;
using GitHubExtension.Security.DAL.Interfaces;
using GitHubExtension.Security.Tests.Extensions;
using GitHubExtension.Security.Tests.Mocks;
using GitHubExtension.Security.WebApi.Controllers;
using GitHubExtension.Security.WebApi.Queries.Interfaces;

using Microsoft.AspNet.Identity;

using NSubstitute;

using Xunit;

namespace GitHubExtension.Security.Tests.TestForControllers
{
    public class RepositoryControllerAssignRolesToUserTests
    {
        private const string expectedErrorForInvalidRole = "Roles '{0}' does not exists in the system";

        private const string roleIndex = "role";

        public static IEnumerable<object[]> DataForInvalidModelStateResult
        {
            get
            {
                yield return
                    new object[]
                        {
                            new List<User> { new User { ProviderId = 1 }, }, 
                            new List<SecurityRole> { new SecurityRole { Name = "Admin" } }, 1, 0, "BussinessAnalytics"
                        };
            }
        }

        public static IEnumerable<object[]> DataForNotFountResult
        {
            get
            {
                yield return new object[] { new List<User>(), 12, 15, "UserRole" };
            }
        }

        public static IEnumerable<object[]> DataForOkResult
        {
            get
            {
                yield return
                    new object[]
                        {
                            new List<User> { new User { ProviderId = 2 }, }, 
                            new List<SecurityRole> { new SecurityRole { Name = "Admin" } }, new User { ProviderId = 1 }, 1, 
                            0, "Admin"
                        };
            }
        }

        [Theory]
        [MemberData("DataForInvalidModelStateResult")]
        public void ErrorMessageForInvalidRoleTest(
            List<User> users, 
            IEnumerable<SecurityRole> roles, 
            int gitHubId, 
            int repoId, 
            string roleToAssign)
        {
            // Arrange
            RepositoryController controller = new RepositoryController(
                Substitute.For<IGitHubQuery>(), 
                MockForContext(roles), 
                MockForUsers(users));

            // Act
            Task<IHttpActionResult> response = controller.AssignRolesToUser(repoId, gitHubId, roleToAssign);

            // Assert
            IHttpActionResult result = response.Result;
            result.Should()
                .BeOfType<InvalidModelStateResult>(
                    "Because impossible to assign role = {0} that doesn't exist in database", 
                    roleToAssign);
            result.Should()
                .BeOfType<InvalidModelStateResult>()
                .Which.GetErrorMessage(roleIndex)
                .Should()
                .Be(string.Format(expectedErrorForInvalidRole, roleToAssign));
        }

        [Theory]
        [MemberData("DataForInvalidModelStateResult")]
        public void InvalidRoleTest(
            List<User> users, 
            IEnumerable<SecurityRole> roles, 
            int gitHubId, 
            int repoId, 
            string roleToAssign)
        {
            // Arrenge
            RepositoryController controller = new RepositoryController(
                Substitute.For<IGitHubQuery>(), 
                MockForContext(roles), 
                MockForUsers(users));

            // Act
            Task<IHttpActionResult> response = controller.AssignRolesToUser(repoId, gitHubId, roleToAssign);

            // Assert
            IHttpActionResult result = response.Result;
            result.Should()
                .BeOfType<InvalidModelStateResult>(
                    "Because impossible to assign role = {0} that doesn't exist in database", 
                    roleToAssign);
        }

        [Theory]
        [MemberData("DataForNotFountResult")]
        public void NotFoundUserTest(List<User> users, int gitHubId, int repoId, string roleToAssign)
        {
            // Arrange
            RepositoryController controller = new RepositoryController(
                Substitute.For<IGitHubQuery>(), 
                Substitute.For<ISecurityContext>(), 
                MockForUsers(users));

            // Act
            Task<IHttpActionResult> response = controller.AssignRolesToUser(repoId, gitHubId, roleToAssign);

            // Assert
            IHttpActionResult result = response.Result;
            result.Should()
                .BeOfType<NotFoundResult>("Because user with providerId= {0} doesn't exists in database", gitHubId);
        }

        [Theory]
        [MemberData("DataForOkResult")]
        public void OkResultTest(
            List<User> users, 
            List<SecurityRole> roles, 
            User userToUpdate, 
            int gitHubId, 
            int repoId, 
            string roleToAssign)
        {
            // Arrange
            users.Add(userToUpdate);
            RepositoryController controller = new RepositoryController(
                Substitute.For<IGitHubQuery>(), 
                MockForContext(roles), 
                MockForAddingClaim(users, userToUpdate));

            // Act
            Task<IHttpActionResult> response = controller.AssignRolesToUser(repoId, gitHubId, roleToAssign);

            // Assert
            IHttpActionResult result = response.Result;
            result.Should().BeOfType<OkResult>();
        }

        private ApplicationUserManager MockForAddingClaim(List<User> users, User userToUpdate)
        {
            var userManager = MockForUsers(users);
            userManager.UpdateAsync(userToUpdate).Returns(IdentityResult.Success);
            userManager.CreateIdentityAsync(userToUpdate, DefaultAuthenticationTypes.ApplicationCookie)
                .Returns(Task.FromResult(new ClaimsIdentity()));
            userManager.AddClaimAsync(userToUpdate.Id, Arg.Any<Claim>()).Returns(IdentityResult.Success);
            return userManager;
        }

        private ISecurityContext MockForContext(IEnumerable<SecurityRole> roles)
        {
            var context = Substitute.For<ISecurityContext>();
            context.SecurityRoles.Returns(new MockForDbSet<SecurityRole>(roles));
            return context;
        }

        private ApplicationUserManager MockForUsers(List<User> users)
        {
            var userManager = Substitute.For<ApplicationUserManager>(Substitute.For<IUserStore<User>>());
            userManager.Users.Returns(new MockForEnumerableQuery<User>(users));
            return userManager;
        }
    }
}