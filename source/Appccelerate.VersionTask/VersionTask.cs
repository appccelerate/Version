// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VersionTask.cs" company="Appccelerate">
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

namespace Appccelerate.VersionTask
{
    using System;
    using System.IO;

    using Appccelerate.Version;
    using Appccelerate.VersionTask.Annotations;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    public class VersionTask : Task
    {
        [Required]
        public string SolutionDirectory { get; [UsedImplicitly] set; }

        [Required]
        public string ProjectFile { get; [UsedImplicitly] set; }

        [Output]
        public string TempAssemblyInfoFilePath { get; [UsedImplicitly] set; }

        public override bool Execute()
        {
            try
            {
                string startingPath = this.SolutionDirectory;

                var repositoryVersionInformationLoader = new RepositoryVersionInformationLoader();

                RepositoryVersionInformation repositoryVersionInformation = repositoryVersionInformationLoader.GetRepositoryVersionInformation(startingPath);

                this.Log.LogMessage(MessageImportance.Normal, "version pattern = " + repositoryVersionInformation.LastTaggedVersion + ", commits since tag = " + repositoryVersionInformation.CommitsSinceLastTaggedVersion);

                var calculator = new VersionCalculator();

                var version = calculator.CalculateVersion(
                    repositoryVersionInformation.LastTaggedVersion,
                    repositoryVersionInformation.AnnotationMessage,
                    repositoryVersionInformation.CommitsSinceLastTaggedVersion,
                    repositoryVersionInformation.PrereleaseOverride);

                this.Log.LogMessage(MessageImportance.Normal, "Version: " + version.Version);
                this.Log.LogMessage(MessageImportance.Normal, "NugetVersion: " + version.NugetVersion);
                this.Log.LogMessage(MessageImportance.Normal, "InformationalVersion:" + version.InformationalVersion);
                this.Log.LogMessage(MessageImportance.Normal, "PrereleaseOverride:" + repositoryVersionInformation.PrereleaseOverride);

                string versionAssemblyInfo = string.Format(
@"
using System;
using System.Reflection;

[assembly: AssemblyVersion(""{0}"")]
[assembly: AssemblyFileVersion(""{0}"")]
[assembly: AssemblyInformationalVersion(""{1}"")]
", 
 version.Version, 
 version.InformationalVersion);

                string tempFolder = Path.Combine(Path.GetTempPath(), "Appccelerate.VersionTask");

                if (!Directory.Exists(tempFolder))
                {
                    Directory.CreateDirectory(tempFolder);
                }

                foreach (string tempFilePath in Directory.GetFiles(tempFolder))
                {
                    try
                    {
                        // we cannot delete just all files because they might be used in other projects currently built
                        if (File.GetLastWriteTime(tempFilePath) < DateTime.Now.AddDays(-1))
                        {
                            File.Delete(tempFilePath);
                        }
                    }
                    // ReSharper disable once EmptyGeneralCatchClause
                    catch
                    {
                        // try next time
                    }
                }
                
                var tempFileName = string.Format("AssemblyInfo_{0}_{1}.g.cs", Path.GetFileNameWithoutExtension(this.ProjectFile), Path.GetRandomFileName());
                this.TempAssemblyInfoFilePath = Path.Combine(tempFolder, tempFileName);
                File.WriteAllText(this.TempAssemblyInfoFilePath, versionAssemblyInfo);
                
                TeamCity.WriteSetParameterMessage("Version", version.Version.ToString(), this.WriteToLog);
                TeamCity.WriteSetParameterMessage("InformationalVersion", version.InformationalVersion, this.WriteToLog);
                TeamCity.WriteSetParameterMessage("NugetVersion", version.NugetVersion, this.WriteToLog);

                return true;
            }
            catch (Exception exception)
            {
                this.Log.LogErrorFromException(exception);
                
                return false;
            }
        }

        private void WriteToLog(string message)
        {
            this.Log.LogMessage(MessageImportance.Normal, message);
        }
    }
}