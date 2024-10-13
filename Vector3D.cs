using System.Drawing;

namespace UFO.Math
{
	/*::::::::::::::::::::::::::::::::::::::::::::: class UFO.Math.Vector3Df ::::::::::::::::::::::::::::::::::::::::::::::*/
	/// <summary> Класс математического вектора. XYZ имеют тип <b>double</b>. </summary>
	/// <remarks> Вектор – это направленный отрезок прямой, т. е. отрезок, имеющий определенную длину и определенное направление. <br/>
	///			  Обозначение: "AB", или "a". У вектора "AB", "A" – начало вектора, а "B" – конец вектора. <br/>
	///			  "A" и "B" имеют по 3 координаты: [A.x, A.y, A.z] и [B.x, B.y, B.z], т.е. вектор имеет 6 координат. <br/>
	///			  У данного вектора есть только 3 координаты конца вектора. Начало вектора в данном классе = (0, 0, 0).
	/// </remarks>
	public struct Vector3Df {
		/// <summary> Координата вектора по оси X. </summary>
		private double x;
		/// <summary> <inheritdoc cref="x"/> </summary>
		public double X { get { return x; } set { x = value; /*location.X = x;*/ } }
		/// <summary> Координата вектора по оси Y. </summary>
		private double y;
		/// <summary> <inheritdoc cref="y"/> </summary>
		public double Y { get { return y; } set { y = value; /*location.Y = y;*/ } }
		/// <summary> Координата вектора по оси Z. </summary>
		private double z;
		/// <summary> <inheritdoc cref="z"/> </summary>
		public double Z { get { return z; } set { z = value; /*location.Z = z;*/ } }

		/// <summary> Конструктор по умолчанию. Заполняет координаты вектора нулями. </summary>
		static Vector3Df() { /*X = default; Y = default; Z = default;*/ }
		/// <summary> Конструктор с 3 параметрами. Заполняет координаты вектора переданными значениями. </summary>
		public Vector3Df(double x, double y, double z) : this() { X = x; Y = y; Z = z; }
		/// <summary> Конструктор с 1 параметром. Заполняет координаты вектора из переданного вектора vec. </summary>
		public Vector3Df(Vector3Df vec) : this() { this = vec; }

		/// <summary> Метод устанавливает значениям this.X; this.Y; this.Z <b>данного объекта</b>, значения <b><paramref name="point_X"/></b>; <b><paramref name="point_Y"/>; <b><paramref name="point_Z"/></b>. </summary>
		public void SetXYZ(Vector3Df point) { SetXYZ(point.X, point.Y, point.Z); }
		/// <summary> Метод устанавливает значениям this.X; this.Y; this.Z <b>данного объекта</b>, значения <b><paramref name="x"/></b>; <b><paramref name="y"/></b>; <b><paramref name="z"/></b>. </summary>
		public void SetXYZ(double x, double y, double z) { X = x; Y = y; Z = z; }

		/// <summary> Метод копирует значения в вектор <b>this</b> из вектора <b>obj</b>. </summary>
		public void Copy(Vector3Df obj) { x = obj.x; y = obj.y; z = obj.z; }
		/// <summary> Метод	вычисляет угол поворота единичного вектора относительно горизонтального отрезка в градусах. </summary>
		/// <remarks> Первая точка имеет координаты <b>this</b> вектора;
		///			  <br/> Вторая точка лежит по координатам <b>Vec(1.0, 0.0, 0.0)</b>.
		///	</remarks>
		/// <returns> Возвращает угол поворота в градусах из единичного ветора <b>данного объекта</b>. </returns>
		public double AngleFromVector() {
			//вариант 1
			return AngleBetween(this, new Vector3Df(1.0f, 0.0f, 0.0f));
			//вариант 2 - такой же точный, но не ясно как по скорости
			//double Q = (atan2(mY, mX) * 180.0 / pi);
			//if (Q < 0) Q += 360.0;
			//return Q;

		}

		/// <summary> Метод умножает этот вектор на число <b>num</b>. </summary>
		public void Multiplication(double num) { X *= num; Y *= num; Z *= num; }
		/// <summary> Метод производит скалярное произведение (умножение) векторов. </summary>
		/// <returns> Возвращает произведение (умножение) вектора <b>this</b> на вектор <b>other</b>. </returns>
		public double ScalarMultiplication(Vector3Df other) { return (X * other.X + Y * other.Y + Z * other.Z); }
		/// <summary> Метод производит скалярное произведение (умножение) векторов. </summary>
		/// <returns> Возвращает произведение (умножение) вектора <b>A</b> на вектор <b>B</b>. </returns>
		public double ScalarMultiplication(Vector3Df A, Vector3Df B) { return (A.X * B.X + A.Y * B.Y + A.Z * B.Z); }

		/// <summary> Метод нормализует вектор <b>данного объекта</b> в единичный вектор с единичной длиной. </summary>
		public void Normalize() { Normalize(X, Y, Z); }
		/// <summary> Метод вычисляет компоненты X/Y вектора по переданным координатам и присваивает результат объекту, вызвавшему этот метод. <br/> Нормализованный вектор имеет длину = 1. </summary>
		public void Normalize(double ax, double ay, double az)	{
			double len = Length(ax, ay, az);
			if (len > 0 && len != 1) { X = ax / len; Y = ay / len; Z = az / len; } else { X = ax; Y = ay; Z = az; }
		}
		/// <summary> Метод проверяет ортогональность (перпендикулярность) векторов: <b>данного объекта</b> и <b>other</b> вектора. </summary>
		/// <returns> Возвращает <b>true</b> если между ними есть 90 градусов и <b>false</b>, если нет. </returns>
		public bool Orthogonality(Vector3Df other) { return ScalarMultiplication(this, other) == 0; }
		/// <summary> Метод проверяет ортогональность (перпендикулярность) векторов <b>A</b> и <b>B</b>. </summary>
		/// <returns> Возвращает <b>true</b> если между ними есть 90 градусов и <b>false</b>, если нет. </returns>
		public bool Orthogonality(Vector3Df A, Vector3Df B) { return ScalarMultiplication(A, B) == 0; }
		/// <summary> Метод вычисляет угол между двумя векторами в градуcах. </summary>
		/// <returns> Возвращает вычисленный угол в градусах от 0 до 360 (0 = 360). </returns>
		public double AngleBetween(Vector3Df A, Vector3Df B) {
			double lenA = Length(A.X, A.Y, A.Z);//вычислить модуль вектора A (длина вектора)
			double lenB = Length(B.X, B.Y, A.Z);//вычислить модуль вектора B (длина вектора)
			if (lenA == 0 || lenB == 0) return 0;
			double AB = ScalarMultiplication(A, B); double rad = AB / (lenA * lenB);//вычисление радиан
			if (rad >= 1.0f) rad = 0.0f; else if (rad <= -1.0f) rad = System.Math.PI; else rad = System.Math.Acos(rad);
			double Angle = rad * 180.0f / System.Math.PI;//радианы в градусы
			if (A.Y < 0) Angle = 360.0f - Angle;//для верхнего полукруга. углы [180; 360]
			return Angle;
		}

		/// <summary> Метод вычисляет длину единичного вектора (модуль вектора). </summary>
		/// <returns> Возвращает длину единичного вектора <b>данного объекта.</b> </returns>
		public double Length() { return Length(0.0f, 0.0f, 0.0f, X, Y, Z); }
		/// <summary> Метод вычисляет длину единичного вектора (модуль вектора). </summary>
		/// <returns> Возвращает длину <b>переданного</b> единичного вектора. </returns>
		public double Length(double ax, double ay, double az) { return Length(0.0f, 0.0f, 0.0f, ax, ay, az); }
		/// <summary> Метод вычисляет длину двух векторов <b>this</b> и <b>other</b> (модуль вектора). </summary>
		/// <returns> Возвращает длину двух векторов. </returns>
		public double Length(Vector3Df other) { return Length(X, Y, Z, other.X, other.Y, other.Z); }
		/// <summary> Метод вычисляет длину полного вектора (модуль вектора). </summary>
		/// <returns> Возвращает длину <b>переданного</b> вектора. </returns>
		public double Length(double ax1, double ay1, double az1, double ax2, double ay2, double az2) {
			double dx = ax2 - ax1; double dy = ay2 - ay1; double dz = az2 - az1;
			double len = System.Math.Sqrt(dx * dx + dy * dy + dz * dz);/*длина*/
			return len;
		}
		/// <summary> Метод осуществляет реверс вектора по оси Х (this.X = -this.X) </summary>
		public void Reverse_X() { X = -X; }
		/// <summary> Метод осуществляет реверс вектора по оси Y (this.Y = -this.Y) </summary>
		public void Reverse_Y() { Y = -Y; }
		/// <summary> Метод осуществляет реверс вектора по оси Y (this.Y = -this.Y) </summary>
		public void Reverse_Z() { Z = -Z; }

		//================================ ПЕРЕГРУЗКА ОПЕРАТОРОВ ================================
		//перегрузки операторов для математических операций с векторами
		//operator '=' не перегружается, но это и не нужно, x = a + b записывается так:
		//Vector2Df vec = vec1 + vec2;
		//Vector2Df vec = vec1.operator+(vec2); 
		//ссылка vec ссылается на созданный новый вектор, который уже просуммирован
		/// <summary> Метод производит складывание двух векторов: <b>left</b> + <b>right</b>. </summary>
		/// <returns> Возвращает копию вектора left, как сумму двух векторов. </returns>
		public static Vector3Df operator +(Vector3Df left, Vector3Df right) {
			left.X += right.X; left.Y += right.Y; left.Z += right.Z; return left; 
		}
		/// <summary> Метод производит вычитание двух векторов: <b>left</b> - <b>right</b>. </summary>
		/// <returns> Возвращает копию вектора left, как разность двух векторов. </returns>
		public static Vector3Df operator -(Vector3Df left, Vector3Df right)	{
			left.X -= right.X; left.Y -= right.Y; left.Z -= right.Z; return left;
		}
		/// <summary> Метод производит умножение двух векторов: <b>left</b> * <b>right</b>. </summary>
		/// <returns> Возвращает копию вектора left, как произведение двух векторов. </returns>
		public static Vector3Df operator *(Vector3Df left, Vector3Df right) {
			left.X *= right.X; left.Y *= right.Y; left.Z *= right.Z; return left;
		}
		/// <summary> Метод производит деление двух векторов: <b>left</b> / <b>right</b>. </summary>
		/// <returns> Возвращает копию вектора left, как частное двух векторов. </returns>
		public static Vector3Df operator /(Vector3Df left, Vector3Df right) {
			left.X /= right.X; left.Y /= right.Y; left.Z /= right.Z; return left;
		}
		/// <summary> Метод производит сравнение двух векторов на равенство: <b>left</b> == <b>right</b>. </summary>
		/// <returns> Возвращает <b>true</b>, если векторы равны и <b>false</b>, если не равны. </returns>
		public static bool operator ==(Vector3Df left, Vector3Df right) {
			return left.X == right.X && left.Y == right.Y && left.Z == right.Z;
		}
		/// <summary> Метод производит сравнение двух векторов на неравенство: <b>left</b> != <b>right</b>. </summary>
		/// <returns> Возвращает <b>true</b>, если векторы НЕ равны и <b>false</b>, если равны. </returns>
		public static bool operator !=(Vector3Df left, Vector3Df right)	{
			return left.X != right.X || left.Y != right.Y || left.Z != right.Z;
		}

		public override bool Equals(object obj) {
			if (obj == null || GetType() != obj.GetType()) return false;
			var right = (Vector3Df)obj; return x == right.x && y == right.y && z == right.z;
		}
		public override int GetHashCode() { return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode(); }
		//================================ ПЕРЕГРУЗКА ОПЕРАТОРОВ ================================

	}
	/*::::::::::::::::::::::::::::::::::::::::::::: class UFO.Math.Vector3Df ::::::::::::::::::::::::::::::::::::::::::::::*/
}

