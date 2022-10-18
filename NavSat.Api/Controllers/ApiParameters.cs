namespace NavSat.Api.Controllers
{
    /// <summary>
    /// Optional Date Parameter
    /// </summary>
    public class ApiforDateParameters
    {
        /// <summary>
        /// Optional Fordate
        /// </summary>
        public DateTimeOffset? ForDate { get; set; }
    }
    /// <summary>
    /// Optional Date Parameter
    /// </summary>
    public class ApiforPeriodParameters
    {
        /// <summary>
        /// Optional FromDate
        /// </summary>
        public DateTimeOffset? FromDate { get; set; }
        /// <summary>
        /// Optional ToDate
        /// </summary>
        public DateTimeOffset? ToDate { get; set; }
    }

}
