using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static UFO.Enums_Structs;

namespace UFO {
    /// <summary> Статический класс. Содержит кастомные контролы. </summary>
    public static class CustomControls {
        /// <summary> Метод рисует на контроле пунктирную/сплошную и прочую рамку. </summary>
        /// <value>
        ///     <b> <paramref name="e"/>: </b> представляет данные и холст для рисования на поверхности. <br/>
        ///     <b> <paramref name="Cl"/>: </b> цвет линии. <br/>
        ///     <b> <paramref name="ts"/>: </b> толщина линии. <br/>
        ///     <b> <paramref name="sz_line"/>: </b> длина видимой части линии. <br/>
        ///     <b> <paramref name="sz_space"/>: </b> длина отступа между пунктирами. При sz_space = 0, линия будет сплошной. <br/>
        ///     <b> <paramref name="Ctrl"/>: </b> контрол на котором происходит рисование. <br/>
        /// </value>
        public static void DottedLine(PaintEventArgs e, Color Cl, float ts, float sz_line, float sz_space, Control Ctrl) {
            Pen pen = new Pen(Cl, ts) { DashPattern = new float[] { sz_line, sz_space }, };
            e.Graphics.DrawRectangle(pen, ts / 2, ts / 2, Ctrl.Width - ts, Ctrl.Height - ts);
        }

        /// <summary> Представляет элемент управления: "круглая/овальная кнопка". <br/> Отрисовка кнопки происходит за счёт класса <b>GraphicsPath</b>. </summary>
        /// <remarks> Если контрол не создан программно, а перенесён на форму из палитры контролов, то чтобы модификация работала, <br/> следует в дизайнере формы переписать создание экземпляра класса с <b>Button</b> на <b>CircleButton</b> <br/> в двух местах (в начале и в конце) файла Form.Designer.cs. </remarks>
        public class CircleButton : Button {
            protected override void OnPaint(PaintEventArgs e) {
                GraphicsPath grPath = new GraphicsPath();
                grPath.AddEllipse(0, 0, ClientSize.Width, ClientSize.Height);
                Region = new Region(grPath);
                base.OnPaint(e);
            }
        }

        /// <summary> Представляет элемент управления: "кнопка со скруглёнными углами". <br/> Отрисовка кнопки происходит за счёт API windows. </summary>
        /// <remarks> Если контрол не создан программно, а перенесён на форму из палитры контролов, то чтобы модификация работала, <br/> следует в дизайнере формы переписать создание экземпляра класса с <b>Button</b> на <b>RoundAPIButton</b> <br/> в двух местах (в начале и в конце) файла Form.Designer.cs. </remarks>
        public class RoundAPIButton : Button {
            [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
            [System.Reflection.Obfuscation(Exclude = true)]
            static extern IntPtr RoundButton(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
                                             int nWidthEllipse, int nHeightEllipse);

            /// <summary> Размер скругления в пикселях. По умолчанию без скругления. </summary>
            int Ellipse_px = 0;
            /// <summary> Размер скругления в процентах. По умолчанию без скругления. </summary>
            int ellipse_percent = 0;
            /// <summary> <inheritdoc cref="ellipse_percent"/> </summary>
            public int Ellipse_Percent {
                get { return ellipse_percent; }
                set { //вычисление знач. Ellipse_px на основе % value
                    if (value < 0) value = 0; else if (value > 100) value = 100;
                    ellipse_percent = value; int min;
                    if (Width < Height) min = Width; else min = Height;
                    Ellipse_px = (int)(((min * 2) / 100.0) * value);
                }
            }

            protected override void OnPaint(PaintEventArgs e) {
                Ellipse_Percent = Ellipse_Percent;//перевычисление знач. Ellipse_px под свежие размеры кнопки
                IntPtr ptr = RoundButton(0, 0, Width, Height, Ellipse_px, Ellipse_px);
                Region = Region.FromHrgn(ptr);
                base.OnPaint(e);
            }
        }
    }
}
