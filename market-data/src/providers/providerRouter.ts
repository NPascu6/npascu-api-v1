import { MarketDataProvider } from './MarketDataProvider';
import { Quote, Candle, TickerLite, OrderBookSnapshot } from '../types';
import { logProviderRequest } from '../db';

export class ProviderRouter implements MarketDataProvider {
  name: 'finnhub' = 'finnhub'; // not used
  constructor(private providers: MarketDataProvider[]) {}

  async getQuote(symbol: string): Promise<Quote> {
    return this.tryProviders(p => p.getQuote(symbol), symbol, 'getQuote');
  }

  async getCandles(symbol: string, interval: '1m'|'5m'|'15m'|'1h'|'1d', from: Date, to: Date): Promise<Candle[]> {
    return this.tryProviders(p => p.getCandles(symbol, interval, from, to), symbol, 'getCandles');
  }

  async searchSymbols(query: string): Promise<TickerLite[]> {
    return this.tryProviders(p => p.searchSymbols(query), undefined, 'searchSymbols');
  }

  async getOrderBookSnapshot(symbol: string, depth: number = 25): Promise<OrderBookSnapshot> {
    return this.tryProviders(p => p.getOrderBookSnapshot(symbol, depth), symbol, 'getOrderBookSnapshot');
  }

  private async tryProviders<T>(fn: (p: MarketDataProvider) => Promise<T>, symbol: string | undefined, endpoint: string): Promise<T> {
    let delay = 250;
    for (const p of this.providers) {
      const start = Date.now();
      try {
        const result = await fn(p);
        await logProviderRequest({
          provider: p.name,
          endpoint,
          symbol,
          statusCode: 200,
          success: true,
          meta: { latency_ms: Date.now() - start }
        });
        return result;
      } catch (err: any) {
        await logProviderRequest({
          provider: p.name,
          endpoint,
          symbol,
          statusCode: err?.status,
          success: false,
          error: err?.message
        });
        await new Promise(res => setTimeout(res, delay));
        delay *= 2;
      }
    }
    throw new Error('All providers failed');
  }
}
