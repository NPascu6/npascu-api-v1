import EventEmitter from 'events';
import { z } from 'zod';

export const normalizedTradeSchema = z.object({
  symbol: z.string(),
  p: z.number(),
  s: z.number(),
  t: z.string(),
  side: z.enum(['buy', 'sell']).optional(),
});
export type NormalizedTrade = z.infer<typeof normalizedTradeSchema>;

export const tradeEmitter = new EventEmitter();

const BUFFER_N = Number(process.env.TRADES_BUFFER_N || '500');
const buffers: Map<string, NormalizedTrade[]> = new Map();

export function addTrade(trade: NormalizedTrade): void {
  const buf = buffers.get(trade.symbol) ?? [];
  buf.push(trade);
  if (buf.length > BUFFER_N) {
    buf.splice(0, buf.length - BUFFER_N);
  }
  buffers.set(trade.symbol, buf);
  tradeEmitter.emit('trade', trade);
}

export function getRecentTrades(symbol: string, limit: number): NormalizedTrade[] {
  const buf = buffers.get(symbol) ?? [];
  return buf.slice(-limit);
}
