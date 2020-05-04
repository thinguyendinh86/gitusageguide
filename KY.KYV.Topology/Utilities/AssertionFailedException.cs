using System;

namespace KY.KYV.Topology.Utilities
{
    /// <summary>
    ///
    /// </summary>
    public class AssertionFailedException : ApplicationException
    {
        /// <summary>
        ///
        /// </summary>
        public AssertionFailedException()
        { }

        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        public AssertionFailedException(string message) : base(message) { }
    }
}
