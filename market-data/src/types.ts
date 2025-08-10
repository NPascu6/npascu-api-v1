export interface Quote {
  symbol: string;
  ts: Date;
  last: number | null;
  bid: number | null;
  ask: number | null;
  volume?: number | null;
  vwap?: number | null;
  provider: string;
  isDelayed: boolean;
  meta?: Record<string, any>;
}

export interface Candle {
  symbol: string;
  interval: '1m'|'5m'|'15m'|'1h'|'1d';
  ts: Date;
  open: number;
  high: number;
  low: number;
  close: number;
  volume?: number | null;
  provider: string;
  meta?: Record<string, any>;
}

export interface TickerLite {
  symbol: string;
  name?: string;
  exchange?: string;
}

export interface ProviderRequestLog {
  provider: string;
  endpoint: string;
  symbol?: string;
  statusCode?: number;
  success: boolean;
  costUnits?: number;
  rateRemaining?: number;
  error?: string;
  meta?: Record<string, any>;
}

export interface OrderBookLevel {
  price: number;
  size: number;
}

export interface OrderBookSnapshot {
  symbol: string;
  ts: Date;
  bids: OrderBookLevel[];
  asks: OrderBookLevel[];
  provider: string;
  meta?: Record<string, any>;
}
