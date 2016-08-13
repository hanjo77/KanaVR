# TXT2InkscapeXMLv1.py

# A simple script to look through a list of text and
# turn each line into an xml entry for use in Inkscape.
# in this script we will just dump all of the text
# ontop of itself, irrespective of the actual layout
# then use inkscape to spread it out.

from subprocess import call
import argparse
 
def get_args():
  parser = argparse.ArgumentParser()
 
  # get all script args
  _, all_arguments = parser.parse_known_args()
  double_dash_index = all_arguments.index('--')
  script_args = all_arguments[double_dash_index + 1: ]
 
  # add parser rules
  parser.add_argument('-i', '--input', help="input file")
  parser.add_argument('-o', '--output', help="output folder")
  parsed_script_args, _ = parser.parse_known_args(script_args)
  return parsed_script_args
 
args = get_args()

outputFolder = args.output # str(input("What is the filename you wish for output?"))
import requests
lists = ['vocabulary', 'kanji']
for lst in lists:
      response = requests.get('https://www.wanikani.com/api/user/4109e82cf49da68b5f7661ba06f95fa5/'+lst)
      data = response.json()
      collection = data["requested_information"]
      if isinstance(collection, dict) and collection["general"]:
            collection = collection["general"]
      for obj in collection:
            if not obj["character"]:
                  print obj
                  continue
            char = obj["character"].encode('utf-8')
            call(["./text-3d.sh", char, args.output])

