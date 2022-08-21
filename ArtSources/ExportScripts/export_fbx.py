import sys
import bpy

argv = sys.argv
argv = argv[argv.index("--") + 1:]  # get all args after "--"

bpy.ops.export_scene.fbx(
    filepath = argv[0], 
    path_mode = 'RELATIVE', 
    use_custom_props = True,
    apply_scale_options = 'FBX_SCALE_UNITS',
    object_types = {'EMPTY','MESH'})