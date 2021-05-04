using Rhino.Geometry;
using Rhino.Geometry.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UrbanDistanceAnalysis
{
    /// <summary>
    /// Hey Brandon, This is the main code.
    /// it takes a list of meshes representing sidewalks as an input. 
    /// takes each vertex of the mesh and finds a point on its closest edge.
    /// draws and expands a circle on the point till it finds an intersection on another boundary
    /// infers width of the space from the radii of the circle.
    /// </summary>
    public static class DistanceAnalyzer
    {
        public static List<Mesh> DistanceAnalysis(List<Mesh> Analysis, int _SocialDistanceRadius, int Codensity)
        {
            if (_SocialDistanceRadius < 1)
            {
                _SocialDistanceRadius = 1;
            }


            if (Codensity < 1)
            {
                Codensity = 1;
            }
            Plane plane = Plane.WorldXY;

            List<Circle> test = new List<Circle>();

            foreach (Mesh swMesh in Analysis)
            {
                List<LineCurve> boundaryList = new List<LineCurve>();
                List<Point3d> boundaryPoints = new List<Point3d>();

                Polyline[] meshboundary = swMesh.GetOutlines(plane);
                MeshVertexList vertexList = swMesh.Vertices;


                foreach (Polyline meshPline in meshboundary)
                {
                    PolylineCurve boundary = new PolylineCurve(meshPline);

                    Line[] boundarysegments = meshPline.GetSegments();

                    boundaryList.AddRange(from Line segment in boundarysegments
                                          select new LineCurve(segment));
                    Point3d[] points;
                    if (boundary.GetLength(1.0) <= 100)
                    {
                        boundary.DivideByCount(4, false, out points);
                        boundaryPoints.AddRange(from Point3d point in points
                                                select point);
                    }
                    else
                    {
                        boundary.DivideByLength(100.0, false, out points);
                        boundaryPoints.AddRange(from Point3d point in points
                                                select point);
                    }

                }

                PointCloud pointCloud = new PointCloud(boundaryPoints);
                int width = 2;
                int testRadii = (_SocialDistanceRadius * Codensity) + 100;

                for (int vertex = 0; vertex < vertexList.Count; vertex++)
                {
                    Point3d v = vertexList.Point3dAt(vertex);
                    int cP = pointCloud.ClosestPoint(v);

                    
                    Point3d point = pointCloud[cP].Location;

                    int rvalue = 255;



                    int intersectionCount = 0;

                    while (intersectionCount <= 3 && width < testRadii)
                    {
                        Curve walkwidth = (new Circle(point, width)).ToNurbsCurve();

                        for (int i = 0; i < boundaryList.Count; i++)
                        {
                            if (Curve.PlanarCurveCollision(walkwidth, boundaryList[i], plane, 0.1))
                            {
                                intersectionCount++;
                            }
                        }

                        width++;
                    }

                    rvalue = intersectionCount >= 3 && ((width / _SocialDistanceRadius) / Codensity) * 255 < 255
                        ? ((width / _SocialDistanceRadius) / Codensity) * 255
                        : intersectionCount > 2 && ((width / _SocialDistanceRadius) / Codensity) * 255 >= 255 ? 255 : 255;

                    if (width < testRadii)
                    {
                    }
                    else
                    {
                        rvalue = 0;
                    }
                    swMesh.VertexColors.SetColor(vertex, System.Drawing.Color.FromArgb(rvalue, 70, 100));
                }
            }

            return Analysis;
        }
    }

}