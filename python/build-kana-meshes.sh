#!/bin/bash

folder='/media/psf/Home/Workspace/KanaVR/python/svg'

rm -rf $folder/*
kanatypes=(hiragana katakana)
for kanatype in "${kanatypes[@]}" ; do
	mkdir $folder/$kanatype
	python txt-to-svg.py
done
for kanatype in "${kanatypes[@]}" ; do
	./convert-svg.sh svg/$kanatype/*
	for file in svg/$kanatype/* ; do
		s=${file##*/}
		base=${s%.svg}

		echo $file
		blender --background --python blender-transforms.py -- --input $file --output $folder/$kanatype/$base.obj
	done
	rm $folder/$kanatype/*.svg
	rm $folder/$kanatype/*.pdf
	rm $folder/$kanatype/*.mtl
done
