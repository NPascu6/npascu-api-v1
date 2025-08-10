import { NormalizedTrade, addTrade } from '../tradesService';

export interface TradesProvider {
  start(): void;
}

export class SyntheticTradesProvider implements TradesProvider {
  private timer?: NodeJS.Timer;
  constructor(private symbols: string[] = ['AAPL']) {}

  start(): void {
    const interval = Number(process.env.TRADES_POLL_MS || '1500');
    this.timer = setInterval(() => {
      const ts = new Date().toISOString();
      for (const symbol of this.symbols) {
        const trade: NormalizedTrade = {
          symbol,
          p: 100 + Math.random(),
          s: Number((Math.random() * 10 + 1).toFixed(2)),
          t: ts,
          side: Math.random() > 0.5 ? 'buy' : 'sell',
        };
        addTrade(trade);
      }
    }, interval);
  }
}
