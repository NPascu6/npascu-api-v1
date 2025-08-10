import { Quote, Candle, TickerLite, OrderBookSnapshot } from '../types';

export interface MarketDataProvider {
  name: 'finnhub' | 'alphaVantage' | 'synthetic';
  getQuote(symbol: string): Promise<Quote>;
  getCandles(symbol: string, interval: '1m'|'5m'|'15m'|'1h'|'1d', from: Date, to: Date): Promise<Candle[]>;
  searchSymbols(query: string): Promise<TickerLite[]>;
  getOrderBookSnapshot(symbol: string, depth?: number): Promise<OrderBookSnapshot>;
}
