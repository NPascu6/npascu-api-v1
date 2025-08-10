import { describe, it, expect } from 'vitest';
import { app } from '../src/server';
import { addTrade, NormalizedTrade } from '../src/tradesService';

type TradesEvent =
  | { type: 'hello'; symbols: string[]; ts: string }
  | { type: 'trade'; symbol: string; trades: NormalizedTrade[]; ts: string }
  | { type: 'heartbeat'; ts: string };

function parseSse(data: string): TradesEvent[] {
  const events: TradesEvent[] = [];
  const messages = data.split('\n\n').filter(Boolean);
  for (const msg of messages) {
    const line = msg.split('\n').find(l => l.startsWith('data: '));
    if (line) {
      events.push(JSON.parse(line.slice(6)) as TradesEvent);
    }
  }
  return events;
}

describe('trades SSE', () => {
  it('emits hello and trade events', async () => {
    const server = app.listen(0);
    const port = (server.address() as any).port;
    const res = await fetch(`http://localhost:${port}/trades/stream?symbols=AAPL`);
    const reader = res.body!.getReader();
    const decoder = new TextDecoder();

    const events: TradesEvent[] = [];
    const readPromise = (async () => {
      let buf = '';
      while (events.length < 2) {
        const { value } = await reader.read();
        if (!value) break;
        buf += decoder.decode(value, { stream: true });
        const parsed = parseSse(buf);
        if (parsed.length) {
          events.push(...parsed.slice(events.length));
        }
      }
    })();

    addTrade({ symbol: 'AAPL', p: 1, s: 1, t: new Date().toISOString() });
    await Promise.race([readPromise, new Promise(res => setTimeout(res, 500))]);
    server.close();

    expect(events[0].type).toBe('hello');
    expect(events.find(e => e.type === 'trade')).toBeTruthy();
  });
});
