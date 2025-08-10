import { MarketDataProvider } from './MarketDataProvider';
import { Quote, Candle, TickerLite } from '../types';

const API_BASE = 'https://www.alphavantage.co/query';

function mapInterval(interval: '1m'|'5m'|'15m'|'1h'|'1d') {
  return {
    '1m': '1min',
    '5m': '5min',
    '15m': '15min',
    '1h': '60min',
    '1d': 'Daily'
  }[interval];
}

export class AlphaVantageProvider implements MarketDataProvider {
  name: 'alphaVantage' = 'alphaVantage';
  constructor(private apiKey: string = process.env.ALPHAVANTAGE_API_KEY || '') {}

  async getQuote(symbol: string): Promise<Quote> {
    const url = `${API_BASE}?function=GLOBAL_QUOTE&symbol=${encodeURIComponent(symbol)}&apikey=${this.apiKey}`;
    const res = await fetch(url);
    if (!res.ok) throw new Error(`AlphaVantage quote error: ${res.status}`);
    const data = await res.json() as any;
    const q = data['Global Quote'] || {};
    return {
      symbol,
      ts: new Date(),
      last: q['05. price'] ? parseFloat(q['05. price']) : null,
      bid: null,
      ask: null,
      volume: q['06. volume'] ? parseInt(q['06. volume'], 10) : null,
      vwap: null,
      provider: this.name,
      isDelayed: true,
      meta: { raw: q }
    };
  }

  async getCandles(symbol: string, interval: '1m'|'5m'|'15m'|'1h'|'1d', from: Date, to: Date): Promise<Candle[]> {
    const avInterval = mapInterval(interval);
    const func = interval === '1d' ? 'TIME_SERIES_DAILY' : 'TIME_SERIES_INTRADAY';
    const url = `${API_BASE}?function=${func}&symbol=${encodeURIComponent(symbol)}${interval === '1d' ? '' : `&interval=${avInterval}`}&apikey=${this.apiKey}&outputsize=full`;
    const res = await fetch(url);
    if (!res.ok) throw new Error(`AlphaVantage candle error: ${res.status}`);
    const data = await res.json() as any;
    const key = interval === '1d' ? 'Time Series (Daily)' : `Time Series (${avInterval})`;
    const series = data[key] || {};
    const candles: Candle[] = [];
    for (const [time, v] of Object.entries<any>(series)) {
      const ts = new Date(time);
      if (ts < from || ts > to) continue;
      candles.push({
        symbol,
        interval,
        ts,
        open: parseFloat(v['1. open']),
        high: parseFloat(v['2. high']),
        low: parseFloat(v['3. low']),
        close: parseFloat(v['4. close']),
        volume: v['5. volume'] ? parseInt(v['5. volume'], 10) : null,
        provider: this.name,
        meta: {}
      });
    }
    return candles;
  }

  async searchSymbols(query: string): Promise<TickerLite[]> {
    const url = `${API_BASE}?function=SYMBOL_SEARCH&keywords=${encodeURIComponent(query)}&apikey=${this.apiKey}`;
    const res = await fetch(url);
    if (!res.ok) throw new Error(`AlphaVantage search error: ${res.status}`);
    const data = await res.json() as any;
    return (data.bestMatches || []).map((m: any) => ({
      symbol: m['1. symbol'],
      name: m['2. name'],
      exchange: m['4. region']
    }));
  }
}
