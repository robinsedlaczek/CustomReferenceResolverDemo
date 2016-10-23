using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomReferenceResolverDemo.BusinessModels
{
    public class SomeBusinessModel
    {
        public SomeBusinessModel(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
