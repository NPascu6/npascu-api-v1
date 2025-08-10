import { MarketDataProvider } from './MarketDataProvider';
import { Quote, Candle, TickerLite } from '../types';

export class SyntheticProvider implements MarketDataProvider {
  name: 'synthetic' = 'synthetic';

  async getQuote(symbol: string): Promise<Quote> {
    return {
      symbol,
      ts: new Date(),
      last: null,
      bid: null,
      ask: null,
      volume: null,
      vwap: null,
      provider: this.name,
      isDelayed: true,
      meta: { source: 'synthetic' }
    };
  }

  async getCandles(_symbol: string, _interval: '1m'|'5m'|'15m'|'1h'|'1d', _from: Date, _to: Date): Promise<Candle[]> {
    return [];
  }

  async searchSymbols(_query: string): Promise<TickerLite[]> {
    return [];
  }
}
