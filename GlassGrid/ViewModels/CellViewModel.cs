using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Input;
using System.Windows.Media;
using GlassGrid.Models.Enums;
using GlassGrid.ViewModels;



public class CellViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    private SolidColorBrush _filterBrush;
    private int _number;
    private Color _color;
    private Color _filtercolor;
    private bool _filterColorEnabled = true;
    private List<Color> _colors = new List<Color>
    {
        Colors.Blue,
        Colors.Green,
        Colors.Red,
        Colors.Yellow,
    };


    public bool FilterColorEnabled
    {
        get => _filterColorEnabled;
        set
        {
            if (_filterColorEnabled != value)
            {
                _filterColorEnabled = value;
                OnPropertyChanged(nameof(FilterColorEnabled)); 
                OnPropertyChanged(nameof(ColorBrush));      
            }
        }


    }


    public SolidColorBrush FilterColor
    {
        get => _filterBrush;
        private set
        {
            
                _filterBrush = value;
                OnPropertyChanged(nameof(FilterColor));
            
        }
    }



    public Color FilterColorSetter
    {
        set
        {
            
                _filtercolor = value;
                FilterColor = new SolidColorBrush(_filtercolor); // ← to kluczowe
                OnPropertyChanged(nameof(FilterColorSetter));
                OnPropertyChanged(nameof(ColorBrush));
            
        }
    }

    public int Number
    {
        get => _number;
        set
        {
            if (_number != value)
            {
                _number = value;
                OnPropertyChanged(nameof(Number));
            }
        }
    }

    public Color Color
    {
        get => _color;
        set
        {
            if (_color != value)
            {
                _color = value;
                OnPropertyChanged(nameof(Color));
                OnPropertyChanged(nameof(ColorBrush));
            }
        }
    }


    public System.Windows.Media.SolidColorBrush ColorBrush
    {
        get
        {
            if (FilterColorEnabled)
            {
                return new System.Windows.Media.SolidColorBrush(
                    ConvertToScreen(_color
                    ));
            }
            else
            {
                return new System.Windows.Media.SolidColorBrush(Color);

            }
        }
    }


    // Wynik=Screen(A,B)=1−(1−Blend)(1−Base)
    // Blend - _filtercolor
    // Base - color

    public Color ConvertToScreen(Color color)
    {
        byte R = InvertChannel( MultiplyChannel(InvertChannel(_filtercolor.R), InvertChannel(color.R)));
        byte G = InvertChannel(MultiplyChannel(InvertChannel(_filtercolor.G), InvertChannel(color.G)));
        byte B = InvertChannel(MultiplyChannel(InvertChannel(_filtercolor.B), InvertChannel(color.B)));
        return System.Windows.Media.Color.FromRgb(R, G, B);
    }

    public byte InvertChannel(byte value1)
    {
        return (byte)(255 - value1);
    }

    public byte MultiplyChannel(byte a, byte b)
    {
      
        return (byte)((a * b) / 255);
    }




    public int IndexInGrid { get; }

   
    public CellViewModel(int number, Color color, int indexInGrid)
    {
        _number = number;
        _color = color;
        IndexInGrid = indexInGrid;

        _filtercolor = _colors[0];
        _filterBrush = new SolidColorBrush(_filtercolor);

      
    }

    public void ChangeFilterColor()
    {
        int currentIndex = _colors.IndexOf(_filtercolor);
        int nextIndex = (currentIndex + 1) % _colors.Count;
        FilterColorSetter = _colors[nextIndex];
        OnPropertyChanged(nameof(FilterColor));
        OnPropertyChanged(nameof(ColorBrush));

    }



    protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
