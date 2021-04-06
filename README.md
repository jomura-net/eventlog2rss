# eventlog2rss

WindowsのイベントログをRSS feedに変換するASP.NET Pageです。

## description

* 対象とするイベントログを指定して、RSS出力します。
* イベントの種類をクエリ文字列で指定して、フィルタ出力できます。
* 日頃利用している各種Blog Readerに登録して、ログ監視するのが吉。
* サンプル http://jomura.net/EventLog2RSS/?logname=Application&type=Warning
* Powered by RSS.NET ( http://www.rssdotnet.com/ )

## Changelog

### version:1.1 (r33)

* プロジェクトをVisual Studio 2008に移行。

[Features]
* 特になし

[Fixed Bugs]
* URLエンコードの実装修正 (#128)
* RSS itemの<author>タグを抑制。 (#129)
* <link>タグ値の改行文字を半角スペースに変換。 (#131)

### version:1.0 (r26)

* 初版作成。
