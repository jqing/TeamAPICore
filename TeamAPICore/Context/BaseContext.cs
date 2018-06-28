using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamAPICore.Settings;

namespace TeamAPICore.Context
{
    public class BaseContext
    {
        protected readonly IOptions<ApplicationOption> _option;
        protected readonly string ConnectionString;
        public BaseContext(IOptions<ApplicationOption> option)
        {
            _option = option;
            ConnectionString = _option.Value.ConnectionString;
        }
        protected double GetDouble(string input)
        {
            if (!double.TryParse(input, out double output))
            {
                output = 0.0;
            }
            return output;
        }

        protected DateTime? GetDate(string input)
        {
            if(!DateTime.TryParse(input, out DateTime output))
            {
                return null;
            }
            return output;
        }
    }

}
