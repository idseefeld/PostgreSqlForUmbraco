using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Umbraco.Cms.Persistence.EFCore;

#nullable disable

namespace Our.Umbraco.PostgreSql.EFCore.Migrations
{
    [DbContext(typeof(UmbracoDbContext))]
    [Migration("20250501121000_UpdateOpenIddictToV5")]
    partial class UpdateOpenIddictToV5
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
        }
    }
}
