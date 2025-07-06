using System.Collections.Generic;

namespace SQLUnitTest.Models
{
    public class BaseTestCase
    {
        public string StoredProcedure { get; set; } = string.Empty;
        public IDictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
        public string? Connection { get; set; }
        public string TestName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}
