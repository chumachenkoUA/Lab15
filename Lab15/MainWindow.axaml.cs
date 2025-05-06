using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;

namespace Lab15;

public partial class MainWindow : Window
{
    /// <summary>Перелік режимів лабораторної (варіант 20).</summary>
    private enum LabTask { Task1, Task2, Task3, Task4 }

    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromMilliseconds(20) };

    // Керовані трансформації
    private readonly ScaleTransform     _scaleT     = new();
    private readonly RotateTransform    _rotateT    = new();
    private readonly TranslateTransform _translateT = new();

    private LabTask _current = LabTask.Task1;   // режим за замовчуванням
    private double  _t       = 0;
    private const double Dt  = 0.02;            // крок часу (с)

    public MainWindow()
    {
        InitializeComponent();

        /* ---------- Підготування фігури ---------- */
        // Path із x:Name="Shape" має бути у XAML
        var shape = this.FindControl<Path>("Shape")
                    ?? throw new ArgumentException("У XAML немає <Path x:Name=\"Shape\" …>");

        // Підміняємо (або створюємо) TransformGroup, щоб мати повний
        // контроль над масштабом, обертанням і позиціюванням.
        var group = shape.RenderTransform as TransformGroup ?? new TransformGroup();
        group.Children.Clear();                 // прибираємо зайве
        group.Children.Add(_scaleT);
        group.Children.Add(_rotateT);
        group.Children.Add(_translateT);
        shape.RenderTransform = group;

        /* ---------- Реакція на вибір завдання ---------- */
        var selector = this.FindControl<ComboBox>("TaskSelector")
                      ?? throw new ArgumentException("У XAML немає <ComboBox x:Name=\"TaskSelector\" …>");
        selector.SelectionChanged += (_, __) =>
        {
            _current = (LabTask)selector.SelectedIndex;
            _t = 0;                 // заново рахуємо час
            ResetTransforms();      // повертаємо фігуру у вихідне положення
        };

        /* ---------- Центруємо фігуру після відкриття вікна ---------- */
        Opened += (_, __) => ResetTransforms();

        /* ---------- Запускаємо анімацію ---------- */
        _timer.Tick += OnTick;
        _timer.Start();
    }

    /// <summary>Повертає трансформації до початкових значень (центр вікна, 0 ° і масштаб 1×).</summary>
    private void ResetTransforms()
    {
        // Якщо вікно ще не намальоване, ClientSize == (0,0)
        var size = (ClientSize.Width == 0 && ClientSize.Height == 0)
            ? new Size(800, 600)   // запасний розмір
            : ClientSize;

        _translateT.X = size.Width  / 2;
        _translateT.Y = size.Height / 2;
        _rotateT.Angle = 0;
        _scaleT.ScaleX = _scaleT.ScaleY = 1;
    }


    /* ---------- Формули варіанта 20 ----------
       φ(t) = –4 cos t
       x(t) =  4 cos(πt/6)
       y(t) = –2 sin²(πt/6)                                   */
    private void OnTick(object? _, EventArgs __)
    {
        if (_current == LabTask.Task1) return;   // статичний режим

        _t += Dt;

        /* ----- Рух ----- */
        var x =  4 * Math.Cos(Math.PI * _t / 6);
        var y = -2 * Math.Pow(Math.Sin(Math.PI * _t / 6), 2);

        var cx = ClientSize.Width  / 2;
        var cy = ClientSize.Height / 2;

        _translateT.X = cx + x * 30;   // 30 — коеф. видимого масштабу
        _translateT.Y = cy - y * 30;

        /* ----- Обертання ----- */
        if (_current >= LabTask.Task3)
        {
            var angleDeg = -4 * Math.Cos(_t) * 180 / Math.PI;
            _rotateT.Angle = angleDeg;
        }
        else
        {
            _rotateT.Angle = 0;
        }

        /* ----- Масштаб ----- */
        if (_current == LabTask.Task4)
        {
            var dx = _translateT.X;                       // відстань до лівого краю
            var dy = ClientSize.Height - _translateT.Y;   // відстань до низу
            var d  = Math.Sqrt(dx * dx + dy * dy);

            // Чим далі в кут, тим менша фігура
            var scale = 4.0 / (1.0 + d / 400);
            _scaleT.ScaleX = _scaleT.ScaleY = scale;
        }
        else
        {
            _scaleT.ScaleX = _scaleT.ScaleY = 1;
        }
    }
}
