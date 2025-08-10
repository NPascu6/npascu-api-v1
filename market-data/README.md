# Market Data Service

Lightweight market data backend using Neon Postgres and free-tier providers.

## Setup

```bash
npm install
npm run build
node dist/server.js
```

Environment variables:

```
DATABASE_URL=postgres://...
FINNHUB_API_KEY=...
ALPHAVANTAGE_API_KEY=...
PORT=8080
TRADES_PROVIDER=SYNTHETIC
TRADES_POLL_MS=1500
TRADES_BUFFER_N=500
```

## Endpoints

- `GET /healthz` – service health
- `GET /symbols?query=tesla` – search tickers
- `GET /quotes/:symbol` – fetch latest quote and store snapshot
- `GET /candles/:symbol?interval=1d&from=...&to=...` – fetch candles
- `GET /stream/quotes?symbols=AAPL,MSFT` – SSE stream with quote updates
- `GET /trades/stream?symbols=AAPL,MSFT` – SSE stream with trades
- `GET /trades/:symbol/recent?limit=200` – recent trades

## Testing

```
npm test
```
