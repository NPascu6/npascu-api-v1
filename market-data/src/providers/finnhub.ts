import { MarketDataProvider } from './MarketDataProvider';
import { Quote, Candle, TickerLite, OrderBookSnapshot } from '../types';

const API_BASE = 'https://finnhub.io/api/v1';

function mapInterval(interval: '1m'|'5m'|'15m'|'1h'|'1d'): string {
  switch(interval){
    case '1m': return '1';
    case '5m': return '5';
    case '15m': return '15';
    case '1h': return '60';
    case '1d': return 'D';
  }
}

export class FinnhubProvider implements MarketDataProvider {
  name: 'finnhub' = 'finnhub';
  constructor(private apiKey: string = process.env.FINNHUB_API_KEY || '') {}

  async getQuote(symbol: string): Promise<Quote> {
    const url = `${API_BASE}/quote?symbol=${encodeURIComponent(symbol)}&token=${this.apiKey}`;
    const res = await fetch(url);
    if (!res.ok) throw new Error(`Finnhub quote error: ${res.status}`);
    const data = await res.json() as any;
    return {
      symbol,
      ts: data.t ? new Date(data.t * 1000) : new Date(),
      last: data.c ?? null,
      bid: data.b ?? null,
      ask: data.a ?? null,
      volume: data.v ?? null,
      vwap: data.avg ?? null,
      provider: this.name,
      isDelayed: true,
      meta: { raw: data }
    };
  }

  async getCandles(symbol: string, interval: '1m'|'5m'|'15m'|'1h'|'1d', from: Date, to: Date): Promise<Candle[]> {
    const resolution = mapInterval(interval);
    const url = `${API_BASE}/stock/candle?symbol=${encodeURIComponent(symbol)}&resolution=${resolution}&from=${Math.floor(from.getTime()/1000)}&to=${Math.floor(to.getTime()/1000)}&token=${this.apiKey}`;
    const res = await fetch(url);
    if (!res.ok) throw new Error(`Finnhub candle error: ${res.status}`);
    const data = await res.json() as any;
    if (data.s !== 'ok') return [];
    const candles: Candle[] = [];
    for (let i = 0; i < data.t.length; i++) {
      candles.push({
        symbol,
        interval,
        ts: new Date(data.t[i] * 1000),
        open: data.o[i],
        high: data.h[i],
        low: data.l[i],
        close: data.c[i],
        volume: data.v[i],
        provider: this.name,
        meta: {}
      });
    }
    return candles;
  }

  async searchSymbols(query: string): Promise<TickerLite[]> {
    const url = `${API_BASE}/search?q=${encodeURIComponent(query)}&token=${this.apiKey}`;
    const res = await fetch(url);
    if (!res.ok) throw new Error(`Finnhub search error: ${res.status}`);
    const data = await res.json() as any;
    return (data.result || []).map((r: any) => ({
      symbol: r.symbol,
      name: r.description,
      exchange: r.exchange
    }));
  }

  async getOrderBookSnapshot(symbol: string, depth: number = 25): Promise<OrderBookSnapshot> {
    const url = `${API_BASE}/stock/orderbook?symbol=${encodeURIComponent(symbol)}&token=${this.apiKey}`;
    const res = await fetch(url);
    if (!res.ok) throw new Error(`Finnhub order book error: ${res.status}`);
    const data = await res.json() as any;
    return {
      symbol,
      ts: data.ts ? new Date(data.ts * 1000) : new Date(),
      bids: (data.bids || []).slice(0, depth).map((b: any) => ({ price: b[0], size: b[1] })),
      asks: (data.asks || []).slice(0, depth).map((a: any) => ({ price: a[0], size: a[1] })),
      provider: this.name,
      meta: { raw: data }
    };
  }
}
