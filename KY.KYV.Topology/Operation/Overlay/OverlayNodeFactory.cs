using KY.KYV.Topology.Geometries;
using KY.KYV.Topology.GeometriesGraph;

namespace KY.KYV.Topology.Operation.Overlay
{
    /// <summary>
    /// Creates nodes for use in the <c>PlanarGraph</c>s constructed during
    /// overlay operations.
    /// </summary>
    public class OverlayNodeFactory : NodeFactory
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        public override Node CreateNode(Coordinate coord)
        {
            return new Node(coord, new DirectedEdgeStar());
        }
    }
}
