import os

root = r"D:\SteamLibrary\steamapps\common\Zero Falls\Tutorials"
dir1 = [os.path.join(root, d) for d in os.listdir(root) if os.path.isdir(os.path.join(root, d))]

for d1 in dir1:
    print(f"DIR 1: {d1}")
    dir2 = [os.path.join(d1, d) for d in os.listdir(d1) if os.path.isdir(os.path.join(d1, d))]
    if not dir2:
        print(f"  [WARNING] DIR 1 has NO subdirectories! {d1}")
    for d2 in dir2:
        xmls = [f for f in os.listdir(d2) if f.endswith('.xml')]
        print(f"  DIR 2: {d2} -> XMLs: {xmls}")
        if not xmls:
            print(f"    [CRITICAL BUG FOUND] Subfolder '{d2}' has NO XML files! This causes NullReferenceException!")
