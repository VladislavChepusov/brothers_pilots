using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;
using Microsoft.Win32;

namespace brothers_pilots
{
    public partial class Form1 : Form
    {
        private int level = 2;//Сложность игры
        private int size = 0;//размер отдельного рычага сейфа
        private int xshift = 0;// Сдвиг кубок от углов панеле
        private int yshift = 0;// Сдвиг кубок от углов панеле
        SoundPlayer win_sound = new SoundPlayer(Properties.Resources.win);// Звук победы
        SoundPlayer bye_sound = new SoundPlayer(Properties.Resources.bye);// Грустный тромбон
        public Button[,] levers;//Рычаги

        private Point loc;
        private int x;
        private int y;
        public Form1()
        {
            InitializeComponent();
            loc = this.Location;
            x = this.Width;
            y = this.Height;
        }


        // Функция расчета размеров экрана и размеров рычагов
        private void configuration()
        {
            this.WindowState = FormWindowState.Maximized;
            //this.TopMost = true;
            //panel1.Width = (int)(this.Width * 0.9);
            //panel2.Width = (int)(this.Width * 0.9);
            panel1.Width = (int)(this.Width);
            panel1.Height = (int)(this.Height * 0.1);
            panel2.Width = (int)(this.Width);
            panel2.Height = (int)(this.Height * 0.89);
            label1.Text = $"Уровень {level - 1}";
            label1.Left = this.Width / 2 - label1.Width;
            size = sizecube(panel2.Height, panel2.Width);
            xshift = (panel2.Width - level * size) / 2;
            yshift = (panel2.Height - level * size) / 2;
        }


        //Загрузка формы
        private void Form1_Load(object sender, EventArgs e)
        {
            configuration();
            StartGame();
        }


        //Отрисовка рычагов и их запутывание
        private void StartGame()
        {
            try
            {
                levers = new Button[level, level]; // Задаем массив кнопок 
                int numberMix;
                int x, y;
                Random rand = new Random();
                for (int i = 0; i < level; i++)
                {
                    for (int j = 0; j < level; j++)
                    {
                        levers[i, j] = new Button
                        {
                            Size = new Size(size, size),
                            ImageKey = "0",
                            Location = new Point(i * size + (xshift), j * size + yshift),
                            Name = i.ToString() + ' ' + j.ToString(),
                            BackColor = Color.FromArgb(240, 240, 240),
                            BackgroundImage = Properties.Resources.horizontal,
                            BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom,

                        };
                        levers[i, j].Click += new System.EventHandler(button_Click);
                        panel2.Controls.Add(levers[i, j]);
                    }
                }
                //Пермеешивание 
                do
                {
                    numberMix = rand.Next(level, level + 20);
                    for (int num = 0; num < numberMix; num++)
                    {
                        x = rand.Next(0, level);
                        for (int j = 0; j < level; ++j)
                            revers(levers[x, j]);
                        y = rand.Next(0, level);
                        for (int i = 0; i < level; ++i)
                            revers(levers[i, y]);
                        revers(levers[x, y]);
                    }
                } while (CheckWin());
            }
            catch
            {
                MessageBox.Show("Ваша машина устала...", "Ошибка");
                panel2.Controls.Clear();
                level = 2;
                System.Threading.Thread.Sleep(700);
                configuration();
                StartGame();
            }
        }


        //Нажатие на рычаг
        private void button_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            string[] strKoord = btn.Name.Split(' ');
            int x = Convert.ToInt32(strKoord[0]);
            int y = Convert.ToInt32(strKoord[1]);

            for (int j = 0; j < level; j++)
                revers(levers[x, j]);
            for (int i = 0; i < level; i++)
                revers(levers[i, y]);
            revers(levers[x, y]);

            if (CheckWin())
                finish();
        }


        // Переход на следующий раунд
        void finish()
        {
            win_sound.Play();
            MessageBox.Show("Да вы медвежатник,шеф", "МОЕ УВАЖЕНИЕ");
            /*
            for (int i = 0; i < level; ++i)
                for (int j = 0; j < level; ++j)
                {
                    panel2.Controls.Remove(levers[i, j]);
                }
            */
            panel2.Controls.Clear();
            level += 1;
            if (endGame())
            {
                MessageBox.Show("Поздравляю с победой!", "Конец игры");
                if (MessageBox.Show("Начать с начала?", "Вопросы к победилю", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Application.Restart();
                }
                else
                {
                    bye_sound.Play();
                    MessageBox.Show("Всего хорошего!", "До новых встреч!");
                    Application.Exit();
                }
            }
            else
            {
                if (MessageBox.Show("Повышаем ставки?", "Вопросы к победилю", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    numericUpDown1.Value = level;
                    configuration();
                    StartGame();
                }
                else
                {
                    bye_sound.Play();
                    MessageBox.Show("Всего хорошего!", "До новых встреч!");
                    Application.Exit();
                }
            }
        }


        // Проверка победы
        bool CheckWin()
        {
            int up = 0;
            int down = 0;
            for (int i = 0; i < level; i++)
                for (int j = 0; j < level; j++)
                {
                    if (levers[i, j].ImageKey == "0")
                        down++;
                    else up++;
                }
            if (down == level * level || up == level * level)
                return true;
            return false;
        }


        //Размер рычагов для вашего экрана
        private int sizecube(int mHeight, int mWidth)
        {
            return Math.Min(mHeight / level, mWidth / level);
        }


        //Переключение рычагов
        void revers(Button button)
        {
            if (button.ImageKey == "0")
            {
                button.ImageKey = "1";
                //button.Image = Properties.Resources.vertical;
                button.BackgroundImage = Properties.Resources.vertical;
            }
            else
            {
                button.ImageKey = "0";
                button.BackgroundImage = Properties.Resources.horizontal;
            }
        }


        // Изменение размерности (читерство)
        private void button1_Click(object sender, EventArgs e)
        {
            /*
            for (int i = 0; i < level; ++i)
                for (int j = 0; j < level; ++j)
                {
                    panel2.Controls.Remove(levers[i, j]);
                }
            */
            panel2.Controls.Clear();
            int nl;
            if (int.TryParse(numericUpDown1.Value.ToString(), out nl))
                level = nl;
            else
            {
                MessageBox.Show("Неправльный ввод размерности...");
                level = 2;
            }
            configuration();
            StartGame();
        }


        //Конец игры
        private bool endGame()
        {
            if (level >= 15)
                return true;
            else
                return false;
        }

        
        private void normal()
        {
            if (this.Location != loc)
                this.Location = loc;
            
            if (this.Width != x)
                this.Width = x;
            
            if (this.Height != y)
                this.Height = y;

            if (this.WindowState != FormWindowState.Maximized)
                this.WindowState = FormWindowState.Maximized;
        }


        private void Form1_Resize(object sender, EventArgs e)
        {
            normal();
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            normal();
        }


     

        /*
          private int sizecube(int mHeight, int mWidth )
          {
              // int(sqrt(высота_прямоугольника*ширина_прямоугольника/количество_квадратов))
              int MaxiSize = (int)(Math.Sqrt(mHeight * mWidth / level));
              MessageBox.Show("Максимальная сторона квадрата =" + MaxiSize.ToString());
              var numbers = new List<int>();
              for (int i = 1; i <= level; i++)
              {
                  int W = (mWidth / i);
                  int H = (mHeight / i);
                  if (W <= MaxiSize && !numbers.Contains(W))
                  {
                      numbers.Add(W);
                  }
                   if (H <= MaxiSize && !numbers.Contains(H))
                  {
                      numbers.Add(H);
                  }
                  numbers.Sort();
                  numbers.Reverse();
              }
              int q = 0;
              MaxiSize = numbers[q];
              int cube_count = (int)(mHeight / MaxiSize) + (int)(mWidth / MaxiSize);
              while (cube_count < level)
              {
                  q += 1;
                  MaxiSize = numbers[q];
                  cube_count = (int)(mHeight / MaxiSize) + (int)(mWidth / MaxiSize);
              }
              return MaxiSize;
          }
       */
    }
}