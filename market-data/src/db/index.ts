import { Pool } from 'pg';
import EventEmitter from 'events';
import { ProviderRequestLog, Quote } from '../types';

export const quoteEmitter = new EventEmitter();

const connectionString = process.env.DATABASE_URL;
const pool = connectionString ? new Pool({ connectionString }) : null;
export default pool as Pool;

export async function logProviderRequest(log: ProviderRequestLog): Promise<void> {
  if (!pool) return;
  const { provider, endpoint, symbol, statusCode, success, costUnits, rateRemaining, error, meta } = log;
  await pool.query(
    `INSERT INTO provider_requests (provider, endpoint, symbol, status_code, success, cost_units, rate_remaining, error, meta)
     VALUES ($1,$2,$3,$4,$5,$6,$7,$8,$9)` ,
    [provider, endpoint, symbol ?? null, statusCode ?? null, success, costUnits ?? null, rateRemaining ?? null, error ?? null, JSON.stringify(meta ?? {})]
  );
}

export async function insertQuoteSnapshot(quote: Quote): Promise<void> {
  if (pool) {
    await pool.query(
      `INSERT INTO quote_snapshots (symbol, ts, last, bid, ask, volume, vwap, provider, is_delayed, meta)
       VALUES ($1,$2,$3,$4,$5,$6,$7,$8,$9,$10)
       ON CONFLICT (symbol, ts, provider) DO NOTHING`,
      [
        quote.symbol,
        quote.ts,
        quote.last,
        quote.bid,
        quote.ask,
        quote.volume ?? null,
        quote.vwap ?? null,
        quote.provider,
        quote.isDelayed,
        JSON.stringify(quote.meta ?? {})
      ]
    );

    await pool.query(
      `INSERT INTO audit_events (actor, action, resource_type, resource_id, meta)
       VALUES ($1,$2,$3,$4,$5)`,
      [
        'system',
        'ingest.quote',
        'quote',
        quote.symbol,
        JSON.stringify({ provider: quote.provider })
      ]
    );
  }

  quoteEmitter.emit('quote', quote);
}
