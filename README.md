# Project-BD2-Operations-on-2D-data-using-own-CLR-and-UDT-data-types
The project aims to develop an API and its implementation to process two-dimensional data using custom CLR data types, UDTs, and methods. The prepared methods allow determining the distance between points, determining the area of a specified region, and checking if a point belongs to a given area.

## API functionality
The application allows performing operations on points and polygons. The user can add an arbitrary number of points defined by integer coordinates X and Y and calculate the distance between them. Moreover, the user can check whether a point belongs to the area defined by the polygon. Polygons are defined by a combination of points, and the order of adding the points to the polygon is not significant. The application implements an algorithm that sorts the vertices. For polygons, it is possible to determine the area of the region.

## Data types and methods
The project defines two User-Defined Types (UDTs):

1. Point - represents a point in two-dimensional space. The class includes the following methods: constructor, getter, setter, Read (deserialization), Write (serialization), and ToString. The class also includes the method distance to calculate the distance between two points.
2. Polygon - represents a polygon. The class includes a list of points that form the polygon and the methods: constructor, isNull, Read, Write, ToString, and isPointInPolygon to check whether a point belongs to the polygon.

The project also includes a console interface that enables users to add, remove, and display points and polygons. The user can also calculate the distance between two points and determine the area of a polygon.
