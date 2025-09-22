using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Core.Entities.Post_Aggregate
{
    public enum Status
    {
       [EnumMember(Value = "Draft")]
       Draft,
       [EnumMember(Value = "Published")]
       Published
    }
}
