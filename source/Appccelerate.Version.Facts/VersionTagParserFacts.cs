// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VersionTagParserFacts.cs" company="Appccelerate">
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
    using FluentAssertions;

    using Xunit;
    using Xunit.Extensions;

    public class VersionTagParserFacts
    {
        private readonly VersionTagParser testee;

        public VersionTagParserFacts()
        {
            this.testee = new VersionTagParser();
        }
        
        [Theory]
        [InlineData("4.{2}-alpha001", "4.{2}.0")]
        [InlineData("11.{13}.0.0", "11.{0}.0")]
        [InlineData("5.0-alpha{0001}#comment", "5.{0}.0")]
        public void ReturnsParsedVersionTag(string expectedVersion, string expectedFileVersion)
        {
            string versionTag = $"v={expectedVersion};fv={expectedFileVersion}";

            VersionTag result = this.testee.Parse(versionTag);

            result.Should().Be(new VersionTag(expectedVersion, expectedFileVersion));
        }

        [Fact]
        public void SetFileVersionToVersionWhenTagContainsNoFileVersion()
        {
            const string ExpectedVersion = "version";
            string versionTag = $"v={ExpectedVersion}";

            VersionTag result = this.testee.Parse(versionTag);

            result.Should().Be(new VersionTag(ExpectedVersion, ExpectedVersion));
        }
    }
}
