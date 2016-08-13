# TXT2InkscapeXMLv1.py

# A simple script to look through a list of text and
# turn each line into an xml entry for use in Inkscape.
# in this script we will just dump all of the text
# ontop of itself, irrespective of the actual layout
# then use inkscape to spread it out.

from subprocess import call

output = "svg" # str(input("What is the filename you wish for output?"))
kanaTypes = ["hiragana", "katakana"]
ListFlm = "kana.csv"
lineNr = 0;
f = open(ListFlm,"r")
for line in f:
      print(line)
      if lineNr > 0:
            for index, kanaType in enumerate(kanaTypes):
                  TXTLine = line.strip()
                  lineArray = TXTLine.split(";")
                  call(["./text-to-obj.sh", lineArray[index+1], output+'/'+kanaType, lineArray[0]])
      lineNr += 1

