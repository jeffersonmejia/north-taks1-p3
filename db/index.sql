--
-- Performance indexes for the NorthwindStore application.
-- Run this AFTER db/schema.sql and db/seed.sql.
--
-- Usage:
--   psql -d northwind -f db/index.sql
--

CREATE EXTENSION IF NOT EXISTS pg_trgm;

CREATE INDEX IF NOT EXISTS ix_products_available
    ON products (discontinued, units_in_stock)
    WHERE is_deleted = false;

CREATE INDEX IF NOT EXISTS ix_products_name_trgm
    ON products USING gin (product_name gin_trgm_ops)
    WHERE is_deleted = false;

CREATE INDEX IF NOT EXISTS ix_products_low_stock
    ON products (units_in_stock, discontinued)
    WHERE is_deleted = false AND units_in_stock > 0;

CREATE INDEX IF NOT EXISTS ix_products_discontinued
    ON products (discontinued)
    WHERE discontinued = true;

CREATE INDEX IF NOT EXISTS ix_order_details_product
    ON order_details (product_id);

CREATE INDEX IF NOT EXISTS ix_orders_customer_date
    ON orders (customer_id, order_date)
    WHERE is_deleted = false;

CREATE INDEX IF NOT EXISTS ix_products_out_of_stock
    ON products (units_in_stock, discontinued)
    WHERE is_deleted = false AND units_in_stock <= 0;

CREATE INDEX IF NOT EXISTS ix_orders_order_date
    ON orders (order_date)
    WHERE is_deleted = false;
