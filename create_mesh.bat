:: This script will search through pointcloud data in the data folder and generate a 3D printable model.
:: Please update these parameters before using it:

:: The latitude and longitude of the center of the square to extract
SET latitude=51.3349443
SET longitude=7.2828901

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
SET blender="C:\Program Files\Blender Foundation\Blender\blender.exe"

:: Nothing to configure below this.

pointcloudtool.exe extract %datadirectory% pointcloud.xyz %latitude% %longitude% %projection% %size%

pointcloudtool.exe fix pointcloud.xyz pointcloud.xyz heightmap.xyz

%meshlab% -i pointcloud.xyz -i heightmap.xyz -o mesh.stl -s filter_script.mlx

pointcloudtool.exe makeSolid mesh.stl mesh.stl cube.stl %size% %verticaloffset%

%blender% -b -P intersect.py -- mesh.stl cube.stl mesh.stl

rm cube.stl
rm pointcloud.xyz
rm heightmap.xyz