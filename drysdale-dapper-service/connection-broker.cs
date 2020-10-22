namespace Drysdale.Dapper.Service
{
    #region Class.ConnectionBroker...
    /// <summary>
    /// Class.ConnectionBroker
    /// </summary>
    public class ConnectionBroker
    {
        /// <summary>
        /// Naked-ID (Connection-String-Identification)
        /// </summary>
        public string NakedID { get; set; }

        /// <summary>
        /// The-Value (Connection-String)
        /// </summary>
        public string TheValue { get; set; }

        /// <summary>
        /// Targeted-Database
        /// </summary>
        public TargetDbType TargetedDatabase { get; set; }
    }
    #endregion
}
