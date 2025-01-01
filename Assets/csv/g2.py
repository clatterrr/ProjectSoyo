import requests
import pandas as pd
from datetime import datetime, timedelta

# 设置参数
coin_id = 'bitcoin'
vs_currency = 'usd'
days = 10  # 获取最近十天的数据
interval = 'daily'  # 时间间隔

# CoinGecko API URL
url = f'https://api.coingecko.com/api/v3/coins/{coin_id}/market_chart'

# 设置请求参数
params = {
    'vs_currency': vs_currency,
    'days': days,
    'interval': interval
}

# 发送请求并获取响应
response = requests.get(url, params=params)
data = response.json()
print(data)

# 提取价格数据
prices = pd.DataFrame(data['prices'], columns=['timestamp', 'price'])
prices['timestamp'] = pd.to_datetime(prices['timestamp'], unit='ms')

# 打印结果
print(prices)