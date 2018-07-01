import bpy
import sys

index = sys.argv.index("--")
file_a = sys.argv[index + 1]
file_b = sys.argv[index + 2]
file_out = sys.argv[index + 3]

bpy.ops.object.select_all(action = 'SELECT')
bpy.ops.object.delete()

bpy.ops.import_mesh.stl(filepath = file_a)
obj_a = bpy.context.selected_objects[0]

bpy.ops.import_mesh.stl(filepath = file_b)
obj_b = bpy.context.selected_objects[0]

bpy.context.scene.objects.active = obj_a
bpy.ops.object.modifier_add(type = 'BOOLEAN')
bpy.context.object.modifiers[0].object = obj_b
bpy.ops.object.modifier_apply(modifier='Boolean')

bpy.context.scene.objects.active = obj_b
bpy.ops.object.delete()

bpy.context.scene.objects.active = obj_a
bpy.ops.export_mesh.stl(filepath = file_out)