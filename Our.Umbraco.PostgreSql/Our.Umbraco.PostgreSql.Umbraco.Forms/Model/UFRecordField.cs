using System;
using System.Collections.Generic;
using System.Text;

namespace Our.Umbraco.PostgreSql.Umbraco.Forms.Model
{
    public class UFRecordField
    {
        public Guid Key { get; set; }
        public Guid FieldId { get; set; }

        public int Record { get; set; }
        public string Alias { get; set; }
        public string DataType { get; set; }
    }
}
