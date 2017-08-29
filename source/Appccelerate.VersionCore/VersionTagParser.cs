// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VersionTagParser.cs" company="Appccelerate">
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
    public class VersionTagParser
    {
        public VersionTag Parse(string versionTag)
        {
            string[] splittedTag = versionTag.Split(' ');
            
            string version = splittedTag[0].Substring(2);
            string fileVersion = version;
            if (splittedTag.Length > 1)
            {
                fileVersion = splittedTag[1].Substring(3);
            }

            return new VersionTag(version, fileVersion);
        }
    }
}