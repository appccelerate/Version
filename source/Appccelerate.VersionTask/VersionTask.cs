namespace Appccelerate.VersionTask
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    using Appccelerate.Version;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    public class VersionTask : Task
    {
        [Required]
        public string SolutionDirectory { get; set; }

        [Required]
        public string ProjectFile { get; set; }

        [Required]
        public ITaskItem[] CompileFiles { get; set; }

        [Output]
        public string TempAssemblyInfoFilePath { get; set; }

        public override bool Execute()
        {
            try
            {
                string startingPath = this.SolutionDirectory;

                var repositoryVersionInformationLoader = new RepositoryVersionInformationLoader();

                RepositoryVersionInformation repositoryVersionInformation = repositoryVersionInformationLoader.GetRepositoryVersionInformation(startingPath);

                base.Log.LogMessage(MessageImportance.Normal, "version pattern = " + repositoryVersionInformation.LastTaggedVersion + " + " + repositoryVersionInformation.CommitsSinceLastTaggedVersion);

                var calculator = new VersionCalculator();

                var version = calculator.CalculateVersion(
                    repositoryVersionInformation.LastTaggedVersion,
                    repositoryVersionInformation.AnnotationMessage,
                    repositoryVersionInformation.CommitsSinceLastTaggedVersion);

                base.Log.LogMessage(MessageImportance.Normal, "Version: " + version.Version);
                base.Log.LogMessage(MessageImportance.Normal, "NugetVersion: " + version.NugetVersion);
                base.Log.LogMessage(MessageImportance.Normal, "InformationalVersion:" + version.InformationalVersion);


                string versionAssemblyInfo = string.Format(@"
using System;
using System.Reflection;

[assembly: AssemblyVersion(""{0}"")]
[assembly: AssemblyFileVersion(""{0}"")]
[assembly: AssemblyInformationalVersion(""{1}"")]
", version.Version, version.InformationalVersion);

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
                    catch
                    {
                        // try next time
                    }
                }
                

                var tempFileName = string.Format("AssemblyInfo_{0}_{1}.g.cs", Path.GetFileNameWithoutExtension(this.ProjectFile), Path.GetRandomFileName());
                this.TempAssemblyInfoFilePath = Path.Combine(tempFolder, tempFileName);
                File.WriteAllText(this.TempAssemblyInfoFilePath, versionAssemblyInfo);


                TeamCity.WriteSetVersionMessage(version.NugetVersion, this.Log);
                TeamCity.WriteSetParameterMessage("Version", version.Version.ToString(), this.Log);
                TeamCity.WriteSetParameterMessage("InformationalVersion", version.InformationalVersion, this.Log);
                TeamCity.WriteSetParameterMessage("NugetVersion", version.NugetVersion, this.Log);

                return true;
            }
            catch (Exception exception)
            {
                base.Log.LogErrorFromException(exception);
                
                return false;
            }
        }

        private void Log(string message)
        {
            base.Log.LogMessage(MessageImportance.Normal, message);
        }
    }
}