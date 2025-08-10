-- 1) Ticker dictionary
CREATE TABLE IF NOT EXISTS tickers (
  symbol         text PRIMARY KEY,
  name           text,
  exchange       text,
  currency       text,
  mic            text,
  country        text,
  updated_at     timestamptz NOT NULL DEFAULT now()
);

-- 2) Latest quote snapshots (point-in-time, dedupe on (symbol, ts, provider))
CREATE TABLE IF NOT EXISTS quote_snapshots (
  id             bigserial PRIMARY KEY,
  symbol         text NOT NULL REFERENCES tickers(symbol) ON DELETE CASCADE,
  ts             timestamptz NOT NULL,
  last           numeric(18,6),
  bid            numeric(18,6),
  ask            numeric(18,6),
  volume         bigint,
  vwap           numeric(18,6),
  provider       text NOT NULL,
  received_at    timestamptz NOT NULL DEFAULT now(),
  is_delayed     boolean NOT NULL DEFAULT true,
  meta           jsonb NOT NULL DEFAULT '{}'::jsonb,
  UNIQUE(symbol, ts, provider)
);
CREATE INDEX IF NOT EXISTS idx_quote_snapshots_symbol_ts
  ON quote_snapshots(symbol, ts DESC);

-- 3) OHLC candles (interval enum)
CREATE TYPE IF NOT EXISTS candle_interval AS ENUM ('1m','5m','15m','1h','1d');
CREATE TABLE IF NOT EXISTS candles (
  id             bigserial PRIMARY KEY,
  symbol         text NOT NULL REFERENCES tickers(symbol) ON DELETE CASCADE,
  interval       candle_interval NOT NULL,
  ts             timestamptz NOT NULL,
  open           numeric(18,6),
  high           numeric(18,6),
  low            numeric(18,6),
  close          numeric(18,6),
  volume         bigint,
  provider       text NOT NULL,
  received_at    timestamptz NOT NULL DEFAULT now(),
  meta           jsonb NOT NULL DEFAULT '{}'::jsonb,
  UNIQUE(symbol, interval, ts, provider)
);
CREATE INDEX IF NOT EXISTS idx_candles_symbol_interval_ts
  ON candles(symbol, interval, ts DESC);

-- 4) Minimal audit events
CREATE TABLE IF NOT EXISTS audit_events (
  id             bigserial PRIMARY KEY,
  ts             timestamptz NOT NULL DEFAULT now(),
  actor          text,
  action         text,
  resource_type  text,
  resource_id    text,
  meta           jsonb NOT NULL DEFAULT '{}'::jsonb
);
CREATE INDEX IF NOT EXISTS idx_audit_ts ON audit_events(ts DESC);

-- 5) Provider call log (helps with rate limiting & debugging)
CREATE TABLE IF NOT EXISTS provider_requests (
  id             bigserial PRIMARY KEY,
  ts             timestamptz NOT NULL DEFAULT now(),
  provider       text NOT NULL,
  endpoint       text NOT NULL,
  symbol         text,
  status_code    int,
  success        boolean NOT NULL DEFAULT false,
  cost_units     int,
  rate_remaining int,
  error          text,
  meta           jsonb NOT NULL DEFAULT '{}'::jsonb
);
CREATE INDEX IF NOT EXISTS idx_provider_requests_ts ON provider_requests(ts DESC);
