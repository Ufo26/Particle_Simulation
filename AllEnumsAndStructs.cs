using SkiaSharp;
using System;
using System.Collections.Generic;
using static System.Math;

namespace UFO {
    public class Enums_Structs {
        //================================================ ENUM ================================================
        /// <summary> Перечисление сложных типов ячеек таблицы <b>DataGridView</b>. </summary>
        public enum TypeCellGrid { TextBoxCell, ImageCell, ComboBoxCell, CheckBoxCell, ButtonCell, LinkCell }
        
        /// <summary> Перечисление алгоритмов проверки столкновения при движении частицы. </summary>
        public enum MoveCheck { MoveCheck, MoveCheckGrid, MoveCheckGridList }
        
        /// <summary> Перечисление режимов отрисовки. </summary>
        public enum DrawMode {
            Default, Grid, TestGrid, OneCellOneCircle
        }

        /// <summary> Перечисление способов демаркации частиц при перекрытии радиусов окружностей: <br/>
        /// <b>Left</b>: ЛЕВАЯ частица отодвигается от ПРАВОЙ, для разрешения коллизии; <br/>
        /// <b>Right</b>: ПРАВАЯ частица отодвигается от ЛЕВОЙ, для разрешения коллизии; <br/>
        /// <b>LeftRight</b>: ЛЕВАЯ и ПРАВАЯ частицы отодвигается друг от друга, на 50% каждая, для разрешения коллизии; <br/>
        /// </summary>
        public enum Demarcation { Left, Right, LeftRight }

        /// <summary> Перечисление цветотипа частицы. </summary>
        /// <remarks> Тип перечислений как битовые флаги, если активен <b>[Flags]</b> и номера имеют степени двойки. </remarks>
        //[Flags]
        public enum ColorType {
            White, Magenta, Red, Green, Blue, Aqua, Yellow, Orange, Purple, Gray, Black 
        }
        /// <summary> Перечисление типа частицы. </summary>
        /// <remarks> Тип перечислений как битовые флаги, если активен <b>[Flags]</b> и номера имеют степени двойки. </remarks>
        //[Flags]
        public enum Type_Particle {
            /// <summary> На частицу не действуют никакие силы. </summary>
            None = 0,
            /// <summary> Частица может сменить тип другой частицы на случайный при касании. </summary>
            Сменить_Тип = 1,
            /// <summary> Частица может приталкивать. </summary>
            Приталкивание = 2,
            /// <summary> Частица может отталкивать. </summary>
            Отталкивание = 3,
            /// <summary> Частица может слипаться. </summary>
            Слипание = 4,//4,
            /// <summary> Частица может установить связь с другой частицей. Если установлена связь, то указатель частицы ссылается на другую частицу и берёт её вектор и скорость. </summary>
            Связь = 5,//8,
            /// <summary> Частица обладает сильным взаимодействием. Чем дальше - тем сильнее. </summary>
            Сильное_Взаимодействие = 6,//16,
            /// <summary> Частица "отъедает" от другой частицы часть радиуса, массы и т.д., уменьшая её саму. </summary>
            Слабое_Поглощение = 7,
            /// <summary> Частица умножает скорость падения на -1. </summary>
            Реверс = 8,
            /// <summary> Частица может поглотить другую частицу полностью, если её масса меньше. <br/> Если массы равны, поглощения не происходит. </summary>
            Полное_Поглощение = 9,//32,
        }

        /// <summary> Перечисление режима работы метода <b>Physics.Fusion();</b> </summary>
        public enum Fusion { Полное_Поглощение, Слабое_Поглощение };

        /// <summary> Перечисление варианта просчёта соприкосновения частицы с краем пространства симуляции. <br/>
        ///     REVERSE = отскок от края; <br/> OPEN = пролёт сквозь край и вылет с другой стороны; <br/> STOP = полная остановка на краю пространства симуляции. </summary>
        public enum BorderWindow { REVERSE, STOP, OPEN };

        //================================================ ENUM ================================================

        //=============================================== STRUCT ===============================================
        public struct Circle {
            /// <summary> Коэффициент радиуса круга. <br/> Используется для коррекции действительного радиуса круга в псевдо-радиус для расчётов симуляции среды. </summary>
            public double Kr;
            /// <summary> Коэффициент лобового сопротивления формы (0.4 для круга). </summary>
            public double cd;
            /// <summary> Высота (толщина) круга. По умолчанию выбрать - 0.1. </summary>
            public double h;

            /// <summary> Масштабированный радиус <b>particle.Radius</b> для расчётов. </summary> <remarks> Вычисляемое значение. </remarks>
            public double R;
            /// <summary> Площадь круга. </summary> <remarks> <inheritdoc cref="R"/> </remarks>
            public double S;
            /// <summary> Объём круга. </summary> <remarks> <inheritdoc cref="R"/> </remarks>
            public double Vc;
            /// <summary> Объём шара. </summary> <remarks> <inheritdoc cref="R"/> </remarks>
            //public double Vsh;
            /// <summary> Плотность круга. </summary> <remarks> <inheritdoc cref="R"/> </remarks>
            public double Po;
            /// <summary> Коэффициент силы вязкого сопротивления среды. </summary> <remarks> <inheritdoc cref="R"/> </remarks>
            //public double k1;
            /// <summary> коэффициент силы инерционного сопротивления среды. </summary> <remarks> <inheritdoc cref="R"/> </remarks>
            //public double k2;
        };

        /// <summary> Структура точки типа <b>double</b>. </summary>
        public struct PointD { public double X, Y; public PointD(double x, double y) { X = x; Y = y; } };
        /// <summary> Структура диапазона типа <b>int</b>. </summary>
        public struct Range { public int Min, Max; public Range(int min, int max) { Min = min; Max = max; } };
        /// <summary> Структура итератора типа <b>int</b>. </summary>
        public struct Iterator { public int Begin, End; public Iterator(int begin, int end) { Begin = begin; End = end; } };

        /// <summary> Структура мини листов, сгруппирвоанных по цветотипу частиц. Эти листы заполняются из основного листа <b>Particles</b>. </summary>
        /// <remarks> После добавления очередного цветотипа в эту структуру, так же следует добавить нужный цветотип и в enum <b>ColorType</b>. </remarks>
        public struct MiniListColor {
            public List<PARTICLE> White;    public List<PARTICLE> Magenta;    public List<PARTICLE> Red;
            public List<PARTICLE> Green;    public List<PARTICLE> Blue;       public List<PARTICLE> Aqua;
            public List<PARTICLE> Yellow;   public List<PARTICLE> Orange;     public List<PARTICLE> Purple;
            public List<PARTICLE> Gray;     public List<PARTICLE> Black;
            public MiniListColor(bool b = true) {
                White = new List<PARTICLE>();    Magenta = new List<PARTICLE>();    Red = new List<PARTICLE>();
                Green = new List<PARTICLE>();    Blue = new List<PARTICLE>();       Aqua = new List<PARTICLE>();
                Yellow = new List<PARTICLE>();   Orange = new List<PARTICLE>();     Purple = new List<PARTICLE>();
                Gray = new List<PARTICLE>();     Black = new List<PARTICLE>();
            }
        }

        /// <summary> Структура цвета <b>HSV.</b> <br/> <b>(H) <paramref name="H"/>ue</b> - Оттенок [0..360°]; <br/> <b>(S) <paramref name="S"/>aturation</b> - Насыщение (от 0.0 до 1.0); <br/> <b>(V) <paramref name="V"/>alue</b> - яркость (от 0.0 до 1.0); <br/> </summary>
        /// <remarks> Конвертация <b>ColorToHSV / HSVToColor</b> описаны в классе <b>UFO.Convert</b> в качестве расширений <b>(extensions)</b>. </remarks>
        public struct HSV {
            /// <summary> Цветовой тон (например, красный, зелёный или синий). Варьируется в пределах [0..360°]. </summary>
            private double _hue;
            /// <summary> <inheritdoc cref="_hue"/> </summary> <remarks> Осуществляется проверка диапазона. </remarks>
            public double Hue { get { return _hue; } set { _hue = value < 0 ? 0 : value > 360 ? 360 : value; }}
            /// <summary> Насыщенность. Диапазон от 0.0 до 1.0, где 0.0 представляет серость, а 1.0 представляет насыщенность. </summary>
            private double _saturation;
            /// <summary> <inheritdoc cref="_saturation"/> </summary> <remarks> <inheritdoc cref="Hue"/> </remarks>
            public double Saturation { get { return _saturation; } set { _saturation = value < 0 ? 0 : value > 1 ? 1 : value; } }
            /// <summary> Яркость. Диапазон от 0.0 до 1.0, где 0.0 представляет чёрное, а 1.0 - белое. </summary>
            private double _value;
            /// <summary> <inheritdoc cref="_value"/> </summary> <remarks> <inheritdoc cref="Hue"/> </remarks>
            public double Value { get { return _value; } set { _value = value < 0 ? 0 : value > 1 ? 1 : value; } }
            public HSV(double H, double S, double V) { _hue = H; _saturation = S; _value = V; }
        }
        //=============================================== STRUCT ===============================================
    }
}
