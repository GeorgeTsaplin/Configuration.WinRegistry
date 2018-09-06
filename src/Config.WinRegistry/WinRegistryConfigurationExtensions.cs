using System;
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
        /// <param name="configureSource">Configures the source.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddRegistrySection(
            this IConfigurationBuilder builder,
            Func<Microsoft.Win32.RegistryKey> registryRoot,
            string sectionPath,
            Action<WinRegistryConfigurationSource> configureSource = null)
        {
            var source = new WinRegistryConfigurationSource(registryRoot, sectionPath);

            configureSource?.Invoke(source);

            return builder.Add(source);
        }
    }
}
