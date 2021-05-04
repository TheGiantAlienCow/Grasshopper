using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace UrbanDistanceAnalysis
{
    public class UrbanDistanceAnalysisSurface : GH_Component
    {

        public UrbanDistanceAnalysisSurface()
          : base("UrbanDistanceAnalysisSurface", "UDM_SRF",
              "Analyze a map for social distancing values with walkable surfaces as Input.",
              "Map Analysis", "Primitive")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("sidewalk Mesh", "sidewalk", "select the Mesh that represent walkable sidewalks.", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Social distancing Radius", "Radius", "Enter the integer value for the length of social distancing radius.", GH_ParamAccess.item, 6);
            pManager.AddIntegerParameter("Density or number of people/width", "density", "Enter the integer value for the number of people crossing a width at the same time.", GH_ParamAccess.item, 3);
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("sidewalk mesh", "sidewalk", "mesh representing sidewalks", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Mesh> sidewalkList = new List<Mesh>();
            int socialDistanceRadius = 00;
            int codensity = 00;

            if (!DA.GetDataList(0, sidewalkList)) return;
            DA.GetData(1, ref socialDistanceRadius);
            DA.GetData(2, ref codensity);




            List<Mesh> analysisMeshes = DistanceAnalyzer.DistanceAnalysis(sidewalkList, socialDistanceRadius, codensity);
            DA.SetDataList(0, analysisMeshes);
        }


        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("FDC62C6D-7C03-412D-8FF8-B76439197730");



    }
}