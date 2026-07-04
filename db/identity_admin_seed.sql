--
-- Seeds the default Admin identity user without recreating databases.
-- Password hash is for the academic default password documented in README.md.
--

\c northwind_identity

INSERT INTO "AspNetRoles" ("Id", "Name", "NormalizedName", "ConcurrencyStamp")
VALUES ('0f7f9bd2-85a6-4f90-9ee5-7a59d7f49f03', 'Admin', 'ADMIN', 'd8c8a94e-d2d9-4df5-8b07-1518f599e3fb')
ON CONFLICT ("NormalizedName") DO UPDATE
SET "Name" = EXCLUDED."Name";

INSERT INTO "AspNetUsers" (
    "Id",
    "UserName",
    "NormalizedUserName",
    "Email",
    "NormalizedEmail",
    "EmailConfirmed",
    "PasswordHash",
    "SecurityStamp",
    "ConcurrencyStamp",
    "PhoneNumber",
    "PhoneNumberConfirmed",
    "TwoFactorEnabled",
    "LockoutEnd",
    "LockoutEnabled",
    "AccessFailedCount",
    "FullName",
    "CustomerId",
    "ActiveSessionId"
)
VALUES (
    '9c9a05e4-c7ce-4e56-8c73-5b0068f03e8f',
    'admin@northwind.local',
    'ADMIN@NORTHWIND.LOCAL',
    'admin@northwind.local',
    'ADMIN@NORTHWIND.LOCAL',
    true,
    'AQAAAAIAAYagAAAAEGCfGeL1WxcLSnc9vD5Ak8u4Cdv1yCVSN37Bho6ru+WaVAsnqzJwqc1iSH+R8OoYBw==',
    '3F0F9071-EDAE-44EC-8C25-C2AAEC644AA7',
    '61f8af6d-b028-477f-a188-a06ee1a0bf41',
    NULL,
    false,
    false,
    NULL,
    true,
    0,
    'Northwind Admin',
    NULL,
    NULL
)
ON CONFLICT ("NormalizedUserName") DO UPDATE
SET
    "Email" = EXCLUDED."Email",
    "NormalizedEmail" = EXCLUDED."NormalizedEmail",
    "EmailConfirmed" = EXCLUDED."EmailConfirmed",
    "PasswordHash" = EXCLUDED."PasswordHash",
    "SecurityStamp" = EXCLUDED."SecurityStamp",
    "FullName" = EXCLUDED."FullName",
    "CustomerId" = NULL,
    "ActiveSessionId" = NULL;

INSERT INTO "AspNetUserRoles" ("UserId", "RoleId")
SELECT u."Id", r."Id"
FROM "AspNetUsers" u
JOIN "AspNetRoles" r ON r."NormalizedName" = 'ADMIN'
WHERE u."NormalizedUserName" = 'ADMIN@NORTHWIND.LOCAL'
  AND NOT EXISTS (
      SELECT 1
      FROM "AspNetUserRoles" ur
      WHERE ur."UserId" = u."Id"
        AND ur."RoleId" = r."Id"
  );
