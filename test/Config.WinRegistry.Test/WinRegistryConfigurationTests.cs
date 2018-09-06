using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Cti.Extensions.Configuration.WinRegistry.Test
{
    public class WinRegistryConfigurationTests
    {
        [Fact]
        public void TryGet_ExistingKey_ShouldReturn()
        {
            // Arrange
            var target = new WinRegistryConfigurationProvider(new WinRegistryConfigurationSource(
                () => Microsoft.Win32.Registry.LocalMachine,
                "SOFTWARE\\Microsoft\\Shell"));

            target.Load();

            // Act
            string actual;
            target.TryGet("USB:NotifyOnUsbErrors", out actual);

            // Assert
            actual.Should().Be("1");
        }

        public enum UCCMode
        {
            TAPI = 0,
            CTIOS = 1,
            CTI = 2
        }

        public class UCC
        {
            //[TypeConverter(typeof(EnumConverter))]
            public UCCMode Mode { get; set; }
        }

        public class SqlAdapter
        {
            public int UseCustomDatabaseConnection { get; set; }

            public string SQLConnection { get; set; }
        }

        public class Dialer
        {
            public UCC UCC { get; set; }

            public SqlAdapter SqlAdapter { get; set; }
        }

        [Fact]
        public void ReadConfig_ExistingSection_ShouldRead()
        {
            // Arrange
            var configBuilder = new ConfigurationBuilder();

            var config = configBuilder.AddRegistrySection(
                () => Microsoft.Win32.Registry.LocalMachine,
                "SOFTWARE\\WOW6432Node\\CTI\\CTI Outbound 5")
                .Build();


            //TypeDescriptor.AddAttributes(typeof(Boolean), new TypeConverterAttribute(typeof(ClassLibrary1.TypeConverter.BooleanConverterEx)));

            // Act
            var actual = config.GetSection("Dialer").Get<Dialer>();

            // Assert
            actual.Should().BeEquivalentTo(
                new Dialer
                {
                    UCC = new UCC
                    {
                        Mode = UCCMode.CTIOS
                    },
                    SqlAdapter = new SqlAdapter
                    {
                        SQLConnection = "Data Source=localhost;Initial Catalog=Outbound5;Persist Security Info=True;User ID=;Password=",
                        UseCustomDatabaseConnection = 0
                    }
                });
        }

        public class DialerAdapted
        {
            public UCC UCC { get; set; }

            public SqlAdapter SqlAdapter { get; set; }

            public string NewKey { get; set; }
        }

        [Fact]
        public void ReadConfig_DataAdapter_ShouldRead()
        {
            // Arrange
            const string RootSection = "Dialer";
            const string NewKeyValue = "some value";

            var configBuilder = new ConfigurationBuilder();

            var config = configBuilder.AddRegistrySection(
                () => Microsoft.Win32.Registry.LocalMachine,
                "SOFTWARE\\WOW6432Node\\CTI\\CTI Outbound 5",
                (source) => source.DataAdapter = (data) => data.Add(ConfigurationPath.Combine(RootSection, nameof(DialerAdapted.NewKey)), NewKeyValue))
                .Build();

            // Act
            var actual = config.GetSection(RootSection).Get<DialerAdapted>();

            // Assert
            actual.Should().BeEquivalentTo(
                new DialerAdapted
                {
                    UCC = new UCC
                    {
                        Mode = UCCMode.CTIOS
                    },
                    SqlAdapter = new SqlAdapter
                    {
                        SQLConnection = "Data Source=localhost;Initial Catalog=Outbound5;Persist Security Info=True;User ID=;Password=",
                        UseCustomDatabaseConnection = 0
                    },
                    NewKey = NewKeyValue
                });
        }

        public class Root
        {
            public Dialer Dialer { get; set; }
        }

        [Fact]
        public void ReadConfig_AddRootSection_ShouldRead()
        {
            // Arrange
            const string RootSection = "RootSection";

            var configBuilder = new ConfigurationBuilder();

            var config = configBuilder.AddRegistrySection(
                () => Microsoft.Win32.Registry.LocalMachine,
                "SOFTWARE\\WOW6432Node\\CTI\\CTI Outbound 5",
                (source) => source.RootSection = RootSection)
                .Build();

            // Act
            var actual = config.GetSection(RootSection).Get<Root>();

            // Assert
            actual.Should().BeEquivalentTo(
                new Root
                {
                    Dialer = new Dialer
                    {
                        UCC = new UCC
                        {
                            Mode = UCCMode.CTIOS
                        },
                        SqlAdapter = new SqlAdapter
                        {
                            SQLConnection = "Data Source=localhost;Initial Catalog=Outbound5;Persist Security Info=True;User ID=;Password=",
                            UseCustomDatabaseConnection = 0
                        }
                    }
                });
        }
    }
}
