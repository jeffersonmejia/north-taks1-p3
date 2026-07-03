--
-- Credentials for the NorthwindStore application user.
-- Creates role 'jef' with a hashed password (16 chars) and grants
-- the necessary permissions on both databases.
--
-- Usage:
--   psql -d northwind          -f db/credentials.sql
--   psql -d northwind_identity -f db/credentials.sql
--

DO $$
BEGIN
    IF NOT EXISTS (SELECT FROM pg_catalog.pg_roles WHERE rolname = 'jef') THEN
        CREATE ROLE jef LOGIN PASSWORD 'a1b2c3d4e5f6g7h8';
    END IF;
END
$$;

GRANT CONNECT ON DATABASE northwind TO jef;
GRANT CONNECT ON DATABASE northwind_identity TO jef;

GRANT USAGE ON SCHEMA public TO jef;

GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO jef;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO jef;
