// Copyright (c) Umbraco.
// See LICENSE for more details.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Our.Umbraco.PostgreSql.EFCore;

public partial class EmptyMigration : Migration
{

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // No op. Existing tables are added by npoco. This will only create the history table.
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // No op. Existing tables are added by npoco. This will only create the history table.
    }
}

// public partial class AddWebhookDto: EmptyMigration { }
