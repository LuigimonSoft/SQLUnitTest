using System.Collections.Generic;

namespace SQLUnitTest.Models
{
    public class BaseTestCase
    {
        public string StoredProcedure { get; set; } = string.Empty;
        public Dictionary<string, object> Parameters { get; set; } = new();
        public string? Connection { get; set; }
        public string TestName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}
