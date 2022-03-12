WPF 客户端经常需要运行在各种不同大小屏幕下，为了显示友好，所以开发的时候都需要考虑响应式设计。  
布局往往通过指定比例，而不直接指定准确的大小来实现响应式布局（如 Width="3*" ），但是具体控件的大小（如 Thickness、CornerRadius）就没有开箱即用的响应式功能了，用 viewbox 来包装，比例就跟设计稿不一样了，看起来很怪。  
嗐，所以又只能自己开发了。这次的目标就是实现跟 css @media 类似的功能。
## 实现目标
- 设计稿都是 1920 × 1080 实现的，在 3840 × 2160 下，应该将所有控件的大小，边框放大两倍。
- 要考虑用户在系统下设定的缩放比例。
- 同事用起来要舒服，要支持热重载。

## 实现逻辑
### 根据屏幕大小和屏幕缩放比例来计算缩放系数。
- 屏幕的 api 当然是白嫖别人写的库啦，我这里用的是 WpfScreenHelper。
``` c#
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
```
### 自定义 xaml 标记
- 适配 Setter 上赋值和直接在控件上赋值的场景。
- 通过各种转换器来转换值。
``` c#
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
```
### 使用
``` xml
<Window
    ...
    xmlns:local="clr-namespace:Responsive" >
    <Window.Resources>
        <Style TargetType="TextBlock">
            <Setter Property="FontSize" Value="{local:ResponsiveSize 19}" />
        </Style>
    </Window.Resources>

    <Border
        Margin="50"
        BorderBrush="LightSeaGreen"
        BorderThickness="{local:ResponsiveSize 12}">
        <Grid Width="{local:ResponsiveSize 200}" Background="LightBlue">
            <TextBlock Text="Test" />
        </Grid>
    </Border>
</Window>
```
### 效果
``` c# 

屏幕大小: 1920 × 1080
屏幕缩放: 100%
originValue: 12,12,12,12, newValue: 12,12,12,12, factor: 1
originValue: 200, newValue: 200, factor: 1
originValue: 16, newValue: 16, factor: 1

屏幕大小: 3840 × 2160
屏幕缩放: 100%
originValue: 12,12,12,12, newValue: 24,24,24,24, factor: 0.5
originValue: 200, newValue: 400, factor: 0.5
originValue: 16, newValue: 32, factor: 0.5

屏幕大小: 3840 × 2160
屏幕缩放: 150%
originValue: 12,12,12,12, newValue: 16,16,16,16, factor: 0.75
originValue: 200, newValue: 266.6666666666667, factor: 0.75
originValue: 16, newValue: 21.333333333333332, factor: 0.75

```

### 最后
#### 响应式设计在 WPF 应该还有很多实现方法，有方便的思路请留言教教我！
#### 用模式匹配写逻辑是真舒服，建议大家都试试，代码简单又明了。
#### 觉得对你有帮助点个推荐或者留言交流一下呗！
#### 源码