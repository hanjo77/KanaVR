# TXT2InkscapeXMLv1.py

# A simple script to look through a list of text and
# turn each line into an xml entry for use in Inkscape.
# in this script we will just dump all of the text
# ontop of itself, irrespective of the actual layout
# then use inkscape to spread it out.


ofilenm = "svg" # str(input("What is the filename you wish for output?"))
kanaTypes = ["hiragana", "katakana"]
ListFlm = "kana.csv"
lineNr = 0;
f = open(ListFlm,"r")
for line in f:
    if lineNr > 0:
        for index, kanaType in enumerate(kanaTypes):
            TXTLine = line.strip()
            lineArray = TXTLine.split(";")
            print (lineArray[1+index])
            outfile = open(ofilenm+'/'+kanaType+'/'+lineArray[0]+'.svg',"w")
            # The block that follows is the Inkscape standard header.
            outfile.write('<?xml version="1.0" encoding="UTF-8" standalone="no"?>\n')
            outfile.write('<!-- Created with Inkscape (http://www.inkscape.org/) -->\n')
            outfile.write('\n')
            outfile.write('<svg\n')
            outfile.write('   xmlns:dc="http://purl.org/dc/elements/1.1/"\n')
            outfile.write('   xmlns:cc="http://creativecommons.org/ns#"\n')
            outfile.write('   xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#"\n')
            outfile.write('   xmlns:svg="http://www.w3.org/2000/svg"\n')
            outfile.write('   xmlns="http://www.w3.org/2000/svg"\n')
            outfile.write('   xmlns:sodipodi="http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd"\n')
            outfile.write('   xmlns:inkscape="http://www.inkscape.org/namespaces/inkscape"\n')
            outfile.write('   width="744.09448819"\n')
            outfile.write('   height="1052.3622047"\n')
            outfile.write('   id="svg2"\n')
            outfile.write('   version="1.1"\n')
            outfile.write('   inkscape:version="0.48.4 r9939"\n')
            outfile.write('   sodipodi:docname="New document 1">\n')
            outfile.write('  <defs\n')
            outfile.write('     id="defs4" />\n')
            outfile.write('  <sodipodi:namedview\n')
            outfile.write('     id="base"\n')
            outfile.write('     pagecolor="#ffffff"\n')
            outfile.write('     bordercolor="#666666"\n')
            outfile.write('     borderopacity="1.0"\n')
            outfile.write('     inkscape:pageopacity="0.0"\n')
            outfile.write('     inkscape:pageshadow="2"\n')
            outfile.write('     inkscape:zoom="3.959798"\n')
            outfile.write('     inkscape:cx="180.8475"\n')
            outfile.write('     inkscape:cy="800.78961"\n')
            outfile.write('     inkscape:document-units="px"\n')
            outfile.write('     inkscape:current-layer="layer1"\n')
            outfile.write('     showgrid="false"\n')
            outfile.write('     inkscape:window-width="1280"\n')
            outfile.write('     inkscape:window-height="742"\n')
            outfile.write('     inkscape:window-x="-2"\n')
            outfile.write('     inkscape:window-y="-3"\n')
            outfile.write('     inkscape:window-maximized="1" />\n')
            outfile.write('  <metadata\n')
            outfile.write('     id="metadata7">\n')
            outfile.write('    <rdf:RDF>\n')
            outfile.write('      <cc:Work\n')
            outfile.write('         rdf:about="">\n')
            outfile.write('        <dc:format>image/svg+xml</dc:format>\n')
            outfile.write('        <dc:type\n')
            outfile.write('           rdf:resource="http://purl.org/dc/dcmitype/StillImage" />\n')
            outfile.write('        <dc:title></dc:title>\n')
            outfile.write('      </cc:Work>\n')
            outfile.write('    </rdf:RDF>\n')
            outfile.write('  </metadata>\n')
            outfile.write('  <g\n')
            outfile.write('     inkscape:label="'+lineArray[0]+'"\n')
            outfile.write('     inkscape:groupmode="layer"\n')
            outfile.write('     id="'+lineArray[0]+'">\n')


            # The section that follows is to build the SVG text elements
            outfile.write('    <text\n')
            outfile.write('        xml:space="preserve"\n')
            outfile.write('        style="font-size:300px;font-style:normal;font-weight:normal;line-height:125%;letter-spacing:0px;word-spacing:0px;fill:#000000;fill-opacity:1;stroke:none;font-family:Sans"\n')
            outfile.write('        x="0"\n')
            outfile.write('        y="0"\n')
            outfile.write('        id="maintext"\n')
            outfile.write('        sodipodi:linespacing="125%"><tspan\n')
            outfile.write('          sodipodi:role="line"\n')
            outfile.write('          id="tspan2991"\n')
            outfile.write('          x="0"\n')
            outfile.write('          y="0">'+lineArray[1+index]+'</tspan></text>\n') #This is the text element from the list
                    
            # The next section is the close off for the SVG xml.

            outfile.write('  </g>\n')
            outfile.write('</svg>\n')

            outfile.close()
    lineNr += 1