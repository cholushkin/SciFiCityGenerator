set blender_path_clean=%blender_path:"=%
set unity_import_path_clean=%unity_assets_path:"=%

"%blender_path_clean%" %1 -b -P export_fbx_object.py -- %unity_import_path_clean%\%3 %2