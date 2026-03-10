using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NPoco;
using System;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseModelDefinitions;
using Umbraco.Cms.Infrastructure.Persistence.Dtos;

namespace Our.Umbraco.PostgreSql.Migrations.Dtos
{
    [TableName("myPackageTable")]
    public class MyPackageTableDto
    {
        [Column("id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [Column("text")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string? Text { get; set; }

        [Column("eventDateUtc")]
        [Constraint(Default = SystemMethods.CurrentUTCDateTime)]
        public DateTime EventDate { get; set; }

        //[Column("status")]
        //[NullSetting(NullSetting = NullSettings.Null)]
        //[Index(IndexTypes.NonClustered, Name = "IX_myPackageTable_status")]
        //public string? Status { get; set; }
    }
}
