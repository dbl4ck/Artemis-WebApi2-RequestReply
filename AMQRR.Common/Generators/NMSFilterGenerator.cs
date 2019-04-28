using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMQRR.Common.Enumerations;

namespace AMQRR.Common.Generators
{
    public class MqFilterGenerator
    {
        public const string AND = "AND";

        private readonly Dictionary<NMSFilter, string> _filters;

        public MqFilterGenerator()
        {
            _filters = new Dictionary<NMSFilter, string>();
        }

        public MqFilterGenerator Add(NMSFilter filter, string value)
        {
            _filters[filter] = value;

            return this;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            var firstElement = true;

            foreach (var filter in _filters)
            {
                if (firstElement != true)
                {
                    builder.Append($" {AND} ");
                }
                else
                {
                    firstElement = false;
                }

                builder.Append($"{filter.Key}='{filter.Value}'");
            }

            return builder.ToString();
        }

    }
}
