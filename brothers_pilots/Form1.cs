using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;


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

        public Form1()
        {
            InitializeComponent();
        }

        public Button[,] levers;//Рычаги
        private void StartGame()
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
                        ImageKey ="0",
                        Location = new Point(i*size + xshift, j*size+ yshift),
                        Name = i.ToString() + ' ' + j.ToString(),
                        BackColor = Color.White,
                        //Image = Properties.Resources.horizontal,
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

        void finish()
        {
            win_sound.Play();
            MessageBox.Show("Да вы медвежатник,шеф","МОЕ УВАЖЕНИЕ");
            
            for (int i = 0; i < level; ++i)
                for (int j = 0; j < level; ++j)
                {
                    panel2.Controls.Remove(levers[i, j]);
                }
               
            if (MessageBox.Show("Повышаем ставки?", "Вопросы к победилю", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                level += 1;
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


        //Размер рычагов для вашего жкрана
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

        // Функция расчета размеров экрана и размеров рычагов
        private void configuration()
        {
            this.WindowState = FormWindowState.Maximized;
            //this.TopMost = true;

            panel1.Width = (int)(this.Width * 0.9);
            panel2.Width = (int)(this.Width * 0.9);
            panel1.Height = (int)(this.Height * 0.1);
            panel2.Height = (int)(this.Height * 0.89);

            label1.Text = $"Уровень {level - 1}";
            label1.Left = panel1.Width / 2 - 50;


            size = sizecube(panel2.Height, panel2.Width);

            //не лучшая моя идея
            xshift = (panel2.Width - level * size) / 2;
            yshift = (panel2.Height - level * size) / 2;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            configuration();
            StartGame();
        }
       

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < level; ++i)
                for (int j = 0; j < level; ++j)
                {
                    panel2.Controls.Remove(levers[i, j]);
                }
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
