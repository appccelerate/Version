// --------------------------------------------------------------------------------------------------------------------
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
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class VersionCalculator
    {
        public VersionInformation CalculateVersion(
            string versionPattern, 
            string informationalVersionPattern, 
            int commitsSinceLastTaggedVersion,
            bool isPullRequest)
        {
            informationalVersionPattern = informationalVersionPattern ?? string.Empty;

            int commentIndex = versionPattern.IndexOf('#');
            versionPattern = commentIndex > 0 ? versionPattern.Substring(0, commentIndex) : versionPattern;

            var r = new Regex(@"\{[0-9]+\}");

            Match match = r.Match(versionPattern);
            if (match.Success)
            {
                int placeholderLength = match.Value.Length - 2;
                int baseNumber = int.Parse(match.Value.Substring(1, placeholderLength));
                int calculatedNumber = baseNumber + commitsSinceLastTaggedVersion;
                string paddedCalculatedNumber = calculatedNumber.ToString(new string('0', placeholderLength));
                versionPattern = versionPattern.Replace(match.Value, paddedCalculatedNumber);
            }

            int dashIndex = versionPattern.IndexOf('-');
            string prerelease = dashIndex > 0 ? versionPattern.Substring(dashIndex + 1) : string.Empty;

            prerelease = isPullRequest ? "PullRequest" : prerelease;

            string version = dashIndex > 0 ? versionPattern.Substring(0, dashIndex) : versionPattern;

            for (int i = version.Count(c => c == '.'); i < 3; i++)
            {
                version += ".0";
            }

            int j = 0;
            int thirdDotIndex = version
                .Select(c => new { Index = j++, Char = c })
                .Where(x => x.Char == '.')
                .Skip(2)
                .Select(x => x.Index)
                .Single();

            string nugetVersion = version.Substring(0, thirdDotIndex) + (prerelease.Any() ? "-" + prerelease : string.Empty);

            string informationalVersion = informationalVersionPattern
                .Replace("{version}", version)
                .Replace("{nugetVersion}", nugetVersion);
            
            return new VersionInformation(
                System.Version.Parse(version),
                nugetVersion,
                informationalVersion);
        }
    }
}