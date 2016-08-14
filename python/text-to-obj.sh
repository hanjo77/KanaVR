#!/bin/bash
script_path="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

if [[ $# -lt 2 ]] ; then
    echo 'Please pass a word to convert to a 3D obj file as first and the output folder as a second parameter.\n\nExample: '$0' myTestWord /myFolder/and/subfolder'
    exit 1
fi
text=$1
output_path=$2
file_name=$text
if [[ $# -eq 3 ]] ; then
    file_name=$3
fi
python $script_path/text-to-svg.py -- --input=$text --output=$output_path
inkscape $output_path/$text.svg --export-text-to-path --export-id=maintext --export-pdf=$output_path/$file_name.pdf
rm $output_path/$text.svg
inkscape -l $output_path/$file_name.svg $output_path/$file_name.pdf
rm $output_path/$file_name.pdf
blender -noaudio --background --python $script_path/blender-transforms.py -- --input $output_path/$file_name.svg --output $output_path/$file_name.obj
rm $output_path/$file_name.svg
rm $output_path/$file_name.mtl
