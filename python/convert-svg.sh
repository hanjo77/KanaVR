#!/bin/bash
for var in "$@"
do
	s=${var##*/}
	base=${s%.svg}
	inkscape $var --export-text-to-path --export-id=maintext --export-pdf=svg/$base.pdf
	rm $var
	inkscape -l $var svg/$base.pdf
	rm svg/$base.pdf
done