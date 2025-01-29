import requests
from bs4 import BeautifulSoup
import xml.dom.minidom

year = "2024" # Change only this on a year to year basis

urls = ["https://www.nfl.com/stats/team-stats/offense/rushing/" + year + "/reg/all", "https://www.nfl.com/stats/team-stats/offense/passing/" + year + "/reg/all", "https://www.nfl.com/stats/team-stats/defense/rushing/" + year + "/reg/all", "https://www.nfl.com/stats/team-stats/defense/passing/" + year + "/reg/all"]

for url in urls:
    # Fetch the webpage
    response = requests.get(url)
    if response.status_code != 200:
        print(f"Failed to fetch the webpage: {response.status_code}")
        exit()

    soup = BeautifulSoup(response.content, 'html.parser')

    # Find table with stats
    table = soup.find('table')
    if not table:
        print("Failed to find the stats table on the webpage.")
        exit()

    #Collect all headers to scrape data from
    headers = [header.text.strip() for header in table.find_all('th')]
    #Change headers to comply with xml naming conventions
    headers[0] = "Name"
    for headerIndex in range(1, len(headers)):
        headers[headerIndex] = headers[headerIndex].replace("%", "Percent").replace("/", "Per").replace("+", "Plus").replace(" ", "").replace("1st", "First").replace("20", "Twenty").replace("40", "Forty")

    rows = table.find_all('tr')[1:]
    data = []
    for row in rows:
        cols = row.find_all('td')
        cols = [col.text.strip() for col in cols]
        if cols:
            data.append(cols)

    #Clean up name (just removing spaces and new line chars)
    for teamIndex in range(0, len(data)):
        data[teamIndex][0] = data[teamIndex][0].split("\n")[0]

    xmlStruct = "<NFLTeamStats>\n"

    #Add each team to xmlStruct
    for team in data:
        teamXmlStruct = "    <Team>\n"

        for index, header in enumerate(headers):
            teamXmlStruct += f"        <{header}>{team[index]}</{header}>\n"

        teamXmlStruct += "    </Team>\n"

        xmlStruct += teamXmlStruct

    xmlStruct += "</NFLTeamStats>"


    with open("data\\NFL-" + url.split("/")[5] + "-" + url.split("/")[6] + ".xml", "w", encoding="utf-8") as file:
        file.write(xmlStruct)
    file.close()