:: This script will search through pointcloud data in the data folder and generate a 3D printable model.
:: Please update these parameters before using it:

:: The center of the square to extract, using the same coordinate system as the XYZ data supplied.
SET x=384257.488234335
SET y=5686683.1372826

:: The size of the square to extract, in meters
SET size=200

:: Add some space between the lowest part of the surface and the bottom of the 3D mesh. In meters.
SET verticaloffset=4

:: The directory where the program will look for .xyz files
SET datadirectory=data

:: Make sure Meshlab and Blender are installed and put their executable locations here.
SET meshlab="C:\Program Files\VCG\MeshLab\meshlabserver.exe"
SET blender="C:\Program Files\Blender Foundation\Blender 2.82\blender.exe"

:: Nothing to configure below this.

pointcloudtool.exe extract %datadirectory% pointcloud.xyz %x% %y% %size% || goto :error

pointcloudtool.exe fix pointcloud.xyz heightmap.xyz || goto :error

%meshlab% -i pointcloud.xyz heightmap.xyz -o mesh.stl -s filter_script.mlx || goto :error

pointcloudtool.exe makeSolid mesh.stl mesh.stl cube.stl %size% %verticaloffset% || goto :error

%blender% -b -P intersect.py -- mesh.stl cube.stl mesh.stl || goto :error

del cube.stl
del pointcloud.xyz
del heightmap.xyz

exit /b

:error
echo Failed to create the mesh. && pause