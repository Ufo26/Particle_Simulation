using OpenTK.Graphics;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using UFO.Logic;
using UFO.Math;
using static UFO.Enums_Structs;
using static System.Math;

namespace UFO {
    /// <summary> Класс частицы. </summary>
    public class PARTICLE {
        /// <summary> Свой кастомный генератор случайных чисел. </summary>
        public RANDOM Random = new RANDOM(RANDOM.INIT.RandomNext);
        /// <summary> Размер 2D пространства симуляции. </summary>
        public Size SizeHolst;

        /// <summary> Хранит тип-цвет частицы. Он указывает что может частица: притягивать, отталкивать, устанавливать связь или производить комбинацию этих действий. </summary>
        private ColorType pcolor;
        /// <summary> <inheritdoc cref="pcolor"/> </summary>
        /// <remarks> Во время присваивания типа, так же происходит присваивание цвета частицы в поле <b>FillColor</b> и <b>OutlineColor</b>. <br/>
        /// </remarks>
        public ColorType pColor {
            get { return pcolor; }
            set { pcolor = value;
                switch (value) {
                    case ColorType.White: FillColor4 = new Color4(1f, 1f, 1f, 1f); break;
                    case ColorType.Red: FillColor4 = new Color4(1f, 0f, 0f, 1f); break;
                    case ColorType.Green: FillColor4 = new Color4(0f, 1f, 0f, 1f); break;
                    case ColorType.Blue: FillColor4 = new Color4(0f, 0f, 1f, 1f); break;
                    case ColorType.Aqua: FillColor4 = new Color4(0f, 1f, 1f, 1f); break;
                    case ColorType.Yellow: FillColor4 = new Color4(1f, 1f, 0f, 1f); break;
                    case ColorType.Orange: FillColor4 = new Color4(1f, 0.5f, 0f, 1f); break;
                    case ColorType.Purple: FillColor4 = new Color4(0.5f, 0.5f, 0f, 1f); break;
                    case ColorType.Magenta: FillColor4 = new Color4(1f, 0f, 0.5f, 1f); break;
                    case ColorType.Gray: FillColor4 = new Color4(0.5f, 0.5f, 0.5f, 1f); break;
                    case ColorType.Black: FillColor4 = new Color4(0f, 0f, 0f, 1f); break;
                }
                OutlineColor4 = new Color4(FillColor4.R > 0 ? 1 - FillColor4.R : 0,
                                           FillColor4.G > 0 ? 1 - FillColor4.G : 0,
                                           FillColor4.B > 0 ? 1 - FillColor4.B : 0,
                                           FillColor4.A);
            }
        }

        /// <summary> Хранит предыдущее положение центра частицы в 2D пространстве симуляции. </summary>
        //public Vector2Df prevPos2D = new Vector2Df();
        /// <summary> Хранит положение центра частицы в 2D пространстве симуляции. </summary>
        public Vector2Df Position2D = new Vector2Df();
        /// <summary> Хранит векторную скорость частицы в 2D пространстве симуляции. </summary>
        public Vector2Df Velocity2D = new Vector2Df();
        /// <summary> <inheritdoc cref="velocity2D"/> </summary>
        /// <remarks> Изменение векторной скорости <b>Velocity</b>, автоматически изменяет скаляр <b>Speed.</b> </remarks>
        //public Vector2Df Velocity2D {
        //    get { return velocity2D; }
        //    set { if (value != velocity2D) { velocity2D = value; speed = GetSpeed(); } }
        //}

        /// <summary> 1. <b>Point</b> хранит координаты ячейки таблицы SIMULATION.Grid, в которой покоится центр круга; <br/>
        /// 2. <b>List.Point</b> хранит список координат Grid(x, y), которые покрывает круг в текущий момент; <br/>
        /// 3. <b>List.Point</b> хранит список координат Grid(x, y), которые круг пересекает в движении, <br/> если движение = 0, List.Count = 0; <br/>
        /// 4. <b>float</b> хранит радиус круга на момент добавленяи в кортеж; <br/>
        /// 5. <b>Vector2Df</b> хранит координаты для центра круга "Point 1." в пространственной системе координат. </summary>
        public (Point, List<Point>, List<Point>, float, Vector2Df) Grid;

        /// <summary> Хранит флаг удаления частицы. Метод поглощения ставит этот флаг в положение <b>true</b>. </summary>
        public bool Delete = false;
        /// <summary> Хранит скалярную скорость частицы. </summary>
        //private double speed;
        ///// <summary> <inheritdoc cref="speed"/> </summary>
        ///// <remarks> <inheritdoc cref="Velocity2D"/> </remarks>
        //public double Speed {
        //    get { return speed; }
        //}
        /// <summary> Хранит величину акселерации частицы (скорость изменения скорости).
        ///           <br/> [1.01; 1.1] = ускорение; <br/> [0.9; 0.5] = замеление; <br/> [1.0] = без изменений; <br/> [0.0] = нулевая скорость. </summary>
        public double Acceleration;
        /// <summary> Хранит массу частицы (по умолчанию совпадает с радиусом или кратна размерам окружности) </summary>
        private float massa;
        /// <summary> <inheritdoc cref="massa"/> </summary>
        /// <remarks> Изменение массы автоматически изменяет Radius, Diameter и Acceleration, зависящие от массы. </remarks>
        public float Mass {
            get { return massa; }
            set { massa = value;
                //Radius = 0.025f * massa;
                //var s = (float)(System.Math.PI * (Radius * Radius));//площадь круга
                Radius = GetRadiusFromMass();// massa / 50f;//90f;
                Radius = Radius >= 0.5 ? Radius : 1;
                Diameter = Radius * 2;
  //              Acceleration = Ka - (Radius * Radius) / Massa;
    //            if (Acceleration < 0) Acceleration = 0;
            }
        }
        /// <summary> Хранит радиус частицы (половина диаметра). </summary>
        public float Radius;
        /// <summary> Хранит диаметр частицы (два радиуса). </summary>
        public float Diameter;
        /// <summary> Хранит заряд частицы. <br/> "-" = отрицательный; <br/> "+" = положительный; <br/> "0" = нейтральный; <br/> Величина = силе заряда. </summary>
        public double Charge;
        /// <summary> Хранит угол направления движения. Переменная для информации. Значение угла получается из величин вектора <b>Velocity</b>. </summary>
        /// <remarks> Q = 0, Q = 360 - вправо; <br/> Q = 90 - вниз; <br/> Q = 180 - влево; <br/> Q = 270 - вверх. </remarks>
        public double Angle;
        /// <summary> Хранит коэффициент упругости частицы (K). 0 - слипание, 1 - отскок, 0.5 - полуслипание/полуотскок. диапазон: [0..1]. </summary>
        public double Elasticity;

        /// <summary> Хранит ссылку на связанную частицу, т.е. эта частица имеет N связей с другими частицами, если все связи ссылаются на одну и ту же частицу, то притяжение к ней возрастает. </summary>
        /// <remarks> Массив ссылок не нужно создавать, он уже создан в необходимом количестве! </remarks>
        public PARTICLE[] LinkPart = new PARTICLE[1];
        /// <summary> Хранит силу связи. </summary>
        public int ValueCoupling;
        /// <summary> Хранит количество связей, установленных с этой частицей. </summary>
        public int CountCoupling;

        /// <summary> Хранит толщину контура окружности частицы. </summary>
        /// <remarks> Графический атрибут. </remarks>
        //public float Thickness;
        /// <summary> Хранит цвет контура окружности частицы. <br/> Формат: RGBA(byte). </summary>
        /// <remarks> <inheritdoc cref="Thickness"/> </remarks>
        //public SKColor OutlineColor;
        /// <summary> Хранит цвет контура окружности частицы. <br/> Формат: RGBA(float [0f..1f]). </summary>
        /// <remarks> <inheritdoc cref="Thickness"/> </remarks>
        public Color4 OutlineColor4;
        /// <summary> Хранит цвет заливки поверхности окружности частицы. <br/> Формат: RGBA(float [0f..1f]). </summary>
        /// <remarks> <inheritdoc cref="Thickness"/> </remarks>
        public Color4 FillColor4;

        /// <summary> Хранит максимальную стартовую массу частицы в симуляции. <br/> Нужно для коррекции цвета с учётом массы в методе <b>Update_Color()</b>. </summary>
        public double MAX_START_MASS_PART;

        /// <summary> Хранит свойства частицы для тонкой настрйоки влияния гравитации и атмосферы. </summary>
        public Circle Property;

        /// <summary> Метод получает (модульную) скалярную скорость частицы по её векторной скорости Velocity. </summary>
        public double GetSpeed() { return Sqrt(Velocity2D.X * Velocity2D.X + Velocity2D.Y * Velocity2D.Y); }
        /// <summary> Метод получает скалярную скорость частицы по переданным компонентам векторной скорости. </summary>
        public double GetSpeed(Vector2Df velocity) { return Sqrt(velocity.X * velocity.X + velocity.Y * velocity.Y); }
        /// <summary> Метод получает скалярную скорость частицы по переданным компонентам векторной скорости. </summary>
        public double GetSpeed(double vx, double vy) { return Sqrt(vx * vx + vy * vy); }
        /// <summary> Метод получает векторную скорость velocity из переданной скалярной скорости speed с сохранением направления. </summary>
        public Vector2Df GetVelocityFromSpeed(double speed) {
            var v = Velocity2D; v.Normalize();
            v.X *= speed; v.Y *= speed;
            return v;
        }

        /// <returns> Метод возвращяет вычисленный радиус круга исходя из его массы. </returns>
        public float GetRadiusFromMass() { float radius = massa / 50f; radius = radius >= 0.5 ? radius : 1; return radius; }

        /// <summary> Метод устанавливает случайную силу заряда от <b>Min</b> до <b>Max</b>. </summary>
        /// <remarks> Если любой <b>Min</b> > <b>Max</b> или если Min/Max у <b>minus/plus</b> будут вне диапазона 0%-100%, заряд будет нейтральным. </remarks>
        /// <value> 
        ///     <b><paramref name="Min"/>, <paramref name="Max"/>:</b> модуль силы заряда. <br/>
        ///     <b><paramref name="minus"/>, <paramref name="plus"/>:</b> процентный диапазон вероятности присвоения знака заряду, минимум 0%, максимум 100%. При Min/Max = 0, знак заряда будет равен нулю. <br/>
        /// </value>
        public void SetCharge(uint Min, uint Max, Range minus, Range plus) {
            if (Min > Max || minus.Min > minus.Max || plus.Min > plus.Max ||
                minus.Min < 0 || minus.Min > 100 || minus.Max < 0 || minus.Max > 100 ||
                plus.Min < 0 || plus.Min > 100 || plus.Max < 0 || plus.Max > 100) { Charge = 0; return; }
            int R = Random.RND(1, 100);
            if (R >= minus.Min/*%*/ && R <= minus.Max/*%*/) Charge = -(int)Random.uRND(Min, Max);
            else if (R >= plus.Min/*%*/ && R <= plus.Max/*%*/) Charge = (int)Random.uRND(Min, Max);
            else Charge = 0;
        }
        /// <summary> Метод устанавливает угол траектории движения. </summary>
        /// <value> <b><paramref name="Q"/>:</b> Угол в градусах [0..360], 0 и 360 задают один и тот же угол. </value>
        /// <remarks> Угол 0 и 360 справа, возрастание по часовой стрелке. </remarks>
        public void SetAngle(double Q = -1) {
            Q = Q == -1 ? Angle : Q;
            Angle = Q > 360.0 ? Q %= 360.0 : Q < 0 ? Q %= 360.0 + 360 : Q;
        }

        /// <summary> Метод вычисляет угол поворота <b>данного объекта</b> по его векторной скорости и присваивает вычисленное значение его полю <b>Angle</b> (угол). </summary>
        /// <returns> <inheritdoc cref="Vector2Df.AngleBetween"/> </returns>
        //public void AngleFromVector() { Q = AngleFromVector(Vector); }
        public void AngleFromVelocity() { Angle = AngleFromVector(Velocity2D); }
        /// <summary> Метод вычисляет угол поворота единичного вектора <b>vec</b> относительно вектора <b>(1.0, 0.0)</b>. </summary>
        /// <returns> <inheritdoc cref="Vector2Df.AngleBetween"/> </returns>
        public double AngleFromVector(Vector2Df vec) {
	        if (vec.X == 0.0 && vec.Y == 0.0) return 0.0;
	        else { //вариант 1
                //Vector2Df v = new Vector2Df(); v.Copy(vec);
                //Vector2Df v = vec;//Copy
                return Velocity2D.AngleBetween(vec, new Vector2Df(1.0, 0.0)); 
		        //вариант 2 (для 2D)- таой же точный, но не ясно как по скорости
		        //double Q = (atan2(vec.Y(), vec.X()) * 180.0 / this->vector.PI());
		        //if (Q < 0) Q += 360.0;
		        //return Q;
	        }
        }

        /// <summary> Метод двигает частицу согласно её вектору скорости (this.Velocity2D) и умножает вектор на акселерацию (Acceleration). </summary>
        public void Move() { Move(Velocity2D.X, Velocity2D.Y); }
        public void Move(Vector2Df velocity) { Move(velocity.X, velocity.Y); }
        /// <summary> Метод двигает частицу с векторной скоростью <b>velocity</b> и умножает вектор на акселерацию (Acceleration). </summary>
        public void Move(double vx, double vy) {
            Position2D.X += vx; Position2D.Y += vy;
            Velocity2D.X *= Acceleration; Velocity2D.Y *= Acceleration;
        }

        /// <summary> <b>MoveCheck(...);</b> вызывается во вложенном цикле <b>for [j] !!!</b> <br/>
        /// Метод передвигает частицу <b>this</b> по её вектору скорости после проверки на столкновение. <br/>
        /// Проверка осуществляется тупым перебором (в лоб) <b>ВСЕХ</b> вложенных частиц [j] и выбирается наименьший вектор скорости по траектории движения для круга <b>this</b>. <br/>
        /// В результате частица смещается по вектору скорости на всю длину, если столкновений не обнаружено <br/> или останавливается в точке столкновения, не пройдя весь вектор скорости. <br/><br/>
        /// Этот код позволяет не дробить скорость на кванты (порции) и не двигать круг по 2 пикселя во вложенном цикле, <br/>
        /// а так же не нужно создавать сетку в SIMULATION, хранить в ней данные, записывать, удалять индексы, <br/>
        /// не нужно создавать кортеж Particle[].Grid с данынми, создавать/удалять списки координат <br/>
        /// и не нужно создавать и сортировать мини лист ближайших соседей по индексам Grid. <br/>
        /// Недостаток этого кода: нужно перебирать ВСЕ частицы во вложенном цикле [j]. <br/>
        /// Перебор 2 вложенных циклов на 1000 частиц = 1000х1000 = 1 млн итераций.
        /// </summary>
        /// <value> 
        ///     <b><paramref name="i"/>, <paramref name="j"/>:</b> индексы частиц в двух массивах <b>Particles[i] / Particles[j]</b>. <br/>
        ///     <b><paramref name="link_SIMULATION"/>:</b> ссылка на объект симуляции. <br/>
        ///     <b>out <paramref name="part"/>:</b> ссылка на частицу, которая вызвала столкновение, если столкновения не было, возвращает <b>null</b>. <br/>
        ///     <b><paramref name="velocity"/>:</b> хранит промежуточные вычисления длины вектора скорости в каждой итерации цикла [j]. <br/>
        /// </value>
        /// <returns> <inheritdoc cref="Physics2D.Collision"/> </returns>
        public bool MoveCheck(int i, int j, cSIMULATION link_SIMULATION, ref PARTICLE part, ref Vector2Df velocity) {
            if (Velocity2D.X == 0 && Velocity2D.Y == 0) return false;
            if (i != j) {
                (double, double)? Dot = Physics2D.GetCollisionPoint_Continious(this, link_SIMULATION.Particles[j]);
                if (Dot != null) {
                    var x = Dot.Value.Item1 - Position2D.X; var y = Dot.Value.Item2 - Position2D.Y;
                    if (x * x + y * y < velocity.X * velocity.X + velocity.Y * velocity.Y) {
                        velocity.X = x; velocity.Y = y;
                        part = link_SIMULATION.Particles[j];
                    }
                }
            }
            if (j == link_SIMULATION.Particles.Count - 1) { //последняя итерация цикла [j]
                if (velocity != Velocity2D && part != null) { //СТОЛКНОВЕНИЕ БЫЛО!
                    Move(velocity.X, velocity.Y);//смещаем круг this до точки столкновения
                    return true;
                } else Move();//смещем круг this на полный вектор, т.к. столкновения не обнаружено
            }
            return false;
        }

        /// <summary> <b>MoveCheckGridList(...);</b> вызывается в главном цикле <b>for [i] !!!</b> <br/>
        /// Метод передвигает частицу <b>this</b> по её вектору скорости после проверки на столкновение. Алгоритм проверки: <br/>
        /// 1. Создаётся лист ссылок на ближайшие частицы, с которыми может быть столкновение: <br/>
        /// .....а) для создания такого листа, берутся данные из сетки SIMULATION.Grid[,]; <br/>
        /// .....b) каждая частица имеет свой периметр по вектору движения в котором будет происходить поиск ближайших соседей (this.Grid); <br/>
        /// 2. Созданный лист сортируется по удалению от круга <b>this</b> (первый круг в отсортированном списке ближе всех к кругу this); <br/>
        /// 3. В цикле перебирается этот лист кругов, перебор прекращается после первого найденного столкновения, <br/> если столкновения нет ни с кем, то цикл отработает до конца; <br/>
        /// 4. После смещения круга, происходит перезацепка индексов в <b>SIMULATION.Grid[,]</b> и в кортеже <b>this.Grid</b>; <br/><br/>
        /// В результате частица смещается по вектору скорости на всю длину, если столкновений не обнаружено <br/> или останавливается в точке столкновения, не пройдя весь вектор скорости. <br/><br/>
        /// ЭТОТ МЕТОД ЭФФЕКТИВЕН, ЕСЛИ В ГЛАВНОМ ЦИКЛЕ НЕ НУЖНО ПЕРЕБИРАТЬ ВЕСЬ ВЛОЖЕННЫЙ СПИСОК Particles[j], <br/>
        /// ТО ЕСТЬ НЕ НУЖНО ПРИМЕНЯТЬ ВЛИЯНИЕ ЧАСТИЦЫ <b>pi</b> НА ВСЕ ОСТАЛЬНЫЕ ЧАСТИЦЫ <b>pj</b>. </summary>
        /// <value> 
        ///     <b><paramref name="link_SIMULATION"/>:</b> ссылка на объект симуляции. <br/>
        ///     <b>out <paramref name="part"/>:</b> ссылка на частицу, которая вызвала столкновение, если столкновения не было, возвращает <b>null</b>. <br/>
        /// </value>
        /// <returns> <inheritdoc cref="Physics2D.Collision"/> </returns>
        public bool MoveCheckGridList(cSIMULATION link_SIMULATION, out PARTICLE part) {
            part = null;
            if (Velocity2D.X == 0 && Velocity2D.Y == 0) return false;
            List<PARTICLE> sorted_list = new List<PARTICLE>();//отсортированный list по удалению от круга this
            //построение списка кругов, которые нужно проверить на столкновение
            List<PARTICLE> list = new List<PARTICLE>();
            for (int n = 0; n < Grid.Item3.Count; n++) //перебор ячеек сетки Grid которые круг пересекает в движении
                for (int j = 0; j < link_SIMULATION.Grid[Grid.Item3[n].X, Grid.Item3[n].Y].Count; j++) {
                    //перебор кругов: circle (pj) = Grid[x, y][j];
                    PARTICLE pj = link_SIMULATION.Grid[Grid.Item3[n].X, Grid.Item3[n].Y][j];
                    //pj = link_SIMULATION.Particles[index];
                    if (this == pj) continue;//if (this == pj) continue;
                    if (pj == null || pj.Delete) continue;
                    if (list.Contains(pj)) continue;
                    list.Add(pj);
            }
            //сортировка списка кругов по удалению от круга this, первый круг в списке sorted_list ближе всех к кругу this
            sorted_list = list.OrderBy(c => Physics2D.Distance(Position2D, c.Position2D)).ToList();
            (double, double)? dot = null; bool IsCollision = false;//default = столкновения не было

            //умное смещение круга pi без пролётов сквозь соседа и смещения на порцию скорости по 2 пикселя за раз
            foreach (var pj in sorted_list) {
                if (this == pj) continue; if (pj == null || pj.Delete) continue;
                dot = Physics2D.GetCollisionPoint_Continious(this, pj);
                if (dot != null) { IsCollision = true; //СТОЛКНОВЕНИЕ БЫЛО!
                    //вектор скорости фактически пройденного пути
                    var velocity = new Vector2Df(dot.Value.Item1 - Position2D.X, dot.Value.Item2 - Position2D.Y);//УДАЛИТЬ
                    Move(dot.Value.Item1 - Position2D.X, dot.Value.Item2 - Position2D.Y);//смещаем круг this до точки столкновения
                    //Position2D.X = dot.Value.Item1; Position2D.Y = dot.Value.Item2;
                    part = pj;
                    break;
                }
            }
            if (!IsCollision) Move();//столкновения не обнаружено, двигаемся на всю катушку
            if (!Delete) link_SIMULATION.Remove_And_Add_Particle_In_AllCellGrid(this);
            return IsCollision;
        }

        /// <summary> <b>MoveCheckGrid(...);</b> вызывается в главном цикле <b>for [i] !!!</b> <br/>
        ///  Метод передвигает частицу <b>this</b> по её вектору скорости после проверки на столкновение. Алгоритм проверки: <br/>
        ///  1. Происходит перебор ближайших ячеек в сетке Grid по вектору движения круга <b>this</b> и кругов в этих ячейках без досрочных выходов из циклов; <br/>
        ///  2. Каждый круг проверяется на столкновение и ищется минимальный вектор скорости; <br/>
        ///  3. После смещения круга, происходит перезацепка индексов в <b>SIMULATION.Grid[,]</b> и в кортеже <b>this.Grid</b>; <br/><br/>
        ///  В результате частица смещается по вектору скорости на всю длину, если столкновений не обнаружено <br/> или останавливается в точке столкновения, не пройдя весь вектор скорости. <br/><br/>
        /// ЭТОТ МЕТОД ЭФФЕКТИВЕН, ЕСЛИ В ГЛАВНОМ ЦИКЛЕ НЕ НУЖНО ПЕРЕБИРАТЬ ВЕСЬ ВЛОЖЕННЫЙ СПИСОК Particles[j], <br/>
        /// ТО ЕСТЬ НЕ НУЖНО ПРИМЕНЯТЬ ВЛИЯНИЕ ЧАСТИЦЫ <b>pi</b> НА ВСЕ ОСТАЛЬНЫЕ ЧАСТИЦЫ <b>pj</b>. </summary>
        /// <value> 
        ///     <b><paramref name="link_SIMULATION"/>:</b> ссылка на объект симуляции. <br/>
        ///     <b>out <paramref name="part"/>:</b> ссылка на частицу, которая вызвала столкновение, если столкновения не было, возвращает <b>null</b>. <br/>
        /// </value>
        /// <returns> <inheritdoc cref="Physics2D.Collision"/> </returns>
        public bool MoveCheckGrid(cSIMULATION link_SIMULATION, out PARTICLE part) {
            part = null; Vector2Df newMinVelocity = Velocity2D; 
            if (Velocity2D.X == 0 && Velocity2D.Y == 0) return false;
            bool result = false;
            for (int n = 0; n < Grid.Item3.Count; n++) { //перебор ячеек сетки Grid которые круг пересекает в движении
                //перебор кругов: circle (pj) = Grid[x, y][n];
                foreach (PARTICLE pj in link_SIMULATION.Grid[Grid.Item3[n].X, Grid.Item3[n].Y]) {
                    if (this == pj) continue; if (pj == null || pj.Delete) continue;
                    (double, double)? Dot = Physics2D.GetCollisionPoint_Continious(this, pj);
                    if (Dot != null) {
                        var x = Dot.Value.Item1 - Position2D.X; var y = Dot.Value.Item2 - Position2D.Y;
                        if (x * x + y * y < newMinVelocity.X * newMinVelocity.X + newMinVelocity.Y * newMinVelocity.Y) {
                            newMinVelocity.X = x; newMinVelocity.Y = y;
                            part = pj;//part - это круг с которым произошло столкновение
                        }
                    }
                }
            }
            if (newMinVelocity != Velocity2D && part != null) { //СТОЛКНОВЕНИЕ БЫЛО!
                Move(newMinVelocity.X, newMinVelocity.Y);//смещаем круг this до точки столкновения
                result = true;
            } else Move();//смещем круг this на полный вектор, т.к. столкновения не обнаружено
            if (!Delete) link_SIMULATION.Remove_And_Add_Particle_In_AllCellGrid(this);
            return result;
        }

        /// <summary> Метод обновляет цвет частицы учитывая её массу. </summary>
        public void Update_Color() {
            HSV h; double V = 1 - Mass / MAX_START_MASS_PART;//чем больше число, тем светлее массивная частица
            var c = Color.FromArgb((int)(FillColor4.A * 255), (int)(FillColor4.R * 255),
                                   (int)(FillColor4.G * 255), (int)(FillColor4.B * 255));
            h = c.ColorToHSV(); h.Value = V;
            var c2 = h.HSVToColor(); 
            FillColor4 = new Color4(c2.R / 255f, c2.G / 255f, c2.B / 255f, c2.A / 255f);//float [0f..1f]
            //h.Value = 1.0 - h.Value; c2 = h.HSVToColor();
            OutlineColor4 = new Color4(FillColor4.R > 0 ? 1 - FillColor4.R : 0, FillColor4.G > 0 ? 1 - FillColor4.G : 0,
                FillColor4.B > 0 ? 1 - FillColor4.B : 0, FillColor4.A);
        }
    }
}
