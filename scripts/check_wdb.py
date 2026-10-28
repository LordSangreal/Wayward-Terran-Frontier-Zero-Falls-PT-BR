import sqlite3

conn = sqlite3.connect(r'D:\SteamLibrary\steamapps\common\Zero Falls\worldsaves\Thiago.wdb')
c = conn.cursor()
c.execute("SELECT name FROM sqlite_master WHERE type='table'")
tables = [r[0] for r in c.fetchall()]
print('Tables in Thiago.wdb:', tables)

for t in tables:
    c.execute(f"SELECT count(*) FROM {t}")
    cnt = c.fetchone()[0]
    print(f"Table {t}: {cnt} rows")

c.execute("SELECT name FROM sqlite_master WHERE type='table' AND name='world'")
if c.fetchone():
    c.execute("SELECT data FROM world LIMIT 1")
    blob = c.fetchone()[0]
    print(f"World blob size: {len(blob)} bytes")
