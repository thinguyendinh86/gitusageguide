using KY.KYV.Topology.Geometries;

namespace KY.KYV.Topology.GeometriesGraph
{
    /// <summary>
    ///
    /// </summary>
    public class NodeFactory
    {
        /// <summary>
        /// The basic node constructor does not allow for incident edges.
        /// </summary>
        /// <param name="coord"></param>
        public virtual Node CreateNode(Coordinate coord)
        {
            return new Node(coord, null);
        }
    }
}
