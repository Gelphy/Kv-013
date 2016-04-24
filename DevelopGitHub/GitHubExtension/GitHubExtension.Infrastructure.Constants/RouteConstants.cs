﻿namespace GitHubExtension.Infrastructure.Constants
{
    public static class RouteConstants
    {
        // routes
        public const string AccountLogout = Logout;

        public const string AccountsRegister = Register;

        public const string ApiAccount = "api/account";

        public const string ApiRepository = "api/repos";

        // route prefixes
        public const string ApiRoles = "api/roles";

        public const string ApiUser = "api/user";

        public const string AssignRolesToUser = ApiRepository + RepositoryId + Collaborators + GitHubId;

        public const string Collaborators = "/collaborators";

        public const string ExternalLogin = "externalLogin";

        public const string GetByIdRepository = ApiRepository + Id_int;

        public const string GetCollaboratorsForRepository = ApiRepository + RepositoryName + Collaborators;

        public const string GetExternalLogin = ExternalLogin;

        public const string GetRepositoryForCurrentUser = ApiUser + Repository;

        public const string GetUser = User + Id_guid;

        public const string GetUserByName = User + UserName;

        public const string GitHubId = "/{gitHubId}";

        public const string Id_guid = "/{id:guid}";

        // segments
        public const string Id_int = "/{id:int}";

        // static segments
        public const string Logout = "logout";

        public const string ObtainLocalAccessToken = "obtainLocalAccessToken";

        public const string Register = "register";

        public const string RegisterExternal = "registerExternal";

        public const string Repository = "/repos";

        public const string RepositoryId = "/{repoId}";

        public const string RepositoryName = "/{repoName}";

        public const string User = "user";

        public const string UserName = "/{username}";
    }
}