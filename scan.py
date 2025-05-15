import os
import json

user_desktop = os.path.join(os.environ["USERPROFILE"], "Desktop")
public_desktop = os.path.join(os.environ["PUBLIC"], "Desktop")

desktop_paths = [user_desktop, public_desktop]
app_map = {}

def friendly_name(path):
    return os.path.splitext(os.path.basename(path))[0].lower().replace(" ", "")

for desktop_path in desktop_paths:
    if not os.path.exists(desktop_path):
        continue
    for file in os.listdir(desktop_path):
        if file.lower().endswith(".lnk"):
            name = friendly_name(file)
            full_path = os.path.join(desktop_path, file)
            # Avoid overwriting if already found in user desktop
            if name not in app_map:
                app_map[name] = full_path

with open("apps.json", "w", encoding="utf-8") as f:
    json.dump(app_map, f, indent=4)

print(f"Saved {len(app_map)} shortcuts from user + public desktop.")
