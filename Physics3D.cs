using System;
using System.Collections.Generic;
using System.Drawing;
using UFO.Math;
using static UFO.Enums_Structs;

namespace UFO.Logic {
	/// <summary> Класс 3D физики, обрабатывающий взаимодействие частиц и прочую логику. </summary>
	public static class Physics3D {
		/// <summary> Метод вычисляет дистанцию между центрами двух частиц через корень квадратов. Работает с дробными числами. </summary>
		/// <returns> Возвращает длину типа <b>double</b> между двумя точками частиц в пикселях. </returns>
		public static double Distance(Vector3Df Left, Vector3Df Right) { return Left.Length(Right); }
		/// <summary> Метод вычисляет дистанцию между двумя точками частиц по быстрому методу (погрешность 3.5% из-за расчётов в целых числах). </summary>
		/// <remarks> Не использовать результат в расчётах, только в условиях! А лучше не использовать. </remarks>
		/// <returns> Возвращает длину типа <b>int</b> между двумя точками частиц в пикселях. </returns>
		public static int Fast_Distance(Vector3Df Left, Vector3Df Right) {
			int dx = (int)System.Math.Abs(Left.X - Right.X);
			int dy = (int)System.Math.Abs(Left.Y - Right.Y);
			int dz = (int)System.Math.Abs(Left.Z - Right.Z);
			int min = dx > dy ? dy > dz ? dz : dy : dx;
			return (dx + dy + dz - (min >> 1) - (min >> 2) + (min >> 4));
		}

		/// <summary> Метод проверяет было ли столкновение частиц. </summary>
		/// <returns> Возвращает <b>true</b> если произошло столкновение и <b>false</b> если нет. </returns>
		public static bool Collision(PARTICLE Left, PARTICLE Right)	{
			return Left.Position3D.Length(Right.Position3D) < Left.Radius + Right.Radius;
			//return Distance(Left.Point, Right.Point) < Left.Radius + Right.Radius;
		}
		/// <summary> Метод проверяет было ли столкновение частиц по быстрому методу (погрешность 3.5% из-за расчётах в целых числах). </summary>
		/// <returns> Возвращает <b>true</b> если произошло столкновение и <b>false</b> если нет. </returns>
		public static bool Fast_Collision(PARTICLE Left, PARTICLE Right) {
			return Fast_Distance(Left.Position3D, Right.Position3D) < Left.Radius + Right.Radius;
		}

		/// <summary> Метод вытаскивает частицу <b>Left</b> из частицы <b>Right</b>, если она вошла внутрь окружности <b>Right</b>. </summary>
		/// <remarks> Если круг <b>Left</b> вошёл в круг <b>Right</b>, дополнительно смещаем круг <b>Left</b> в обратном направлении на дельту. </remarks>
		public static void Demarcation(PARTICLE Left, PARTICLE Right) {
			//while (true) {
			double distance = Left.Position3D.Length(Right.Position3D); //расстояние между двумя точками
			if (distance - Left.Radius - Right.Radius < 0.0) {
				double overlap = 0.52 * (distance - Left.Radius - Right.Radius); //0.6 растояния вхождения друг в друга
				 //double overlap = 1.25f * (distance - Left.Radius - Right.Radius);
				double dx = Left.Position3D.X - Right.Position3D.X;
				double dy = Left.Position3D.Y - Right.Position3D.Y;
				double dz = Left.Position3D.Z - Right.Position3D.Z;
				if (dx == 0) dx = 1; if (dy == 0) dy = 1; if (dz == 0) dz = 1; if (distance == 0) distance = 1;
				//Логика: при перекрытии растаскиваемся на половину величины перекрытия вдоль линии соединения центров.
				//Если нужно сместить только первого, то последнюю строчку закомментировть, а в overlap 0.5 заменить на 1.0
				Left.Position3D.X -= overlap * dx / distance;
				Left.Position3D.Y -= overlap * dy / distance;
				Left.Position3D.Z -= overlap * dz / distance;
				Right.Position3D.X += overlap * dx / distance;
				Right.Position3D.Y += overlap * dy / distance;
				Right.Position3D.Z += overlap * dz / distance;
			}
			//if (!Collision(Left, Right)) break;
			//}
			//var dd = Left.Position3D.Length(Right.Position3D);
			//bool b = Collision(Left, Right);
			//int xx = 0;
			//if (b) xx = 1;
		}

		/// <summary> Абсолютно не упругий удар (слипание) с учётом массы и с нарушением закона сохранения импульса (импульс угасает). </summary>
		/// <remarks> Метод вычисляет новую скорость и направление двух слипшихся частиц. </remarks>
		/// <value> 
		///		<b><paramref name="Left"/>:</b> первая частица участвующая во взаимодействии. <br/>
		///		<b><paramref name="Right"/>:</b> вторая частица участвующая во взаимодействии. <br/>
		///	</value>
		public static void InelasticImpact(PARTICLE Left, PARTICLE Right) {
			var m1 = Left.Massa; var m2 = Right.Massa; if (m1 + m2 == 0) return;
			var b1vx = Left.Velocity3D.X; var b1vy = Left.Velocity3D.Y; var b1vz = Left.Velocity3D.Z;
			var b2vx = Right.Velocity3D.X; var b2vy = Right.Velocity3D.Y; var b2vz = Right.Velocity3D.Z;

			var vx = (m1 * b1vx + m2 * b2vx) / (m1 + m2);
			var vy = (m1 * b1vy + m2 * b2vy) / (m1 + m2);
			var vz = (m1 * b1vz + m2 * b2vz) / (m1 + m2);

			Left.Velocity3D.X = vx; Right.Velocity3D.X = vx;
			Left.Velocity3D.Y = vy; Right.Velocity3D.Y = vy;
			Left.Velocity3D.Z = vz; Right.Velocity3D.Z = vz;
		}

		/// <summary> Абсолютно упругий удар (отскок) с учётом массы и с СОБЛЮДЕНИЕМ закона сохранения импульса. <br/>
		///		Wikipedia version. Минимальный код. Отсечена вся избыточность. Результат = ElasticCollision3(); </summary>
		/// <remarks> <inheritdoc cref="ElasticImpact"/> </remarks>
		public static void ElasticImpact4(PARTICLE Left, PARTICLE Right) {
			if (Left.Massa + Right.Massa == 0) return;
			var b1vx = Left.Velocity3D.X; var b1vy = Left.Velocity3D.Y; var b1vz = Left.Velocity3D.Z;
			var b2vx = Right.Velocity3D.X; var b2vy = Right.Velocity3D.Y; var b2vz = Right.Velocity3D.Z;

			var fDistance = Distance(Left.Position3D, Right.Position3D); if (fDistance <= 0) return;//Distance between balls
																								//normal
			var nx = (Right.Position3D.X - Left.Position3D.X) / fDistance;
			var ny = (Right.Position3D.Y - Left.Position3D.Y) / fDistance;
			var nz = (Right.Position3D.Z - Left.Position3D.Z) / fDistance;
			//Wikipedia version
			var kx = (b1vx - b2vx); var ky = (b1vy - b2vy); var kz = (b1vz - b2vz);
			var p = 2.0f * (nx * kx + ny * ky + nz * kz) / (Left.Massa + Right.Massa);
			//update ball velocities
			Left.Velocity3D.X = b1vx - p * Right.Massa * nx;
			Left.Velocity3D.Y = b1vy - p * Right.Massa * ny;
			Left.Velocity3D.Z = b1vz - p * Right.Massa * nz;
			Right.Velocity3D.X = b2vx + p * Left.Massa * nx;
			Right.Velocity3D.Y = b2vy + p * Left.Massa * ny;
			Right.Velocity3D.Z = b2vz + p * Left.Massa * nz;
		}

		/// <summary> Абсолютно упругий удар (отскок) с учётом массы и с СОБЛЮДЕНИЕМ закона сохранения импульса. </summary>
		/// <remarks> <inheritdoc cref="ElasticImpact"/> </remarks>
		public static void ElasticImpact3(PARTICLE Left, PARTICLE Right) {
			if (Left.Massa + Right.Massa == 0) return;
			var b1vx = Left.Velocity3D.X; var b1vy = Left.Velocity3D.Y; var b1vz = Left.Velocity3D.Z;
			var b2vx = Right.Velocity3D.X; var b2vy = Right.Velocity3D.Y; var b2vz = Right.Velocity3D.Z;

			var fDistance = Distance(Left.Position3D, Right.Position3D); if (fDistance <= 0) return;//Distance between balls
																								//normal
			var nx = (Right.Position3D.X - Left.Position3D.X) / fDistance;
			var ny = (Right.Position3D.Y - Left.Position3D.Y) / fDistance;
			var nz = (Right.Position3D.Z - Left.Position3D.Z) / fDistance;
			//tangent (касательная)
			var tx = -ny;
			var ty = nx;
			var tz = nz;
			//dot product tangent (скалярное произведение с касательной)
			var dpTan1 = b1vx * tx + b1vy * ty + b1vz * tz;
			var dpTan2 = b2vx * tx + b2vy * ty + b2vz * tz;
			//dot product normal (скалярное произведение с нормалью)
			var dpNorm1 = b1vx * nx + b1vy * ny + b1vz * nz;
			var dpNorm2 = b2vx * nx + b2vy * ny + b2vz * nz;
			//conservation of momentum in 1D (сохранение импульса в 3D)
			var m1 = (dpNorm1 * (Left.Massa - Right.Massa) + 2.0f * Right.Massa * dpNorm2) / (Left.Massa + Right.Massa);
			var m2 = (dpNorm2 * (Right.Massa - Left.Massa) + 2.0f * Left.Massa * dpNorm1) / (Left.Massa + Right.Massa);
			//update ball velocities
			Left.Velocity3D.X = tx * dpTan1 + nx * m1; 
			Left.Velocity3D.Y = ty * dpTan1 + ny * m1;
			Left.Velocity3D.Z = tz * dpTan1 + nz * m1;
			Right.Velocity3D.X = tx * dpTan2 + nx * m2;
			Right.Velocity3D.Y = ty * dpTan2 + ny * m2;
			Right.Velocity3D.Z = tz * dpTan2 + nz * m2;
		}

		/// <summary> Абсолютно упругий удар (отскок) с учётом массы и с СОБЛЮДЕНИЕМ закона сохранения импульса. <br/>
		///		Для этого способа важно чтобы в момент удара между кругами расстояние равнялось нулю. Если этого не сделать, скорость будет расти.
		/// </summary>
		/// <remarks> <inheritdoc cref="ElasticImpact"/> </remarks>
		public static void ElasticImpact2(PARTICLE Left, PARTICLE Right) {
			if (Left.Massa == 0 || Right.Massa == 0) return;
			var dx = Left.Position3D.X - Right.Position3D.X; 
			var dy = Left.Position3D.Y - Right.Position3D.Y;
			var dz = Left.Position3D.Z - Right.Position3D.Z;
			var vx1 = Left.Velocity3D.X; var vy1 = Left.Velocity3D.Y; var vz1 = Left.Velocity3D.Z;
			var vx2 = Right.Velocity3D.X; var vy2 = Right.Velocity3D.Y; var vz2 = Right.Velocity3D.Z;
			var dvx = vx2 - vx1; var dvy = vy2 - vy1; var dvz = vz2 - vz1;
			var dvdr = dx * dvx + dy * dvy + dz * dvz;
			var dist = Left.Radius + Right.Radius; if (dist <= 0) return;
			// magnitude of normal force
			var magnitude = 2 * Left.Massa * Right.Massa * dvdr / ((Left.Massa + Right.Massa) * dist);
			// normal force, and in x and y directions
			var fx = magnitude * dx / dist; var fy = magnitude * dy / dist; var fz = magnitude * dz / dist;
			// update velocities according to normal force
			Left.Velocity3D.X += fx / Left.Massa;
			Left.Velocity3D.Y += fy / Left.Massa;
			Left.Velocity3D.Z += fz / Left.Massa;
			Right.Velocity3D.X -= fx / Right.Massa; 
			Right.Velocity3D.Y -= fy / Right.Massa;
			Right.Velocity3D.Z -= fz / Right.Massa;
		}

		/// <summary> Абсолютно упругий удар (отскок) без учёта массы и с СОБЛЮДЕНИЕМ закона сохранения импульса. </summary>
		/// <remarks> Метод вычисляет новую скорость и направление двух частиц после отскока. </remarks>
		public static void ElasticImpact(PARTICLE Left, PARTICLE Right) {
			var dist = Distance(Left.Position3D, Right.Position3D); if (dist <= 0) return;
			var dx = Left.Position3D.X - Right.Position3D.X;
			var dy = Left.Position3D.Y - Right.Position3D.Y;
			var dz = Left.Position3D.Z - Right.Position3D.Z;

			//вычисляем sin и cos углов для каждой оси
			var SinX = dx / dist;  var CosX = System.Math.Sqrt(1 - SinX * SinX);
			var SinY = dy / dist;  var CosY = System.Math.Sqrt(1 - SinY * SinY);
			var SinZ = dz / dist;  var CosZ = System.Math.Sqrt(1 - SinZ * SinZ);

			var vx1 = Left.Velocity3D.X; var vy1 = Left.Velocity3D.Y; var vz1 = Left.Velocity3D.Z;
			var vx2 = Right.Velocity3D.X; var vy2 = Right.Velocity3D.Y; var vz2 = Right.Velocity3D.Z;

			//вычисляем новые компоненты направления
			var Vn1X = vx1 * SinX + vy1 * CosX;  var Vn2X = vx2 * SinX + vy2 * CosX;
			var Vn1Y = vx1 * SinY + vy1 * CosY;  var Vn2Y = vx2 * SinY + vy2 * CosY;
			var Vn1Z = vx1 * SinZ + vz1 * CosZ;  var Vn2Z = vx2 * SinZ + vz2 * CosZ;
			var Vt1X = -vx1 * CosX + vy1 * SinX; var Vt2X = -vx2 * CosX + vy2 * SinX;
			var Vt1Y = -vx1 * CosY + vy1 * SinY; var Vt2Y = -vx2 * CosY + vy2 * SinY;
			var Vt1Z = -vx1 * CosZ + vz1 * SinZ; var Vt2Z = -vx2 * CosZ + vz2 * SinZ;

			//обновляем компоненты скорости для каждой оси
			Left.Velocity3D.X = Vn1X * SinX - Vt1X * CosX;
			Left.Velocity3D.Y = Vn1Y * SinY - Vt1Y * CosY;
			Left.Velocity3D.Z = Vn1Z * SinZ - Vt1Z * CosZ;
			Right.Velocity3D.X = Vn2X * SinX - Vt2X * CosX;
			Right.Velocity3D.Y = Vn2Y * SinY - Vt2Y * CosY;
			Right.Velocity3D.Z = Vn2Z * SinZ - Vt2Z * CosZ;
		}

		/// <summary> Реальный удар. Промежуточные варианты между отскоком и слипанием. <br/> С учётом массы и нарушением закона сохранения импульса (импульс угасает). <br/> Каждая частица имеет свой коэффициент double <b>K</b> [0..1], который влияет на степень упругости взаимодествия. </summary>
		public static void Real_Impact(PARTICLE Object1, PARTICLE Object2) {
			var m1 = Object1.Massa; var m2 = Object2.Massa; if (m1 + m2 == 0) return;
			var K1 = Object1.Elasticity; var K2 = Object2.Elasticity;
			var v1 = Object1.Velocity3D; var v2 = Object2.Velocity3D;

			//вычисляем скорость после удара
			Object1.Velocity3D.X = v1.X - (1f + K1) * (m2 / (m1 + m2)) * (v1.X - v2.X);
			Object1.Velocity3D.Y = v1.Y - (1f + K1) * (m2 / (m1 + m2)) * (v1.Y - v2.Y);
			Object1.Velocity3D.Z = v1.Z - (1f + K1) * (m2 / (m1 + m2)) * (v1.Z - v2.Z);
			Object2.Velocity3D.X = v2.X + (1f + K2) * (m1 / (m1 + m2)) * (v1.X - v2.X);
			Object2.Velocity3D.Y = v2.Y + (1f + K2) * (m1 / (m1 + m2)) * (v1.Y - v2.Y);
			Object2.Velocity3D.Z = v2.Z + (1f + K2) * (m1 / (m1 + m2)) * (v1.Z - v2.Z);
		}

		/// <summary> Абсолютно упругий удар (отскок) — модель соударения, при которой полная кинетическая энергия системы сохраняется. <br/>
		///		Хорошим приближением к модели абсолютно упругого удара является столкновение бильярдных шаров или упругих мячиков.
		/// </summary>
		public static void ElasticImpact5(PARTICLE Object1, PARTICLE Object2) {
			var m1 = Object1.Massa; var m2 = Object2.Massa; if (m1 + m2 == 0) return;
			var v1 = Object1.Velocity3D; var v2 = Object2.Velocity3D;

			//вычисляем скорость после удара
			var speedX1 = (2 * m2 * v2.X + v1.X * (m1 - m2)) / (m1 + m2);
			var speedY1 = (2 * m2 * v2.Y + v1.Y * (m1 - m2)) / (m1 + m2);
			var speedZ1 = (2 * m2 * v2.Z + v1.Z * (m1 - m2)) / (m1 + m2);
			var speedX2 = (2 * m1 * v1.X + v2.X * (m2 - m1)) / (m1 + m2);
			var speedY2 = (2 * m1 * v1.Y + v2.Y * (m2 - m1)) / (m1 + m2);
			var speedZ2 = (2 * m1 * v1.Z + v2.Z * (m2 - m1)) / (m1 + m2);

			var dx = Object2.Position3D.X - Object1.Position3D.X; //дельта X
			var dy = Object2.Position3D.Y - Object1.Position3D.Y; //дельта Y
			var dz = Object2.Position3D.Z - Object1.Position3D.Z; //дельта Z
			var dist = (float)System.Math.Sqrt(dx * dx + dy * dy + dz * dz);
			if (dist == 0) dist = 0.01f;//во избежание деления на ноль 
			var ax = dx / dist; var ay = dy / dist; var az = dz / dist;

			var Vn1 = speedX1 * ax + speedY1 * ay + speedZ1 * az;
			var Vn2 = speedX2 * ax + speedY2 * ay + speedZ2 * az;
			var Vt1X = speedX1 - Vn1 * ax; var Vt1Y = speedY1 - Vn1 * ay; var Vt1Z = speedZ1 - Vn1 * az;
			var Vt2X = speedX2 - Vn2 * ax; var Vt2Y = speedY2 - Vn2 * ay; var Vt2Z = speedZ2 - Vn2 * az;
			var VN1 = Vn1 * 0.5f - Vn2 * 0.5f; var VN2 = Vn2 * 0.5f - Vn1 * 0.5f;

			Object1.Velocity3D.X = VN1 * ax + Vt1X; Object1.Velocity3D.Y = VN1 * ay + Vt1Y;	Object1.Velocity3D.Z = VN1 * az + Vt1Z;
			Object2.Velocity3D.X = VN2 * ax + Vt2X; Object2.Velocity3D.Y = VN2 * ay + Vt2Y;	Object2.Velocity3D.Z = VN2 * az + Vt2Z;
		}

		/// <summary> Абсолютно упругий удар (отскок) с учётом массы и СОБЛЮДЕНИЕМ закона сохранения импульса. </summary>
		/// <remarks> Столкновение двух движущихся тел. Отскок не правдоподобный в некоторых случаях. </remarks>
		public static void InMovingImpact(PARTICLE Object1, PARTICLE Object2) {
			//угол соприкосновения
			var tmp1 = Object1.Velocity3D; tmp1.Normalize(); var tmp2 = Object2.Velocity3D; tmp2.Normalize();
			var q = Object1.Position3D.AngleBetween(tmp1, tmp2);//угол между векторами в градусах
			var m1 = Object1.Massa; var m2 = Object2.Massa;
			Object1.AngleFromVelocity(); Object2.AngleFromVelocity();
			var Q1 = Object1.Angle; var Q2 = Object2.Angle;
			var v1 = Object1.Velocity3D; var v2 = Object2.Velocity3D;
			try	{
				Object1.Velocity3D.X = (float)((v1.X * System.Math.Cos(Q1 - q) * (m1 - m2) + 2 * m2 * v2.X * System.Math.Cos(Q2 - q)) /
						(m1 + m2) * System.Math.Cos(q) + v1.X * System.Math.Sin(Q1 - q) * System.Math.Cos(q + System.Math.PI / 2));
				Object1.Velocity3D.Y = (float)((v1.Y * System.Math.Cos(Q1 - q) * (m1 - m2) + 2 * m2 * v2.Y * System.Math.Cos(Q2 - q)) /
						(m1 + m2) * System.Math.Sin(q) + v1.Y * System.Math.Sin(Q1 - q) * System.Math.Sin(q + System.Math.PI / 2));
				Object1.Velocity3D.Z = (float)((v1.Z * System.Math.Cos(Q1 - q) * (m1 - m2) + 2 * m2 * v2.Z * System.Math.Cos(Q2 - q)) /
						(m1 + m2) * System.Math.Sin(q) + v1.Z * System.Math.Sin(Q1 - q) * System.Math.Sin(q + System.Math.PI / 2));

				Object2.Velocity3D.X = (float)((v2.X * System.Math.Cos(Q2 - q) * (m2 - m1) + 2 * m1 * v1.X * System.Math.Cos(Q1 - q)) /
						(m2 + m1) * System.Math.Cos(q) + v2.X * System.Math.Sin(Q2 - q) * System.Math.Cos(q + System.Math.PI / 2));
				Object2.Velocity3D.Y = (float)((v2.Y * System.Math.Cos(Q2 - q) * (m2 - m1) + 2 * m1 * v1.Y * System.Math.Cos(Q1 - q)) /
						(m2 + m1) * System.Math.Sin(q) + v2.Y * System.Math.Sin(Q2 - q) * System.Math.Sin(q + System.Math.PI / 2));
				Object2.Velocity3D.Z = (float)((v2.Z * System.Math.Cos(Q2 - q) * (m2 - m1) + 2 * m1 * v1.Z * System.Math.Cos(Q1 - q)) /
						(m2 + m1) * System.Math.Sin(q) + v2.Z * System.Math.Sin(Q2 - q) * System.Math.Sin(q + System.Math.PI / 2));
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

			var r1 = obj1.Radius; var r2 = obj2.Radius; var d = Distance(obj1.Position3D, obj2.Position3D); if (d <= 0) return;//дистанция
			if (d > SIM.KSd) return; //ограничиваем дальность действия
			if (r1 <= 0) r1 = 1.0f; if (r2 <= 0) r2 = 1.0f;
			var dx = obj2.Position3D.X - obj1.Position3D.X;
			var dy = obj2.Position3D.Y - obj1.Position3D.Y;
			var dz = obj2.Position3D.Z - obj1.Position3D.Z;
			//величина сильного взаимодействия. сила притягивания.
			var Ur = SIM.Fw * d / (r1 + r2);
			if (d > r1 + r2) { //если нет касания
				obj1.Velocity3D.X += Ur * dx / d;
				obj1.Velocity3D.Y += Ur * dy / d;
				obj1.Velocity3D.Z += Ur * dz / d;
			} else { obj1.Velocity3D.X = 0; obj1.Velocity3D.Y = 0; obj1.Velocity3D.Z = 0; }
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

			var d = Distance(obj1.Position3D, obj2.Position3D); if (d <= 0) return;//дистанция
			if (d > SIM.KEd) return; //ограничиваем дальность действия
			var q1 = obj1.Charge; var q2 = obj2.Charge;
			var dx = obj2.Position3D.X - obj1.Position3D.X;
			var dy = obj2.Position3D.Y - obj1.Position3D.Y;
			var dz = obj2.Position3D.Z - obj1.Position3D.Z;

			var F = SIM.Kp * (q1 * q2) / (d * d);//сила взаимодействия
			var gH = F / q1;//величина электромагнитного ускорения на высоте h. чем дальше от объекта, тем слабее

			if (d > obj1.Radius + obj2.Radius) { //если нет касания
				//отталкивание, заряды с одинаковым знаком 
				obj1.Velocity3D.X -= gH * dx / d;
				obj1.Velocity3D.Y -= gH * dy / d;
				obj1.Velocity3D.Z -= gH * dz / d;
			} else { obj1.Velocity3D.X = 0; obj1.Velocity3D.Y = 0; obj1.Velocity3D.Z = 0; }
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

			var d = Distance(obj1.Position3D, obj2.Position3D); if (d <= 0) return;//дистанция
			if (d > SIM.KGd) return;//ограничиваем дальность действия
			var m1 = obj1.Massa; var m2 = obj2.Massa;
			var dx = obj2.Position3D.X - obj1.Position3D.X; 
			var dy = obj2.Position3D.Y - obj1.Position3D.Y;
			var dz = obj2.Position3D.Z - obj1.Position3D.Z;

			var F = SIM.G * (m1 * m2) / (d * d);//сила взаимного притяжения
			var gH = F / m1;//гравитационное ускорение свободного падения на высоте h. чем дальше от объекта, тем слабее притяжение
			if (d > obj1.Radius + obj2.Radius) { //если нет касания
				obj1.Velocity3D.X += gH * dx / d;
				obj1.Velocity3D.Y += gH * dy / d;
				obj1.Velocity3D.Z += gH * dz / d;
			} else { obj1.Velocity3D.X = 0; obj1.Velocity3D.Y = 0; obj1.Velocity3D.Z = 0; }
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
				var impuls_1 = obj1.Massa * obj1.GetSpeed() + obj1.Massa;
				var impuls_2 = obj2.Massa * obj2.GetSpeed() + obj2.Massa;
				if (impuls_1 > impuls_2) {
					//СИЛЬНОЕ ПОГЛОЩЕНИЕ
					if (Mode == Type_Particle.Полное_Поглощение) {
						obj1.Massa += obj2.Massa; obj1.Charge += obj2.Charge; obj1.Update_Color(); obj2.Delete = true;
					}
					//СЛАБОЕ ПОГЛОЩЕНИЕ
					else if (Mode == Type_Particle.Слабое_Поглощение) {
						if (obj2.Massa > 1) {
							obj1.Massa += 1.0f; obj2.Massa -= 1.0f;
							if (obj2.Charge > 1) { obj1.Charge += 1.0f; obj2.Charge -= 1.0f; }
							else { obj1.Charge += obj2.Charge; obj2.Charge = 0; }
						} else { obj1.Massa += obj2.Massa; obj2.Delete = true; }
						obj1.Update_Color();
					}
					//коррекция позиции obj1 после слияния
					if (obj2.Delete) {
						var NewMass = obj1.Massa + obj2.Massa;
						var dx = obj2.Position3D.X - obj1.Position3D.X; 
						var dy = obj2.Position3D.Y - obj1.Position3D.Y;
						var dz = obj2.Position3D.Z - obj1.Position3D.Z;
						obj1.Position3D.X += (dx * obj2.Massa) / NewMass;
						obj1.Position3D.Y += (dy * obj2.Massa) / NewMass;
						obj1.Position3D.Z += (dz * obj2.Massa) / NewMass;
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
			int n = -1; for (int i = 0; i < obj1.LinkPart.Length; i++) if (obj1.LinkPart[i] == null) { n = i; break; }
			if (n < 0) return false;
			//var dist = Distance(obj1.Position, obj2.Position);
			//if (dist < obj1.ValueCoupling && dist > 0) {
			obj1.LinkPart[n] = obj2; obj2.CountCoupling++; return true;
			//} else return false;
		}
		/// <summary> Метод осуществляет взаимодействие двух связанных частиц, ведомой и ведущей. Ведомая следует за ведущей. </summary>
		public static void MooveCoupling(PARTICLE obj1, ref PARTICLE LinkPart, double KFC) {
			var dist = Distance(obj1.Position3D, LinkPart?.Position3D ?? obj1?.Position3D ?? new Vector3Df());
			if (dist < obj1.ValueCoupling && dist > 0) { //на этой дистанции частица 1 следует за связанной частицей 2
				var LPX = LinkPart?.Position3D.X ?? obj1.Position3D.X;
				var LPY = LinkPart?.Position3D.Y ?? obj1.Position3D.Y;
				var LPZ = LinkPart?.Position3D.Z ?? obj1.Position3D.Z;
				var dx = LPX - obj1.Position3D.X; 
				var dy = LPY - obj1.Position3D.Y;
				var dz = LPZ - obj1.Position3D.Z;
				var r1 = obj1.Radius; var r2 = LinkPart?.Radius ?? 0;
				//if (dx == 0 && dy == 0 && dz == 0) return;
				var Ur = KFC * dist;
				if (dist > r1 + r2) { 
					obj1.Velocity3D.X += Ur * dx; obj1.Velocity3D.Y += Ur * dy; obj1.Velocity3D.Z += Ur * dz;
				} //else { obj1.Velocity.X = 0; obj1.Velocity.Y = 0; obj1.Velocity.Z = 0; }
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
			var d = Distance(obj1.Position3D, obj2.Position3D);
			if (d > 0 && d <= dist) {
				if (d > obj1.Radius + obj2.Radius) {
					var dx = obj2.Position3D.X - obj1.Position3D.X;
					var dy = obj2.Position3D.Y - obj1.Position3D.Y;
					var dz = obj2.Position3D.Z - obj1.Position3D.Z;
					var F = g / d;
					obj1.Velocity3D.X += F * dx; obj1.Velocity3D.Y += F * dy; obj1.Velocity3D.Z += F * dz;
				} //else { pi.Velocity.X *= 0.5f; pi.Velocity.Y *= 0.5f; pi.Velocity.Z *= 0.5f; }
			}
		}

		public static void Rule_For(List<PARTICLE> List1, List<PARTICLE> List2, float g, float dist) {
			foreach (var pi in List1) {
				foreach (var pj in List2) {
					if (pi == pj) continue;
					//var d = pi.Position.Length(pj.Position);// Distance(obj1.Position, obj2.Position);
					var d = Distance(pi.Position3D, pj.Position3D);
					if (d > 0 && d <= dist) { //ограничиваем дальность действия
						if (d > pi.Radius + pj.Radius) {
							var dx = pj.Position3D.X - pi.Position3D.X; 
							var dy = pj.Position3D.Y - pi.Position3D.Y;
							var dz = pj.Position3D.Z - pi.Position3D.Z;
							var F = g / d;
							pi.Velocity3D.X += F * dx; 
							pi.Velocity3D.Y += F * dy;
							pi.Velocity3D.Z += F * dz;
						} //else { pi.Velocity.X *= 0.5f; pi.Velocity.Y *= 0.5f; pi.Velocity.Z *= 0.5f; }
					}
					if (pi.pColor != ColorType.White && pi.pColor != ColorType.Aqua && pj.pColor == ColorType.Aqua && pj.CountCoupling < 1)
						Coupling(pi, pj);
					//if (Collision(pi, pj)) Demarcation(pi, pj);
				}
			}
		}
		//======================================= МЕТОДЫ ИЗ ИНТЕРНЕТА =======================================

		/// <summary> Метод обрабатывает границы пространства симуляции. 
		/// <br/> <b><paramref name="bv"/>:</b> <inheritdoc cref="BorderWindow"/> <br/>
		/// <b><paramref name="Part"/>:</b> частица, для которой нужно произвести расчёт. <br/>
		/// <b><paramref name="sz"/>:</b> размер пространства симуляции. <br/>
		/// </summary>
		public static void Border_Processing(BorderWindow bv, PARTICLE Part, Size3D sz) {
			var p = Part.Position3D; var r = Part.Radius;
			switch (bv)	{
				case BorderWindow.REVERSE: //отскок от границы симуляции - инвертировать угол и вектор
					if (p.X - r < 0) { Part.Velocity3D.X = -Part.Velocity3D.X; Part.Position3D.X = r; }//слева
					else if (p.X + r > sz.Width) { Part.Velocity3D.X = -Part.Velocity3D.X; Part.Position3D.X = sz.Width - r; }//справа
					if (p.Y - r < 0) { Part.Velocity3D.Y = -Part.Velocity3D.Y; Part.Position3D.Y = r; }//сверху
					else if (p.Y + r > sz.Height) { Part.Velocity3D.Y = -Part.Velocity3D.Y; Part.Position3D.Y = sz.Height - r; }//снизу
					if (p.Z - r < 0) { Part.Velocity3D.Z = -Part.Velocity3D.Z; Part.Position3D.Z = r; }//передний план Z
					else if (p.Z + r > sz.Depth) { Part.Velocity3D.Z = -Part.Velocity3D.Z; Part.Position3D.Z = sz.Depth - r; }//задний план Z
					break;
				case BorderWindow.OPEN: //пролёт пространства симуляции насквозь и вылет с другой стороны
					if (p.X < 0.0) Part.Position3D.X = sz.Width; else if (p.X > sz.Width) Part.Position3D.X = 0;
					if (p.Y < 0.0) Part.Position3D.Y = sz.Height; else if (p.Y > sz.Height) Part.Position3D.Y = 0;
					//if (p.Z < 0.0) Part.Position3D.Z = sz.Depth; else if (p.Z > sz.Depth) Part.Position3D.Z = 0;
					break;
				case BorderWindow.STOP: //полная остановка на краю симуляции
					bool b = false;
					if (p.X - r < 0.0) { Part.Position3D.X = r; b = true; }
					else if (p.X + r > sz.Width) { Part.Position3D.X = sz.Width - r; b = true; }
					if (p.Y - r < 0.0) { Part.Position3D.Y = r; b = true; }
					else if (p.Y + r > sz.Height) { Part.Position3D.Y = sz.Height - r; b = true; }
					if (p.Z - r < 0.0) { Part.Position3D.Z = r; b = true; }
					else if (p.Z + r > sz.Depth) { Part.Position3D.Z = sz.Depth - r; b = true; }
					if (b) { Part.Velocity3D.X = 0; Part.Velocity3D.Y = 0; Part.Velocity3D.Z = 0; }
					break;
				default: break;
			}
		}
	}
}
