using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter.Model.Responses
{
    /// <summary>
    /// frankfurter Historical response model
    /// </summary>
    public class FrankfurterHistoricalResponse
    {
        public string Base { get; set; } = string.Empty;
        public Dictionary<string, Dictionary<string, decimal>> Rates { get; set; } = new();
    }
}
