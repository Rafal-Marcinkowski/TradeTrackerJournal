using System.Windows;

namespace SharedProject.Helpers
{
    public class MenuProvider : Freezable
    {
        public static readonly DependencyProperty DataContextProperty =
            DependencyProperty.Register("DataContext", typeof(object), typeof(MenuProvider));

        public object DataContext
        {
            get => GetValue(DataContextProperty);
            set => SetValue(DataContextProperty, value);
        }

        protected override Freezable CreateInstanceCore() => new MenuProvider();
    }
}
