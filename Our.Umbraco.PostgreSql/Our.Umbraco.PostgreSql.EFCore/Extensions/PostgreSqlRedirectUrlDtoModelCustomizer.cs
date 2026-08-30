// Copyright (c) Umbraco.
// See LICENSE for more details.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Umbraco.Cms.Infrastructure.Persistence.Dtos.EFCore;
using Umbraco.Cms.Infrastructure.Persistence.EFCore;

namespace Our.Umbraco.PostgreSql.EFCore.Extensions;

/// <summary>
/// Adds PostgreSQL-specific included columns to <see cref="RedirectUrlDto"/> indexes.
/// </summary>
public class PostgreSqlRedirectUrlDtoModelCustomizer : IEFCoreModelCustomizer<RedirectUrlDto>
{
    public string? ProviderName => Constants.ProviderName;

    public void Customize(EntityTypeBuilder<RedirectUrlDto> builder) =>
        builder.HasIndex(x => x.CreateDateUtc)
            .HasDatabaseName($"IX_{RedirectUrlDto.TableName}_culture_hash")
            .IncludeProperties(x => new { x.Culture, x.Url, x.UrlHash, x.ContentKey });
}
