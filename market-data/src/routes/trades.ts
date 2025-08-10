import { Router } from 'express';
import { z } from 'zod';
import { tradeEmitter, getRecentTrades, NormalizedTrade } from '../tradesService';

const tradesRouter = Router();

tradesRouter.get('/stream', (req, res) => {
  const symbolsParam = (req.query.symbols as string) || '';
  const symbols = symbolsParam.split(',').map(s => s.trim().toUpperCase()).filter(Boolean);

  res.writeHead(200, {
    'Content-Type': 'text/event-stream',
    'Cache-Control': 'no-cache',
    Connection: 'keep-alive',
  });

  const hello = { type: 'hello', symbols, ts: new Date().toISOString() };
  res.write(`data: ${JSON.stringify(hello)}\n\n`);

  const listener = (trade: NormalizedTrade) => {
    if (symbols.length === 0 || symbols.includes(trade.symbol)) {
      const evt = { type: 'trade', symbol: trade.symbol, trades: [trade], ts: new Date().toISOString() };
      res.write(`data: ${JSON.stringify(evt)}\n\n`);
    }
  };
  tradeEmitter.on('trade', listener);

  const hb = setInterval(() => {
    const hbEvt = { type: 'heartbeat', ts: new Date().toISOString() };
    res.write(`data: ${JSON.stringify(hbEvt)}\n\n`);
  }, 10000);

  req.on('close', () => {
    tradeEmitter.off('trade', listener);
    clearInterval(hb);
  });
});

const recentParams = z.object({ symbol: z.string() });
const recentQuery = z.object({ limit: z.coerce.number().min(1).max(1000).default(200) });

tradesRouter.get('/:symbol/recent', (req, res) => {
  const { symbol } = recentParams.parse(req.params);
  const { limit } = recentQuery.parse(req.query);
  const trades = getRecentTrades(symbol.toUpperCase(), limit);
  res.setHeader('Cache-Control', 'public, max-age=1');
  res.json(trades);
});

export default tradesRouter;
