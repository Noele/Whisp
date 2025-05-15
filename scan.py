import os
import json

user_desktop = os.path.join(os.environ["USERPROFILE"], "Desktop")
public_desktop = os.path.join(os.environ["PUBLIC"], "Desktop")

desktop_paths = [user_desktop, public_desktop]
apps_list = []

def friendly_name(path):
    # Remove extension and spaces, lowercase
    return os.path.splitext(os.path.basename(path))[0].lower().replace(" ", "")

seen_names = set()

for desktop_path in desktop_paths:
    if not os.path.exists(desktop_path):
        continue
    for file in os.listdir(desktop_path):
        if file.lower().endswith(".lnk"):
            name = friendly_name(file)
            if name not in seen_names:
                full_path = os.path.join(desktop_path, file)
                apps_list.append({"program": name, "path": full_path})
                seen_names.add(name)

with open("apps.json", "w", encoding="utf-8") as f:
    json.dump(apps_list, f, indent=4)

print(f"Saved {len(apps_list)} shortcuts as list of dicts.")
