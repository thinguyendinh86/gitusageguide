using KY.KYV.Topology.Geometries;

namespace KY.KYV.Topology.Triangulate
{
    /// <summary>
    /// An interface for factories which create a {@link ConstraintVertex}
    /// </summary>
    /// <author>Martin Davis</author>
    public interface ConstraintVertexFactory
    {
        ConstraintVertex CreateVertex(Coordinate p, Segment constraintSeg);
    }
}