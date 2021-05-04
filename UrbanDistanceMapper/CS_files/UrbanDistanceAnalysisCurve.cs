using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.Geometry.Collections;
using System;
using System.Collections.Generic;
using System.Linq;


namespace UrbanDistanceAnalysis
{
    public class UrbanDistanceAnalysisCurve : GH_Component
    {

        public UrbanDistanceAnalysisCurve()
          : base("UrbanDistanceAnalysisCurve", "UDM_CRV",
              "Analyze a map for social distancing values with curves as Input.",
              "Map Analysis", "Primitive")
        {
        }


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("sidewalk curves", "sidewalk", "select the curves that represent sidewalks.", GH_ParamAccess.list);
            pManager.AddCurveParameter("Building footprint curves", "Building", "select the curves that represent the footprint of all the buildings.", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Social distancing Radius", "Radius", "Enter the integer value for the length of social distancing radius.", GH_ParamAccess.item, 6);
            pManager.AddIntegerParameter("Density or number of people/width", "density", "Enter the integer value for the number of people crossing a width at the same time.", GH_ParamAccess.item, 3);
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("sidewalk mesh", "sidewalk", "mesh representing sidewalks", GH_ParamAccess.list);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> sidewalkList = new List<Curve>();
            List<Curve> footprintList = new List<Curve>();
            int socialDistanceRadius = 00;
            int codensity = 00;

            if (!DA.GetDataList(0, sidewalkList)) return;
            if (!DA.GetDataList(1, footprintList)) return;
            DA.GetData(2, ref socialDistanceRadius);
            DA.GetData(3, ref codensity);



            List<Mesh> crvMeshes = SidewalkCrvToMesh(sidewalkList, footprintList);

            List<Mesh> analysisMeshes = DistanceAnalyzer.DistanceAnalysis(crvMeshes, socialDistanceRadius, codensity);

            DA.SetDataList(0, analysisMeshes);
        }

        /// <summary>
        /// This Method converts the two input curvelist to a meshlist
        /// </summary>
        /// <param name="sidewalkList"></param>
        /// <param name="_FootprintList"></param>
        /// <returns></returns>
        private static List<Mesh> SidewalkCrvToMesh(List<Curve> sidewalkList, List<Curve> _FootprintList)
        {
            List<Brep> outLine = new List<Brep>();
            List<Brep> inLine = new List<Brep>();


            foreach (Curve sw in sidewalkList)
            {
                Curve sidewalk = sw.Simplify(CurveSimplifyOptions.All, 1, 2);
                Brep[] planarSidewalkList = Brep.CreatePlanarBreps(sidewalk, 1.0);

                outLine.AddRange(from Brep planarSidewalk in planarSidewalkList
                                 select planarSidewalk);
            }

            Curve[] footprintCrvList = Curve.CreateBooleanUnion(_FootprintList, 1.0);

            foreach (Curve footprint in footprintCrvList)
            {
                Brep[] planarBuildingList = Brep.CreatePlanarBreps(footprint, 1.0);

                inLine.AddRange(from Brep planarFootprints in planarBuildingList
                                select planarFootprints);
            }

            Brep[] sidewalks = Brep.CreateBooleanDifference(outLine, inLine, 1.0);
            List<Mesh> sidewalkMeshes = new List<Mesh>();

            foreach (Brep sw in sidewalks)
            {
                Mesh[] sideWalkMeshList = Mesh.CreateFromBrep(sw, MeshingParameters.FastRenderMesh);

                sidewalkMeshes.AddRange(from Mesh mesh in sideWalkMeshList
                                        select mesh);
                BrepEdgeList edges = sw.Edges;

            }

            return sidewalkMeshes;
        }



        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("d2574a27-ca57-4702-82bc-e2e4dcff0bbc");
    }


}
