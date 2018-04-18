using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRM_Data.Models
{
    public class Condition : Entity
    {
        public ConditionType Type { get; set; }
        public string Selector { get; set; }
        public string Value { get; set; }
    }

    public enum ConditionType
    {
        LessThan,
        GreaterThan,
        Equals,
        NotEquals
    }
}
