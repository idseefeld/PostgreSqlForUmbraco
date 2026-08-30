using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Umbraco.Cms.Infrastructure.Persistence.EFCore;
using Umbraco.Cms.Infrastructure.Persistence.EFCore.Extensions;
using Umbraco.Cms.Infrastructure.Persistence.EFCore.Migrations;
using Umbraco.Cms.Persistence.EFCore;
using Umbraco.Cms.Persistence.EFCore.Migrations;
using Umbraco.Extensions;

namespace Our.Umbraco.PostgreSql.EFCore.Services
{
    public class PostgreSqlMigrationProvider : IMigrationProvider
    {
        private readonly IDbContextFactory<UmbracoDbContext> _dbContextFactory;

        public PostgreSqlMigrationProvider(IDbContextFactory<UmbracoDbContext> dbContextFactory) => _dbContextFactory = dbContextFactory;

        public string ProviderName => Constants.ProviderName;
        public async Task MigrateAsync(EFCoreMigration migration)
        {
            UmbracoDbContext context = await _dbContextFactory.CreateDbContextAsync();

            var migrationType = GetMigrationType(migration);
            await context.MigrateDatabaseAsync(migrationType);
        }

        public async Task MigrateAllAsync()
        {
            UmbracoDbContext context = await _dbContextFactory.CreateDbContextAsync();
            await context.Database.MigrateAsync();
        }

        private static Type GetMigrationType(EFCoreMigration migration) =>
            migration switch
            {
                EFCoreMigration.InitialCreate => typeof(Migrations.InitialCreate),
                EFCoreMigration.AddOpenIddict => typeof(Migrations.AddOpenIddict),
                EFCoreMigration.UpdateOpenIddictToV5 => typeof(Migrations.UpdateOpenIddictToV5),
                EFCoreMigration.UpdateOpenIddictToV7 => typeof(Migrations.UpdateOpenIddictToV7),
                EFCoreMigration.AddWebhookDto => typeof(EmptyMigration),
                EFCoreMigration.AddLastSyncedDto => typeof(EmptyMigration),
                EFCoreMigration.AddKeyValueDto => typeof(EmptyMigration),
                EFCoreMigration.AddLanguageDto => typeof(EmptyMigration),
                EFCoreMigration.AddDomainDto => typeof(EmptyMigration),
                EFCoreMigration.AddAuditDtos => typeof(EmptyMigration),
                EFCoreMigration.AddLongRunningOperationDto => typeof(EmptyMigration),
                EFCoreMigration.AddRelationDtos => typeof(EmptyMigration),
                EFCoreMigration.AddContentVersionCleanupPolicyDto => typeof(EmptyMigration),
                EFCoreMigration.AddPublicAccessDto => typeof(EmptyMigration),
                EFCoreMigration.AddDistributedJobDto => typeof(EmptyMigration),
                EFCoreMigration.AddConsentDto => typeof(EmptyMigration),
                EFCoreMigration.AddDictionaryDto => typeof(EmptyMigration),
                EFCoreMigration.AddContentTypeDtos => typeof(EmptyMigration),
                EFCoreMigration.MemberPropertyTypeToEFCore => typeof(EmptyMigration),
                EFCoreMigration.AddRedirectUrlDto => typeof(Migrations.Add20260830Migrations),
                _ => throw new ArgumentOutOfRangeException(nameof(migration), $@"Not expected migration value: {migration}"),
            };
    }
}
