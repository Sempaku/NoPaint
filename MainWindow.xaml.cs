using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lab2Sem2_NoPaint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            mcolor = new ColorRGB();
            mcolor.red = 0;
            mcolor.green = 0;
            mcolor.blue = 0;

            curColor.Background = new SolidColorBrush(Color.FromRgb(mcolor.red, mcolor.green, mcolor.blue));

            var color_query =
            from PropertyInfo property in typeof(Colors).GetProperties()
            orderby property.Name
            //orderby ((Color)property.GetValue(null, null)).ToString()
            select new ColorInfo(
            property.Name,
            (Color)property.GetValue(null, null));

            //cmbColors.ItemsSource = color_query;
        }
        // Used to display color name, RGB value, and sample.
        public class ColorInfo
        {
            public string ColorName { get; set; }
            public Color Color { get; set; }

            public SolidColorBrush SampleBrush
            {
                get { return new SolidColorBrush(Color); }
            }
            public string HexValue
            {
                get { return Color.ToString(); }
            }

            public ColorInfo(string color_name, Color color)
            {
                ColorName = color_name;
                Color = color;
            }
        }
        private void cmbColors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string text_color = (string)Application.Current.TryFindResource("hxvalue");

            MessageBox.Show(text_color);
            //mainCanvas.DefaultDrawingAttributes.Color = cmdColor;

        }
        public class ColorRGB
        {
            public byte red { get; set; }
            public byte green { get; set; }
            public byte blue { get; set; }

        }
        public ColorRGB mcolor { get; set; }

        public Color clr { get; set; }

        private void Click_New(object sender, RoutedEventArgs e)
        {
            mainCanvas.Strokes.Clear();
        }

        private void Click_Open(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openimgdlg = new OpenFileDialog();
            //openimgdlg.DefaultExt = ".PNG";
            //openimgdlg.Filter = "Image (.PNG)|*.PNG";

            if(openimgdlg.ShowDialog() == true)
            {
                ImageBrush brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri(openimgdlg.FileName, UriKind.Relative));
                mainCanvas.Background = brush;
            }

        }
        
        private void Click_Exit(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Click_Save(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveimg = new SaveFileDialog();
            saveimg.DefaultExt = ".PNG";
            saveimg.Filter = "Image (.PNG)|*.PNG";
            if (saveimg.ShowDialog() == true)
            {
                ToImageSource(mainCanvas, saveimg.FileName);  
            }
        }
        public static void ToCanvasSourse(Image img, string filename)
        {
            
        }
        public static void ToImageSource(InkCanvas canvas, string filename) //Преобразуем из InkCanvas в Image 
        {
            RenderTargetBitmap rendertargetbitmap = new RenderTargetBitmap((int)canvas.ActualWidth, (int)canvas.ActualHeight, 96d, 96d, PixelFormats.Pbgra32);
            canvas.Measure(new Size((int)canvas.ActualWidth, (int)canvas.ActualHeight));
            canvas.Arrange(new Rect(new Size((int)canvas.ActualWidth, (int)canvas.ActualHeight)));
            rendertargetbitmap.Render(canvas);
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rendertargetbitmap));
            FileStream file = File.Create(filename);
            encoder.Save(file);
        }
        #region Sliders
        private void sld_Color_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = sender as Slider;
            string crlName = slider.Name; //Определяем имя контрола, который покрутили
            double value = slider.Value; // Получаем значение контрола
                                         //В зависимости от выбранного контрола, меняем ту или иную компоненту и переводим ее в тип byte
            if (crlName.Equals("sldrRed"))
            {
                mcolor.red = Convert.ToByte(value);
            }
            if (crlName.Equals("sldrGreen"))
            {
                mcolor.green = Convert.ToByte(value);
            }
            if (crlName.Equals("sldrBlue"))
            {
                mcolor.blue = Convert.ToByte(value);
            }

            //Задаем значение переменной класса clr 
            clr = Color.FromRgb(mcolor.red, mcolor.green, mcolor.blue);
            //Устанавливаем фон для контрола Label 
            curColor.Background = new SolidColorBrush(Color.FromRgb(mcolor.red, mcolor.green, mcolor.blue));
            // Задаем цвет кисти для контрола InkCanvas
            mainCanvas.DefaultDrawingAttributes.Color = clr;
        }
        #endregion

        #region Menu Edit
        private void mnEditSelect_Click(object sender, RoutedEventArgs e)
        {
            mainCanvas.EditingMode = InkCanvasEditingMode.Select;
        }

        private void mnEditPresentation_Click(object sender, RoutedEventArgs e)
        {
            mainCanvas.EditingMode = InkCanvasEditingMode.GestureOnly;
        }

        private void mnEditClearStroke_Click(object sender, RoutedEventArgs e)
        {
            mainCanvas.EditingMode = InkCanvasEditingMode.EraseByStroke;
        }

        private void mnEditClearPoint_Click(object sender, RoutedEventArgs e)
        {
            mainCanvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
        }

        private void mnEditPaint_Click(object sender, RoutedEventArgs e)
        {
            mainCanvas.EditingMode = InkCanvasEditingMode.Ink;

        }


        #endregion

        #region Add Text Events
        private void mnAddText_Click(object sender, RoutedEventArgs e)
        {
            TextBox textbox = new TextBox
            { 
                Width = 100,
                Height = 50,
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Color.FromRgb(250, 250, 250)),
                Margin = new Thickness(20, 20, 0, 0)
            };
            //Добавление контрола tb
            this.mainCanvas.Children.Add(textbox);
            //Переключение фокуса на элемент, чтоб можно было сразу ввести текст с клавиатуры
            textbox.Focus();

        }


        #endregion

        
    }

}
