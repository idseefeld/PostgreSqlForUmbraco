using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;

namespace Our.Umbraco.PostgreSql.Umbraco.Forms.FormsExtentions
{
    public class PostgreSqlDataSourceType : global::Umbraco.Forms.Core.FormDataSourceType
    {
        private readonly ILogger<PostgreSqlDataSourceType> _logger;

        private bool _supportsGetRecords = true;

        private bool _supportsInsert = true;

        private bool _supportsPrevalues = true;

        public override bool SupportsGetRecords
        {
            get
            {
                return _supportsGetRecords;
            }
            set
            {
                _supportsGetRecords = value;
            }
        }

        public override bool SupportsInsert
        {
            get
            {
                return _supportsInsert;
            }
            set
            {
                _supportsInsert = value;
            }
        }

        public override bool SupportsPreValues
        {
            get
            {
                return _supportsPrevalues;
            }
            set
            {
                _supportsPrevalues = value;
            }
        }

        [global::Umbraco.Forms.Core.Attributes.Setting("ConnectionString",
            Description = "PostgreSQL specific connection string.",
            View = "Umb.PropertyEditorUi.TextArea")]
        public string ConnectionString { get; set; } = string.Empty;

        [global::Umbraco.Forms.Core.Attributes.Setting("Table",
            Description = "The database table",
            View = "Umb.PropertyEditorUi.TextBox")]
        public string Table { get; set; } = string.Empty;

        public PostgreSqlDataSourceType(ILogger<PostgreSqlDataSourceType> logger)
        {
            _logger = logger;

            this.Name = "PostgreSQL Data Source";
            this.Id = new Guid("97908e3b-e75f-4a5c-aea6-fa9281293d72");
            this.Description = "A data source for PostgreSQL databases.";
            this.Icon = "icon-database";
        }
        
        public override void Dispose()
        {

        }

        public override Dictionary<object, FormDataSourceField> GetAvailableFields()
        {
            var rVal = new Dictionary<object, FormDataSourceField>();

            return rVal;
        }

        public override Dictionary<object, FormDataSourceField> GetMappedFields()
        {
            var rVal = new Dictionary<object, FormDataSourceField>();

            return rVal;
        }

        public override Dictionary<object, string> GetPreValues(Field field, Form form)
        {
            var rVal = new Dictionary<object, string>();

            return rVal;
        }

        public override List<Record> GetRecords(Form form, int page, int maxItems, object sortByField, RecordSorting order)
        {
            var rVal = new List<Record>();


            return rVal;
        }

        public override Record InsertRecord(Record record)
        {
            var rVal = record;

            return rVal;
        }

        public override List<Exception> ValidateSettings()
        {
            var rVal = new List<Exception>();

            return rVal;
        }
    }
}
