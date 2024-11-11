using System.Drawing;
using System.Runtime.InteropServices;

namespace GraphicRedactor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public void draw8Pixels(int x, int y, int x0, int y0,Color color, Bitmap map)
        {
            map.SetPixel(x + x0, y + y0, color);
            map.SetPixel(x + x0, -y + y0, color);
            map.SetPixel(-x + x0, y + y0, color);
            map.SetPixel(-x + x0, -y + y0, color);
            map.SetPixel(y + x0, x + y0, color);
            map.SetPixel(y + x0, -x + y0, color);
            map.SetPixel(-y + x0, x + y0, color);
            map.SetPixel(-y + x0, -x + y0, color);
        }
        public void bresenharmCircle(int x0, int y0, int R, Bitmap map)
        {
            int x = 0; int y = R; int d = 3 - 2 * R;
            while (y >= x)
            {
                draw8Pixels(x, y, x0, y0, Color.Blue, map);
                if (d <= 0) d = d + 4 * x + 6;
                else
                {
                    d = d + 4 * (x - y) + 10;
                    y--;
                }
                x++;
            }
        }
        public void curveBesie(double[,] coordinates,double t, Bitmap map)
        {
            Graphics g = Graphics.FromImage(map);
            int m = coordinates.GetLength(0) - 1;
            for (double j=0; j<=1; j+= t)
            {
                double x = 0;
                double y = 0;
                for (int i = 0; i < coordinates.GetLength(0); i++)
                {                    
                    x += (cIM(m, i) * Math.Pow(j, i) * Math.Pow(1 - j, m - i) * coordinates[i, 0]);
                    y += (cIM(m, i) * Math.Pow(j, i) * Math.Pow(1 - j, m - i) * coordinates[i, 1]);               
                }
                map.SetPixel((int)x, (int)y, Color.Red);
               
            }  
        }
        public double cIM(int m, int i)
        {
            return (double)factorial(m) / (factorial(i) * factorial(m - i));
        }
        public int factorial(int m)
        {
            if (m == 1 || m == 0) return 1;
            else return m*factorial(m-1);
        }
        public void myCircle(int x0,int y0,int r1,int r2,int omega1,int omega2,Bitmap map)
        {
            int steps = 12000;           
            double[,] coordinates;
            int x1 = x0;
            int y1 = y0;
            bool isFirstPoint = true;
            for (int i = 0; i < steps; i++)
            {
                double t = (2 * Math.PI / steps) * i;               
                int x = (int)(x0 + r1 * Math.Cos(omega1 * t));
                int y = (int)(y0 + r2 * Math.Sin(omega2 * t));
                
                if (x >= 0 && x < map.Width && y >= 0 && y < map.Height)
                {
                    map.SetPixel(x,y,Color.Black);
                    coordinates = new double[,] { {x1, y1},{x,y } };
                    if(!isFirstPoint) curveBesie(coordinates, 0.01, map);
                    x1 = x; y1 = y;isFirstPoint = false;
                }
            }
        }

        //Не модифицированный аалгоритм затравки
        public void Zatravka_simple(int x, int y, Bitmap map, Color color, int k)
        {
            if (k < 50000)
            {
                Color backcolor = map.GetPixel(x, y);
                map.SetPixel(x, y, color);
                //закраска вверх
                if ((y + 1 <= map.Height) && (map.GetPixel(x, y + 1) == backcolor))
                {
                    k++;
                    Zatravka_simple(x, y + 1, map, color, k);
                }
                //закраска вправо
                if ((x + 1 <= map.Height) && (map.GetPixel(x + 1, y) == backcolor))
                {
                    k++;
                    Zatravka_simple(x + 1, y, map, color, k++);
                }
                //закраска вниз при условии того, что пиксел ниже не раскрашен
                if ((y - 1 >= 0) && (map.GetPixel(x, y - 1) == backcolor))
                {
                    k++;
                    Zatravka_simple(x, y - 1, map, color, k++);
                }
                //закраска влево при условии того, что пиксел левее не раскрашен
                if ((x - 1 >= 0) && (map.GetPixel(x - 1, y) == backcolor))
                {
                    k++;
                    Zatravka_simple(x - 1, y, map, color, k++);
                }
            }
        }

        public void fill(int x,int y,Bitmap map,Color color) 
        {
            Color backcolor = map.GetPixel(x,y);
            if (backcolor == color) throw new Exception("Цвет заливки и фона одинаковый!");
            int xLeft = x; int xRight = x+1;
            while (xRight < map.Width-1 && map.GetPixel(xRight, y) == backcolor)
            {
                map.SetPixel(xRight, y, color);
                xRight++;
            }
            xRight--;
            while (xLeft > 0&&map.GetPixel(xLeft,y)==backcolor) 
            {
                map.SetPixel(xLeft,y,color);
                xLeft--;
            }
            xLeft++;
            for(int i = xLeft; i < xRight; i++)
            {
                if (y > 0 && map.GetPixel(i, y - 1) == backcolor)
                {
                    fill(i,y-1,map,color);
                }
                if(y<map.Height-1&&map.GetPixel(i,y+1) == backcolor)
                {
                    fill(i,y+1,map,color);
                }
            }            
        }

        public void fill(int x ,int y, Color[,] pattern,Bitmap map)
        {
            Color backcolor = map.GetPixel(x, y);           
            int xLeft = x; int xRight = x + 1;
            int w=pattern.GetLength(0);
            int h=pattern.GetLength(1);
            while (xRight < map.Width - 1 && map.GetPixel(xRight, y) == backcolor)
            {
                map.SetPixel(xRight, y, pattern[xRight%w,y%h]);
                xRight++;
            }
            xRight--;
            while (xLeft > 0 && map.GetPixel(xLeft, y) == backcolor)
            {
                map.SetPixel(xLeft, y, pattern[xLeft % w, y % h]);
                xLeft--;
            }
            xLeft++;
            for (int i = xLeft; i <= xRight; i++)
            {
                if (y > 0 && map.GetPixel(i, y - 1) == backcolor)
                {
                    fill(i, y - 1,  pattern,map);
                }
                if (y < map.Height - 1 && map.GetPixel(i, y + 1) == backcolor)
                {
                    fill(i, y + 1, pattern, map);
                }
            }
        }
        public void fillZatravka(int x, int y, Bitmap map, Color color, int k)
        {
            if (k < 50000)
            {
                Color bgcolor = map.GetPixel(x, y);
                if (bgcolor == color) return; 

                map.SetPixel(x, y, color);

                // Закраска вверх
                if (y + 1 < map.Height && map.GetPixel(x, y + 1) == bgcolor)
                {
                    k++;
                    fillZatravka(x, y + 1, map, color, k++);
                }
                // Закраска вправо
                if (x + 1 < map.Width && map.GetPixel(x + 1, y) == bgcolor)
                {
                    k++;
                    fillZatravka(x + 1, y, map, color, k++);
                }
                // Закраска вниз
                if (y - 1 >= 0 && map.GetPixel(x, y - 1) == bgcolor)
                {
                    k++;
                    fillZatravka(x, y - 1, map, color, k++);
                }
                // Закраска влево
                if (x - 1 >= 0 && map.GetPixel(x - 1, y) == bgcolor)
                {
                    k++;
                    fillZatravka(x - 1, y, map, color, k++);
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap map = new Bitmap(800, 800);
            double[,] coordinates = new double[,] { { 200,200},{200,100 },{ 100,0},{0,0} };
            Color[,] pattern = new Color[,] { { Color.Black, Color.Red, Color.Black }, 
                                                { Color.Red, Color.Black, Color.Red }, 
                                                { Color.Black, Color.Red, Color.Black } };
            curveBesie(coordinates,0.001, map);
            myCircle(335, 225,50,75,90,60, map);
            bresenharmCircle(335, 225, 100, map);
            fill(335, 220, pattern,map);
            fillZatravka(335,226, map, Color.Yellow, 0);
            fill(400, 226, map, Color.BlueViolet);
            Zatravka_simple(350,226,map, Color.Gray,0);
            Zatravka_simple(320, 226, map, Color.Pink, 0);

            pictureBox1.Image = map;


        }
    }
}
