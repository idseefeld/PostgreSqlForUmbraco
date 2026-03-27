using global::Umbraco.Forms.Core;
using global::Umbraco.Forms.Core.Models;
using System;
using System.Collections.Generic;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Models;

namespace Our.Umbraco.PostgreSql.Umbraco.Forms.FormsExtentions
{
    public class FixedListPrevalueSource : FieldPreValueSourceType
    {
        public FixedListPrevalueSource()
        {
            Id = new Guid("42C8158D-2AA8-4621-B653-6A63C7545768");
            Name = "Fixed List";
            Description = "Example prevalue source providing a fixed list of values.";
        }

        public List<PreValue> GetPreValues(Field field, Form form) =>
            new List<PreValue>
            {
                new PreValue
                {
                    Id = "82d5612c-c877-4827-83e1-14678220de7f",
                    Value = "item-one",
                    Caption = "Item One"
                },
                new PreValue
                {
                    Id = "47fb9514-61d8-44e3-91f7-81672459e306",
                    Value = "item-two",
                    Caption = "Item Two"
                }
            };

        public override Task<List<PreValue>> GetPreValuesAsync(Field field, Form form)
        {
            return Task.FromResult(GetPreValues(field, form));
        }

        /// <inheritdoc/>
        public override List<Exception> ValidateSettings()
        {
            // this is used to validate any dynamic settings you might apply to the PreValueSource
            // if there are no dynamic settings, return an empty list of Exceptions:
            var exceptions = new List<Exception>();
            return exceptions;
        }
    }
}