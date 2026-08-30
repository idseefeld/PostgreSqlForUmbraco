using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Our.Umbraco.PostgreSql.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class Add20260830Migrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cmsContentTypeAllowedContentType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    AllowedId = table.Column<int>(type: "integer", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cmsContentTypeAllowedContentType", x => new { x.Id, x.AllowedId });
                });

            migrationBuilder.CreateTable(
                name: "cmsDictionary",
                columns: table => new
                {
                    pk = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    parent = table.Column<Guid>(type: "uuid", nullable: true),
                    key = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cmsDictionary", x => x.pk);
                    table.UniqueConstraint("AK_cmsDictionary_id", x => x.id);
                    table.ForeignKey(
                        name: "FK_cmsDictionary_cmsDictionary_parent",
                        column: x => x.parent,
                        principalTable: "cmsDictionary",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "cmsDocumentType",
                columns: table => new
                {
                    contentTypeNodeId = table.Column<int>(type: "integer", nullable: false),
                    templateNodeId = table.Column<int>(type: "integer", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cmsDocumentType", x => new { x.contentTypeNodeId, x.templateNodeId });
                });

            migrationBuilder.CreateTable(
                name: "cmsPropertyTypeGroup",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    uniqueID = table.Column<Guid>(type: "uuid", nullable: false),
                    contenttypeNodeId = table.Column<int>(type: "integer", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    text = table.Column<string>(type: "text", nullable: true),
                    alias = table.Column<string>(type: "text", nullable: false),
                    sortorder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cmsPropertyTypeGroup", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "umbracoAudit",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    performingUserId = table.Column<int>(type: "integer", nullable: false),
                    performingUserKey = table.Column<Guid>(type: "uuid", nullable: true),
                    performingDetails = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    performingIp = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    eventDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    affectedUserId = table.Column<int>(type: "integer", nullable: false),
                    affectedUserKey = table.Column<Guid>(type: "uuid", nullable: true),
                    affectedDetails = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    eventType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    eventDetails = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_umbracoAudit", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "umbracoCacheInstruction",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    utcStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    jsonInstruction = table.Column<string>(type: "text", nullable: false),
                    originated = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    instructionCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_umbracoCacheInstruction", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "umbracoConsent",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    current = table.Column<bool>(type: "boolean", nullable: false),
                    source = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    context = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    action = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    createDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    state = table.Column<int>(type: "integer", nullable: false),
                    comment = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_umbracoConsent", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "umbracoContentVersionCleanupPolicy",
                columns: table => new
                {
                    contentTypeId = table.Column<int>(type: "integer", nullable: false),
                    preventCleanup = table.Column<bool>(type: "boolean", nullable: false),
                    keepAllVersionsNewerThanDays = table.Column<int>(type: "integer", nullable: true),
                    keepLatestVersionPerDayForDays = table.Column<int>(type: "integer", nullable: true),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_umbracoContentVersionCleanupPolicy", x => x.contentTypeId);
                });

            migrationBuilder.CreateTable(
                name: "umbracoDistributedJob",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    lastRun = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    period = table.Column<long>(type: "bigint", nullable: false),
                    IsRunning = table.Column<bool>(type: "boolean", nullable: false),
                    lastAttemptedRun = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_umbracoDistributedJob", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "umbracoDomain",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    key = table.Column<Guid>(type: "uuid", nullable: false),
                    domainDefaultLanguage = table.Column<int>(type: "integer", nullable: true),
                    domainRootStructureID = table.Column<int>(type: "integer", nullable: true),
                    domainName = table.Column<string>(type: "text", nullable: false),
                    sortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_umbracoDomain", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "umbracoKeyValue",
                columns: table => new
                {
                    key = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    value = table.Column<string>(type: "text", nullable: true),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_umbracoKeyValue", x => x.key);
                });

            migrationBuilder.CreateTable(
                name: "umbracoLanguage",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    languageKey = table.Column<Guid>(type: "uuid", nullable: false),
                    languageISOCode = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    languageCultureName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    isDefaultVariantLang = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    mandatory = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    fallbackLanguageId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_umbracoLanguage", x => x.id);
                    table.ForeignKey(
                        name: "FK_umbracoLanguage_umbracoLanguage_fallbackLanguageId",
                        column: x => x.fallbackLanguageId,
                        principalTable: "umbracoLanguage",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "umbracoLastSynced",
                columns: table => new
                {
                    machineId = table.Column<string>(type: "text", nullable: false),
                    lastSyncedInternalId = table.Column<int>(type: "integer", nullable: true),
                    LastSyncedExternalId = table.Column<int>(type: "integer", nullable: true),
                    lastSyncedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_umbracoLastSynced", x => x.machineId);
                });

            migrationBuilder.CreateTable(
                name: "umbracoLongRunningOperation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    result = table.Column<string>(type: "text", nullable: true),
                    createDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_umbracoLongRunningOperation", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "umbracoNode",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    uniqueId = table.Column<Guid>(type: "uuid", nullable: false),
                    parentId = table.Column<int>(type: "integer", nullable: false),
                    level = table.Column<int>(type: "integer", nullable: false),
                    path = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    sortOrder = table.Column<int>(type: "integer", nullable: false),
                    trashed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    nodeUser = table.Column<int>(type: "integer", nullable: true),
                    text = table.Column<string>(type: "text", nullable: true),
                    nodeObjectType = table.Column<Guid>(type: "uuid", nullable: true),
                    createDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_umbracoNode", x => x.id);
                    table.UniqueConstraint("AK_umbracoNode_uniqueId", x => x.uniqueId);
                    table.ForeignKey(
                        name: "FK_umbracoNode_umbracoNode_parentId",
                        column: x => x.parentId,
                        principalTable: "umbracoNode",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "umbracoRelationType",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    typeUniqueId = table.Column<Guid>(type: "uuid", nullable: false),
                    dual = table.Column<bool>(type: "boolean", nullable: false),
                    parentObjectType = table.Column<Guid>(type: "uuid", nullable: true),
                    childObjectType = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "text", nullable: false),
                    alias = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    isDependency = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_umbracoRelationType", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "umbracoUser",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userDisabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    key = table.Column<Guid>(type: "uuid", nullable: false),
                    userNoConsole = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    userName = table.Column<string>(type: "text", nullable: false),
                    userLogin = table.Column<string>(type: "character varying(125)", maxLength: 125, nullable: true),
                    userPassword = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    passwordConfig = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    userEmail = table.Column<string>(type: "text", nullable: false),
                    userLanguage = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    securityStampToken = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    failedLoginAttempts = table.Column<int>(type: "integer", nullable: true),
                    lastLockoutDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    lastPasswordChangeDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    lastLoginDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    emailConfirmedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    invitedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    createDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    kind = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    avatar = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "umbracoWebhook",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    key = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    url = table.Column<string>(type: "text", nullable: false),
                    enabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_umbracoWebhook", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cmsLanguageText",
                columns: table => new
                {
                    pk = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    languageId = table.Column<int>(type: "integer", nullable: false),
                    UniqueId = table.Column<Guid>(type: "uuid", nullable: false),
                    value = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cmsLanguageText", x => x.pk);
                    table.ForeignKey(
                        name: "FK_cmsLanguageText_cmsDictionary_UniqueId",
                        column: x => x.UniqueId,
                        principalTable: "cmsDictionary",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_cmsLanguageText_umbracoLanguage_languageId",
                        column: x => x.languageId,
                        principalTable: "umbracoLanguage",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "cmsContentType",
                columns: table => new
                {
                    pk = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nodeId = table.Column<int>(type: "integer", nullable: false),
                    alias = table.Column<string>(type: "text", nullable: true),
                    icon = table.Column<string>(type: "text", nullable: true),
                    thumbnail = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "character varying(1500)", maxLength: 1500, nullable: true),
                    listView = table.Column<Guid>(type: "uuid", nullable: true),
                    isElement = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    allowedInLibrary = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    allowAtRoot = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    variations = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cmsContentType", x => x.pk);
                    table.ForeignKey(
                        name: "FK_cmsContentType_umbracoNode",
                        column: x => x.nodeId,
                        principalTable: "umbracoNode",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "cmsContentType2ContentType",
                columns: table => new
                {
                    parentContentTypeId = table.Column<int>(type: "integer", nullable: false),
                    childContentTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cmsContentType2ContentType", x => new { x.parentContentTypeId, x.childContentTypeId });
                    table.ForeignKey(
                        name: "FK_cmsContentType2ContentType_umbracoNode_child",
                        column: x => x.childContentTypeId,
                        principalTable: "umbracoNode",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_cmsContentType2ContentType_umbracoNode_parent",
                        column: x => x.parentContentTypeId,
                        principalTable: "umbracoNode",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "umbracoAccess",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nodeId = table.Column<int>(type: "integer", nullable: false),
                    loginNodeId = table.Column<int>(type: "integer", nullable: false),
                    noAccessNodeId = table.Column<int>(type: "integer", nullable: false),
                    createDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_umbracoAccess", x => x.id);
                    table.ForeignKey(
                        name: "FK_umbracoAccess_umbracoNode_id",
                        column: x => x.nodeId,
                        principalTable: "umbracoNode",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_umbracoAccess_umbracoNode_id1",
                        column: x => x.loginNodeId,
                        principalTable: "umbracoNode",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_umbracoAccess_umbracoNode_id2",
                        column: x => x.noAccessNodeId,
                        principalTable: "umbracoNode",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "umbracoDataType",
                columns: table => new
                {
                    nodeId = table.Column<int>(type: "integer", nullable: false),
                    propertyEditorAlias = table.Column<string>(type: "text", nullable: false),
                    propertyEditorUiAlias = table.Column<string>(type: "text", nullable: true),
                    dbType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    config = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_umbracoDataType", x => x.nodeId);
                    table.ForeignKey(
                        name: "FK_umbracoDataType_umbracoNode",
                        column: x => x.nodeId,
                        principalTable: "umbracoNode",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "umbracoRedirectUrl",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    contentKey = table.Column<Guid>(type: "uuid", nullable: false),
                    createDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    url = table.Column<string>(type: "text", nullable: false),
                    culture = table.Column<string>(type: "text", nullable: true),
                    urlHash = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_umbracoRedirectUrl", x => x.id);
                    table.ForeignKey(
                        name: "FK_umbracoRedirectUrl_umbracoNode_contentKey",
                        column: x => x.contentKey,
                        principalTable: "umbracoNode",
                        principalColumn: "uniqueId");
                });

            migrationBuilder.CreateTable(
                name: "umbracoRelation",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    parentId = table.Column<int>(type: "integer", nullable: false),
                    childId = table.Column<int>(type: "integer", nullable: false),
                    relType = table.Column<int>(type: "integer", nullable: false),
                    datetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_umbracoRelation", x => x.id);
                    table.ForeignKey(
                        name: "FK_umbracoRelation_umbracoNode",
                        column: x => x.parentId,
                        principalTable: "umbracoNode",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_umbracoRelation_umbracoNode1",
                        column: x => x.childId,
                        principalTable: "umbracoNode",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_umbracoRelation_umbracoRelationType_relType",
                        column: x => x.relType,
                        principalTable: "umbracoRelationType",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "umbracoLog",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userId = table.Column<int>(type: "integer", nullable: true),
                    NodeId = table.Column<int>(type: "integer", nullable: false),
                    entityType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Datestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    logHeader = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    logComment = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    parameters = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_umbracoLog", x => x.id);
                    table.ForeignKey(
                        name: "FK_umbracoLog_umbracoUser_userId",
                        column: x => x.userId,
                        principalTable: "umbracoUser",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "umbracoWebhook2ContentTypeKeys",
                columns: table => new
                {
                    webhookId = table.Column<int>(type: "integer", nullable: false),
                    entityKey = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_webhookEntityKey2Webhook", x => new { x.webhookId, x.entityKey });
                    table.ForeignKey(
                        name: "FK_umbracoWebhook2ContentTypeKeys_umbracoWebhook_webhookId",
                        column: x => x.webhookId,
                        principalTable: "umbracoWebhook",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "umbracoWebhook2Events",
                columns: table => new
                {
                    webhookId = table.Column<int>(type: "integer", nullable: false),
                    @event = table.Column<string>(name: "event", type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_webhookEvent2WebhookDto", x => new { x.webhookId, x.@event });
                    table.ForeignKey(
                        name: "FK_umbracoWebhook2Events_umbracoWebhook_webhookId",
                        column: x => x.webhookId,
                        principalTable: "umbracoWebhook",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "umbracoWebhook2Headers",
                columns: table => new
                {
                    webhookId = table.Column<int>(type: "integer", nullable: false),
                    Key = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_headers2WebhookDto", x => new { x.webhookId, x.Key });
                    table.ForeignKey(
                        name: "FK_umbracoWebhook2Headers_umbracoWebhook_webhookId",
                        column: x => x.webhookId,
                        principalTable: "umbracoWebhook",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "umbracoAccessRule",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    accessId = table.Column<Guid>(type: "uuid", nullable: false),
                    ruleValue = table.Column<string>(type: "text", nullable: true),
                    ruleType = table.Column<string>(type: "text", nullable: true),
                    createDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_umbracoAccessRule", x => x.id);
                    table.ForeignKey(
                        name: "FK_umbracoAccessRule_umbracoAccess_accessId",
                        column: x => x.accessId,
                        principalTable: "umbracoAccess",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cmsPropertyType",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    dataTypeId = table.Column<int>(type: "integer", nullable: false),
                    contentTypeId = table.Column<int>(type: "integer", nullable: false),
                    propertyTypeGroupId = table.Column<int>(type: "integer", nullable: true),
                    Alias = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    sortOrder = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    mandatory = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    mandatoryMessage = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    validationRegExp = table.Column<string>(type: "text", nullable: true),
                    validationRegExpMessage = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    labelOnTop = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    variations = table.Column<int>(type: "integer", nullable: false),
                    UniqueId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cmsPropertyType", x => x.id);
                    table.ForeignKey(
                        name: "FK_cmsPropertyType_cmsPropertyTypeGroup",
                        column: x => x.propertyTypeGroupId,
                        principalTable: "cmsPropertyTypeGroup",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_cmsPropertyType_umbracoDataType",
                        column: x => x.dataTypeId,
                        principalTable: "umbracoDataType",
                        principalColumn: "nodeId");
                });

            migrationBuilder.CreateTable(
                name: "cmsMemberType",
                columns: table => new
                {
                    pk = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NodeId = table.Column<int>(type: "integer", nullable: false),
                    propertytypeId = table.Column<int>(type: "integer", nullable: false),
                    memberCanEdit = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    viewOnProfile = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    isSensitive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cmsMemberType", x => x.pk);
                    table.ForeignKey(
                        name: "FK_cmsMemberType_cmsPropertyType",
                        column: x => x.propertytypeId,
                        principalTable: "cmsPropertyType",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_cmsContentType",
                table: "cmsContentType",
                column: "nodeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cmsContentType_icon",
                table: "cmsContentType",
                column: "icon");

            migrationBuilder.CreateIndex(
                name: "IX_cmsContentType2ContentType_childContentTypeId",
                table: "cmsContentType2ContentType",
                column: "childContentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_cmsDictionary_id",
                table: "cmsDictionary",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cmsDictionary_key",
                table: "cmsDictionary",
                column: "key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cmsDictionary_Parent",
                table: "cmsDictionary",
                column: "parent");

            migrationBuilder.CreateIndex(
                name: "IX_cmsLanguageText_languageId",
                table: "cmsLanguageText",
                columns: new[] { "languageId", "UniqueId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cmsLanguageText_UniqueId",
                table: "cmsLanguageText",
                column: "UniqueId");

            migrationBuilder.CreateIndex(
                name: "IX_cmsMemberType_propertytypeId",
                table: "cmsMemberType",
                column: "propertytypeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cmsPropertyType_dataTypeId",
                table: "cmsPropertyType",
                column: "dataTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_cmsPropertyType_propertyTypeGroupId",
                table: "cmsPropertyType",
                column: "propertyTypeGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_cmsPropertyTypeAlias",
                table: "cmsPropertyType",
                column: "Alias");

            migrationBuilder.CreateIndex(
                name: "IX_cmsPropertyTypeUniqueID",
                table: "cmsPropertyType",
                column: "UniqueId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cmsPropertyTypeGroupUniqueID",
                table: "cmsPropertyTypeGroup",
                column: "uniqueID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_umbracoAccess_loginNodeId",
                table: "umbracoAccess",
                column: "loginNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_umbracoAccess_noAccessNodeId",
                table: "umbracoAccess",
                column: "noAccessNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_umbracoAccess_nodeId",
                table: "umbracoAccess",
                column: "nodeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_umbracoAccessRule",
                table: "umbracoAccessRule",
                columns: new[] { "ruleValue", "ruleType", "accessId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_umbracoAccessRule_accessId",
                table: "umbracoAccessRule",
                column: "accessId");

            migrationBuilder.CreateIndex(
                name: "IX_umbracoDomain_key",
                table: "umbracoDomain",
                column: "key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_umbracoLanguage_fallbackLanguageId",
                table: "umbracoLanguage",
                column: "fallbackLanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_umbracoLanguage_languageISOCode",
                table: "umbracoLanguage",
                column: "languageISOCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_umbracoLanguage_languageKey",
                table: "umbracoLanguage",
                column: "languageKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_umbracoLog",
                table: "umbracoLog",
                column: "NodeId");

            migrationBuilder.CreateIndex(
                name: "IX_umbracoLog_datestamp",
                table: "umbracoLog",
                columns: new[] { "Datestamp", "userId", "NodeId" });

            migrationBuilder.CreateIndex(
                name: "IX_umbracoLog_datestamp_logheader",
                table: "umbracoLog",
                columns: new[] { "Datestamp", "logHeader" });

            migrationBuilder.CreateIndex(
                name: "IX_umbracoLog_userId",
                table: "umbracoLog",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_umbracoNode_Level",
                table: "umbracoNode",
                columns: new[] { "level", "parentId", "sortOrder", "nodeObjectType", "trashed" });

            migrationBuilder.CreateIndex(
                name: "IX_umbracoNode_ObjectType",
                table: "umbracoNode",
                columns: new[] { "nodeObjectType", "trashed" });

            migrationBuilder.CreateIndex(
                name: "IX_umbracoNode_ObjectType_trashed_sorted",
                table: "umbracoNode",
                columns: new[] { "nodeObjectType", "trashed", "sortOrder", "id" });

            migrationBuilder.CreateIndex(
                name: "IX_umbracoNode_parentId_nodeObjectType",
                table: "umbracoNode",
                columns: new[] { "parentId", "nodeObjectType" });

            migrationBuilder.CreateIndex(
                name: "IX_umbracoNode_Path",
                table: "umbracoNode",
                column: "path");

            migrationBuilder.CreateIndex(
                name: "IX_umbracoNode_Trashed",
                table: "umbracoNode",
                column: "trashed");

            migrationBuilder.CreateIndex(
                name: "IX_umbracoNode_UniqueId",
                table: "umbracoNode",
                column: "uniqueId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_umbracoRedirectUrl",
                table: "umbracoRedirectUrl",
                columns: new[] { "urlHash", "contentKey", "culture", "createDateUtc" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_umbracoRedirectUrl_contentKey",
                table: "umbracoRedirectUrl",
                column: "contentKey");

            migrationBuilder.CreateIndex(
                name: "IX_umbracoRedirectUrl_culture_hash",
                table: "umbracoRedirectUrl",
                column: "createDateUtc");

            migrationBuilder.CreateIndex(
                name: "IX_umbracoRelation_childId",
                table: "umbracoRelation",
                column: "childId");

            migrationBuilder.CreateIndex(
                name: "IX_umbracoRelation_parentChildType",
                table: "umbracoRelation",
                columns: new[] { "parentId", "childId", "relType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_umbracoRelation_relType",
                table: "umbracoRelation",
                column: "relType");

            migrationBuilder.CreateIndex(
                name: "IX_umbracoRelationType_alias",
                table: "umbracoRelationType",
                column: "alias",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_umbracoRelationType_name",
                table: "umbracoRelationType",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_umbracoRelationType_UniqueId",
                table: "umbracoRelationType",
                column: "typeUniqueId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_umbracoUser_userKey",
                table: "umbracoUser",
                column: "key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_umbracoUser_userLogin",
                table: "umbracoUser",
                column: "userLogin");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cmsContentType");

            migrationBuilder.DropTable(
                name: "cmsContentType2ContentType");

            migrationBuilder.DropTable(
                name: "cmsContentTypeAllowedContentType");

            migrationBuilder.DropTable(
                name: "cmsDocumentType");

            migrationBuilder.DropTable(
                name: "cmsLanguageText");

            migrationBuilder.DropTable(
                name: "cmsMemberType");

            migrationBuilder.DropTable(
                name: "umbracoAccessRule");

            migrationBuilder.DropTable(
                name: "umbracoAudit");

            migrationBuilder.DropTable(
                name: "umbracoCacheInstruction");

            migrationBuilder.DropTable(
                name: "umbracoConsent");

            migrationBuilder.DropTable(
                name: "umbracoContentVersionCleanupPolicy");

            migrationBuilder.DropTable(
                name: "umbracoDistributedJob");

            migrationBuilder.DropTable(
                name: "umbracoDomain");

            migrationBuilder.DropTable(
                name: "umbracoKeyValue");

            migrationBuilder.DropTable(
                name: "umbracoLastSynced");

            migrationBuilder.DropTable(
                name: "umbracoLog");

            migrationBuilder.DropTable(
                name: "umbracoLongRunningOperation");

            migrationBuilder.DropTable(
                name: "umbracoRedirectUrl");

            migrationBuilder.DropTable(
                name: "umbracoRelation");

            migrationBuilder.DropTable(
                name: "umbracoWebhook2ContentTypeKeys");

            migrationBuilder.DropTable(
                name: "umbracoWebhook2Events");

            migrationBuilder.DropTable(
                name: "umbracoWebhook2Headers");

            migrationBuilder.DropTable(
                name: "cmsDictionary");

            migrationBuilder.DropTable(
                name: "umbracoLanguage");

            migrationBuilder.DropTable(
                name: "cmsPropertyType");

            migrationBuilder.DropTable(
                name: "umbracoAccess");

            migrationBuilder.DropTable(
                name: "umbracoUser");

            migrationBuilder.DropTable(
                name: "umbracoRelationType");

            migrationBuilder.DropTable(
                name: "umbracoWebhook");

            migrationBuilder.DropTable(
                name: "cmsPropertyTypeGroup");

            migrationBuilder.DropTable(
                name: "umbracoDataType");

            migrationBuilder.DropTable(
                name: "umbracoNode");
        }
    }
}
