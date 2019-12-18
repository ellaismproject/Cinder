﻿using Cinder.Core.SharedKernel;

namespace Cinder.Api.Host.Infrastructure
{
    public class Settings
    {
        public DatabaseSettings Database { get; set; } = new DatabaseSettings();

        public class DatabaseSettings : IDatabaseSettings
        {
            public string ConnectionString { get; set; }
            public string Tag { get; set; }
            public string Locale { get; set; }
        }
    }
}
