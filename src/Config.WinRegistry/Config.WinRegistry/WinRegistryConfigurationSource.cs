﻿using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;

namespace Cti.Extensions.Configuration.WinRegistry
{
    /// <summary> Represents Windows Registry as and <see cref="IConfigurationSource"/>
    /// </summary>
    public class WinRegistryConfigurationSource : IConfigurationSource
    {
        /// <summary> Creates <see cref="IConfigurationSource"/> for specified <paramref name="sectionPath"/> of Windows Registry
        /// </summary>
        /// <param name="registryRoot">registry root getter</param>
        /// <param name="sectionPath">section path</param>
        public WinRegistryConfigurationSource(Func<Microsoft.Win32.RegistryKey> registryRoot, string sectionPath)
        {
            if (registryRoot == null) throw new ArgumentNullException(nameof(registryRoot));

            if (string.IsNullOrWhiteSpace(sectionPath)) throw new ArgumentException("Registry section name must be specified", nameof(sectionPath));

            this.SectionPath = sectionPath;

            this.RegistryRoot = registryRoot;
        }

        /// <summary> Gets Windows Registry section path
        /// </summary>
        public string SectionPath { get; }

        /// <summary> Gets Registry root getter
        /// </summary>
        public Func<RegistryKey> RegistryRoot { get; }

        /// <inheritdoc />
        public IConfigurationProvider Build(IConfigurationBuilder builder)
            => new WinRegistryConfigurationProvider(this);
    }
}
