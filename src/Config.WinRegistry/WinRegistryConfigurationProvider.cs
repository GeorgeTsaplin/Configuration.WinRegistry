using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;

namespace Cti.Extensions.Configuration.WinRegistry
{
    /// <summary> Configuration provider for Windows Registry
    /// </summary>
    public class WinRegistryConfigurationProvider : ConfigurationProvider
    {
        private readonly WinRegistryConfigurationSource source;

        /// <summary> Creates configuration provider for Windows Registry.
        /// </summary>
        /// <param name="source">The source settings.</param>
        public WinRegistryConfigurationProvider(WinRegistryConfigurationSource source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            this.source = source;
        }

        /// <inheritdoc />
        public override void Load()
        {
            var root = this.source.RegistryRoot.Invoke();

            if (root == null)
            {
                throw new ArgumentOutOfRangeException(nameof(this.source), $"{nameof(WinRegistryConfigurationSource.RegistryRoot)} must return not null instance of {this.source.RegistryRoot.Method.ReturnType.Name}");
            }

            var section = root.OpenSubKey(this.source.SectionPath);

            if (section == null)
            {
                this.Data = new Dictionary<string, string>();
            }

            var data = new Dictionary<string, string>();
            var prefixStack = new Stack<string>();

            if (!string.IsNullOrWhiteSpace(this.source.RootSection))
            {
                prefixStack.Push(this.source.RootSection);
            }

            try
            {
                ReadSection(section, data, prefixStack);
            }
            finally
            {
                section.Dispose();
            }

            this.source.DataAdapter?.Invoke(data);

            this.Data = data;
        }

        private static void ReadSection(RegistryKey section, IDictionary<string, string> data, Stack<string> prefixStack)
        {
            foreach (var subkeyName in section.GetSubKeyNames())
            {
                prefixStack.Push(subkeyName);

                using (var subkey = section.OpenSubKey(subkeyName))
                {
                    ReadSection(subkey, data, prefixStack);
                }

                prefixStack.Pop();
            }

            foreach (var valueName in section.GetValueNames())
            {
                prefixStack.Push(valueName.Replace(".", string.Empty));

                data[ConfigurationPath.Combine(prefixStack.Reverse())] = section.GetValue(valueName)?.ToString();

                prefixStack.Pop();
            }
        }
    }
}
