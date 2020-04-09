:: This script will search through pointcloud data in the data folder and generate a 3D printable model.
:: Please update these parameters before using it:

:: The latitude and longitude of the center of the square to extract
SET latitude=50.9413153
SET longitude=6.9577452

:: The size of the square to extract, in meters
SET size=200

:: The projection of the XYZ data.
:: The projection will not be changed, this info is used to convert the center coordinates.
SET projection="WGS 84"

:: Add some space between the lowest part of the surface and the bottom of the 3D mesh. In meters.
SET verticaloffset=4

:: The directory where the program will look for .xyz files
SET datadirectory=data

:: Make sure Meshlab and Blender are installed and put their executable locations here.
SET meshlab="C:\Program Files\VCG\MeshLab\meshlabserver.exe"
SET blender="C:\Program Files\Blender Foundation\Blender 2.82\blender.exe"

:: Nothing to configure below this.

pointcloudtool.exe extract %datadirectory% pointcloud.xyz %latitude% %longitude% %projection% %size% || goto :error

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