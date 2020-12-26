# NGenderApp

根據中文名字猜測性別

## 原理

根據貝氏定理：`P(Y|X) = P(X|Y) * P(Y) / P(X)`

當 X 條件獨立時 `P(X|Y) = P(X1|Y) * P(X2|Y) * ...`，應用到由名字猜測性別上。

```
P(gender=女|name=瑋芳) 
= P(name=瑋芳|gender=女) * P(gender=女) / P(name=瑋芳)
= P(name has 瑋|gender=女) * P(name has 芳|gender=女) * P(gender=女) / P(name=瑋芳)
```

## 訓練資料來源

`charfreq.csv` 適從曾經在網路上流傳的訂房紀錄資料集整理而來。
