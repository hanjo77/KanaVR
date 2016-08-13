import bpy, math
from bpy.props import *
from mathutils import *
from math import *  
import argparse
 
def get_args():
  parser = argparse.ArgumentParser()
 
  # get all script args
  _, all_arguments = parser.parse_known_args()
  double_dash_index = all_arguments.index('--')
  script_args = all_arguments[double_dash_index + 1: ]
 
  # add parser rules
  parser.add_argument('-i', '--input', help="input file")
  parser.add_argument('-o', '--output', help="output file")
  parsed_script_args, _ = parser.parse_known_args(script_args)
  return parsed_script_args
 
args = get_args()

scene = bpy.context.scene
bpy.ops.object.mode_set(mode='OBJECT')

bpy.ops.object.select_all(action='SELECT')
bpy.ops.object.delete()

bpy.ops.import_curve.svg(filepath=args.input, filter_glob="*.svg")
i = 0
for tmp in bpy.data.objects:
  tmp.select = True
  scene.objects.active = tmp
bpy.ops.object.join()
bpy.ops.object.select_pattern(pattern="Curve")
bpy.ops.object.convert(target="MESH")
bpy.ops.object.mode_set(mode='EDIT')
bpy.ops.mesh.select_mode(type='FACE')
bpy.ops.mesh.select_all(action='SELECT')
bpy.ops.mesh.normals_make_consistent(inside=False)
bpy.ops.object.mode_set(mode='OBJECT')
bpy.ops.transform.resize(value=(100.0, 100.0, 100.0))
obj = bpy.context.active_object
dm = obj.modifiers.new('Solidify','SOLIDIFY')
bpy.ops.export_scene.obj(filepath=args.output, axis_forward='Y', axis_up='Z', use_normals=True)
print ('Writing to ' + args.output)
