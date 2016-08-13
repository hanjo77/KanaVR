#!/bin/bash

folder='/media/psf/Home/Workspace/KanaVR/python/svg/wanikani'

rm -rf $folder/*
python wanikani-to-svg.py -- --output=$folder
