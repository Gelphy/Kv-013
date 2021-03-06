﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

using GitHubExtension.Statistics.WebApi.CommunicationModels;
using GitHubExtension.Statistics.WebApi.Mappers.GitHub;
using GitHubExtension.Statistics.WebApi.Queries.Interfaces;

namespace GitHubExtension.Statistics.WebApi.Queries.Implementations
{
    public class StatisticsQuery : IStatisticsQuery
    {
        private readonly IGitHubQuery _gitHubQuery;

        public StatisticsQuery(IGitHubQuery gitHubQuery)
        {
            _gitHubQuery = gitHubQuery;
        }

        public ICollection<string> GetActivityMonths()
        {
            int countMounthInYear = 12;
            DateTime timeTo = DateTime.Now;
            DateTime timeFrom = GetTimeFrom();
            ICollection<string> activityMonths = new List<string>();
            int countMonthInFromTo = GetMonthsInFromTo(timeTo, timeFrom);

            if (timeTo.Year != timeFrom.Year)
            {
                countMonthInFromTo += countMounthInYear * (timeTo.Year - timeFrom.Year);
            }

            for (int i = 0; i < countMonthInFromTo; i++)
            {
                activityMonths.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(timeFrom.AddMonths(i).Month));
            }

            return activityMonths;
        }

        public async Task<ICollection<ICollection<int>>> GetCommitsRepositories(string userName, string token)
        {
            ICollection<ICollection<int>> commitsRepositories = new List<ICollection<int>>();

            ICollection<RepositoryModel> repositories = await _gitHubQuery.GetRepositories(userName, token);
            foreach (var item in repositories)
            {
                ICollection<int> commitsInYear = await _gitHubQuery.GetCommitsRepository(userName, token, item.Name);
                ICollection<int> commitsInMonths = _gitHubQuery.ToMonths(commitsInYear);
                commitsRepositories.Add(commitsInMonths);
            }

            return commitsRepositories;
        }

        public async Task<ICollection<int>> GetCommitsRepository(string userName, string token, string repository)
        {
            ICollection<int> commitsRepository = await _gitHubQuery.GetCommitsRepository(userName, token, repository);
            return commitsRepository;
        }

        public async Task<int> GetFollowerCount(string userName, string token)
        {
            int countFollower = await _gitHubQuery.GetFollowersCount(userName, token);
            return countFollower;
        }

        public async Task<int> GetFollowingCount(string userName, string token)
        {
            int countFollowing = await _gitHubQuery.GetFolowingCount(userName, token);
            return countFollowing;
        }

        public ICollection<int> GetGroupCommits(ICollection<ICollection<int>> commitsEverRepository)
        {
            ICollection<int> commitsForYear = _gitHubQuery.ToGroupCommits(commitsEverRepository);
            return commitsForYear;
        }

        public int GetMonthsInFromTo(DateTime to, DateTime from)
        {
            int countMonthInFromTo = to.Month - from.Month;
            return countMonthInFromTo;
        }

        public async Task<ICollection<RepositoryModel>> GetRepositories(string userName, string token)
        {
            ICollection<RepositoryModel> repositories = new List<RepositoryModel>();
            repositories = await _gitHubQuery.GetRepositories(userName, token);
            return repositories;
        }

        public async Task<int> GetRepositoriesCount(string userName, string token)
        {
            ICollection<RepositoryModel> repositories = await _gitHubQuery.GetRepositories(userName, token);
            int countRepositories = repositories.Count;
            return countRepositories;
        }

        public DateTime GetTimeFrom()
        {
            int countDaysInYear = 364;
            DateTime timeFrom = DateTime.Now.AddDays(-countDaysInYear);
            return timeFrom;
        }

        public ICollection<int> GetToMonths(ICollection<int> commits)
        {
            ICollection<int> commitsMonths = _gitHubQuery.ToMonths(commits);
            return commitsMonths;
        }
    }
}