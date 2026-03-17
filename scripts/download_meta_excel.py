import os
import requests

# Jota's Meta Sheet Excel export URL
XLSX_URL = "https://docs.google.com/spreadsheets/d/1PE3qqsS5qjOzrqzWkCIv6rRRV0NylciecDPlTsC60Qg/export?format=xlsx"

# Paths
SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
BASE_DIR = os.path.dirname(SCRIPT_DIR)
OUTPUT_DIR = os.path.join(BASE_DIR, "assets", "meta_source")
OUTPUT_FILE = os.path.join(OUTPUT_DIR, "Bazaar Meta Jota.xlsx")

def download_excel():
    if not os.path.exists(OUTPUT_DIR):
        os.makedirs(OUTPUT_DIR)

    print(f"Downloading Excel from: {XLSX_URL}")
    response = requests.get(XLSX_URL)
    
    if response.status_code == 200:
        with open(OUTPUT_FILE, 'wb') as f:
            f.write(response.content)
        print(f"Success! File saved to: {OUTPUT_FILE}")
    else:
        print(f"Error downloading: Code {response.status_code}")

if __name__ == "__main__":
    download_excel()
