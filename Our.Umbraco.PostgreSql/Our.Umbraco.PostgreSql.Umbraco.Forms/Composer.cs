using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Our.Umbraco.PostgreSql.Services;
using Our.Umbraco.PostgreSql.Umbraco.Forms.FormsExtentions;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Forms.Core.Healthchecks;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Providers.DatasourceTypes;
using Umbraco.Forms.Core.Providers.Extensions;
using Umbraco.Forms.Core.Providers.PreValues;
using Umbraco.Forms.Core.Providers.WorkflowTypes;
using Umbraco.Forms.Web.Models.Backoffice;

namespace Our.Umbraco.PostgreSql.Umbraco.Forms;

/// <summary>
/// Adds fixes for Umbraco.Forms sql statements when using PostgreSQL as the database provider.
/// </summary>
public class Composer : IComposer
{
    /// <inheritdoc />
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.TryAddEnumerable(ServiceDescriptor
            .Singleton<IPostgreSqlFixService, PostgreSqlFixUmbracoFormsService>());

        builder.FormsDataSources().Exclude<MsSql>();
        // builder.WithCollectionBuilder<DataSourceTypeCollectionBuilder>().Add<PostgreSqlDataSourceType>();

        builder.HealthChecks().Exclude<DatabaseIntegrityHealthCheck>();

        builder.FormsFieldPreValueSources().Exclude<ReadOnlySql>();
        builder.FormsFieldPreValueSources().Exclude<DataSource>();

        builder.FormsWorkflows().Exclude<PostToUrl>();
        builder.FormsWorkflows().Exclude<PostAsXml>();
        builder.FormsWorkflows().Exclude<SaveAsFile>();
        builder.FormsWorkflows().Exclude<SaveAsUmbracoNode>();
        builder.FormsWorkflows().Exclude<ChangeRecordState>();

        builder.FormsWorkflows().Exclude<Slack>();
        builder.FormsWorkflows().Exclude<SlackV2>();
    }
}
