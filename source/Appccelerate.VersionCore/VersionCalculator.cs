﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VersionCalculator.cs" company="Appccelerate">
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
    using System.Linq;
    using System.Text.RegularExpressions;

    public class VersionCalculator
    {
        private static readonly Regex PlaceholderRegex = new Regex(@"(?<=\{)[0-9]+(?=\})", RegexOptions.Compiled);

        public VersionInformation CalculateVersion(
            string versionPattern, 
            string fileVersionPattern, 
            string informationalVersionPattern, 
            int commitsSinceLastTaggedVersion, 
            string prereleaseVersionOverride)
        {
            informationalVersionPattern = informationalVersionPattern ?? string.Empty;

            int commentIndex = versionPattern.IndexOf('#');
            versionPattern = commentIndex > 0 ? versionPattern.Substring(0, commentIndex) : versionPattern;
            
            versionPattern = ReplaceCommitCountPlaceholder(versionPattern, commitsSinceLastTaggedVersion);

            int dashIndex = versionPattern.IndexOf('-');
            string prerelease = dashIndex > 0 ? versionPattern.Substring(dashIndex + 1) : string.Empty;

            prerelease = prereleaseVersionOverride ?? prerelease;

            string version = dashIndex > 0 ? versionPattern.Substring(0, dashIndex) : versionPattern;

            string normalizedVersion = NormalizeVersion(version);

            string fileVersion = ReplaceCommitCountPlaceholder(fileVersionPattern, commitsSinceLastTaggedVersion);
            string normalizedFileVersion = NormalizeVersion(fileVersion);

            int j = 0;
            int thirdDotIndex = normalizedVersion
                .Select(c => new { Index = j++, Char = c })
                .Where(x => x.Char == '.')
                .Skip(2)
                .Select(x => x.Index)
                .Single();

            string nugetVersion = normalizedVersion.Substring(0, thirdDotIndex) + (prerelease.Any() ? "-" + prerelease : string.Empty);

            string informationalVersion = informationalVersionPattern
                .Replace("{version}", normalizedVersion)
                .Replace("{nugetVersion}", nugetVersion);
            
            return new VersionInformation(
                Version.Parse(normalizedVersion),
                Version.Parse(normalizedFileVersion),
                nugetVersion,
                informationalVersion);
        }

        private static string ReplaceCommitCountPlaceholder(string pattern, int commitsSinceLastTaggedVersion)
        {
            Match match = PlaceholderRegex.Match(pattern);
            if (match.Success)
            {
                int placeholderLength = match.Value.Length;
                int baseNumber = int.Parse(match.Value);
                int calculatedNumber = baseNumber + commitsSinceLastTaggedVersion;
                string paddedCalculatedNumber = calculatedNumber.ToString(new string('0', placeholderLength));
                pattern = pattern.Replace("{" + match.Value + "}", paddedCalculatedNumber);
            }
            else
            {
                if (commitsSinceLastTaggedVersion > 0)
                {
                    throw new InvalidOperationException(
                        FormatCannotVersionDueToMissingCommitsCountingPlaceholderExceptionMessage(pattern));
                }
            }

            return pattern;
        }

        private static string NormalizeVersion(string version)
        {
            string normalizedVersion = version;
            for (int i = normalizedVersion.Count(c => c == '.'); i < 3; i++)
            {
                normalizedVersion += ".0";
            }

            return normalizedVersion;
        }

        public static string FormatCannotVersionDueToMissingCommitsCountingPlaceholderExceptionMessage(string versionPattern)
        {
            return "Cannot calculate version because the latest version tag has no placeholder to count commits and there are commits since the tagged commit. Last version tag = " + versionPattern;
        }
    }
}