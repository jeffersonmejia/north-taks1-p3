using Microsoft.EntityFrameworkCore;

namespace NorthwindStore.Data;

public static class DbInitializer
{
    public const string SoftDeleteSql = """
        ALTER TABLE products ADD COLUMN IF NOT EXISTS is_deleted boolean NOT NULL DEFAULT false;
        ALTER TABLE products ADD COLUMN IF NOT EXISTS deleted_at timestamp NULL;
        ALTER TABLE products ADD COLUMN IF NOT EXISTS deleted_by varchar(256) NULL;
        ALTER TABLE orders ADD COLUMN IF NOT EXISTS is_deleted boolean NOT NULL DEFAULT false;
        ALTER TABLE orders ADD COLUMN IF NOT EXISTS deleted_at timestamp NULL;
        ALTER TABLE orders ADD COLUMN IF NOT EXISTS deleted_by varchar(256) NULL;
        ALTER TABLE customers ADD COLUMN IF NOT EXISTS is_deleted boolean NOT NULL DEFAULT false;
        ALTER TABLE customers ADD COLUMN IF NOT EXISTS deleted_at timestamp NULL;
        ALTER TABLE customers ADD COLUMN IF NOT EXISTS deleted_by varchar(256) NULL;
        """;

    public static async Task EnsureNorthwindExtensionsAsync(NorthwindContext context)
    {
        await context.Database.ExecuteSqlRawAsync(SoftDeleteSql);
    }
}
