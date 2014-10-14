// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Appccelerate">
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

    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                string startingPath = args[0];

                var repositoryVersionInformationLoader = new RepositoryVersionInformationLoader();

                RepositoryVersionInformation repositoryVersionInformation = repositoryVersionInformationLoader.GetRepositoryVersionInformation(startingPath);
                
                var calculator = new VersionCalculator();

                var version = calculator.CalculateVersion(
                    repositoryVersionInformation.LastTaggedVersion,
                    repositoryVersionInformation.AnnotationMessage,
                    repositoryVersionInformation.CommitsSinceLastTaggedVersion,
                    repositoryVersionInformation.PrereleaseOverride);

                Console.WriteLine("{");
                Console.WriteLine("\"Version\": \"" + version.Version + "\",");
                Console.WriteLine("\"NugetVersion\": \"" + version.NugetVersion + "\",");
                Console.WriteLine("\"InformationalVersion\": \"" + version.InformationalVersion + "\"");
                Console.WriteLine("}");
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error occured: " + exception);
            }
        }
    }
}
