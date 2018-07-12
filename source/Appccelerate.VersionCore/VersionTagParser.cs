// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VersionTagParser.cs" company="Appccelerate">
//   Copyright (c) 2008-2018 Appccelerate
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
    using System.Text.RegularExpressions;

    public class VersionTagParser
    {
        private static readonly Regex VersionRegex =
            new Regex(@"\bv=(?<version>[^;]*)(;fv=(?<fileVersion>[^ ]*))?", RegexOptions.Compiled);

        public VersionTag Parse(string versionTag)
        {
            Match match = VersionRegex.Match(versionTag);

            string version = match.Groups["version"].Value;

            var matchGroup = match.Groups["fileVersion"];
            string fileVersion = matchGroup.Success ? matchGroup.Value : version;

            return new VersionTag(version, fileVersion);
        }
    }
}