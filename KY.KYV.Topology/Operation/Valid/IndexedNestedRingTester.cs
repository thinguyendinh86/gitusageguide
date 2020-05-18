using KY.KYV.Topology.Algorithm;
using KY.KYV.Topology.Geometries;
using KY.KYV.Topology.GeometriesGraph;
using KY.KYV.Topology.Index;
using KY.KYV.Topology.Index.Strtree;
using System.Collections.Generic;

namespace KY.KYV.Topology.Operation.Valid
{
    /**
     * Tests whether any of a set of {@link LinearRing}s are
     * nested inside another ring in the set, using a spatial
     * index to speed up the comparisons.
     *
     * @version 1.7
     */
    public class IndexedNestedRingTester
    {
        private readonly GeometryGraph _graph;  // used to find non-node vertices
        private readonly IList<LineString> _rings = new List<LineString>();
        private readonly Envelope _totalEnv = new Envelope();
        private ISpatialIndex<LineString> _index;
        private Coordinate _nestedPt;

        public IndexedNestedRingTester(GeometryGraph graph)
        {
            _graph = graph;
        }

        public Coordinate NestedPoint => _nestedPt;

        public void Add(LinearRing ring)
        {
            _rings.Add(ring);
            _totalEnv.ExpandToInclude(ring.EnvelopeInternal);
        }

        public bool IsNonNested()
        {
            BuildIndex();

            for (int i = 0; i < _rings.Count; i++)
            {
                var innerRing = (LinearRing)_rings[i];
                var innerRingPts = innerRing.Coordinates;

                var results = _index.Query(innerRing.EnvelopeInternal);
                for (int j = 0; j < results.Count; j++)
                {
                    var searchRing = (LinearRing)results[j];
                    var searchRingPts = searchRing.Coordinates;

                    if (innerRing == searchRing)
                        continue;

                    if (!innerRing.EnvelopeInternal.Intersects(searchRing.EnvelopeInternal))
                        continue;

                    var innerRingPt = IsValidOp.FindPointNotNode(innerRingPts, searchRing, _graph);
                    // Diego Guidi: removed => see Issue 121
                    //Assert.IsTrue(innerRingPt != null, "Unable to find a ring point not a node of the search ring");
                    /**
                     * If no non-node pts can be found, this means
                     * that the searchRing touches ALL of the innerRing vertices.
                     * This indicates an invalid polygon, since either
                     * the two holes create a disconnected interior,
                     * or they touch in an infinite number of points
                     * (i.e. along a line segment).
                     * Both of these cases are caught by other tests,
                     * so it is safe to simply skip this situation here.
                     */
                    if (innerRingPt == null)
                        continue;

                    bool isInside = PointLocation.IsInRing(innerRingPt, searchRingPts);
                    if (isInside)
                    {
                        _nestedPt = innerRingPt;
                        return false;
                    }
                }
            }
            return true;
        }

        private void BuildIndex()
        {
            _index = new STRtree<LineString>();

            for (int i = 0; i < _rings.Count; i++)
            {
                var ring = (LinearRing)_rings[i];
                var env = ring.EnvelopeInternal;
                _index.Insert(env, ring);
            }
        }
    }
}