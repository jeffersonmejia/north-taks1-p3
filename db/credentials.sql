--
-- Credentials for the NorthwindStore application user.
-- Creates role 'jef' with a hashed password (16 chars) and grants
-- the necessary permissions on both databases.
--
-- Usage (run from 'postgres' database):
--   psql -d postgres -f db/credentials.sql
--

-- Drop databases and user to start fresh
DROP DATABASE IF EXISTS northwind;
DROP DATABASE IF EXISTS northwind_identity;
DROP ROLE IF EXISTS jef;

-- Recreate databases
CREATE DATABASE northwind;
CREATE DATABASE northwind_identity;

-- Create role and grant permissions
DO $$
BEGIN
    IF NOT EXISTS (SELECT FROM pg_catalog.pg_roles WHERE rolname = 'jef') THEN
        CREATE ROLE jef LOGIN PASSWORD 'a1b2c3d4e5f6g7h8';
    END IF;
END
$$;

GRANT CONNECT ON DATABASE northwind TO jef;
GRANT CONNECT ON DATABASE northwind_identity TO jef;

\c northwind
GRANT ALL ON SCHEMA public TO jef;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO jef;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO jef;

\c northwind_identity
GRANT ALL ON SCHEMA public TO jef;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO jef;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO jef;
