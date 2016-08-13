#!/bin/bash

folder='/media/psf/Home/Workspace/KanaVR/python/svg'

kanatypes=(hiragana katakana)
for kanatype in "${kanatypes[@]}" ; do
	rm -rf $folder/$kanatype
	mkdir $folder/$kanatype
done
python txt-to-obj.py
