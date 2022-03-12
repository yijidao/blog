using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using WpfScreenHelper;

namespace Responsive
{
    public static class AppExtension
    {
        private static double? _factor;

        /// <summary>
        /// 获取当前应用的缩放系数
        /// 如果 4K 屏幕，需要放大 2 倍
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static double GetFactor(this Application app)
        {
            if (_factor is not null) return _factor.Value;

            var screen = app.MainWindow?.GetScreen() ?? throw new ArgumentNullException(nameof(app.MainWindow));
            _factor = screen.PixelBounds switch
            {
                { Width: >= 3840, Height: >= 2160 } => screen.ScaleFactor / 2,
                _ => screen.ScaleFactor
            };

            Debug.WriteLine($"屏幕大小: {screen.PixelBounds.Width} × {screen.PixelBounds.Height}");
            Debug.WriteLine($"屏幕缩放: {screen.ScaleFactor * 100}%");

            return _factor.Value;
        }

        /// <summary>
        /// 根据屏幕大小和缩放系统，转换不同的数据类型
        /// </summary>
        /// <param name="app"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static object ConvertForScreen(this Application app, object o) =>
            o switch
            {
                double d => app.ConvertDoubleForScreen(d),
                Thickness t => app.ConvertThicknessForScreen(t),
                CornerRadius t => app.ConvertCornerRadiusForScreen(t),
                _ => throw new NotSupportedException("不支持的转换类型")
            };


        public static double ConvertDoubleForScreen(this Application app, double value)
        {
            var factor = app.GetFactor();
            return value / factor;
        }

        public static Thickness ConvertThicknessForScreen(this Application app, Thickness value)
        {
            var factor = app.GetFactor();
            return new Thickness(value.Left / factor, value.Top / factor, value.Right / factor, value.Bottom / factor);
        }

        

        public static CornerRadius ConvertCornerRadiusForScreen(this Application app, CornerRadius value)
        {
            var factor = app.GetFactor();
            return new CornerRadius(value.TopLeft / factor, value.TopRight / factor, value.BottomRight / factor,
                value.BottomLeft / factor);
        }

        

        /// <summary>
        /// 获取当前窗体所在的屏幕
        /// </summary>
        /// <param name="window">当前窗体</param>
        /// <returns>窗体所在的屏幕</returns>
        public static Screen GetScreen(this Window window)
        {
            var intPtr = new WindowInteropHelper(window).Handle; //获取当前窗口的句柄
            return Screen.FromHandle(intPtr); //获取当前屏幕
        }

        


    }
}
