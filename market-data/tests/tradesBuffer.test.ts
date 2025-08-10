import { describe, it, expect } from 'vitest';

describe('trade buffer', () => {
  it('maintains a ring buffer of trades', async () => {
    process.env.TRADES_BUFFER_N = '3';
    const { addTrade, getRecentTrades } = await import('../src/tradesService');
    const now = new Date().toISOString();
    addTrade({ symbol: 'AAPL', p: 1, s: 1, t: now });
    addTrade({ symbol: 'AAPL', p: 2, s: 1, t: now });
    addTrade({ symbol: 'AAPL', p: 3, s: 1, t: now });
    addTrade({ symbol: 'AAPL', p: 4, s: 1, t: now });
    const trades = getRecentTrades('AAPL', 10);
    expect(trades.length).toBe(3);
    expect(trades[0].p).toBe(2);
    expect(trades[2].p).toBe(4);
  });
});
