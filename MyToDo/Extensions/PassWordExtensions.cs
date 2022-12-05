using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MyToDo.Extensions
{
    public class PassWordExtensions
    {
        //DependencyObject类表示参与依赖属性系统的对象,用于给UI添加依赖和附加属性，注重数据与UI的关联。
        public static string GetPassWord(DependencyObject obj)
        {
            return (string)obj.GetValue(PassWordProperty);
        }

        public static void SetPassWord(DependencyObject obj, string value)
        {
            obj.SetValue(PassWordProperty, value);
        }

        public static readonly DependencyProperty PassWordProperty =
            DependencyProperty.RegisterAttached("PassWord", typeof(string), typeof(PassWordExtensions), new FrameworkPropertyMetadata(string.Empty, OnPassWordPropertyChanged));

        static void OnPassWordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {

            var passBox = sender as PasswordBox;

            string password = (string)e.NewValue;

            // password是新变化的值，passWord.Password是依赖属性对象存储的值。
            // 不一致就要把passWord.Password进行更新
            if (passBox != null && passBox.Password != password)
                passBox.Password = password;
        }
    }

    public class PasswordBehavior : Behavior<PasswordBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            
            AssociatedObject.PasswordChanged += AssociatedObject_PasswordChanged;
        }

        private void AssociatedObject_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            string password = PassWordExtensions.GetPassWord(passwordBox);

            if (passwordBox != null && passwordBox.Password != password)
                PassWordExtensions.SetPassWord(passwordBox, passwordBox.Password);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PasswordChanged -= AssociatedObject_PasswordChanged;
        }
    }

}
