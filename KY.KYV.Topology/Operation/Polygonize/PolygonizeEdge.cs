using KY.KYV.Topology.Geometries;
using KY.KYV.Topology.Planargraph;

namespace KY.KYV.Topology.Operation.Polygonize
{
    /// <summary>
    /// An edge of a polygonization graph.
    /// </summary>
    public class PolygonizeEdge : Edge
    {
        private readonly LineString line;

        /// <summary>
        ///
        /// </summary>
        /// <param name="line"></param>
        public PolygonizeEdge(LineString line)
        {
            this.line = line;
        }

        /// <summary>
        ///
        /// </summary>
        public LineString Line => line;
    }
}
