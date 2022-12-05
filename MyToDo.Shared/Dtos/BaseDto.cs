using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.Shared.Dtos
{
    public class BaseDto : INotifyPropertyChanged
    {
        public int Id { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 实现通知更新
        /// </summary>
        /// <param name="propertyName"></param>
        public void OnPropertyChanged([CallerMemberName] string propertyName="")
        {
            // ?. 运算符称作 null 条件运算符。 它在计算运算符右侧之前会检查是否存在空引用。
            // 最终结果为：如果 PropertyChanged 事件没有订阅者，则不执行用于引发该事件的代码。
            // 在这种情况下，如果不执行此检查，则会引发 NullReferenceException。
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
