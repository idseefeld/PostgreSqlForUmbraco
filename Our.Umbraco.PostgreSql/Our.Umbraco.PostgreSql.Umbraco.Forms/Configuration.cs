using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Our.Umbraco.PostgreSql.Umbraco.Forms
{
    public class Configuration
    {
        public const string SectionName = "PostgreSqlProvider";

        public FormsConfiguration Forms { get; set; } = new FormsConfiguration();
    }

    public class FormsConfiguration
    {
        public bool FixDateInRecordJson { get; set; } = true;
    }
}
