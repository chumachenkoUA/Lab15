using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;

namespace Lab15;

public partial class MainWindow : Window
{
    private enum LabTask { Task1, Task2, Task3, Task4 }

    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromMilliseconds(20) };
    
    private readonly ScaleTransform     _scaleT     = new();
    private readonly RotateTransform    _rotateT    = new();
    private readonly TranslateTransform _translateT = new();

    private LabTask _current = LabTask.Task1; 
    private double  _t = 0;
    private const double Dt  = 0.02; 

    public MainWindow()
    {
        InitializeComponent();
        
        var shape = this.FindControl<Path>("Shape")
                    ?? throw new ArgumentException("У XAML немає <Path x:Name=\"Shape\" …>");
        
        var group = shape.RenderTransform as TransformGroup ?? new TransformGroup();
        group.Children.Clear();                 
        group.Children.Add(_scaleT);
        group.Children.Add(_rotateT);
        group.Children.Add(_translateT);
        shape.RenderTransform = group;
        
        var selector = this.FindControl<ComboBox>("TaskSelector")
                      ?? throw new ArgumentException("У XAML немає <ComboBox x:Name=\"TaskSelector\" …>");
        selector.SelectionChanged += (_, __) =>
        {
            _current = (LabTask)selector.SelectedIndex;
            _t = 0;                 
            ResetTransforms();      
        };

        
        Opened += (_, __) => ResetTransforms();
        
        _timer.Tick += OnTick;
        _timer.Start();
    }
    
    private void ResetTransforms()
    {
        var size = (ClientSize.Width == 0 && ClientSize.Height == 0)
            ? new Size(800, 600)
            : ClientSize;

        _translateT.X = size.Width  / 2;
        _translateT.Y = size.Height / 2;
        _rotateT.Angle = 0;
        _scaleT.ScaleX = _scaleT.ScaleY = 1;
    }


    /* ---------- Формули варіант 20 ----------
   φ(t) = –4cost
   x(t) =  4cos(πt/6)
   y(t) = –2sin²(πt/6) */
    private void OnTick(object? sender, EventArgs e)
    {
        if (_current == LabTask.Task1) return;

        _t += Dt;
        
        var x =  4 * Math.Cos(Math.PI * _t / 6);
        var y = -2 * Math.Pow(Math.Sin(Math.PI * _t / 6), 2);

        var cx = ClientSize.Width  / 2;
        var cy = ClientSize.Height / 2;

        _translateT.X = cx + x * 40;
        _translateT.Y = cy - y * 40;

        switch (_current)
        {
            case LabTask.Task2:
                _rotateT.Angle = 0;
                _scaleT.ScaleX = _scaleT.ScaleY = 1;
                break;
            
            case LabTask.Task3:
                _rotateT.Angle = -4 * Math.Cos(_t) * 180 / Math.PI;
                _scaleT.ScaleX = _scaleT.ScaleY = 1;
                break;
            
            case LabTask.Task4:
                _rotateT.Angle = -4 * Math.Cos(_t) * 180 / Math.PI;

                var dx = _translateT.X;
                var dy = ClientSize.Height - _translateT.Y;
                var d  = Math.Sqrt(dx * dx + dy * dy);

                var scale = 4.0 / (1.0 + d / 400);
                _scaleT.ScaleX = _scaleT.ScaleY = scale;
                break;
        }
    }
}
