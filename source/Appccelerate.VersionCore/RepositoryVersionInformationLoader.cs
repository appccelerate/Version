// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepositoryVersionInformationLoader.cs" company="Appccelerate">
//   Copyright (c) 2008-2014
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Appccelerate.Version
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LibGit2Sharp;

    public class RepositoryVersionInformationLoader
    {
        public RepositoryVersionInformation GetRepositoryVersionInformation(string startingPath)
        {
            string repositoryPath = Repository.Discover(startingPath);

            if (string.IsNullOrEmpty(repositoryPath))
            {
                throw new InvalidOperationException("The path is not part of a git repository: " + startingPath);
            }

            var repository = new Repository(repositoryPath);

            return this.GetRepositoryVersionInformation(repository);
        }

        public RepositoryVersionInformation GetRepositoryVersionInformation(Repository repository)
        {
            IEnumerable<Tag> allVersionTags = FindAllVersionTags(repository).ToArray();
            Commit lastTaggedCommit = FindLastVersionTaggedCommit(repository, allVersionTags);
            Tag latestVersionTag = FindLatestVersionTag(allVersionTags, lastTaggedCommit);
            int commits = CountCommitsSinceVersionTag(repository, lastTaggedCommit);
            string prereleaseOverride = DeterminePrereleaseOverride(repository);

            return new RepositoryVersionInformation(
                latestVersionTag.Name.Substring(2),
                commits,
                latestVersionTag.IsAnnotated ? latestVersionTag.Annotation.Message.Replace("\n", string.Empty).Replace("\r", string.Empty) : string.Empty,
                prereleaseOverride);
        }

        private static Commit FindLastVersionTaggedCommit(Repository repository, IEnumerable<Tag> allVersionTags)
        {
            Commit lastTaggedCommit =
                repository.Head.Commits.FirstOrDefault(commit => allVersionTags.Any(tag => tag.Target == commit));

            if (lastTaggedCommit == null)
            {
                throw new InvalidOperationException("No version tag found. Add a tag with name 'v=<version pattern>'");
            }

            return lastTaggedCommit;
        }

        private static Tag FindLatestVersionTag(IEnumerable<Tag> allVersionTags, Commit lastTaggedCommit)
        {
            return allVersionTags.Last(tag => tag.Target.Sha == lastTaggedCommit.Sha);
        }

        private static IEnumerable<Tag> FindAllVersionTags(Repository repository)
        {
            return repository.Tags.Where(tag => tag.Name.StartsWith("v="));
        }

        private static int CountCommitsSinceVersionTag(Repository repository, Commit lastTaggedCommit)
        {
            var query = new CommitFilter
                            {
                                Since = repository.Head.Tip,
                                Until = lastTaggedCommit,
                                SortBy = CommitSortStrategies.Topological | CommitSortStrategies.Time
                            };

            return repository.Commits.QueryBy(query).Count();
        }

        private static string DeterminePrereleaseOverride(Repository repository)
        {
            return repository.Head.CanonicalName.Equals("(no branch)", StringComparison.OrdinalIgnoreCase)
                       ? "commit" + repository.Head.Tip.Sha.Substring(0, 8)
                       : null;
        }
    }
}