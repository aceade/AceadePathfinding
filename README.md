# AceadePathfinding
A pathfinding system that I am developing using Unity 4.6. I plan to use this as a base for several other projects,
and figure that this might be useful to someone else.

## Component Overview
The basic component of this is the Node class. Each Node has several variables: a position, a list of neighbours, a
height, the illumination at that node and whether or not a unit can actually walk there. 

A NavigationMesh is simply a collection of Nodes. I have chosen to use a Dictionary, with the Nodes acting as values and
their positions acting as keys, as the Nodes must have unique positions. The NavigationMesh currently contains methods to
find the closest Node to an input position, name the NavigationMesh, and add Nodes. More will come later.

The BuildNavMesh component is simply a tool that actually builds the NavigationMesh. It is attached to a cube in the
scene, and casts rays down at regular interval. If the ray hits a collider, the position is recorded and a new Node
created at that position. There are also methods to calculate the intensity of any Spot Lights at a Node, calculate the
neighbours of each Node, and display the Nodes when the cube is selected.

The GameManager script is a static class that I am using to load NavigationMeshes for each level and save them to the
disc for future use. It contains a Dictionary of level names and the corresponding NavigationMesh, allowing you to
pregenerate and then load NavigationMeshes at runtime.

The Preloading script simply preloads everything, and displays a logo. I've included mine as a template.

The SerializableVector3 script was taken from a post on the Unity3D forums, and is basically a serialisable version of
the Vector3 struct. The reason for this is that saving and loading meshes relies on serialisation, but the default Vector3
struct is not serialisable. The original post was found at http://goo.gl/5XKomt

The AStar Class is a complete, but currently untested implementation of the standard A* path-finding algorithm. This can be extended
as you see fit. This may require some form of multithreading, and I will upload the plugin I use for this (once I figure out how to add external folders to my repository).

## How to use this
Clone it with git, or download as a zip folder to the desktop. Once you have it, copy the folder into your Unity project, underneath the Assets folder.

## Terms of Use
The code and assets here are available under the Creative Commons Attribution Licence (CC BY 4.0). In short, you may use
these assets for your own projects, including commercial projects, but you must attribute them to me, ideally with a link to the repository.

## How to contact me
If you have any suggestions, queries or complaints, send them to me at philiprowlands90@gmail.com

## Credits
The code here, along with the multithreading plugin I am using myself, was originally based on code from the Unity Gems tutorial website. However, the site appears to have disappeared, so unfortunately I can't link to it.

