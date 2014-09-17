namespace Viking.Converters
{
    using Viking.Model;
    using System;
    using System.Globalization;
    using System.Net;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;
    public class PlayerColorConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty AttackerColorProperty =
                    DependencyProperty.Register("AttackerColor", typeof(SolidColorBrush), typeof(PlayerColorConverter), null);

        public static readonly DependencyProperty DefenderColorProperty =
                    DependencyProperty.Register("DefenderColor", typeof(SolidColorBrush), typeof(PlayerColorConverter), null);

        public static readonly DependencyProperty KingColorProperty =
                    DependencyProperty.Register("KingColor", typeof(SolidColorBrush), typeof(PlayerColorConverter), null);

        public SolidColorBrush AttackerColor
        {
            get { return (SolidColorBrush)GetValue(AttackerColorProperty); }
            set { SetValue(AttackerColorProperty, value); }
        }

        public SolidColorBrush DefenderColor
        {
            get { return (SolidColorBrush)GetValue(DefenderColorProperty); }
            set { SetValue(DefenderColorProperty, value); }
        }

        public SolidColorBrush KingColor
        {
            get { return (SolidColorBrush)GetValue(KingColorProperty); }
            set { SetValue(KingColorProperty, value); }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush retVal = new SolidColorBrush(Colors.Magenta);
            if (value is Piece)
            {
                Piece piece = value as Piece;
                if (piece != null)
                {
                    if (piece.Player == Player.Attacker)
                    {
                        retVal = AttackerColor;
                    }
                    else if (!piece.IsKing)
                    {
                        retVal = DefenderColor;
                    }
                    else
                    {
                        retVal = KingColor;
                    }
                }
            }
            else if (value != null)
            {
                string stringVal = value.ToString();
                if (stringVal == Player.Attacker.ToString())
                {
                    retVal = AttackerColor;
                }
                else if (stringVal == Player.Defender.ToString())
                {
                    retVal = DefenderColor;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Invalid player value passed to PlayerColorConverter: " + stringVal);
                }
            }

            return retVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class KingStrokeConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty StrokeThicknessProperty =
                    DependencyProperty.Register("StrokeThickness", typeof(double), typeof(KingStrokeConverter), null);

        public static readonly DependencyProperty StrokeProperty =
                    DependencyProperty.Register("Stroke", typeof(Brush), typeof(KingStrokeConverter), null);

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public Brush StrokeColor
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Piece piece = value as Piece;
            object retVal = null;
            if (targetType == typeof(double))
            {
                if (piece != null && piece.IsKing)
                {
                    retVal = StrokeThickness;
                }
                else
                {
                    retVal = 0;
                }
            }
            else
            {
                if (piece != null && piece.IsKing)
                {
                    retVal = StrokeColor;
                }
                else
                {
                    retVal = new SolidColorBrush(Colors.Transparent);
                }
            }

            return retVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
