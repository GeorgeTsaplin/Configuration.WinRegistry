using System;
using System.Collections.Generic;
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
        /// <param name="dataAdapter">configuration data adapter (optional). Will be invoked after reading data from registry.</param>
        public WinRegistryConfigurationSource(
            Func<Microsoft.Win32.RegistryKey> registryRoot,
            string sectionPath,
            Action<IDictionary<string, string>> dataAdapter = null)
        {
            if (registryRoot == null) throw new ArgumentNullException(nameof(registryRoot));

            if (string.IsNullOrWhiteSpace(sectionPath)) throw new ArgumentException("Registry section name must be specified", nameof(sectionPath));

            this.SectionPath = sectionPath;

            this.RegistryRoot = registryRoot;

            this.DataAdapter = dataAdapter;
        }

        /// <summary> Gets Windows Registry section path
        /// </summary>
        public string SectionPath { get; }

        /// <summary> Gets Registry root getter
        /// </summary>
        public Func<RegistryKey> RegistryRoot { get; }

        /// <summary> Gets optinal configuration data adapter.
        /// Will be invoked after reading data from registry.
        /// </summary>
        public Action<IDictionary<string, string>> DataAdapter { get; }

        /// <inheritdoc />
        public IConfigurationProvider Build(IConfigurationBuilder builder)
            => new WinRegistryConfigurationProvider(this);
    }
}
