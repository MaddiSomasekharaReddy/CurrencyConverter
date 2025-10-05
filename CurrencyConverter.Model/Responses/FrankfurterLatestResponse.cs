using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter.Model.Responses
{
    /// <summary>
    /// Frankfurter Latest response model
    /// </summary>
    public class FrankfurterLatestResponse
    {
        public string Base { get; set; } = string.Empty;
        public Dictionary<string, decimal> Rates { get; set; } = new();
    }
}
