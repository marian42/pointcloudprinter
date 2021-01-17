# Pointcloudprinter
A tool to turn pointcloud data from aerial lidar scans into solid meshes for 3D printing.
You can find examples of meshes made with this software on [Thingiverse](https://www.thingiverse.com/thing:2993625).

![](https://i.imgur.com/LaZ5C9A.jpg)

## Deprecated project

Since I published this project, the dataset I used is no longer available and the API in Meshlab that I used was dropped in a recent version of Meshlab.
If you just found this project, it is very likely that it will be completely useless.
I'm leaving it up in case people want to learn from it.
If this project is still useful, please [let me know](mailto:mail@marian42.de).


## Requirements
- Data: Your data needs to be one or multiple `.xyz` text files that contain comma separated numbers. I've tested this tool with data from [this website](https://www.opengeodata.nrw.de/produkte/geobasis/dom/dom1l/) which provides free data for the German state NRW.
- A computer running Windows. (You can port this software to Linux though)
- Have [Blender](https://www.blender.org/download/) and [Meshlab](http://www.meshlab.net/#download) installed.
The current version was tested with Meshlab 2020.03 (It won't work with newer versions of Meshlab!) and Blender 2.82.

## Usage
1. Download and unpack [this software](https://github.com/marian42/pointcloudprinter/releases/download/1.2/pointcloudtool.zip).
2. Download your pointcloud data and move your `.xyz` files into the data folder.
You can also put them somewhere else and configure the location later.
3. Decide on the location of the square you would like to extract from the data.
I suggest you use [Google Maps](https://www.google.com/maps/) to find the right place. Copy the two numbers in your Google Maps URL.
They are the latitude and longitude.
4. Find out what coordinate system your XYZ data use.
Use a tool like [this](https://epsg.io/transform#s_srs=4326&t_srs=4647) to convert your coordinates to the same system that your data uses.
The latitude and longitude are in the `EPSG:4326 WGS 84` format, which will be the input coordinate system in the transform coordinates app.
5. Edit the file `create_mesh.bat` and put in your configuration.
You need to set your x and y coordinates of the center of the square you'd like to extract.
You can also set the size the square.
6. Double click the `create_mesh.bat` file.
It will now run all the steps required to generate the mesh.
Depending on how much data there is to process, this will take between a few minutes and an hour.
Once finished, the window closes and if everything worked, a file called `mesh.stl` can be found in the project directory.

If you followed these steps and it did or did not work, please [tell me about it](mailto:mail@marian42.de)!

## How it works
This paragraph will explain what each line of the batch file does.

	pointcloudtool.exe extract %datadirectory% pointcloud.xyz %x% %y% %size%

This line runs the code from this repository to search all `.xyz`files in the `%datadirectory%`. It collects all points that are in the specified square and writes them to the file `pointcloud.xyz`.

	pointcloudtool.exe fix pointcloud.xyz pointcloud.xyz heightmap.xyz

This line runs the same program to fix holes in the pointcloud by adding new points where there are holes.
These holes confuse the mesh reconstruction algorithm later. It also creates a simplified heightmap of the pointcloud with normals.

	%meshlab% -i pointcloud.xyz -i heightmap.xyz -o mesh.stl -s filter_script.mlx

This line runs Meshlab with a script that contains instructions on how to reconstruct the mesh.
First, Meshlab runs a surface reconstruction algorithm on the heightmap.
This can be done because normals where calculated for it earlier.
Then, the normals of this mesh are transferred to the actual, high-detail pointcloud.
The mesh reconstruction algorithm is run again, now on the big pointcloud.
This mesh is then saved to mesh.stl.

	pointcloudtool.exe makeSolid mesh.stl mesh.stl cube.stl %size% %verticaloffset%
	
This line, again, uses the program from this project.
It adds new triangles around the seam of the mesh and connects them, resulting in a watertight, solid mesh. This could already be 3D-printed!
Furthermore, it creates a box with the exact horizontal and vertical dimensions that where configured.

	%blender% -b -P intersect.py -- mesh.stl cube.stl mesh.stl

This line uses Blender calculate the boolean intersection between the solid mesh and the box. The result of this is the final mesh.

## License

This project is distributed under the MIT licensense. Exempt from the license are the parts that link a source, as I didn't write them.

License for the preview image in this document:   
Land NRW (2018)   
Datenlizenz Deutschland - Namensnennung - Version 2.0 (www.govdata.de/dl-de/by-2-0)   
https://www.opengeodata.nrw.de/produkte/geobasis/dom/dom1l/   
