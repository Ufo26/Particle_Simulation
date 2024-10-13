using System;
using System.Collections.Generic;
using System.Drawing;
using UFO.Math;
using static UFO.Enums_Structs;
using static System.Math;

namespace UFO {
    /// <summary> Класс симуляции. </summary>
    public partial class cSIMULATION {
        /// <summary> Свой кастомный генератор случайных чисел. </summary>
        public RANDOM Random = new RANDOM(RANDOM.INIT.RandomNext);

        /// <summary> Размер 2D пространства симуляции. </summary>
        public Size Size;
        /// <summary> Хранит перечисление выбора метода вычисления столкновения при движении. <br/>
        /// <b>MoveCheck</b>: метод MoveCheck(); <br/> <b>MoveCheckGrid:</b> метод MoveCheckGrid(); <br/> <b>MoveCheckGridList:</b> метод MoveCheckGridList(); </summary>
        public MoveCheck MovingCheck = MoveCheck.MoveCheck;

        /// <summary> Хранит гравитационную постоянную 6.67 * 10^-11 px (притяжение - ослабевает с расстоянием). </summary>
        public double G;
        /// <summary> Хранит коэффициент расстояния в <b>px</b> при котором выполняется гравитация. </summary>
        public double KGd;
        /// <summary> Хранит коэффициент пропорциональности Кулона (магнетизм - ослабевает с расстоянием). </summary>
        public double Kp;
        /// <summary> Хранит коэффициент расстояния в <b>px</b> при котором выполняется электромагнитное взаимодействие. </summary>
        public double KEd;
        /// <summary> Хранит силу взаимодействия между частицами в сильном взаимодействии. </summary>
        public double Fw; 
        /// <summary> Хранит коэффициент расстояния в <b>px</b> при котором выполняется сильное взаимодействие. </summary>
        public double KSd;

        /// <summary> Коэффициент масштаба высоты. 1 пиксель = K_Scale_H высоты. По умолчанию выбрать - 0.1. <br/>
        /// Позволяет применять силу архимеда динамически в зависимости от высоты. <br/>
        /// 1 пиксель = 0.1 ед. высоты; 100 пикселей = 10 ед. высоты; 1000 пикселей = 100 ед. высоты; <br/> Чем больше высота, тем выше дефолтная плотность <b>AtmosphereDensity</b> для расчётов. </summary>
        //public double K_Scale_H;
        /// <summary> Хранит вязкость среды в нc/м^2. <br/> Воздух = 0.182; <br/> Вода = 1.002; </summary>
        /// <remarks> Плотность (Density) и вязкость (Viscosity) среды связаны. <br/>
        /// Если Density = 0, то и (Viscosity) тоже = 0! </remarks>
        public double Viscosity;
        /// <summary> Хранит плотность среды в кг/м^3. <br/>
        /// Воздух = 1.22 кг/м^3; <br/> вода = 1000 кг/м^3; <br/>
        /// Лёгкие частицы падают дольше, чем тяжёлые, частицы с большим диаметром могут получить эффект парашюта. <br/>
        /// Частицы легче <b>Density</b> поднимаются вверх вместо падения на дно. </summary>
        /// <remarks> <inheritdoc cref="Viscosity"/> </remarks>
        public double Density;
        /// <summary> Хранит коэффициент упругости границ симуляции по периметру экрана (K). 0 - слипание, 1 - отскок, 0.5 - полуслипание/полуотскок. диапазон: [0..1]. </summary>
        public double Elasticity;
        /// <summary> Хранит ускорение свободного падения на дно симуляции. </summary>
        public double g;
        /// <summary> Хранит коэффициент силы связи частиц. </summary>
        public double KFС;
        /// <summary> Хранит величину стартовой акселерации для каждой частицы (скорость изменения скорости).
        ///           <br/> [1.01; 1.1] = ускорение; <br/> [0.9; 0.5] = замеление; <br/> [1.0] = без изменений; <br/> [0.0] = нулевая скорость. </summary>
        public double Acceleration;

        /// <summary> Хранит вариант просчёта соприкосновения частицы с краем пространства симуляции. </summary>
        public BorderWindow BorderWindow;
        private bool isopen = false;
        /// <summary> Хранит информацию статуса симуляции. <b>true</b> = симуляцию можно запустить, <b>false</b> = нельзя. </summary>
        public bool IsOpen { get { return isopen; } }
        private bool iswork = false;
        /// <summary> Хранит информацию статуса симуляции. <b>true</b> = симуляция сейчас запущена, <b>false</b> = нет. </summary>
        public bool IsWork { get { return iswork; } }

        /// <summary> Размер таблицы в ячейках. </summary>
        public Size SizeGrid = new Size();
        /// <summary> Размер одной ячейки таблицы в пикселях. </summary>
        public Size SizeCellsPixels = new Size();
        /// <summary> Таблица-массив индексов частиц, у каждой частицы проверяются её координаты и она записывается в ту или иную ячейку листа 2-мерного массива. </summary>
        public List<PARTICLE>[,] Grid = null;

        /// <summary> Содержит коллекцию частиц со всеми их свойствами. </summary>
        public List<PARTICLE> Particles = new List<PARTICLE>();

        /// <summary> Содержит коллекцию частиц со всеми их свойствами. </summary>
        /// <remarks> Обёртка <b>LIST</b> перегружает квадратные скобки <b>[]</b> и проверяет элемент списка по переданному индексу на выход из диапазона и значение null. <br/>
        ///     <b>get:</b> Если индекс выходит за границу диапазона или элемент списка по индексу Particles[i] = null, Particles[i] вернёт null вместо ошибки; <br/> 
        ///     <b>set:</b> Если индекс выходит за границу диапазона или элемент списка по индексу Particles[i] = null, присвоение не произойдёт вместо ошибки. </remarks>
        //public LIST<PARTICLE> Particles = new LIST<PARTICLE>();
        /// <summary> Класс-обёртка для <b>List</b> с перегрузкой <b>[]</b> для проверки элемента списка на выход из диапазона и значение null. </summary>
        public class LIST<T> : List<T> {
            public new T this[int i] {
                get { if (i < Count) return base[i]; else return default; }//вернуть null при выходе за диапазон
                set { if (i < Count) base[i] = value; }//при выходе за диапазон, присвоение не произойдёт
            }
        }

        /// <summary> Конструктор по умолчанию. Заполняет поля класса значениями по умолчанию. </summary>
        public cSIMULATION() {
            Size = new Size(800, 600); 
    	    G = 0.1; KGd = 500.0;//гравитация - сила и дальность
            Kp = 1.0; KEd = 500.0;//электромагнетизм - сила и дальность - сильнее гравитации
            Fw = 5.0; KSd = 10.0;//сильное взаимодействие - сила и дальность - сильнее магнетизма, но малый радиус действия
            Density = 500; g = 0.001; Elasticity = 1.0;
            KFС = 0.1; Acceleration = 0.95;
            BorderWindow = BorderWindow.REVERSE;
            isopen = false;
            SizeGrid.Width = 20; SizeGrid.Height = 10;
        }

        /// <summary> Метод ставит флаг <b>РАЗРЕШИТЬ</b> запуск симуляции (isopen = true). </summary>
        public void Open() { isopen = true; }
        /// <summary> Методт ставит флаг <b>ЗАПРЕТИТЬ</b> запуск симуляции (isopen = false). </summary>
        public void Close() { isopen = false; }
        /// <summary> Метод ставит флаг - симуляция <b>ЗАПУЩЕНА</b> (iswork = true). </summary>
        public void Strat() { iswork = true; }
        /// <summary> Метод ставит флаг - симуляция <b>ОСТАНОВЛЕНА</b> (iswork = false). </summary>
        public void Stop() { iswork = false; }

        /// <summary> Метод очищает пространство симуляции от частиц. </summary>
        public void Clear_Particles() { Particles.Clear(); }
        /// <summary> Метод добавляет частицы заданного типа и количества в пространство симуляции со случайными параметрами. </summary>
        public void Add_Particles(ColorType pColor, int Masa, double MAX_START_MASS_PART, int Speed) {
            var tmp = Add_Particles(pColor, Masa, Masa, MAX_START_MASS_PART, Speed, Speed, 0, 360,/*угол*/ 0, 100,/*Elasticity - эластичность*/
                100, 1000, new Range(0, 0), new Range(1, 100));//для заряда charge
            Particles.Add(tmp);//добавить экземпляр класса с заполненными полями в список
        }
        /// <summary> Метод добавляет частицы заданного типа и количества в пространство симуляции. </summary>
        /// <remarks> Радиус и ускорение задаются через массу. Массивные объекты слабо ускоряются, лёгкие - сильно. Чем тяжелее частица, тем больше её радиус и тем она темнее. </remarks>
        public PARTICLE Add_Particles(ColorType pColor, int m1 = 1, int m2 = 1000, double MAX_START_MASS_PART = 1000,
            int s1 = 0, int s2 = 0, int Q1 = 0, int Q2 = 360, int k1 = 0, int k2 = 100,
            uint Min = 100, uint Max = 1000, Range minus = default, Range plus = default)//для заряда charge
        {
            float speed = Random.RND(s1, s2);
            var Particle = new PARTICLE {
                SizeHolst = Size, Delete = false, ValueCoupling = 0, CountCoupling = 0,
                Acceleration = Acceleration,//0.95,
                Elasticity = 1,// Random.RND(k1, k2) / 100.0,
                Mass = Random.RND(m1, m2),
                pColor = pColor,
                MAX_START_MASS_PART = MAX_START_MASS_PART,
            };
            setParticleProperty(Particle);
            //чем крупнее и массивнее - тем больше трения, чем легче и мельче - тем меньше трения
            //Particle.Acceleration = //1 - Particle.Radius * Particle.Massa * 0.5 / (Particle.Radius * MAX_START_MASS_PART);
            //Particle.SetAngle(Random.RND(Q1, Q2));//Q - угол, для информации
            //Q = 0/360 - движение вправо   Q = 90 - движение вниз
            //Q = 180 - движение влево  Q = 270 - движение вверх
            //устанавливаем векторную скорость с помощью угла
            double Q = Random.RND(0, 360); Q *= (PI / 180.0);/*to_rad*/
                Particle.Velocity2D.SetXY(Cos(Q), Sin(Q));//единичный вектор
                Particle.Velocity2D = Particle.GetVelocityFromSpeed(speed);//единичный вектор в векторную скорость

            if (pColor == ColorType.Blue) Particle.SetCharge(Min, Max, minus/*%*/, plus/*%*/);
            else if (pColor != ColorType.White) Particle.SetCharge(1, Min, minus/*%*/, plus/*%*/);
            else Particle.SetCharge(0, 1, minus/*%*/, plus/*%*/);

            if (pColor == ColorType.White) Particle.ValueCoupling = Random.RND(5, 25);
            else if (pColor == ColorType.Aqua) Particle.ValueCoupling = Random.RND(25, 100);
            else Particle.ValueCoupling = Random.RND(25, 50);

            Particle.Position2D.X = Random.RND((int)Particle.Radius, Size.Width - (int)Particle.Radius);
            Particle.Position2D.Y = Random.RND((int)Particle.Radius, Size.Height - (int)Particle.Radius);

            //Particle.Thickness = thickness;
            Particle.Update_Color();
            return Particle;
        }

        /// <summary> Метод создаёт мини лист индексов частиц определённого цветотипа (type). </summary>
        /// <remarks> ЕСЛИ ЧАСТИЦА ПОМЕНЯЕТ СВОЙ ИНДЕКС В ЛИСТЕ PARTICLES, НУМЕРАЦИЯ БУДЕТ НАРУШЕНА! </remarks>
        /// <returns> Возвращает мини лист индексов частиц с цветотипом <b>type</b>. </returns>
        public List<int> CreateMiniListParticlesIndex(ColorType type) {
            var tmp = new List<int>();
            for (int i = 0; i < Particles.Count; i++) { if (Particles[i].pColor == type) tmp.Add(i); }
            return tmp;
        }
        /// <summary> Метод создаёт мини лист ссылок на частицы из листа Particles определённого цветотипа (type). </summary>
        /// <returns> Возвращает мини лист ссылок на частицы с цветотипом <b>type</b>. </returns>
        public List<PARTICLE> CreateMiniListParticles(ColorType type) {
            var tmp = new List<PARTICLE>();
            foreach (var part in Particles) { if (part.pColor == type) tmp.Add(part); }
            return tmp;
        }

        /// <summary> Метод размечает сетку Grid новыми индексами частиц Particles и затирает прежние значения, если они были. </summary>
        /// <remarks> Суть алгоритма: <br/> - создаётся новая таблица = new Grid; <br/> - осуществляется перебор всех частиц Particles; <br/> - координаты каждой частицы в симуляции преобразовываются в координаты сетки Grid; <br/> - в таблицу Grid[x, y].Add(index); добавляется порядковый номер (index) частицы по вычисленным преобразованным координатам x/y. </remarks>
        public void GridMarkup() {
            if (Grid == null) { Grid = new List<PARTICLE>[SizeGrid.Width, SizeGrid.Height];
                for (int y = 0; y < SizeGrid.Height; y++) for (int x = 0; x < SizeGrid.Width; x++) Grid[x, y] = new List<PARTICLE>();
            } else {
                for (int y = 0; y < SizeGrid.Height; y++) for (int x = 0; x < SizeGrid.Width; x++) Grid[x, y].Clear();
            }
            for (int i = 0; i < Particles.Count; i++) {
                int X = (int)Particles[i].Position2D.X / SizeCellsPixels.Width;
                int Y = (int)Particles[i].Position2D.Y / SizeCellsPixels.Height;
                X = X < 0 ? 0 : X >= SizeGrid.Width ? SizeGrid.Width - 1 : X;
                Y = Y < 0 ? 0 : Y >= SizeGrid.Height ? SizeGrid.Height - 1 : Y;
                Particles[i].Grid.Item2 = new List<Point>();
                Particles[i].Grid.Item3 = new List<Point>();
                AddParticleInAllGrid(Particles[i], (int)Particles[i].Position2D.X, (int)Particles[i].Position2D.Y, X, Y,
                    Particles[i].Radius, Particles[i].GetSpeed());
            }
        }

        /// <summary> Метод вычисляет координаты вершин квадрата в который вписан круг и переводит их в систему координат Grid. </summary>
        /// <returns> Возвращает кортеж из 4 координат в системе координат Grid. </returns>
        private (int, int, int, int) getPointsGridCircle(double CX, double CY, float newRadius) {
            //коры углов квадрата в который вписан круг в пространственной системе координат
            int x1 = (int)(CX - newRadius); int y1 = (int)(CY - newRadius);
            int x2 = (int)(CX + newRadius); int y2 = (int)(CY + newRadius);
            //коры углов в системе координат SIMULATION.Grid
            int X1 = x1 / SizeCellsPixels.Width; int X2 = x2 / SizeCellsPixels.Width;
            int Y1 = y1 / SizeCellsPixels.Height; int Y2 = y2 / SizeCellsPixels.Height;
            X1 = X1 < 0 ? 0 : X1 >= SizeGrid.Width ? SizeGrid.Width - 1 : X1;
            Y1 = Y1 < 0 ? 0 : Y1 >= SizeGrid.Height ? SizeGrid.Height - 1 : Y1;
            X2 = X2 < 0 ? 0 : X2 >= SizeGrid.Width ? SizeGrid.Width - 1 : X2;
            Y2 = Y2 < 0 ? 0 : Y2 >= SizeGrid.Height ? SizeGrid.Height - 1 : Y2;
            return (X1, X2, Y1, Y2);
        }

        /// <summary> Метод отцепляет круг из сетки SIMULATION.Grid. </summary>
        //private void RemoveParticleInAllGrid(PARTICLE pi, double oldCX, double oldCY, float oldRadius) {
        //    (int, int, int, int) result = getPointsGridCircle(oldCX, oldCY, oldRadius);
        //    int X1 = result.Item1; int X2 = result.Item2; int Y1 = result.Item3; int Y2 = result.Item4;
        //    //отцепляем все перекрытия круга [i] в сетке grid по старым координатам Old
        //    for (int y = Y1; y < Y2 + 1; y++) for (int x = X1; x < X2 + 1; x++) { Grid[x, y].Remove(pi); }
        //}

        /// <summary> Метод зацепляет круг в сетку SIMULATION.Grid и Particles[i].Grid. </summary>
        private void AddParticleInAllGrid(PARTICLE pi, double newCX, double newCY, int X, int Y, float newRadius, double speed) {
            pi.Grid.Item1.X = X; pi.Grid.Item1.Y = Y;//коры в сетке SIMULATION.Grid
            pi.Grid.Item2.Clear();//удаление устаревших ячеек, которые ранее покрывал круг
            pi.Grid.Item3.Clear();//удаление устаревших ячеек, которые ранее покрывал круг

            (int, int, int, int) result = getPointsGridCircle(newCX, newCY, newRadius);
            int X1 = result.Item1; int X2 = result.Item2; int Y1 = result.Item3; int Y2 = result.Item4;
            //зацепляем все перекрытия круга [i] в сетке grid по новым координатам New
            for (int y = Y1; y < Y2 + 1; y++) for (int x = X1; x < X2 + 1; x++) {
                    pi.Grid.Item2.Add(new Point(x, y));//кора ячейки которую покрывает радиус круга в сетке Particles
                Grid[x, y].Add(pi);//кора ячейки которую покрывает радиус круга в сетке SIMULATION
            }

            Vector2Df vec; double newCX2 = newCX; double newCY2 = newCY;
            while (speed > 0) {
                double quant = pi.Radius;
                if (speed > quant) { vec = pi.GetVelocityFromSpeed(quant); }
                else { vec = pi.GetVelocityFromSpeed(speed); }
                newCX2 += vec.X; newCY2 += vec.Y; speed -= quant;
                result = getPointsGridCircle(newCX2, newCY2, newRadius);
                X1 = result.Item1; X2 = result.Item2; Y1 = result.Item3; Y2 = result.Item4;
                for (int y = Y1; y < Y2 + 1; y++) for (int x = X1; x < X2 + 1; x++) {
                    var p = new Point(x, y);
                    if (!pi.Grid.Item3.Contains(p)) pi.Grid.Item3.Add(p);//кора ячейки которую покрывает радиус круга в сетке Particles
                }
            }
            pi.Grid.Item4 = newRadius;
            pi.Grid.Item5.X = newCX; pi.Grid.Item5.Y = newCY;//коры в пространственной системе координат
        }

        /// <summary> Метод осуществляет перецепку частицы в другую ячейку таблицы Grid после смены позиции, если это необходимо. <br/> Если частица не покинула ячейку, перезацепка не происходит. <br/>
        /// Перезацепка происходит если текущие координаты круга в сетке изменились относительно старых Particles.Grid. <br/>
        /// Кроме зацепки в ячейку SIMULATION.Grid по координатам круга, так же происходит зацепка в ячейки, которые круг перекрывает своим радиусом. </summary>
        /// <value> <b><paramref name="i"/>:</b> порядковый номер частицы в массиве Particles. <br/> </value>
        public void Remove_And_Add_Particle_In_AllCellGrid(PARTICLE pi) {
            int NewX = (int)(pi.Position2D.X / SizeCellsPixels.Width);
            int NewY = (int)(pi.Position2D.Y / SizeCellsPixels.Height);
            NewX = NewX < 0 ? 0 : NewX >= SizeGrid.Width ? SizeGrid.Width - 1 : NewX;
            NewY = NewY < 0 ? 0 : NewY >= SizeGrid.Height ? SizeGrid.Height - 1 : NewY;
            if (NewX != pi.Grid.Item1.X || NewY != pi.Grid.Item1.Y) {
                //RemoveParticleInAllGrid(pi, pi.Grid.Item5.X, pi.Grid.Item5.Y, pi.Grid.Item4);

                //отцепляем все перекрытия круга [i] в сетке grid по старым координатам Old
                for (int n = 0; n < pi.Grid.Item2.Count; n++) {
                    Grid[pi.Grid.Item2[n].X, pi.Grid.Item2[n].Y].Remove(pi);
                }
                AddParticleInAllGrid(pi, (int)pi.Position2D.X, (int)pi.Position2D.Y, NewX, NewY, pi.Radius, pi.GetSpeed());
            }
        }

        /// <summary> Метод вычисляет все значения полей <b>part.Property</b>. <br/>
        /// Метод вызывается всякий раз, когда у <b>частицы</b> были изменены: <br/> -- масса, радиус, диаметр, высота круга, коэффициент лобового сопротивления. <br/><br/>
        /// Значения: <br/>
        /// <b><paramref name="part"/>:</b> частица из массива Particles. <br/>
        /// <b><paramref name="cd"/>:</b> <inheritdoc cref="Circle.cd"/> <br/>
        /// <b><paramref name="height"/>:</b> <inheritdoc cref="Circle.h"/> <br/>
        /// <b><paramref name="kr"/>:</b> Коэффициент. <inheritdoc cref="Circle.R"/> <br/>
        /// </summary>
        /// <remarks> Если кроме частицы <b>part</b>, не передавать другие значения, то метод выставит значения по умолчанию. </remarks>
        public void setParticleProperty(PARTICLE part, double cd = 0.4, double height = 0.3, double kr = 0.005) {
            //radius - допустимые значения [0.001..2.5]
            part.Property.cd = cd; part.Property.h = height; part.Property.Kr = kr;
            part.Property.R = part.Radius * part.Property.Kr;//радиус круга в метрах
            part.Property.S = PI * part.Property.R * part.Property.R;//площадь круга Pi * R^2 (поперечное сечение)
            part.Property.Vc = part.Property.S * part.Property.h;//объём V круга PI * R^2 * h
            part.Property.Po = part.Mass * part.Property.Vc;//плотность круга
            //part.Property.Vsh = 3.0 / 4.0 * PI * part.Radius * part.Radius * part.Radius;//объём V шара 3/4 * PI * R^3
        }

        /// <summary> Метод эмитирует ускорение свободного падения частицы с учётом наличия или отсутствия плотности атмосферы. </summary>
        /// <remarks> Метод применяет действие трёх сил на частицу: <br/>
        /// 1. Сила ускорения свободного падения (SIMULATION.g) направленная вниз; <br/>
        /// 2. Сила сопротивления атмосферы (SIMULATION.AtmosphereDensity) направленная вверх; <br/>
        /// 3. Сила Архимеда выталкивающая частицу (частица тяжелее среды - падает; легче - поднимается вверх; равная = покоистя на месте); <br/><br/>
        /// Более подробно: <br/>
        /// 1. При отключённой атмосфере, ускорение свободного падения (g) действует на все частицы одинаково, независимо от их массы и диаметра; <br/>
        /// 2. При наличии атмосферы, логика усложняется: <br/>
        /// -- диаметр круга имеет эффект купола парашюта: чем больше диаметр, тем больше торможение скорости падения; <br/>
        /// -- с возрастанием скорости частицы - сопротивление атмосферы так же возрастает и это усугубляется с увеличением диаметра; <br/><br/>
        /// Далее следуют сравнения 2 кругов при нулевой начальной скорости и одинаковой высоте: <br/>
        /// -- два круга с одинаковым диаметром и массой - достигнут дна примерно одновременно; <br/>
        /// -- два круга с разным диаметром, но одинаковой массой - достигнут дна по разному: меньший диаметр достигнет дна раньше; <br/>
        /// -- два круга с одинаковым диаметром, но разной массой - достигнут дна по разному: бОльшая масса достигнет дна раньше; <br/> </remarks>
        /// <value> <b><paramref name="part"/>:</b> частица, к которой нужно применить акселерацию. <br/> </value>
        public void setForce3In1(PARTICLE part) {
            //test(); return;
            //radius - допустимые значения [0.001..2.5]
            var _ = part.Property;
            double densityH = Density - (Density / part.Position2D.Y);//плотность среды на высоте, чем ниже - тем плотнее
            double uH = Viscosity - (Viscosity / part.Position2D.Y);//вязкость среды на высоте, чем ниже - тем больше
            double k1 = 6 * PI * uH * part.Property.R;//коэффициент силы вязкого сопротивления среды
            double k2 = 0.5 * part.Property.cd * part.Property.S * densityH;//коэффициент силы инерционного сопротивления среды

            double Fa = (_.Po - densityH) * _.Vc * g;//подъёмная сила Архимеда в среде, направленная вверх, в ньютонах
            //сила сопротивления среды направленная против движения частицы
            double FcY = -(k1 * part.Velocity2D.Y + k2 * (part.Velocity2D.Y * Abs(part.Velocity2D.Y))) / (1 + part.Mass);
            double FcX = -(k1 * part.Velocity2D.X + k2 * (part.Velocity2D.X * Abs(part.Velocity2D.X))) / (1 + part.Mass);
            if (Abs(FcX) > Abs(part.Velocity2D.X)) FcX = -part.Velocity2D.X;
            if (Abs(FcY) > Abs(part.Velocity2D.Y)) FcY = -part.Velocity2D.Y;

            double aX = FcX; double aY = g + Fa + FcY;//результирующий вектор изменения скорости частицы
            part.Velocity2D.X += aX; part.Velocity2D.Y += aY;

            //Damping - множитель амплитуды отскока:
            //при Damping = 1.1+ -- рост амплитуды, отскок выше высоты падения (батут)
            //при Damping = 1.0 -- стабильная амплитуда, отскок равный высоте падения (идеальный отскок в сферическом вакууме)
            //при Damping = 0.5 -- затухание амплитуды, отскок раза в 2 меньше высоты падения (кожаный мяч)
            //при Damping = 0.0 -- нет амплитуды, отскока не будет (игрушка "лизун")
            double Damping = Elasticity * part.Elasticity;
            if (part.Position2D.Y + part.Radius >= Size.Height) { part.Velocity2D.Y *= Damping; }//гасим скорость при отскоке от дна
        }

        void test() {
            //radius - допустимые значения [0.001..2.5]
            //mass - допустимые значения [0.001..5000]
            double radius = 250.0 * 0.001;// = radius / MAX_RADIUS // радиус круга в метрах
            double mass = 1000;// = mass / MAX_MASS * 100% // масса круга в килограммах

            double g = 1.0;// 9.81; // ускорение свободного падения симуляции в м/с^2
            double cd = 0.4; // коэффициент лобового сопротивления формы (0.4 для круга)
            double density = 1.22;///1000; // плотность среды в кг/м^3     воздух=1.22; вода=1000
            double u = 0.182;//вязкость среды       воздух=0.182;    вода=1.002
            double h = 0.1;//высота (толщина) круга
            double S = PI * radius * radius;//площадь круга Pi * R^2 (поперечное сечение)
            double Vc = S * h;//объём V круга PI * R^2 * h
            double Po = mass * Vc;//плотность круга
            double Vsh = 3.0 / 4.0 * PI * radius * radius * radius;//объём V шара 3/4 * PI * R^3
            double k1 = 6 * PI * u * radius;//коэффициент силы вязкого сопротивления среды
            double k2 = 0.5 * cd * S * density;//коэффициент силы инерционного сопротивления среды

            double VelocityX = 2; // начальная скорость по оси X в м/с
            double VelocityY = 0; // начальная скорость по оси Y в м/с
            double velY = 0, posY = 0;
            double PositionX = 0; // начальная позиция по оси X в метрах
            double PositionY = 0; // начальная позиция по оси Y в метрах
            string result = $"dAir = {density}    radius = {radius}    mass = {mass}    плотность = {mass * S:F2}    g = {g}\n"; int i = 0;
            while (i <= 420) {
                //сила Архимеда не действует в невесомости при g = 0
                double H = PositionY * 0.1;//конвертация пикселей в условную высоту h
                double densityH = density * H;//плотность воздуха на высоте, чем ниже тем плотнее
                double Fg = mass * g;//сила тяжести направленная вниз
                double Fa = (Po - densityH) * Vc * g;//подъёмная сила Архимеда в среде равна весу объёма вытесненной жидкости, направленная вверх
                //double Kair = (-0.5 * density * cd * S);//коэффициент сопротивления среды направленный против движения частицы
                //double Fc_X = Kair * VelocityX * Abs(VelocityX);//сила сопротивления среды направленная против движения частицы
                //double Fc_Y = Kair * VelocityY * Abs(VelocityY);//сила сопротивления среды направленная против движения частицы

                //double aY = -(k1 * VelocityY + k2 * (VelocityY * Abs(VelocityY)) - mass * g -  mass * Fa) / (1 + mass);
                double aY = -(k1 * VelocityY + k2 * (VelocityY * Abs(VelocityY))) / (1 + mass);
                double aX = -(k1 * VelocityX + k2 * (VelocityX * Abs(VelocityX))) / (1 + mass);
                if (Abs(aX) > Abs(VelocityX)) aX = -VelocityX;
                if (Abs(aY) > Abs(VelocityY)) aY = -VelocityY;

                Vector2Df fg = new Vector2Df(0, g);
                Vector2Df fa = new Vector2Df(0, Fa);
                Vector2Df fc = new Vector2Df(aX, aY);
                Vector2Df a = (fg + fc + fa);//результирующий вектор силы

                // Вычисление новой скорости и позиции круга
                VelocityX += a.X;// fc.X; // новая скорость по оси X
                VelocityY += a.Y;// + Fa;// g * fc.Y - fa.Y;// aY;// + ResistanceAirY; // новая скорость по оси Y
                velY += g; posY += velY;
                PositionX += VelocityX; // новая позиция по оси X
                PositionY += VelocityY; // новая позиция по оси Y

                // Вывод результатов симуляции
                if (i % 10 == 0)
                    result += $"[{i}] vY={velY};  Y={posY}     Pos: ({PositionX:F1},  {PositionY:F1})    " +
                        $"[{i}] Velocity: ({VelocityX:F2},  {VelocityY:F1})\n";//    aY={aY:F2}\n";
                i++;
            }
            System.Windows.Forms.MessageBox.Show(result);
        }

        /// <summary> Метод проверяет соприкосновение частицы с границей симуляции или выход за её пределы. </summary>
        /// <returns> Возвращает <b>true</b> если частица у границы или за границей симуляции, в противном случае возвращает <b>false</b>. </returns>
        //public bool Border1(PARTICLE part) {
        //    var c = part.Position2D; var r = part.Radius;
        //    //return (c.X - r <= 0 || c.X + r >= Size.Width || c.Y - r <= 0 || c.Y + r >= Size.Height);
        //    return (c.X - r <= 0 || c.X + r >= Size.Width ||
        //            c.Y - r <= 0 || c.Y + r >= Size.Height);
        //}

        /// <summary> Метод обрабатывает границы пространства симуляции. <br/>
        /// <b><paramref name="Part"/>:</b> частица, для которой нужно произвести расчёт. <br/>
        /// <b><paramref name="bv"/>:</b> <inheritdoc cref="BorderWindow"/> <br/>
        /// </summary>
        public void Border_Processing(PARTICLE Part, BorderWindow bv) {
			var p = Part.Position2D; var r = Part.Radius; var sz = Size;
			switch (bv)	{
				case BorderWindow.REVERSE: //отскок от границы симуляции - инвертировать угол и вектор
					if (p.X - r < 0.0) { Part.Velocity2D.X = -Part.Velocity2D.X; Part.Position2D.X = r; }//слева
					else if (p.X + r > sz.Width) { Part.Velocity2D.X = -Part.Velocity2D.X; Part.Position2D.X = sz.Width - r; }//справа
					if (p.Y - r < 0.0) { Part.Velocity2D.Y = -Part.Velocity2D.Y; Part.Position2D.Y = r; }//сверху
					else if (p.Y + r > sz.Height) { Part.Velocity2D.Y = -Part.Velocity2D.Y; Part.Position2D.Y = sz.Height - r; }//снизу
					break;
				case BorderWindow.OPEN: //пролёт пространства симуляции насквозь и вылет с другой стороны
					if (p.X < 0.0) Part.Position2D.X = sz.Width; else if (p.X > sz.Width) Part.Position2D.X = 0;
					if (p.Y < 0.0) Part.Position2D.Y = sz.Height; else if (p.Y > sz.Height) Part.Position2D.Y = 0;
					break;
				case BorderWindow.STOP: //полная остановка на краю симуляции
					bool b = false;
					if (p.X - r < 0.0) { Part.Position2D.X = r; b = true; }
					else if (p.X + r > sz.Width) { Part.Position2D.X = sz.Width - r; b = true; }
					if (p.Y - r < 0.0) { Part.Position2D.Y = r; b = true; }
					else if (p.Y + r > sz.Height) { Part.Position2D.Y = sz.Height - r; b = true; }
					if (b) { Part.Velocity2D.X = 0; Part.Velocity2D.Y = 0; }
					break;
				default: break;
			}
		}
    }
}
