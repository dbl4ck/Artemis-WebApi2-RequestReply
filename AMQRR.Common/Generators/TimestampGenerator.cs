using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMQRR.Common.Generators
{
    public class TimestampGenerator
    {
        private string _format;
        private string _prefix;
        private string _suffix;

        public TimestampGenerator(string format = "HH:mm:ss.ffffff", string prefix = "[", string suffix = "]")
        {
            _format = format;
            _prefix = prefix;
            _suffix = suffix;
        }

        public string Generate()
        {
            return $"{_prefix}{DateTime.Now.ToString(_format)}{_suffix}";
        }

        public string Format
        {
            get => _format;
            set => _format = value;
        } 

    }
} 
