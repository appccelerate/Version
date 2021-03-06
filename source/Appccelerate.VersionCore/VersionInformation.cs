﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VersionInformation.cs" company="Appccelerate">
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
    using System;

    public struct VersionInformation
    {
        public VersionInformation(
            Version version,
            Version fileVersion,
            string nugetVersion,
            string informationalVersion)
            : this()
        {
            this.Version = version;
            this.FileVersion = fileVersion;
            this.InformationalVersion = informationalVersion;
            this.NugetVersion = nugetVersion;
        }

        public Version Version { get; private set; }

        public Version FileVersion { get; private set; }

        public string NugetVersion { get; private set; }

        public string InformationalVersion { get; set; }
    }
}