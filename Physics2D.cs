using System;
using System.Collections.Generic;
using System.Drawing;
using UFO.Math;
using static UFO.Enums_Structs;

namespace UFO.Logic {
    /// <summary> Класс 2D физики, обрабатывающий взаимодействие частиц и прочую логику. </summary>
    public static class Physics2D {
		/// <summary> Метод вычисляет дистанцию между центрами двух кругов через корень квадратов. Работает с дробными числами. </summary>
		/// <returns> Возвращает длину типа <b>double</b> между двумя точками кругов в пикселях. </returns>
		public static double Distance(Vector2Df Left, Vector2Df Right) { return Left.Length(Right); }
		/// <summary> Метод вычисляет дистанцию между двумя точками кругов по быстрому методу (погрешность 3.5% из-за расчётов в целых числах). </summary>
		/// <remarks> Не использовать результат в расчётах, только в условиях! А лучше не использовать. </remarks>
		/// <returns> Возвращает длину типа <b>int</b> между двумя точками кругов в пикселях. </returns>
		public static int Fast_Distance(Vector2Df Left, Vector2Df Right) {
			int dx = (int)System.Math.Abs(Left.X - Right.X);
			int dy = (int)System.Math.Abs(Left.Y - Right.Y);
			int min = dx > dy ? dy : dx; return (dx + dy - (min >> 1) - (min >> 2) + (min >> 4));
		}

		/// <summary> Метод проверяет было ли столкновение кругов <b>Left</b> и <b>Right</b>. </summary>
		/// <returns> Возвращает <b>true</b> если произошло столкновение и <b>false</b> если нет. </returns>
		public static bool Collision(PARTICLE Left, PARTICLE Right) {
			return Left.Position2D.Length(Right.Position2D) < Left.Radius + Right.Radius;
			//return Distance(Left.Point, Right.Point) < Left.Radius + Right.Radius;
		}
		/// <summary> Метод проверяет было ли столкновение кругов по быстрому методу (погрешность 3.5% из-за расчётах в целых числах). </summary>
		/// <returns> Возвращает <b>true</b> если произошло столкновение и <b>false</b> если нет. </returns>
		public static bool Fast_Collision(PARTICLE Left, PARTICLE Right)	{
			return Fast_Distance(Left.Position2D, Right.Position2D) < Left.Radius + Right.Radius;
		}

		/// <summary> Метод вычисляет точку столкновения двух кругов с учётом их радиусов на отрезке (луче) движения частицы <b>Left</b>. <br/>
		/// Если нет прямого столкновения двух частиц на векторе скорости (луче) круга <b>left</b>, <br/> вычисляется столкновение вне отрезка (луча) движения частицы <b>Left</b> по его направлению движения. </summary>
		/// <returns> 1. Возвращает координату столкновения кругов <b>Left</b> и <b>Right</b> на отрезке движения круга <b>Left</b>, как точку типа <b>Vector2Df</b>. <br/>
		/// 2. Если прямого столкновения нет, возвращает координату за пределами луча по направлению вектора движения. <br/>
		/// Если оба поиска не дали точку столкновения, значит столкновения нет и метод возвращает <b>null</b>. </returns>
		public static (double, double)? GetCollisionPoint_Continious(PARTICLE left, PARTICLE right) {
			Vector2Df velocity = left.Velocity2D;
			var a = velocity.ScalarMultiplication(velocity);
			Vector2Df deltaCenter = left.Position2D - right.Position2D;
			var b = 2 * velocity.ScalarMultiplication(deltaCenter);
			var sumRadius = left.Radius + right.Radius;
			var c = deltaCenter.ScalarMultiplication(deltaCenter) - sumRadius * sumRadius;

			var discriminant = b * b - 4 * a * c;
			if (discriminant < 0) return null;//Нет столкновения
			//t = [0..1] 0 = 0%, 1 = 100%, 0.5 = 50% - коэффициент пройденного пути, % расстояния в котором круги сталкиваются
			var t = (-b - System.Math.Sqrt(discriminant)) / (2 * a);
			//if (t < 0 || t > 1) return null;//Столкновение происходит вне отрезка движения
			if (t < 0) return null;//Столкновение происходит ДО отрезка движения
			t -= 0.0001;//костыль: круг left чуть-чуть не докосается круга right, Collision() всегда = false!
			if (t > 0) return (left.Position2D.X + t * velocity.X, left.Position2D.Y + t * velocity.Y);//Столкновение происходит ПОСЛЕ отрезка движения
			//t += 0.0002;//костыль: круг left чуть-чуть перекрывает круг right, Collision() всегда = true!
			return (left.Position2D.X + t * velocity.X, left.Position2D.Y + t * velocity.Y);
		}

		/// <summary> Метод вытаскивает одну частицу из другой или обе друг из друга согласно параметру <b>dem</b>, если есть перекрытие радиусов окружностей. </summary>
		/// <remarks> Если круг <b>Left</b> вошёл в круг <b>Right</b>, дополнительно смещаем круг указанный в параметре <b>dem</b> на дельту перекрытия радиуса окружности. <br/>
		/// Метод плохо растаскивает окружности с параметрами: <b>Right</b>, <b>LeftRight</b>, <br/> потому что при переборе элементов отодвинутый кружок <b>Right</b> от <b>Left</b>-круга, <br/> снова придвинется к нему обратно, если до него дойдёт очередь в цикле и не предвинется, если цикл его уже проверил. </remarks>
		public static void Demarcation(PARTICLE Left, PARTICLE Right, Demarcation dem) {
			int count = 10;
			while (true) {
				double distance = Left.Position2D.Length(Right.Position2D); //расстояние между двумя точками
				if (distance - Left.Radius - Right.Radius < 0.0) {
					double overlap = (distance - Left.Radius - Right.Radius); double coeff;
					double dx = Left.Position2D.X - Right.Position2D.X; double dy = Left.Position2D.Y - Right.Position2D.Y;
					if (dx == 0 && dy == 0) { dx = 1; dy = 1; }//центр LEFT влетел в центр RIGHT
					if (distance == 0) distance = 1;
					//Логика: при перекрытии растаскиваемся на половину величины перекрытия вдоль линии соединения центров.
					//Если нужно сместить только один круг, то coeff = 1.0, если нужно сместить обоих, то coeff = 0.5
					if (dem == Enums_Structs.Demarcation.LeftRight)	coeff = 0.501; else coeff = 1.01f;//% растояния вхождения друг в друга [0..1]%
					if (dem == Enums_Structs.Demarcation.Left || dem == Enums_Structs.Demarcation.LeftRight) {
						Left.Position2D.X -= coeff * overlap * dx / distance;
						Left.Position2D.Y -= coeff * overlap * dy / distance;
					}
					if (dem == Enums_Structs.Demarcation.Right || dem == Enums_Structs.Demarcation.LeftRight) {
						Right.Position2D.X += coeff * overlap * dx / distance;
						Right.Position2D.Y += coeff * overlap * dy / distance;
					}
				}
				//int fd =0;
				//if (count < 10)
				//	fd = 1;
				if (!Collision(Left, Right) || count <= 0) break;
				count--;
			}
			//bool b = Collision(Left, Right);
			//int xx = 0;	if (b) xx = 1;
		}

		/// <summary> Абсолютно не упругий удар (слипание) с учётом массы и с нарушением закона сохранения импульса (импульс угасает). </summary>
		/// <remarks> Метод вычисляет новую скорость и направление двух слипшихся частиц. </remarks>
		/// <value> 
		///		<b><paramref name="Left"/>:</b> первая частица участвующая во взаимодействии. <br/>
		///		<b><paramref name="Right"/>:</b> вторая частица участвующая во взаимодействии. <br/>
		///	</value>
		public static void InelasticImpact(PARTICLE Left, PARTICLE Right) {
			var m1 = Left.Mass; var m2 = Right.Mass; if (m1 + m2 == 0) return;
			var b1vx = Left.Velocity2D.X; var b1vy = Left.Velocity2D.Y;
			var b2vx = Right.Velocity2D.X; var b2vy = Right.Velocity2D.Y;

			var vx = (m1 * b1vx + m2 * b2vx) / (m1 + m2);
			var vy = (m1 * b1vy + m2 * b2vy) / (m1 + m2);

			Left.Velocity2D.X = vx;	Right.Velocity2D.X = vx;
			Left.Velocity2D.Y = vy;	Right.Velocity2D.Y = vy;
		}

		/// <summary> Абсолютно упругий удар (отскок) с учётом массы и с СОБЛЮДЕНИЕМ закона сохранения импульса. <br/>
		///		Wikipedia version. Минимальный код. Отсечена вся избыточность. Результат = ElasticCollision3(); </summary>
		/// <remarks> <inheritdoc cref="ElasticImpact"/> </remarks>
		public static void ElasticImpact4(PARTICLE Left, PARTICLE Right) {
			if (Left.Mass + Right.Mass == 0) return;
			var b1vx = Left.Velocity2D.X; var b1vy = Left.Velocity2D.Y;
			var b2vx = Right.Velocity2D.X; var b2vy = Right.Velocity2D.Y;

			var fDistance = Distance(Left.Position2D, Right.Position2D); if (fDistance <= 0) return;//Distance between balls
																						     //normal
			var nx = (Right.Position2D.X - Left.Position2D.X) / fDistance;
			var ny = (Right.Position2D.Y - Left.Position2D.Y) / fDistance;
			//Wikipedia version
			var kx = (b1vx - b2vx); var ky = (b1vy - b2vy);
			var p = 2.0f * (nx * kx + ny * ky) / (Left.Mass + Right.Mass);
			//update ball velocities
			Left.Velocity2D.X = b1vx - p * Right.Mass * nx;	Left.Velocity2D.Y = b1vy - p * Right.Mass * ny;
			Right.Velocity2D.X = b2vx + p * Left.Mass * nx;	Right.Velocity2D.Y = b2vy + p * Left.Mass * ny;
		}

		/// <summary> Абсолютно упругий удар (отскок) с учётом массы и с СОБЛЮДЕНИЕМ закона сохранения импульса. </summary>
		/// <remarks> <inheritdoc cref="ElasticImpact"/> </remarks>
		public static void ElasticImpact3(PARTICLE Left, PARTICLE Right) {
			if (Left.Mass + Right.Mass == 0) return;
			var b1vx = Left.Velocity2D.X; var b1vy = Left.Velocity2D.Y;
			var b2vx = Right.Velocity2D.X; var b2vy = Right.Velocity2D.Y;

			var fDistance = Distance(Left.Position2D, Right.Position2D); if (fDistance <= 0) return;//Distance between balls
																							 //normal
			var nx = (Right.Position2D.X - Left.Position2D.X) / fDistance;
			var ny = (Right.Position2D.Y - Left.Position2D.Y) / fDistance;
			//tangent
			var tx = -ny; var ty = nx;
			//dot product tangent
			var dpTan1 = b1vx * tx + b1vy * ty;
			var dpTan2 = b2vx * tx + b2vy * ty;
			//dot product normal
			var dpNorm1 = b1vx * nx + b1vy * ny;
			var dpNorm2 = b2vx * nx + b2vy * ny;
			//conservation of momentum in 1D
			var m1 = (dpNorm1 * (Left.Mass - Right.Mass) + 2.0f * Right.Mass * dpNorm2) / (Left.Mass + Right.Mass);
			var m2 = (dpNorm2 * (Right.Mass - Left.Mass) + 2.0f * Left.Mass * dpNorm1) / (Left.Mass + Right.Mass);

			//update ball velocities
			Left.Velocity2D.X = tx * dpTan1 + nx * m1;	Left.Velocity2D.Y = ty * dpTan1 + ny * m1;
			Right.Velocity2D.X = tx * dpTan2 + nx * m2;	Right.Velocity2D.Y = ty * dpTan2 + ny * m2;
		}

		/// <summary> Абсолютно упругий удар (отскок) с учётом массы и с СОБЛЮДЕНИЕМ закона сохранения импульса. <br/>
		///		Для этого способа важно чтобы в момент удара между кругами расстояние равнялось нулю. Если этого не сделать, скорость будет расти.
		/// </summary>
		/// <remarks> <inheritdoc cref="ElasticImpact"/> </remarks>
		public static void ElasticImpact2(PARTICLE Left, PARTICLE Right)	{
			if (Left.Mass == 0 || Right.Mass == 0) return;
			var dx = Left.Position2D.X - Right.Position2D.X; var dy = Left.Position2D.Y - Right.Position2D.Y;
			var vx1 = Left.Velocity2D.X; var vy1 = Left.Velocity2D.Y;
			var vx2 = Right.Velocity2D.X; var vy2 = Right.Velocity2D.Y;
			var dvx = vx2 - vx1; var dvy = vy2 - vy1;
			var dvdr = dx * dvx + dy * dvy;
			var dist = Left.Radius + Right.Radius; if (dist <= 0) return;
			// magnitude of normal force
			var magnitude = 2 * Left.Mass * Right.Mass * dvdr / ((Left.Mass + Right.Mass) * dist);
			// normal force, and in x and y directions
			var fx = magnitude * dx / dist;	var fy = magnitude * dy / dist;
			// update velocities according to normal force
			Left.Velocity2D.X += fx / Left.Mass;	Left.Velocity2D.Y += fy / Left.Mass;
			Right.Velocity2D.X -= fx / Right.Mass;	Right.Velocity2D.Y -= fy / Right.Mass;
		}

		/// <summary> Абсолютно упругий удар (отскок) без учёта массы и с СОБЛЮДЕНИЕМ закона сохранения импульса. </summary>
		/// <remarks> Метод вычисляет новую скорость и направление двух частиц после отскока. </remarks>
		public static void ElasticImpact(PARTICLE Left, PARTICLE Right) {
			if (Left.Mass == 0 || Right.Mass == 0) return;
			var dist = Distance(Left.Position2D, Right.Position2D); if (dist <= 0) return;
			var Sin = (Left.Position2D.X - Right.Position2D.X) / dist; //sin угла
			var Cos = (Left.Position2D.Y - Right.Position2D.Y) / dist; //cos угла
			var vx1 = Left.Velocity2D.X; var vy1 = Left.Velocity2D.Y;
			var vx2 = Right.Velocity2D.X; var vy2 = Right.Velocity2D.Y;

			//вычисляем новые XY компоненты направления
			var Vn1 = vx1 * Sin + vy1 * Cos; var Vn2 = vx2 * Sin + vy2 * Cos;
			var Vt1 = -vx1 * Cos + vy1 * Sin; var Vt2 = -vx2 * Cos + vy2 * Sin;
			var a = Vn2; Vn2 = Vn1; Vn1 = a;
			//вычисляем XY компоненты скорости
			Left.Velocity2D.X = Vn1 * Sin - Vt1 * Cos;	Left.Velocity2D.Y = Vn1 * Cos + Vt1 * Sin;
			Right.Velocity2D.X = Vn2 * Sin - Vt2 * Cos;	Right.Velocity2D.Y = Vn2 * Cos + Vt2 * Sin;
		}

		/// <summary> Реальный удар. Промежуточные варианты между отскоком и слипанием. <br/> С учётом массы и нарушением закона сохранения импульса (импульс угасает). <br/> Каждая частица имеет свой коэффициент double <b>K</b> [0..1], который влияет на степень упругости взаимодествия. </summary>
		public static void Real_Impact(PARTICLE Object1, PARTICLE Object2)	{
			var m1 = Object1.Mass; var m2 = Object2.Mass;	if (m1 + m2 == 0) return;
			var K1 = Object1.Elasticity; var K2 = Object2.Elasticity;
			var v1 = Object1.Velocity2D; var v2 = Object2.Velocity2D;

			//вычисляем скорость после удара
			Object1.Velocity2D.X = v1.X - (1f + K1) * (m2 / (m1 + m2)) * (v1.X - v2.X);
			Object1.Velocity2D.Y = v1.Y - (1f + K1) * (m2 / (m1 + m2)) * (v1.Y - v2.Y);
			Object2.Velocity2D.X = v2.X + (1f + K2) * (m1 / (m1 + m2)) * (v1.X - v2.X);
			Object2.Velocity2D.Y = v2.Y + (1f + K2) * (m1 / (m1 + m2)) * (v1.Y - v2.Y);
		}

		/// <summary> Абсолютно упругий удар (отскок) — модель соударения, при которой полная кинетическая энергия системы сохраняется. <br/>
		///		Хорошим приближением к модели абсолютно упругого удара является столкновение бильярдных шаров или упругих мячиков.
		/// </summary>
		public static void ElasticImpact5(PARTICLE Object1, PARTICLE Object2) {
			var m1 = Object1.Mass; var m2 = Object2.Mass;	if (m1 + m2 == 0) return;
			var v1 = Object1.Velocity2D; var v2 = Object2.Velocity2D;

			//вычисляем скорость после удара
			var speedX1 = (2 * m2 * v2.X + v1.X * (m1 - m2)) / (m1 + m2);
			var speedY1 = (2 * m2 * v2.Y + v1.Y * (m1 - m2)) / (m1 + m2);
			var speedX2 = (2 * m1 * v1.X + v2.X * (m2 - m1)) / (m1 + m2);
			var speedY2 = (2 * m1 * v1.Y + v2.Y * (m2 - m1)) / (m1 + m2);

			var dx = Object2.Position2D.X - Object1.Position2D.X; //дельта X
			var dy = Object2.Position2D.Y - Object1.Position2D.Y; //дельта Y
			var dist = (float)System.Math.Sqrt(dx * dx + dy * dy);
			if (dist == 0) dist = 0.01f;//во избежание деления на ноль 
			var ax = dx / dist; var ay = dy / dist;

			var Vn1 = speedX1 * ax + speedY1 * ay; var Vn2 = speedX2 * ax + speedY2 * ay;
			var Vt1 = speedX1 * ay + speedY1 * ax; var Vt2 = speedX2 * ay + speedY2 * ax;

			var VN1 = Vn1 * 0.5f - Vn2 * 0.5f; speedX1 = VN1 * ax - Vt1 * ay; speedY1 = VN1 * ay + Vt1 * ax;
			var VN2 = Vn2 * 0.5f - Vn1 * 0.5f; speedX2 = VN2 * ax - Vt2 * ay; speedY2 = VN2 * ay + Vt2 * ax;

			Object1.Velocity2D.X = speedX1;	Object2.Velocity2D.X = speedX2;
			Object1.Velocity2D.Y = speedY1;	Object2.Velocity2D.Y = speedY2;
		}

		/// <summary> Абсолютно упругий удар (отскок) с учётом массы и СОБЛЮДЕНИЕМ закона сохранения импульса. </summary>
		/// <remarks> Столкновение двух движущихся тел. Отскок не правдоподобный в некоторых случаях. </remarks>
		public static void InMovingImpact(PARTICLE Object1, PARTICLE Object2) {
			//угол соприкосновения
			//var tmp1 = new Vector2Df(); tmp1.Copy(Object1.Velocity); tmp1.Normalize();
			//var tmp2 = new Vector2Df(); tmp2.Copy(Object2.Velocity); tmp2.Normalize();
			var tmp1 = Object1.Velocity2D; tmp1.Normalize();
			var tmp2 = Object2.Velocity2D; tmp2.Normalize();
			var q = Object1.Position2D.AngleBetween(tmp1, tmp2);
			var m1 = Object1.Mass; var m2 = Object2.Mass;
			Object1.AngleFromVelocity(); Object2.AngleFromVelocity();
			var Q1 = Object1.Angle; var Q2 = Object2.Angle;
			var v1 = Object1.Velocity2D; var v2 = Object2.Velocity2D;
            try {
				Object1.Velocity2D.X = (float)((v1.X * System.Math.Cos(Q1 - q) * (m1 - m2) + 2 * m2 * v2.X * System.Math.Cos(Q2 - q)) /
						(m1 + m2) * System.Math.Cos(q) + v1.X * System.Math.Sin(Q1 - q) * System.Math.Cos(q + System.Math.PI / 2));
				Object1.Velocity2D.Y = (float)((v1.Y * System.Math.Cos(Q1 - q) * (m1 - m2) + 2 * m2 * v2.Y * System.Math.Cos(Q2 - q)) /
						(m1 + m2) * System.Math.Sin(q) + v1.Y * System.Math.Sin(Q1 - q) * System.Math.Sin(q + System.Math.PI / 2));

				Object2.Velocity2D.X = (float)((v2.X * System.Math.Cos(Q2 - q) * (m2 - m1) + 2 * m1 * v1.X * System.Math.Cos(Q1 - q)) /
						(m2 + m1) * System.Math.Cos(q) + v2.X * System.Math.Sin(Q2 - q) * System.Math.Cos(q + System.Math.PI / 2));
				Object2.Velocity2D.Y = (float)((v2.Y * System.Math.Cos(Q2 - q) * (m2 - m1) + 2 * m1 * v1.Y * System.Math.Cos(Q1 - q)) /
						(m2 + m1) * System.Math.Sin(q) + v2.Y * System.Math.Sin(Q2 - q) * System.Math.Sin(q + System.Math.PI / 2));
			}
			catch { return; }
		}

		//========================================================================================================================
		//=================================== Описание 4 фундаментальных взаимодействий частиц ===================================
		/// <summary> СИЛЬНОЕ ВЗАИМОДЕЙСТВИЕ. <br/> Вычисление влияния сильного взаимодействия тела obj2 на obj1. Сильнее электромагнетизма, но малый радиус действия. <br/>
		///		Притяжение усиливается с растоянием, радиус действия: <b>KSd</b>. </summary>
		///		<remarks> Метод не выполняет свой код, если частица далеко и данная сила на неё не действует. </remarks>
		/// <value> 
		///		<b><paramref name="obj1"/>:</b> первая частица участвующая во взаимодействии. <br/>
		///		<b><paramref name="obj2"/>:</b> вторая частица участвующая во взаимодействии. <br/>
		///		<b><paramref name="SIM"/>:</b> объект симуляции. <br/>
		///	</value>
		public static void StrongInteraction(PARTICLE obj1, PARTICLE obj2, cSIMULATION SIM) {
			//формула "Потенциал Юкавы":	V = Fw^2 * (exp(Ks * R) / d)
			//формула "Потенциал Юкавы" 2 вариант: U(r) = Ks * (exp(R / d) / R)
			//формула радиуса ядра: R = d * A^1.3
			//где V, U(r) - сила притягивания, Fw - константа, задающая интенсивность ядерного взаимодействия
			//Ks - константа сильного взаимодействия, R - радиус ядра, d - дистанция между частицами, A - кол-во нуклонов в ядре

			var r1 = obj1.Radius; var r2 = obj2.Radius; var d = Distance(obj1.Position2D, obj2.Position2D); if (d <= 0) return;//дистанция
			if (d > SIM.KSd) return; //ограничиваем дальность действия
			if (r1 <= 0) r1 = 1.0f; if (r2 <= 0) r2 = 1.0f; 
			var dx = obj2.Position2D.X - obj1.Position2D.X; var dy = obj2.Position2D.Y - obj1.Position2D.Y;
			//величина сильного взаимодействия. сила притягивания.
			var Ur = SIM.Fw * d / (r1 + r2); 
			if (d > r1 + r2) { //если нет касания
				obj1.Velocity2D.X += Ur * dx / d; obj1.Velocity2D.Y += Ur * dy / d;
			} else { obj1.Velocity2D.X = 0; obj1.Velocity2D.Y = 0; }
		}

		/// <summary> ЭЛЕКТРОМАГНИТНОЕ ВЗАИМОДЕЙСТВИЕ. <br/> Вычисление электромагнитного влияния тела obj2 на obj1. В разы сильнее гравитации. <br/>
		///		Противоположные заряды притягиваются / одинаковые заряды отталкиваются / на нейтральные заряды электромагнитизм не действует. <br/> 
		///		Действи ослабевает с расстоянием, радиус действия: <b>KEd</b>. </summary>
		///		<remarks> Метод не выполняет свой код, если частица далеко и данная сила на неё не действует. </remarks>
		/// <value> <inheritdoc cref="StrongInteraction"/> </value>
		public static void Electromagnetism(PARTICLE obj1, PARTICLE obj2, cSIMULATION SIM) {
			if (obj1 == null || obj2 == null) return;
			//формула Кулона аналогична формуле из Gravity(). При этом роль тяжёлых масс играют электрические заряды q1 и q2
			//формула 1: F = (K (q1 * q2) / |r12^3|) * r12		формула 2: gH = F / q1
			//где F - сила общего взаимодействия с которой заряды действуют на друг друга
			//К - коэффициент пропорциональности типа гравитационной постоянной
			//q1, q2 - величина зарядов, r12 - радиус-вектор. расстояние между центрами частиц, |r12^3| - модуль радиус-вектора в кубе (d^3)
			if (obj1.Charge == 0 || obj2.Charge == 0) return;//заряд нейтральный. электромагнитного взаимодействия нет

			var d = Distance(obj1.Position2D, obj2.Position2D); if (d <= 0) return;//дистанция
			if (d > SIM.KEd) return; //ограничиваем дальность действия
			var q1 = obj1.Charge; var q2 = obj2.Charge;
			var dx = obj2.Position2D.X - obj1.Position2D.X; var dy = obj2.Position2D.Y - obj1.Position2D.Y;

			var F = SIM.Kp * (q1 * q2) / (d * d);//сила взаимодействия
			var gH = F / q1;//величина электромагнитного ускорения на высоте h. чем дальше от объекта, тем слабее

			if (d > obj1.Radius + obj2.Radius) { //если нет касания
				obj1.Velocity2D.X -= gH * dx / d; obj1.Velocity2D.Y -= gH * dy / d;//отталкивание, заряды с одинаковым знаком 
			} else { obj1.Velocity2D.X = 0; obj1.Velocity2D.Y = 0; }
		}

		//ГРАВИТАЦИЯ. Вычисление гравитационного влияния тела obj2 на obj1 
		/// <summary> ГРАВИТАЦИЯ. <br/> Вычисление гравитационного влияния тела obj2 на obj1. <br/>
		///		Притяжение ослабевает с расстоянием, радиус действия: <b>KGd</b>. </summary>
		///		<remarks> Метод не выполняет свой код, если частица далеко и данная сила на неё не действует. </remarks>
		/// <value> <inheritdoc cref="StrongInteraction"/> </value>
		public static void Gravity(PARTICLE obj1, PARTICLE obj2, cSIMULATION SIM) {
			//формула 1: F = G * (m1 * m2) / r^2		формула 2: gH = F / m1
			//где F - общая сила притяжения obj1 и obj2, G - гравитационная постоянна, m1,m2 - масса obj1 и obj2,
			//r - расстояние между центрами частиц

			var d = Distance(obj1.Position2D, obj2.Position2D); if (d <= 0) return;//дистанция
			if (d > SIM.KGd) return;//ограничиваем дальность действия
			var m1 = obj1.Mass; var m2 = obj2.Mass;
			var dx = obj2.Position2D.X - obj1.Position2D.X; var dy = obj2.Position2D.Y - obj1.Position2D.Y;

			var F = SIM.G * (m1 * m2) / (d * d);//сила взаимного притяжения
			var gH = F / m1;//гравитационное ускорение свободного падения на высоте h. чем дальше от объекта, тем слабее притяжение
			if (d > obj1.Radius + obj2.Radius) { //если нет касания
				obj1.Velocity2D.X += gH * dx / d; obj1.Velocity2D.Y += gH * dy / d;
			} else { obj1.Velocity2D.X = 0; obj1.Velocity2D.Y = 0; }
		}
		//=================================== Описание 4 фундаментальных взаимодействий частиц ===================================
		//========================================================================================================================

		/// <summary> Метод работает в 2 режимах: <br/>
		///		<b>1. Type_Particle.Полное_Поглощение:</b> осуществляет слияние 2 коснувшихся частиц в одну. Осуществляется проверка: если импульс obj1 больше импульса obj2, то obj1 поглощает obj2 <br/> и поглощённая частица помечается как удаляемая, но из списка частиц в этом методе НЕ удаляется! <br/>
		///		<b>2. Type_Particle.Слабое_Поглощение:</b> осуществляет порционное высасывание массы объектом 1 у объекта 2, если у объекта 1 импульс больше чем у объекта 2. Если <b>obj2</b> выжат полностью, он помечается как удаляемый, но из списка частиц в этом методе НЕ удаляется!
		///	</summary>
		/// <value> <inheritdoc cref="InelasticImpact"/>
		/// <b><paramref name="Mode"/>:</b> перечисление режима работы метода, принимает 2 варианта: <b>Полное_Поглощение</b> или <b>Слабое_Поглощение.</b> <br/>
		/// </value>
		/// <returns> Возвращает <b>true</b> если слияние произошло и <b>false</b> если слияния не было. </returns>
		public static bool Collapse(PARTICLE obj1, PARTICLE obj2, Type_Particle Mode) {
			try { //обёртка из-за многопоточности
				var impuls_1 = obj1.Mass * obj1.GetSpeed() + obj1.Mass; 
				var impuls_2 = obj2.Mass * obj2.GetSpeed() + obj2.Mass;
				if (impuls_1 > impuls_2) {
					//СИЛЬНОЕ ПОГЛОЩЕНИЕ
					if (Mode == Type_Particle.Полное_Поглощение) { 
						obj1.Mass += obj2.Mass; obj1.Charge += obj2.Charge; obj1.Update_Color(); obj2.Delete = true;
					} 
					//СЛАБОЕ ПОГЛОЩЕНИЕ
					else if (Mode == Type_Particle.Слабое_Поглощение) {
						if (obj2.Mass > 1) { obj1.Mass += 1.0f; obj2.Mass -= 1.0f;
							if (obj2.Charge > 1) { obj1.Charge += 1.0f; obj2.Charge -= 1.0f; }
							else { obj1.Charge += obj2.Charge; obj2.Charge = 0; }
						}
						else { obj1.Mass += obj2.Mass; obj2.Delete = true; }
						obj1.Update_Color();
                    }
					//коррекция позиции obj1 после слияния
					if (obj2.Delete) {
						var NewMass = obj1.Mass + obj2.Mass;
						var dx = obj2.Position2D.X - obj1.Position2D.X; var dy = obj2.Position2D.Y - obj1.Position2D.Y;
						obj1.Position2D.X += (dx * obj2.Mass) / NewMass;
						obj1.Position2D.Y += (dy * obj2.Mass) / NewMass;
						return true;
					} else return false;
				} else return false;
			}
			catch { return false; } //выходим из метода если что-то пошло не так
		}

		/// <summary> Метод осуществляет связь двух частиц. <br/> Ведущая частица = obj2, ведомая = obj1. <br/> Если обе частицы имеют тип <b>Связь</b>, то они могут образовать двойную связь друг на друга. </summary>
		/// <value> <inheritdoc cref="StrongInteraction"/> </value>
		/// <returns> Возвращает <b>true</b> если связь установлена и <b>false</b> если нет. </returns>
		public static bool Coupling(PARTICLE obj1, PARTICLE obj2) {
			//if (obj2.CountCoupling >= obj2.LinkPart.Length) return false;
			int n = -1;	for (int i = 0; i < obj1.LinkPart.Length; i++) if (obj1.LinkPart[i] == null) { n = i; break; }
			if (n < 0) return false;
			//var dist = Distance(obj1.Position, obj2.Position);
			//if (dist < obj1.ValueCoupling && dist > 0) {
				obj1.LinkPart[n] = obj2; obj2.CountCoupling++; return true;
			//} else return false;
		}
		/// <summary> Метод осуществляет взаимодействие двух связанных частиц, ведомой и ведущей. Ведомая следует за ведущей. </summary>
		public static void MooveCoupling(PARTICLE obj1, ref PARTICLE LinkPart, double KFC) {
			var dist = Distance(obj1.Position2D, LinkPart?.Position2D ?? obj1?.Position2D ?? new Vector2Df());
			if (dist < obj1.ValueCoupling && dist > 0) { //на этой дистанции частица 1 следует за связанной частицей 2
				var LPX = LinkPart?.Position2D.X ?? obj1.Position2D.X;
				var LPY = LinkPart?.Position2D.Y ?? obj1.Position2D.Y;
				var dx = LPX - obj1.Position2D.X; var dy = LPY - obj1.Position2D.Y;
				var r1 = obj1.Radius; var r2 = LinkPart?.Radius ?? 0;
				//if (dx == 0 && dy == 0) return;
				var Ur = KFC * dist;
				if (dist > r1 + r2) { obj1.Velocity2D.X += Ur * dx;	obj1.Velocity2D.Y += Ur * dy;	}
				//else { obj1.Velocity.X = 0; obj1.Velocity.Y = 0; }
			} else { 
				LinkPart.CountCoupling--; LinkPart = null; 
			}//на этой дистанции связь разрывается
		}

		//======================================= МЕТОДЫ ИЗ ИНТЕРНЕТА =======================================
		/// <summary> Метод применяет правило притяжения/отталкивания, основанное на силе <b>g</b>, масса роли не играет! <br/> Частица obj2 осуществляет влияние на частицу obj1. <br/> Чтобы частица 2 влияла на частицу 1 через массу, следует в вызове метода передавать параметр <b>g</b> через массу. <br/> Правило действует только на те частицы, которые прописаны типом <b>tp1/tp2.</b> </summary>
		/// <value>
		/// <b><paramref name="tp1"/>:</b> тип цвета частицы obj1 на которую действует правило. <br/>
		/// <b><paramref name="tp2"/>:</b> тип цвета частицы obj2 которая влияет на obj1. <br/>
		/// <b><paramref name="g"/>:</b> сила влияния частицы obj2, 0 = нет влияния, +N = obj2 притягивает obj1, -N = obj2 отталкивает obj1. <br/>
		/// <b><paramref name="dist"/>:</b> дистанция в пикселях на которой действует заданное правило. <br/>
		/// </value>
		public static void Rule(PARTICLE obj1, PARTICLE obj2, ColorType tp1, ColorType tp2, float g, float dist) {
			if (obj1.pColor != tp1 || obj2.pColor != tp2) return;
			var d = Distance(obj1.Position2D, obj2.Position2D);
			if (d > 0 && d <= dist) {
				if (d > obj1.Radius + obj2.Radius) {
					var dx = obj2.Position2D.X - obj1.Position2D.X; var dy = obj2.Position2D.Y - obj1.Position2D.Y;
					var F = g / d;
					obj1.Velocity2D.X += F * dx; obj1.Velocity2D.Y += F * dy;
				} //else { pi.Velocity2D.X *= 0.5f; pi.Velocity2D.Y *= 0.5f; }
			}
		}

		public static void Rule_For(List<PARTICLE> List1, List<PARTICLE> List2, float g, float dist) {
            foreach (var pi in List1) {
				foreach (var pj in List2) {
					if (pi == pj) continue;
					//var d = pi.Position.Length(pj.Position);// Distance(obj1.Position, obj2.Position);
					var d = Distance(pi.Position2D, pj.Position2D);
					if (d > 0 && d <= dist) { //ограничиваем дальность действия
						if (d > pi.Radius + pj.Radius) {
							var dx = pj.Position2D.X - pi.Position2D.X;	var dy = pj.Position2D.Y - pi.Position2D.Y;
							var F = g / d;
							pi.Velocity2D.X += F * dx;	pi.Velocity2D.Y += F * dy;
						} //else { pi.Velocity.X *= 0.5f; pi.Velocity.Y *= 0.5f; }
					}
					if (pi.pColor != ColorType.White && pi.pColor != ColorType.Aqua && pj.pColor == ColorType.Aqua && pj.CountCoupling < 1)
						Coupling(pi, pj);
					//if (Collision(pi, pj)) Demarcation(pi, pj);
				}
			}
		}
		//======================================= МЕТОДЫ ИЗ ИНТЕРНЕТА =======================================
	}
}
