using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace Responsive
{
    /// <summary>
    /// 响应式 Size
    /// </summary>
    public class ResponsiveSizeExtension : MarkupExtension
    {
        private static readonly Lazy<DoubleConverter> _lazyDouble = new();
        private static readonly Lazy<ThicknessConverter> _lazyThickness = new();
        private static readonly Lazy<CornerRadiusConverter> _lazyCornerRadius = new();

        private DoubleConverter _doubleConverter => _lazyDouble.Value;
        private ThicknessConverter _thickConvert => _lazyThickness.Value;
        private CornerRadiusConverter _cornerRadiusConvert => _lazyCornerRadius.Value;

        [ConstructorArgument("value")]
        public object Value { get; set; }

        public ResponsiveSizeExtension(object value)
        {
            if (value is string s && string.IsNullOrWhiteSpace(s)) value = "0";
            Value = value;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var target = (IProvideValueTarget)serviceProvider;
            var type = target switch
            {
                { TargetObject: Setter setter } => setter.Property.PropertyType,
                { TargetProperty: DependencyProperty dp } => dp.PropertyType,
                _ => throw new NotSupportedException($"不是 Setter 对象或者依赖属性")
            };

            TypeConverter converter = type switch
            {
                not null when type == typeof(double) => _doubleConverter,
                not null when type == typeof(Thickness) => _thickConvert,
                not null when type == typeof(CornerRadius) => _cornerRadiusConvert,
                _ => throw new NotSupportedException($"{type} 类型不支持")
            };

            var originValue = converter.ConvertFrom(Value) ?? throw new ArgumentException(nameof(Value));
            var newValue = Application.Current.ConvertForScreen(originValue);
            PrintLog(originValue, newValue);
            return newValue;
        }

        private void PrintLog(object originValue, object newValue) => Debug.WriteLine($"originValue: {originValue}, newValue: {newValue}, factor: {Application.Current.GetFactor()}");
    }
}
