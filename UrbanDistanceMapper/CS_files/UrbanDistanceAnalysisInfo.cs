using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace UrbanDistanceAnalysis
{
    public class UrbanDistanceMapInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "UrbanDistanceMap";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("b370d0a8-4784-473b-87b9-1bb32e1bbae8");
            }
        }

        public override string AuthorName
        {
            get
            {
                return "Mukul Gupta";
            }
        }
        public override string AuthorContact
        {
            get
            {
                return "www.mukulgupta.org";
            }
        }
    }
}
