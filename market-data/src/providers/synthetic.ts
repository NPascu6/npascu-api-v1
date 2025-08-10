import { MarketDataProvider } from './MarketDataProvider';
import { Quote, Candle, TickerLite, OrderBookSnapshot } from '../types';

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

  async getOrderBookSnapshot(symbol: string, depth: number = 5): Promise<OrderBookSnapshot> {
    const mid = 100;
    const bids = [] as OrderBookSnapshot['bids'];
    const asks = [] as OrderBookSnapshot['asks'];
    for (let i = 0; i < depth; i++) {
      bids.push({ price: mid - i * 0.1, size: 1 });
      asks.push({ price: mid + i * 0.1, size: 1 });
    }
    return { symbol, ts: new Date(), bids, asks, provider: this.name, meta: { source: 'synthetic' } };
  }
}
