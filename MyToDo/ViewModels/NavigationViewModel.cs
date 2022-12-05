using MyToDo.Extensions;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.ViewModels
{
    /// <summary>
    /// 为从页面A导航到页面B的这一过程，织入其他行为
    /// </summary>
    /// INavigationAware：为导航中涉及的对象提供一种通知导航活动的能力，主要是三个方法IsNavigationTarget、
    /// OnNavigatedFrom、OnNavigatedFrom。
    public class NavigationViewModel : BindableBase, INavigationAware
    {
        private readonly IContainerProvider containerProvider;
        public readonly IEventAggregator aggregator;

        public NavigationViewModel(IContainerProvider containerProvider)
        {
            this.containerProvider = containerProvider;
            // 这里通过Prism容器得到事件聚合器
            aggregator = containerProvider.Resolve<IEventAggregator>();
        }

        // Called to determine if this instance can handle the navigation request.
        // 可以根据navigationContext导航上下文的信息，确定是否接受这次导航请求
        // 这里总是返回true，表示始终接受.
        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        // Called when the implementer is being navigated away from.
        // 这里是提供一个织入点，它的调用时机是刚才页面A离开，该方法执行后执行IsNavigationTarget
        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        // Called when the implementer has been navigated to.
        // 这里是提供一个织入点，它的调用时机是页面已经导航到页面B之后
        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {

        }

        // 在 MVVM 中，对于 View 和 ViewModel 之间的交互，可以使用 INotifyProperty 和 ICommand 来实现。
        // 而对于不同 ViewModel 之间的通信，为了实现低耦合，Prism 提供了EventAggregator。
        // 通过事件聚合器发起UpdateLoadingEvent事件，事件携带的信息由UpdateModel来进行描述
        // UpdateLoading是自定义的扩展方法，定义位于DialogExtensions中。可以发现，UpdateLoading是对
        // UpdateLoadingEvent这个事件的发布，传递的信息就是这里定义的匿名对象UpdateModel
        public void UpdateLoading(bool IsOpen)
        {
            aggregator.UpdateLoading(new Common.Events.UpdateModel()
            {
                IsOpen = IsOpen
            });
            
        }
    }
}
