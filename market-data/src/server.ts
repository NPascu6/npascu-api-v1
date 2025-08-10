import express from 'express';
import { ProviderRouter } from './providers/providerRouter';
import { FinnhubProvider } from './providers/finnhub';
import { AlphaVantageProvider } from './providers/alphaVantage';
import { SyntheticProvider } from './providers/synthetic';
import { quoteEmitter, insertQuoteSnapshot } from './db';

const app = express();
app.use(express.json());

const router = new ProviderRouter([
  new FinnhubProvider(process.env.FINNHUB_API_KEY),
  new AlphaVantageProvider(process.env.ALPHAVANTAGE_API_KEY),
  new SyntheticProvider()
]);

app.get('/healthz', (_req, res) => {
  res.json({ ok: true });
});

app.get('/quotes/:symbol', async (req, res) => {
  const symbol = req.params.symbol.toUpperCase();
  try {
    const quote = await router.getQuote(symbol);
    await insertQuoteSnapshot(quote);
    res.json(quote);
  } catch (err: any) {
    res.status(500).json({ error: err.message });
  }
});

app.get('/candles/:symbol', async (req, res) => {
  const symbol = req.params.symbol.toUpperCase();
  const interval = (req.query.interval as any) || '1d';
  const from = req.query.from ? new Date(req.query.from as string) : new Date(Date.now() - 24 * 60 * 60 * 1000);
  const to = req.query.to ? new Date(req.query.to as string) : new Date();
  try {
    const candles = await router.getCandles(symbol, interval, from, to);
    res.json(candles);
  } catch (err: any) {
    res.status(500).json({ error: err.message });
  }
});

app.get('/orderbook/:symbol/snapshot', async (req, res) => {
  const symbol = req.params.symbol.toUpperCase();
  const depth = req.query.depth ? parseInt(req.query.depth as string, 10) : 25;
  try {
    const snapshot = await router.getOrderBookSnapshot(symbol, depth);
    res.json(snapshot);
  } catch (err: any) {
    res.status(500).json({ error: err.message });
  }
});

app.get('/symbols', async (req, res) => {
  const query = (req.query.query as string) || '';
  try {
    const results = await router.searchSymbols(query);
    res.json(results);
  } catch (err: any) {
    res.status(500).json({ error: err.message });
  }
});

app.get('/stream/quotes', (req, res) => {
  const symbolsParam = (req.query.symbols as string) || '';
  const symbols = symbolsParam.split(',').map(s => s.trim().toUpperCase()).filter(Boolean);
  res.writeHead(200, {
    'Content-Type': 'text/event-stream',
    'Cache-Control': 'no-cache',
    Connection: 'keep-alive'
  });
  const listener = (quote: any) => {
    if (symbols.length === 0 || symbols.includes(quote.symbol)) {
      res.write(`data: ${JSON.stringify(quote)}\n\n`);
    }
  };
  quoteEmitter.on('quote', listener);
  req.on('close', () => {
    quoteEmitter.off('quote', listener);
  });
});

const port = Number(process.env.PORT) || 8080;
app.listen(port, () => {
  console.log(`Server listening on ${port}`);
});
