using System;
using System.Drawing;

namespace UFO.Math {
	/*::::::::::::::::::::::::::::::::::::::::::::: class UFO.Math.Vector2Df ::::::::::::::::::::::::::::::::::::::::::::::*/
	/// <summary> Класс математического вектора. X/Y имеют тип <b>double</b>. </summary>
	/// <remarks> Вектор – это направленный отрезок прямой, т. е. отрезок, имеющий определенную длину и определенное направление. <br/>
	///			  Обозначение: "AB", или "a". У вектора "AB", "A" – начало вектора, а "B" – конец вектора. <br/>
	///			  "A" и "B" имеют по 2 координаты: [A.x, A.y] и [B.x, B.y], т.е. вектор имеет 4 координаты. <br/>
	///			  У данного вектора есть только 2 координаты конца вектора. Начало вектора в данном классе = (0, 0).
	/// </remarks>
	public struct Vector2Df {
		/// <summary> Координата вектора по оси X. </summary>
		private double x;
		/// <summary> <inheritdoc cref="x"/> </summary>
		public double X { get { return x; } set { x = value; /*location.X = x;*/ } }
		/// <summary> Координата вектора по оси Y. </summary>
		private double y;
		/// <summary> <inheritdoc cref="y"/> </summary>
		public double Y { get { return y; } set { y = value; /*location.Y = y;*/ } }
		/// <summary> Локация вектора. Хранит пару координат X/Y и изменяет поля <b>X</b> и <b>Y</b>. </summary>
		//private PointF location;
		///// <summary> <inheritdoc cref="location"/> </summary>
		//public PointF Location {
		//	get { return location; }
		//	set { location = value; x = value.X; y = value.Y; }
		//}

		/// <summary> Конструктор по умолчанию. Заполняет координаты вектора нулями. </summary>
		static Vector2Df() { /*X = default; Y = default;*/ }
		/// <summary> Конструктор с 2 параметрами. Заполняет координаты вектора переданными значениями. </summary>
		public Vector2Df(double x, double y) : this() { X = x; Y = y; }
		/// <summary> Конструктор с 1 параметром. Заполняет координаты вектора из переданного вектора vec. </summary>
		public Vector2Df(Vector2Df vec) : this() { this = vec;
			//x = vec.x; y = vec.y; pi = vec.pi; resolution = vec.resolution; location = vec.location;
		}

		/// <summary> Метод устанавливает значениям this.X; this.Y <b>данного объекта</b>, значения <b><paramref name="point_X"/></b>; <b><paramref name="point_Y"/></b>. </summary>
		public void SetXY(Vector2Df point) { SetXY(point.X, point.Y); }
		/// <summary> Метод устанавливает значениям this.X; this.Y <b>данного объекта</b>, значения <b><paramref name="x"/></b>; <b><paramref name="y"/></b>. </summary>
		public void SetXY(double x, double y) { X = x; Y = y; }

		/// <summary> Метод копирует значения в вектор <b>this</b> из вектора <b>obj</b>. </summary>
		public void Copy(Vector2Df obj) { x = obj.x; y = obj.y; /*location = obj.location;*/ }
		/// <summary> Метод	вычисляет угол поворота единичного вектора относительно горизонтального отрезка в градусах. </summary>
		/// <remarks> Первая точка имеет координаты <b>this</b> вектора;
		///			  <br/> Вторая точка лежит по координатам <b>Vec(1.0, 0.0)</b>.
		///	</remarks>
		/// <returns> Возвращает угол поворота в градусах из единичного ветора <b>данного объекта</b>. </returns>
		public double AngleFromVector() {
			//вариант 1
			return AngleBetween(this, new Vector2Df(1.0f, 0.0f));
			//вариант 2 - такой же точный, но не ясно как по скорости
			//double Q = (atan2(mY, mX) * 180.0 / pi);
			//if (Q < 0) Q += 360.0;
			//return Q;

		}

		/// <summary> Метод умножает этот вектор на число <b>num</b>. </summary>
		public void Multiplication(double num) { X *= num; Y *= num; }
		/// <summary> Метод производит скалярное произведение (умножение) векторов. <br/> В других классах векторов аналогичный метод может называться <b>Dot(v1, v2)</b>. </summary>
		/// <returns> Возвращает произведение (умножение) вектора <b>this</b> на вектор <b>other</b>. </returns>
		public double ScalarMultiplication(Vector2Df other) { return (X * other.X + Y * other.Y); }
		/// <summary> Метод производит скалярное произведение (умножение) векторов. <br/> В других классах векторов аналогичный метод может называться <b>Dot(v1, v2)</b>. </summary>
		/// <returns> Возвращает произведение (умножение) вектора <b>A</b> на вектор <b>B</b>. </returns>
		public double ScalarMultiplication(Vector2Df A, Vector2Df B) { return (A.X * B.X + A.Y * B.Y); }

		/// <summary> Метод нормализует вектор <b>данного объекта</b> в единичный вектор с единичной длиной. </summary>
		public void Normalize() { Normalize(X, Y); }
		/// <summary> Метод вычисляет компоненты X/Y вектора по переданным координатам и присваивает результат объекту, вызвавшему этот метод. <br/> Нормализованный вектор имеет длину = 1. </summary>
		public void Normalize(double ax, double ay) {
			double len = Length(ax, ay);
			if (len > 0 && len != 1) { X = ax / len; Y = ay / len; } else { X = ax; Y = ay; } 
		}
		/// <summary> Метод проверяет ортогональность (перпендикулярность) векторов: <b>данного объекта</b> и <b>other</b> вектора. </summary>
		/// <returns> Возвращает <b>true</b> если между ними есть 90 градусов и <b>false</b>, если нет. </returns>
		public bool Orthogonality(Vector2Df other) { return ScalarMultiplication(this, other) == 0; }
		/// <summary> Метод проверяет ортогональность (перпендикулярность) векторов <b>A</b> и <b>B</b>. </summary>
		/// <returns> Возвращает <b>true</b> если между ними есть 90 градусов и <b>false</b>, если нет. </returns>
		public bool Orthogonality(Vector2Df A, Vector2Df B) { return ScalarMultiplication(A, B) == 0; }
		/// <summary> Метод вычисляет угол между двумя векторами в градуcах. </summary>
		/// <returns> Возвращает вычисленный угол в градусах от 0 до 360 (0 = 360). </returns>
		public double AngleBetween(Vector2Df A, Vector2Df B) {
			double lenA = Length(A.X, A.Y);//вычислить модуль вектора A (длина вектора)
			double lenB = Length(B.X, B.Y);//вычислить модуль вектора B (длина вектора)
			if (lenA == 0 || lenB == 0) return 0;
			double AB = ScalarMultiplication(A, B); double rad = AB / (lenA * lenB);//вычисление радиан
			if (rad >= 1.0f) rad = 0.0f; else if (rad <= -1.0f) rad = System.Math.PI; else rad = System.Math.Acos(rad);
			double Angle = rad * 180.0f / System.Math.PI;//радианы в градусы
			if (A.Y < 0) Angle = 360.0f - Angle;//для верхнего полукруга. углы [180; 360]
			return Angle;
		}

		/// <summary> Метод вычисляет длину единичного вектора (модуль вектора). </summary>
		/// <returns> Возвращает длину единичного вектора <b>данного объекта.</b> </returns>
		public double Length() { return Length(0.0f, 0.0f, X, Y); }
		/// <summary> Метод вычисляет длину единичного вектора (модуль вектора). </summary>
		/// <returns> Возвращает длину <b>переданного</b> единичного вектора. </returns>
		public double Length(double ax, double ay) { return Length(0.0f, 0.0f, ax, ay); }
		/// <summary> Метод вычисляет длину двух векторов <b>this</b> и <b>other</b> (модуль вектора). </summary>
		/// <returns> Возвращает длину двух векторов. </returns>
		public double Length(Vector2Df other) { return Length(X, Y, other.X, other.Y);	}
		/// <summary> Метод вычисляет длину полного вектора (модуль вектора). </summary>
		/// <returns> Возвращает длину <b>переданного</b> вектора. </returns>
		public double Length(double ax1, double ay1, double ax2, double ay2) {
			double dx = ax2 - ax1; double dy = ay2 - ay1;
			double len = System.Math.Sqrt(dx * dx + dy * dy);/*длина*/
			return len;
		}
		/// <summary> Метод осуществляет реверс вектора по оси Х (this.X = -this.X) </summary>
		public void Reverse_X() { X = -X; }
		/// <summary> Метод осуществляет реверс вектора по оси Y (this.Y = -this.Y) </summary>
		public void Reverse_Y() { Y = -Y; }

		//================================ ПЕРЕГРУЗКА ОПЕРАТОРОВ ================================
		//перегрузки операторов для математических операций с векторами
		//operator '=' не перегружается, но это и не нужно, x = a + b записывается так:
		//Vector2Df vec = vec1 + vec2;
		//Vector2Df vec = vec1.operator+(vec2); 
		//ссылка vec ссылается на созданный новый вектор, который уже просуммирован
		/// <summary> Метод производит складывание двух векторов: <b>left</b> + <b>right</b>. </summary>
		/// <returns> Возвращает копию вектора left, как сумму двух векторов. </returns>
		public static Vector2Df operator +(Vector2Df left, Vector2Df right) {
			left.X += right.X; left.Y += right.Y;
			return left; //new Vector2Df(left.X + right.X, left.Y + right.Y);
		}
		/// <summary> Метод производит вычитание двух векторов: <b>left</b> - <b>right</b>. </summary>
		/// <returns> Возвращает копию вектора left, как разность двух векторов. </returns>
		public static Vector2Df operator -(Vector2Df left, Vector2Df right) {
			left.X -= right.X; left.Y -= right.Y;
			return left;//new Vector2Df(left.X - right.X, left.Y - right.Y);
		}
		/// <summary> Метод производит умножение двух векторов: <b>left</b> * <b>right</b>. </summary>
		/// <returns> Возвращает копию вектора left, как произведение двух векторов. </returns>
		public static Vector2Df operator *(Vector2Df left, Vector2Df right) {
			left.X *= right.X; left.Y *= right.Y;
			return left;// new Vector2Df(left.X * right.X, left.Y * right.Y);
		}
		/// <summary> Метод производит деление двух векторов: <b>left</b> / <b>right</b>. </summary>
		/// <returns> Возвращает копию вектора left, как частное двух векторов. </returns>
		public static Vector2Df operator /(Vector2Df left, Vector2Df right) {
			left.X /= right.X; left.Y /= right.Y;
			return left;// new Vector2Df(left.X / right.X, left.Y / right.Y); 
		}
		/// <summary> Метод производит сравнение двух векторов на равенство: <b>left</b> == <b>right</b>. </summary>
		/// <returns> Возвращает <b>true</b>, если векторы равны и <b>false</b>, если не равны. </returns>
		public static bool operator ==(Vector2Df left, Vector2Df right) {
			return left.X == right.X && left.Y == right.Y;
		}
		/// <summary> Метод производит сравнение двух векторов на неравенство: <b>left</b> != <b>right</b>. </summary>
		/// <returns> Возвращает <b>true</b>, если векторы НЕ равны и <b>false</b>, если равны. </returns>
		public static bool operator !=(Vector2Df left, Vector2Df right)	{
			return left.X != right.X || left.Y != right.Y;
		}
		/// <summary> Метод возвращает значение, указывающее, действительно ли заданное значение: <b>left</b> больше <b>right</b>. </summary>
		/// <remarks> Сравнение происходит с возведением в квадраты, но без извлечения квадратного корня (эконом вариант)! </remarks>
		/// <returns> Возвращает <b>true</b>, если вектор <b>left</b> больше <b>right</b> и <b>false</b>, если нет. </returns>
		public static bool operator >(Vector2Df left, Vector2Df right) {
			return (left.X * left.X + left.Y * left.Y) > (right.X * right.X + right.Y * right.Y);
		}
		/// <summary> Метод возвращает значение, указывающее, действительно ли заданное значение: <b>left</b> меньше <b>right</b>. </summary>
		/// <remarks> <inheritdoc cref="operator >"/> </remarks>
		/// <returns> Возвращает <b>true</b>, если вектор <b>left</b> меньше <b>right</b> и <b>false</b>, если нет. </returns>
		public static bool operator <(Vector2Df left, Vector2Df right) {
			return (left.X * left.X + left.Y * left.Y) < (right.X * right.X + right.Y * right.Y);
		}
		/// <summary> Метод возвращает значение, указывающее, действительно ли заданное значение: <b>left</b> больше или равно <b>right</b>. </summary>
		/// <remarks> <inheritdoc cref="operator >"/> </remarks>
		/// <returns> Возвращает <b>true</b>, если вектор <b>left</b> больше или равно <b>right</b> и <b>false</b>, если нет. </returns>
		public static bool operator >=(Vector2Df left, Vector2Df right) {
			return (left.X * left.X + left.Y * left.Y) >= (right.X * right.X + right.Y * right.Y);
		}
		/// <summary> Метод возвращает значение, указывающее, действительно ли заданное значение: <b>left</b> меньше <b>right</b>. </summary>
		/// <remarks> <inheritdoc cref="operator >"/> </remarks>
		/// <returns> Возвращает <b>true</b>, если вектор <b>left</b> меньше или равно <b>right</b> и <b>false</b>, если нет. </returns>
		public static bool operator <=(Vector2Df left, Vector2Df right) {
			return (left.X * left.X + left.Y * left.Y) <= (right.X * right.X + right.Y * right.Y);
		}

		public override bool Equals(object obj) {
			if (obj == null || GetType() != obj.GetType()) return false;
			Vector2Df right = (Vector2Df)obj;
			return System.Math.Abs(x - right.x) < 1e-10 && System.Math.Abs(y - right.y) < 1e-10;
        }
        public override int GetHashCode() {
			unchecked { int hash = 17;
				hash = hash * 23 + BitConverter.ToInt64(BitConverter.GetBytes(x), 0).GetHashCode();
				hash = hash * 23 + BitConverter.ToInt64(BitConverter.GetBytes(y), 0).GetHashCode();
				return hash;
			}
			//return x.GetHashCode() ^ y.GetHashCode();
		}
		//================================ ПЕРЕГРУЗКА ОПЕРАТОРОВ ================================

	}
/*::::::::::::::::::::::::::::::::::::::::::::: class UFO.Math.Vector2Df ::::::::::::::::::::::::::::::::::::::::::::::*/
}

