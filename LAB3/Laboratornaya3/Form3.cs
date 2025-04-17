using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Laboratornaya3
{
    public partial class Form3 : Form
    {
        private List<Image> deck = new List<Image>(); // вся колода
        private List<PlacedCard> placedCards = new List<PlacedCard>();

        private Rectangle draggingCardBounds;
        private bool isDragging = false;
        private Image draggingCardImage;
        private int cardIndex = -1;
        private List<string> cardNames = new List<string>
        {
            "6c", "6d", "6h", "6s",
            "7c", "7d", "7h", "7s",
            "8c", "8d", "8h", "8s",
            "9c", "9d", "9h", "9s",
            "10c", "10d", "10h", "10s",
            "Jc", "Jd", "Jh", "Js",
            "Qc", "Qd", "Qh", "Qs",
            "Kc", "Kd", "Kh", "Ks",
            "Ac", "Ad", "Ah", "As",
            "Joker1", "Joker2"
        };
        private List<Image> faceImages = new List<Image>();

        // класс для размещённой карты на столе
        private class PlacedCard
        {
            public Image Image { get; set; } // изображение карты
            public Point Position { get; set; } // позиция карты на экране
            public float Angle { get; set; } // угол поворота карты
        }

        // конструктор формы
        public Form3()
        {
            InitializeComponent();
            this.DoubleBuffered = true; // включаем двойную буферизацию

            // инициализируем колоду карт (включая рубашки)
            for (int i = 0; i < 38; i++)
            {
                deck.Add(Properties.Resources.back); // добавляем рубашку карты
                string name = cardNames[i];
                Image img = (Image)Properties.Resources.ResourceManager.GetObject("_" + name); // получаем изображение карты
                faceImages.Add(img);
            }
        }

        private void Form3_Load(object sender, EventArgs e)
        {
                
        }

        // обработчик события закрытия формы
        private void Form3_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit(); // выходим из приложения
        }

        // для рисования градиентного фона
        private void gradientBack(Graphics g)
        {
            Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
            using (LinearGradientBrush brush = new LinearGradientBrush(rect, Color.DarkGreen, Color.LightGreen, LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, rect); // заполняем фон градиентом
            }
        }

        // для рисования всех элементов на форме
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            gradientBack(e.Graphics); // рисуем фон

            // рисуем размещённые карты на столе с учётом поворота
            foreach (var placed in placedCards)
            {
                if (placed.Image != null)
                {
                    GraphicsState gstate = e.Graphics.Save(); // сохраняем состояние графического контекста

                    e.Graphics.TranslateTransform(placed.Position.X + 75, placed.Position.Y + 90); // сдвигаем координаты для поворота

                    e.Graphics.RotateTransform(placed.Angle); // поворачиваем на заданный угол

                    e.Graphics.DrawImage(placed.Image, new Rectangle(-75, -90, 150, 180)); // рисуем карту с учётом поворота

                    e.Graphics.Restore(gstate); // восстанавливаем состояние графического контекста
                }
            }

            // рисуем оставшиеся карты в колоде
            int x = 550;
            int y = 80;
            for (int i = 0; i < deck.Count; i++)
            {
                if (i == cardIndex && isDragging) continue; // не рисуем перетаскиваемую карту
                e.Graphics.DrawImage(deck[i], new Rectangle(x, y, 150, 180));
                x += 1;
                y -= 1;
            }

            // рисуем перетаскиваемую карту поверх всех остальных
            if (isDragging && draggingCardImage != null)
            {
                e.Graphics.DrawImage(draggingCardImage, new Rectangle(draggingCardBounds.X, draggingCardBounds.Y, 150, 180));
            }
        }

        // обработчик события нажатия кнопки мыши
        private void Form3_MouseDown(object sender, MouseEventArgs e)
        {
            if (deck.Count > 0)
            {
                // проверяем, была ли нажата верхняя карта колоды
                Rectangle topCardRect = new Rectangle(550 + deck.Count - 1, 80 - (deck.Count - 1), 150, 180);
                if (topCardRect.Contains(e.Location))
                {
                    isDragging = true;
                    cardIndex = deck.Count - 1;
                    draggingCardImage = deck[cardIndex];
                    draggingCardBounds = new Rectangle(e.X - 75, e.Y - 90, 150, 180); // центрируем карту под курсором
                    Invalidate();
                }
            }
        }

        // обработчик события перемещения мыши
        private void Form3_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                draggingCardBounds.Location = new Point(e.X - 75, e.Y - 90); // следим за положением карты
                Invalidate(); // перерисовываем форму
            }
        }

        // обработчик события отпускания кнопки мыши
        private void Form3_MouseUp(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Random rnd = new Random();
                int randomIndex = rnd.Next(faceImages.Count); // выбираем случайную лицевую карту
                Image face = faceImages[randomIndex];

                float randomAngle = rnd.Next(-30, 30); // генерируем случайный угол поворота

                placedCards.Add(new PlacedCard
                {
                    Image = face,
                    Position = draggingCardBounds.Location,
                    Angle = randomAngle
                });

                // удаляем карту из колоды
                if (cardIndex >= 0 && cardIndex < deck.Count)
                    deck.RemoveAt(cardIndex);

                isDragging = false; // сбрасываем состояние перетаскивания
                draggingCardImage = null;
                cardIndex = -1;
                Invalidate(); // перерисовываем форму
            }
        }

        // обработчик нажатия клавиш на клавиатуре
        private void Form3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2) // если нажата клавиша F2
            {
                placedCards.Clear(); // очищаем все размещённые карты

                // перемещаем все карты обратно в колоду
                deck.Clear();
                for (int i = 0; i < 36; i++)
                {
                    deck.Add(Properties.Resources.back); // добавляем рубашку карты
                }

                Invalidate(); // перерисовываем форму
            }
        }
    }
}
