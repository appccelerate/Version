// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TeamCity.cs" company="Appccelerate">
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

    public class TeamCity
    {
        public static void WriteSetParameterMessage(string name, string value, Action<string> log)
        {
            string escapedValue = EscapeValue(value);

            log(string.Format("##teamcity[setParameter name='Appccelerate.Version.{0}' value='{1}']", name, escapedValue));
            log(string.Format("##teamcity[setParameter name='system.Appccelerate.Version.{0}' value='{1}']", name, escapedValue));
        }

        public static void WriteSetVersionMessage(string versionToUseForBuildNumber, Action<string> log)
        {
            log(string.Format("##teamcity[buildNumber '{0}']", EscapeValue(versionToUseForBuildNumber)));
        }

        private static string EscapeValue(string value)
        {
            if (value == null)
            {
                return null;
            }

            // List of escape values from http://confluence.jetbrains.com/display/TCD8/Build+Script+Interaction+with+TeamCity
            value = value.Replace("|", "||");
            value = value.Replace("'", "|'");
            value = value.Replace("[", "|[");
            value = value.Replace("]", "|]");
            value = value.Replace("\r", "|r");
            value = value.Replace("\n", "|n");

            return value;
        }
    }
}