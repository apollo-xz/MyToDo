using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.Shared.Parameters
{
    public class ToDoParameter : QueryParameter
    {
        // 事项分为待办和已完成，因此需要Status属性来表明
        public int? Status { get; set; }
    }
}
