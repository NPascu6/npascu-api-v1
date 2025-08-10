import { describe, it, expect } from 'vitest';
import { ProviderRouter } from '../src/providers/providerRouter';
import { MarketDataProvider } from '../src/providers/MarketDataProvider';
import { Quote, OrderBookSnapshot } from '../src/types';

class MockProvider implements MarketDataProvider {
  constructor(
    public name: any,
    private quoteHandler: (() => Promise<Quote>) | null = null,
    private orderBookHandler: (() => Promise<OrderBookSnapshot>) | null = null
  ) {}
  async getQuote(_symbol: string): Promise<Quote> {
    if (!this.quoteHandler) throw new Error('no quote handler');
    return this.quoteHandler();
  }
  async getCandles(): Promise<any[]> { return []; }
  async searchSymbols(): Promise<any[]> { return []; }
  async getOrderBookSnapshot(_symbol: string, _depth?: number): Promise<OrderBookSnapshot> {
    if (!this.orderBookHandler) throw new Error('no orderbook handler');
    return this.orderBookHandler();
  }
}

describe('ProviderRouter', () => {
  it('falls back to secondary provider when primary fails', async () => {
    const failing = new MockProvider('finnhub', async () => { throw new Error('fail'); });
    const quote: Quote = {
      symbol: 'AAPL',
      ts: new Date(),
      last: 1,
      bid: null,
      ask: null,
      volume: null,
      vwap: null,
      provider: 'alphaVantage',
      isDelayed: true
    };
    const succeeding = new MockProvider('alphaVantage', async () => quote);
    const router = new ProviderRouter([failing, succeeding]);
    const result = await router.getQuote('AAPL');
    expect(result.provider).toBe('alphaVantage');
    expect(result.last).toBe(1);
  });

  it('falls back for order book snapshots', async () => {
    const failing = new MockProvider('finnhub', null, async () => { throw new Error('fail'); });
    const snapshot: OrderBookSnapshot = {
      symbol: 'AAPL',
      ts: new Date(),
      bids: [{ price: 99, size: 1 }],
      asks: [{ price: 101, size: 1 }],
      provider: 'synthetic'
    };
    const succeeding = new MockProvider('synthetic', null, async () => snapshot);
    const router = new ProviderRouter([failing, succeeding]);
    const result = await router.getOrderBookSnapshot('AAPL', 5);
    expect(result.provider).toBe('synthetic');
    expect(result.bids.length).toBe(1);
  });
});
