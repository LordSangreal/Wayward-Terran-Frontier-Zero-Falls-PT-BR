import sqlite3

db_path = r'D:\SteamLibrary\steamapps\common\Zero Falls\parameterdb\world_params.sqlite'
conn = sqlite3.connect(db_path)
c = conn.cursor()
c.execute("SELECT name FROM sqlite_master WHERE type='table'")
tables = c.fetchall()
print('Tables:', tables)

for t in tables:
    t_name = t[0]
    c.execute(f"PRAGMA table_info({t_name})")
    cols = [col[1] for col in c.fetchall()]
    c.execute(f"SELECT count(*) FROM {t_name}")
    cnt = c.fetchone()[0]
    print(f"Table: {t_name} ({cnt} rows), Columns: {cols}")

    # If text columns exist, sample some rows
    c.execute(f"SELECT * FROM {t_name} LIMIT 3")
    sample = c.fetchall()
    print(" Sample:", sample)
