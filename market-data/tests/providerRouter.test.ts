import { describe, it, expect } from 'vitest';
import { ProviderRouter } from '../src/providers/providerRouter';
import { MarketDataProvider } from '../src/providers/MarketDataProvider';
import { Quote } from '../src/types';

class MockProvider implements MarketDataProvider {
  constructor(public name: any, private handler: () => Promise<Quote>) {}
  async getQuote(symbol: string): Promise<Quote> { return this.handler(); }
  async getCandles(): Promise<any[]> { return []; }
  async searchSymbols(): Promise<any[]> { return []; }
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
});
