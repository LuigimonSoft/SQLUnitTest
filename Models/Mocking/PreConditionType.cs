namespace SQLUnitTest.Models.Mocking
{
    /// <summary>
    /// Supported types of preconditions.
    /// </summary>
    public enum PreConditionType
    {
        /// <summary>
        /// Raw SQL query string.
        /// </summary>
        Query,
        /// <summary>
        /// Stored procedure name.
        /// </summary>
        StoredProcedure,
        /// <summary>
        /// Path to a JSON file containing additional preconditions.
        /// </summary>
        JsonFile,
        /// <summary>
        /// Path to a .sql file whose contents should be executed.
        /// </summary>
        SqlFile,
        /// <summary>
        /// Ensures LocalDB is installed.
        /// </summary>
        InstallLocalDb
    }
}

