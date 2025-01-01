from pycoingecko import CoinGeckoAPI
import pandas as pd
cg = CoinGeckoAPI()
ohlc = cg.get_coin_ohlc_by_id(id = "ethereum", vs_currency = "usd", days = "30")

df = pd.DataFrame(ohlc)
df.columns = ["date", "open", "high", "low", "close"]
df["date"] = pd.to_datetime(df["date"], unit = "ms")
df.set_index("date", inplace = True)
df.to_csv("bitcoin_prices.csv", index=False)