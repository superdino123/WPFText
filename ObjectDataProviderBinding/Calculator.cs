using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDataProviderBinding
{
    class Calculator
    {
        public string Add(string arg1, string arg2)
        {
            double x = 0;
            double y = 0;
            double z = 0;
            if (double.TryParse(arg1, out x) && double.TryParse(arg2, out y))
            {
                z = x + y;
                return z.ToString();
            }
            return "Input Error!";
        }
    }
}
