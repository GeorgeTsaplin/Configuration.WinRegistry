using System;
using System.Collections.Generic;
using Cti.Extensions.Configuration.WinRegistry;

namespace Microsoft.Extensions.Configuration
{
    /// <summary> Extension methods for adding <see cref="WinRegistryConfigurationProvider"/>
    /// </summary>
    public static class WinRegistryConfigurationExtensions
    {
        /// <summary> Adds Windows Registry configuration provider for <paramref name="sectionPath"/> to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="registryRoot">registry root getter</param>
        /// <param name="sectionPath">section path</param>
        /// <param name="dataAdapter">configuration data adapter (optional). Invoked after reading data from registry</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddRegistrySection(
            this IConfigurationBuilder builder,
            Func<Microsoft.Win32.RegistryKey> registryRoot,
            string sectionPath,
            Action<IDictionary<string, string>> dataAdapter = null)
            => builder.Add(new WinRegistryConfigurationSource(registryRoot, sectionPath));
    }
}
