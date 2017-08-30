﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VersionCalculatorFacts.cs" company="Appccelerate">
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

namespace Appccelerate.Version.Facts
{
    using System;

    using FluentAssertions;

    using Xunit;
    using Xunit.Extensions;

    public class VersionCalculatorFacts
    {
        private readonly VersionCalculator testee;

        public VersionCalculatorFacts()
        {
            this.testee = new VersionCalculator();
        }

        [Theory]
        [InlineData("1")]
        [InlineData("1.0")]
        [InlineData("1.0.0")]
        public void AddsMissingVersionPartsAsZeros(string versionPattern)
        {
            VersionInformation result = this.testee.CalculateVersion(versionPattern, "2.0.0.0", null, 0, null);

            result.Should().Be(new VersionInformation(new Version("1.0.0.0"), new Version("2.0.0.0"), "1.0.0", string.Empty));
        }

        [Theory]
        [InlineData("1.{0}", "3.{0}", 0, "1.0.0.0", "3.0.0.0", "1.0.0")]
        [InlineData("1.2.{0}", "4.5.{0}", 0, "1.2.0.0", "4.5.0.0", "1.2.0")]
        [InlineData("1.{2}.3", "4.{5}.6", 0, "1.2.3.0", "4.5.6.0", "1.2.3")]
        [InlineData("1.{2}", "4.{5}", 5, "1.7.0.0", "4.10.0.0", "1.7.0")]
        public void ReplacesPlaceholderWithNumberOfCommitsSinceVersionTaggedCommit(
            string versionPattern, 
            string fileVersionPattern,
            int commitsSinceVersionTaggedCommit, 
            string expectedVersion, 
            string expectedFileVersion, 
            string expectedNugetVersion)
        {
            VersionInformation result = this.testee.CalculateVersion(versionPattern, fileVersionPattern, null, commitsSinceVersionTaggedCommit, null);

            result.Should().Be(new VersionInformation(new Version(expectedVersion), new Version(expectedFileVersion), expectedNugetVersion, string.Empty));
        }

        [Theory]
        [InlineData("1")]
        [InlineData("1.0")]
        [InlineData("1.0.0")]
        public void AddsMissingFileVersionPartsAsZeros(string fileVersionPattern)
        {
            VersionInformation result = this.testee.CalculateVersion("2.0.0.0", fileVersionPattern, null, 0, null);

            result.Should().Be(new VersionInformation(new Version("2.0.0.0"), new Version("1.0.0.0"), "2.0.0", string.Empty));
        }
        
        [Fact]
        public void KeepsFormattingOfPlaceholder()
        {
            VersionInformation result = this.testee.CalculateVersion("1-pre{0002}", "1.0.{0}.0", null, 125, null);

            result.Should().Be(new VersionInformation(new Version("1.0.0.0"), new Version("1.0.125.0"), "1.0.0-pre0127", string.Empty));
        }

        [Fact]
        public void SupportsNugetPreReleaseVersions()
        {
            VersionInformation result = this.testee.CalculateVersion("1.2.3-pre", "2.0.0.0", null, 0, null);

            result.Should().Be(new VersionInformation(new Version("1.2.3.0"), new Version("2.0.0.0"), "1.2.3-pre", string.Empty));
        }

        [Fact]
        public void SupportsNugetPreReleaseVersionsWithCommitsCountingInPrereleasePart()
        {
            VersionInformation result = this.testee.CalculateVersion("1.2.3-pre{2}", "2.0.{0}.0", null, 3, null);

            result.Should().Be(new VersionInformation(new Version("1.2.3.0"), new Version("2.0.3.0"), "1.2.3-pre5", string.Empty));
        }

        [Fact]
        public void SupportsNugetPreReleaseVersionsWithCommitsCountingInVersionPart()
        {
            VersionInformation result = this.testee.CalculateVersion("1.{2}.3-pre", "2.0.{0}.0", null, 3, null);

            result.Should().Be(new VersionInformation(new Version("1.5.3.0"), new Version("2.0.3.0"), "1.5.3-pre", string.Empty));
        }

        [Theory]
        [InlineData("1.2#comment", "1.2.0.0", "1.2.0")]
        [InlineData("1.2-pre#comment", "1.2.0.0", "1.2.0-pre")]
        public void IgnoresComments(string versionPattern, string expectedVersion, string expectedNugetVersion)
        {
            VersionInformation result = this.testee.CalculateVersion(versionPattern, "2.0.0.0", null, 0, null);

            result.Should().Be(new VersionInformation(new Version(expectedVersion), new Version("2.0.0.0"), expectedNugetVersion, string.Empty));
        }

        [Fact]
        public void ReplacesVersionPlaceholderInInformationalVersion()
        {
            VersionInformation result = this.testee.CalculateVersion("1.{2}.3-pre", "2.0.{0}.0", "RC 1 {version}", 3, null);

            result.Should().Be(new VersionInformation(new Version("1.5.3.0"), new Version("2.0.3.0"),  "1.5.3-pre", "RC 1 1.5.3.0"));
        }

        [Fact]
        public void ReplacesNugetVersionPlaceholderInInformationalVersion()
        {
            VersionInformation result = this.testee.CalculateVersion("1.{2}.3-pre", "2.0.{0}.0", "RC 1 {nugetVersion}", 3, null);

            result.Should().Be(new VersionInformation(new Version("1.5.3.0"), new Version("2.0.3.0"), "1.5.3-pre", "RC 1 1.5.3-pre"));
        }

        [Fact]
        public void AddsPrereleaseInformation_WhenItIsAPullRequest()
        {
            VersionInformation result = this.testee.CalculateVersion("1.{2}", "2.0.{0}.0", null, 0, "override");

            result.Should().Be(new VersionInformation(new Version("1.2.0.0"), new Version("2.0.0.0"), "1.2.0-override", string.Empty));
        }

        [Fact]
        public void ReplacesPrereleaseInformation_WhenItIsAPullRequest()
        {
            VersionInformation result = this.testee.CalculateVersion("1.{2}-pre", "2.0.0.0", null, 0, "override");

            result.Should().Be(new VersionInformation(new Version("1.2.0.0"), new Version("2.0.0.0"), "1.2.0-override", string.Empty));
        }

        [Fact]
        public void KeepsVersion_WhenVersionPatternWithoutPlaceholderAndNoCommitsSinceVersionTaggedCommit()
        {
            const string Version = "1.2.3.0";

            VersionInformation result = this.testee.CalculateVersion(Version, "2.0.0.0", null, 0, null);

            result.Should().Be(new VersionInformation(new Version(Version), new Version("2.0.0.0"), "1.2.3", string.Empty));
        }

        [Fact]
        public void ThrowsInvalidOperationException_WhenVersionPatternWithoutPlaceholderAndCommitsSinceVersionTaggedCommitGreaterZero()
        {
            const string VersionPattern = "1.2.3.0";

            Action action = () => this.testee.CalculateVersion(VersionPattern, "2.0.0.0", null, 1, null);

            action.ShouldThrow<InvalidOperationException>()
                .And.Message.Should().Be(VersionCalculator.FormatCannotVersionDueToMissingCommitsCountingPlaceholderExceptionMessage(VersionPattern));
        }
    }
}
