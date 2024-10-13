using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static UFO.Enums_Structs;

namespace UFO {
	/// <summary> Статический класс. Содержит пользовательские методы: конвертации, итерполяции, преобразования и т.д. </summary>
	static class Converter {
		/// <summary> Возвращает размер в текущем разрешении экрана без каких-либо преобразований. <br/>
		/// <b>Bounds:</b> <inheritdoc cref="Screen.Bounds"/> </summary>
		/// <returns> Возвращает размер текущего разрешения экрана. </returns>
		public static Size ScreenBounds_Size() { return Screen.PrimaryScreen.Bounds.Size; }
		/// <summary> Возвращает прямоугольник в текущем разрешении экрана без каких-либо преобразований. <br/>
		/// <b>Bounds:</b> <inheritdoc cref="Screen.Bounds"/> </summary>
		/// <returns> Возвращает прямоугольник текущего разрешения экрана. </returns>
		public static Rectangle Screen_Bounds() { return Screen.PrimaryScreen.Bounds; }
		/// <summary> Возвращает размер в текущем разрешении экрана без каких-либо преобразований. <br/>
		///		<b>WorkingArea:</b> <inheritdoc cref="Screen.WorkingArea"/> </summary>
		/// <returns> Возвращает текущее разрешение экрана. </returns>
		public static Size ScreenWorkingArea_Size() { return Screen.PrimaryScreen.WorkingArea.Size;	}
		/// <summary> Возвращает прямоугольник в текущем разрешении экрана без каких-либо преобразований. <br/>
		///		<b>WorkingArea:</b> <inheritdoc cref="Screen.WorkingArea"/> </summary>
		/// <returns> Возвращает текущее разрешение экрана. </returns>
		public static Rectangle Screen_WorkingArea() { return Screen.PrimaryScreen.WorkingArea; }
		/// <summary>
		/// <b>C</b>urrent <b>S</b>creen <b>R</b>esolution - Текущее Разрешение Экрана. <br/>
		/// Функция преобразовывает значение <b> NUM </b> записанное в разрешении экрана 1920х1080 в теущее разрешение экрана. <br/>
		/// Расчёты идут относительно всего дисплея монитора. Формула: <b>NUM / 1080.0 * Screen...Height;</b>
		/// </summary>
		/// <returns> Возвращает преобразованный <b> NUM </b> в текущем разрешении экрана. </returns>
		public static int ToCSR(int NUM) { return (int)(NUM / 1080.0 * Screen.PrimaryScreen.Bounds.Size.Height); }
		/// <summary> <inheritdoc cref="ToCSR"/> </summary> <returns> <inheritdoc cref="ToCSR"/> </returns>
		public static float ToCSR(float NUM) { return (float)(NUM / 1080.0 * Screen.PrimaryScreen.Bounds.Size.Height);	}
		/// <summary> <inheritdoc cref="ToCSR"/> </summary> <returns> <inheritdoc cref="ToCSR"/> </returns>
		public static double ToCSR(double NUM) { return (double)(NUM / 1080.0 * Screen.PrimaryScreen.Bounds.Size.Height); }
		/// <summary> Метод преобразовывает процентное значение <b>per</b> в абсолютные значения в пикселях монитора с учётом текущего разрешения экрана. <br/> Пример: per = 50% - функция выдаст половину экрана; per = 100% - функция выдаст весь размер экрана. </summary>		
		/// <value>	<b><paramref name="per"/>:</b> дробное число типа <b>double</b> в процентах, в диапазоне [0..100]. <br/> </value>
		/// <returns> Возвращает преобразованное значение в пикселях монитора относительно его <b>ширины</b> Width. </returns>
		public static int PercentToPixelWidth(double per) { return (int)(per / 100 * Screen.PrimaryScreen.Bounds.Size.Width); }
		/// <summary> <inheritdoc cref="PercentToPixelWidth"/> </summary>
		/// <value> <inheritdoc cref="PercentToPixelWidth"/> </value>
		/// <returns> Возвращает преобразованное значение в пикселях монитора относительно его <b>высоты</b> Height. </returns>
		public static int PercentToPixelHeight(double per) { return (int)(per / 100 * Screen.PrimaryScreen.Bounds.Size.Height); }
		/// <summary> Метод преобразовывает коэффициент процента <b>coefficient</b> в абсолютные значения в пикселях монитора с учётом текущего разрешения экрана. <br/> Пример: coefficient = 0.5 - функция выдаст половину экрана; coefficient = 1.0 - функция выдаст весь размер экрана. </summary>
		/// <value>	<b><paramref name="coefficient"/>:</b> дробное число типа <b>double</b> выражающее коэффициент процента, в диапазоне [0..1]. <br/> </value>
		/// <returns> <inheritdoc cref="PercentToPixelWidth"/> </returns>
		public static int CoefficientToPixelWidth(double coefficient) { return (int)(coefficient * Screen.PrimaryScreen.Bounds.Size.Width); }
		/// <summary> <inheritdoc cref="CoefficientToPixelWidth"/> </summary>
		/// <value> <inheritdoc cref="CoefficientToPixelWidth"/> </value>
		/// <returns> <inheritdoc cref="PercentToPixelHeight"/> </returns>
		public static int CoefficientToPixelHeight(double coefficient) { return (int)(coefficient * Screen.PrimaryScreen.Bounds.Size.Height); }

		/// <summary> 
		///		Функция преобразовывает десятичную дробь в секунды (в сутках 86'400 секунд = [0..86'399]). <br/>
		///		Числитель - это часы. Например если в числителе будет 48.ххх - это равно 48 часам, 2 суткам. <br/>
		///		Мантисса - это проценты по сути. Напрмер 0.1428 = 60/100 * 14,28% = 8,568 = 8 часов + 56,8% минут = 60/100 * 56,8 = 34 минуты + 0,8% милисек.
		/// </summary>
		/// <returns> Возвращает время в секундах. <br/>
		///		Если значение <b> Double </b> превысит 24.ххх часа, то функция вернёт число превышающее 86'399.
		/// </returns>
		public static int ToTimeSecond(double Double) { return (int)(Double * 3600); }

		/// <summary> 
		///		Функция преобразовывает десятичную дробь в милисекунды (в сутках 86'400'000 миллисекунд = [0..86'399'999]). <br/>
		///		Числитель - это часы. Например если в числителе будет 48.ххх - это равно 48 часам, 2 суткам. <br/>
		///		Мантисса - это проценты по сути. Напрмер 0.1428 = 60/100 * 14,28% = 8,568 = 8 часов + 56,8% минут = 60/100 * 56,8 = 34 минуты + 0,8% милисек = 1000/100 * 8 = 80 милисек.
		/// </summary>
		/// <returns> Возвращает время в милисекундах. <br/>
		///		Если значение <b> Double </b> превысит 24.ххх часа, то функция вернёт число превышающее 86'399'999.
		/// </returns>
		public static int ToTimeMilliSecond(double Double) { return (int)(Double * 3600000); }

		/// <summary> Функция преобразовывает компоненты времени в секунды (в сутках 86'400 секунд = [0..86'399]). </summary>
		/// <remarks> <b> hour: </b> часы. <br/> <b> min: </b> минуты. <br/> <b> sec: </b> секунды. <br/> </remarks>
		/// <returns> Возвращает время в секундах. <br/>
		///		Если значение <b> hour </b> превысит 24 часа, то функция вернёт число превышающее 86'399.
		/// </returns>
		public static uint ToTimeSecond(uint hour, uint min, uint sec) {
			return hour * 3600 + min * 60 + sec;
		}

		/// <summary> Функция преобразовывает компоненты времени в миллисекунды (в сутках 86'400'000 миллисекунд = [0..86'399'999]). </summary>
		/// <remarks> <b> hour: </b> часы. <br/> <b> min: </b> минуты. <br/> <b> sec: </b> секунды. <br/> <b> msec: </b> миллисекунды. <br/> </remarks>
		/// <returns> Возвращает время в миллисекундах. <br/>
		///		Если значение <b> hour </b> превысит 24 часа, то функция вернёт число превышающее 86'399'999.
		/// </returns>
		public static uint ToTimeMilliSecond(uint hour, uint min, uint sec, uint msec) {
			return hour * 3600000 + min * 60000 + sec * 1000 + msec;
		}

		/// <summary> Функция преобразовывает время ["чч:мм:сс"] в секунды. </summary>
		/// <remarks> <b> time: </b> строка времени <b> string </b> формата ["чч:мм:сс"]. <br/> </remarks>
		/// <returns> Возвращает время в миллисекундах. <br/>
		///		Если значение <b> time </b> превысит 24 часа, то функция вернёт число превышающее 86'399'999.
		/// </returns>
		public static int ToTimeSecond(string time) {
			int x = 0, hour = 0, min = 0, sec = 0; string tmp = ""; time += ":";
            for (int i = 0; i < time.Length; i++) if (time[i] != ':') tmp += time[i]; else {
				try { 
					if (x == 0) hour = Convert.ToInt32(tmp);
					if (x == 1) min = Convert.ToInt32(tmp);
					if (x == 2) sec = Convert.ToInt32(tmp);
				}
				catch (FormatException) {
					MessageBox.Show("Ошибка. class Convert;\nСтрока time = '" + time + "' имеет не верный формат.\n" +
						"Правильный формат для обработки: 'чч:мм:сс'. Пример: time = '12:36:28'.\n return 0;"
					);
					return 0;
                }
				x++; tmp = "";
            }
			return hour * 3600 + min * 60 + sec;
		}

		/// <summary> Функция преобразовывает секунды формата <b>int</b> в строку ["чч:мм:сс"] (в сутках 86'400 секунд = [0..86'399]). </summary>
		/// <remarks> <b> second: </b> время в секундах. <br/> <b> h: </b> текстовый разделитель "ч" - часы на выбранном языке. <br/> <b> m: </b> текстовый разделитель "м" - минуты на выбранном языке. <br/> <b> s: </b> текстовый разделитель "с" - секунды на выбранном языке. <br/> </remarks>
		/// <returns> Возвращает время как строка <b>["чч:мм:сс"]</b>. с разделителями на выбранном языке, <br/>
		///		пример: <b>03ч:12м:47с</b>. Если разделители не указывать, строка будет иметь вид: <b>03:12:47</b> <br/>
		///		Если значение <b>second</b> превысит <b>86'399</b>, то функция вернёт время превышающее <b>"23:59:59"</b>. <br/>
		///		Если значение <b>second</b> будет отрицательным, то функция вернёт: <b>"?:?:?"</b>
		/// </returns>
		public static string ToTimeString(int second, string h = "", string m = "", string s = "") { return SecondToTimeString(second, h, m, s);	}
		/// <summary> Функция преобразовывает секунды формата <b>long</b> в строку ["чч:мм:сс"] (в сутках 86'400 секунд = [0..86'399]). </summary>
		/// <remarks> <inheritdoc cref="ToTimeString"/> </remarks> <returns> <inheritdoc cref="ToTimeString"/> </returns>
		public static string ToTimeString(long second, string h = "", string m = "", string s = "") { return SecondToTimeString(second, h, m, s);	}
		private static string SecondToTimeString(long second, string h = "", string m = "", string s = "") {
			if (second < 0) return "?:?:?";	string time = "";
			long hour = second / 3600; if (hour < 10) time = "0";	time += hour + h + ":";
			long min = second / 60 % 60; if (min < 10) time += "0";	time += min + m + ":";
			long sec = second % 60; if (sec < 10) time += "0";		if (sec < 0) time += "?"; else time += sec + s;
			return time;
        }
		/// <summary> Функция преобразовывает миллисекунды в строку ["чч:мм:сс:мс"] (в сутках 86'400'000 миллисекунд = [0..86'399'999]). </summary>
		/// <remarks> <b> millisecond: </b> время в миллисекундах. <br/> <b> h: </b> текстовый разделитель "ч" - часы на выбранном языке. <br/> <b> m: </b> текстовый разделитель "м" - минуты на выбранном языке. <br/> <b> s: </b> текстовый разделитель "с" - секунды на выбранном языке. <br/> <b> ms: </b> текстовый разделитель "мс" - милисекунды на выбранном языке. <br/> </remarks>
		/// <returns> Возвращает время как строка ["чч:мм:сс:мс"]. <br/>
		///		Если значение <b> millisecond </b> превысит 86'399'999, то функция вернёт время превышающее "23:59:59:999". <br/>
		///		Если значение <b>millisecond</b> будет отрицательным, то функция вернёт: <b>"??:??:??:???"</b>
		/// </returns>
		public static string msToTimeString(int millisecond, string h = "", string m = "", string s = "", string ms = "") {
			if (millisecond < 0) return "??:??:??:???";	string time = "";
			int hour = millisecond / 3600000; if (hour < 10) time = "0";
			time += hour + h + ":";
			int min = millisecond / 60000 % 60000; if (min < 10) time += "0";
			time += min + m + ":";
			int sec = millisecond / 1000 % 60;	if (sec < 10) time += "0";
			time += sec + s + ":";
			int msec = millisecond % 1000;	if (msec < 10) time += "00"; else if (msec < 100) time += "0";
			time += msec + ms;
			return time;
		}

		/// <summary> Метод форматирует число <b>value</b> добавляя отступы между разрядами. Пример: 1'000'000'000. <br/> Метод работает с целыми и дробными значениями. </summary>
		/// <value>	
		///		<b><paramref name="value"/>:</b> число в формате string. <br/>
		///		<b><paramref name="tab"/>:</b> разделитель. По умолчанию строка разделяется пробелами. <br/>
		/// </value>
		/// <returns> Возвращает отформатированное число в виде строки с пробелами между разрядами: xx'xxx'xxx'xxx. </returns>
		public static string toTABString(string value, string tab = " ") {
			if (value.Length < 4) return value; else { List<char> CHR = new List<char>(); int counter = 0;
				for (int i = value.Length - 1; i >= 0; i--) { counter++;
					if (value[i] == '.' || value[i] == ',') { CHR.Add('.'); counter = 0; }
					else if (counter == 4) { CHR.Add(tab[0]); counter = 0; i++;	} 
					else CHR.Add(value[i]);
				} CHR.Reverse(); return new string(CHR.ToArray());
			}
		}

		/// <summary> Проверка всех промежуточных разрешений экрана. <br/> Метод проверяет: входит ли текущая высота экрана в диапазон размеров. <br/>	При отсутствии одного из размеров, метод проверит только нижнюю или верхнюю границу. </summary>
		/// <value>	<b><paramref name="min"/>:</b> минимальный размер экрана. <br/>	<b><paramref name="max"/>:</b> максимальный размер экрана. </value>
		/// <returns> Возвращает <b>true</b> если высота экрана текущего разрешения попадает в множество. <br/> Возвращает <b>false</b> если оба параметра оказались <b>default</b> или значения вне диапазона. </returns>
		public static bool SCREEN(int min = -1, int max = -1) {
			if (min <= -1 && max <= -1) return false;
			else {
				if (max <= -1) { if (Screen.PrimaryScreen.Bounds.Size.Height < min) return true; } 
					else if (min <= -1) { if (Screen.PrimaryScreen.Bounds.Size.Height >= max) return true; }
						else if (Screen.PrimaryScreen.Bounds.Size.Height < max &&
								 Screen.PrimaryScreen.Bounds.Size.Height >= min) return true;
						return false;
			}
		}

		/// <summary> Метод преобразует пользовательское число в компоненту цвета <b>RGB</b>. <br/> Метод проверяет нижнюю и верхнюю границу преобразованного числа. </summary>
		/// <returns> Возвращает компоненту цвета <b>RGB</b> в диапазоне [0..255]. </returns>
		public static byte ToRGB<T> (T cl) {
			byte result = 0; ulong unsigned_cl = 0; long signed_cl = -1;
			if (cl is ulong) unsigned_cl = (ulong)(object)cl;	if (cl is long) signed_cl = (long)(object)cl;
			if (cl is uint) unsigned_cl = (uint)(object)cl;		if (cl is int) signed_cl = (int)(object)cl;
			if (cl is ushort) unsigned_cl = (ushort)(object)cl;	if (cl is short) signed_cl = (short)(object)cl;
			if (cl is sbyte) unsigned_cl = (byte)(object)cl;	if (cl is byte) signed_cl = (byte)(object)cl;
			if (cl is ulong || cl is uint || cl is ushort || cl is sbyte)
				if (unsigned_cl <= 0) result = 0; else if (unsigned_cl >= 255) result = 255; else result = (byte)unsigned_cl;
			if (typeof(T) == typeof(long) || typeof(T) == typeof(int) || typeof(T) == typeof(short) || typeof(T) == typeof(byte)) 
				if (signed_cl <= 0) result = 0; else if (signed_cl >= 255) result = 255; else result = (byte)signed_cl;
			return result;
		}

		/// <summary> Метод проверяет нижнюю/верхнюю границу компоненты цвета и корректирует в случае необхомости. </summary>
		/// <remarks> Ссылочное значение <b>cl</b> является изменяемым в ходе выполнения кода. </remarks>
		public static void toRGB(ref int cl) { if (cl < 0) cl = byte.MinValue; else if (cl > 255) cl = byte.MaxValue; }
		/// <summary> Метод проверяет нижнюю/верхнюю границу каждой компоненты цвета и корректирует в случае необхомости. </summary>
		/// <remarks> Ссылочные значения являются изменяемыми в ходе выполнения кода. </remarks>
		public static void toRGB(ref int R, ref int G, ref int B, ref int A) {
			if (R < 0) R = 0; else if (R > 255) R = 255;	if (G < 0) G = 0; else if (G > 255) G = 255;
			if (B < 0) B = 0; else if (B > 255) B = 255;	if (A < 0) A = 0; else if (A > 255) A = 255;
		}
		/// <summary> Метод проверяет нижнюю/верхнюю границу каждой компоненты цвета и корректирует в случае необхомости. </summary>
		/// <remarks> Ссылочные значения являются изменяемыми в ходе выполнения кода. </remarks>
		public static void toRGB (ref int R, ref int G, ref int B) {
			if (R < 0) R = 0; else if (R > 255) R = 255;	if (G < 0) G = 0; else if (G > 255) G = 255;
			if (B < 0) B = 0; else if (B > 255) B = 255;
		}
		/// <summary> Метод вычисляет новое промежуточное значение из пары чисел с учётом <b>Alpha</b>. <br/> Эти значениями могут быть как компоненты цвета, так и другие величины, где нужна интерполяция. <br/> Параметр <paramref name="Alpha"/> проверяется на допустимые значения, в случае выхода из диапазона, приводится к нижней или верхней границе. </summary>
		/// <value>
		///		<b><paramref name="First"/>:</b> первое значение, для которого применяется интерполяция <b>Alpha</b>. <br/> <b><paramref name="Second"/>:</b> второе значение, из которого получается возвращаемое. <br/>
		///		<b><paramref name="Alpha"/>:</b> величина интерполяции или прозрачности для значения <paramref name="First"/>, диапазон: <b>[0..1],</b> <br/> где 0 - полностью прозрачный, 1 - 100% не прозрачный, 0.5 - полупрозрачный (если параметры являются компонентами цвета). <br/>
		/// </value>
		/// <returns> Возвращает новое промежуточное значение с учётом <b>Alpha</b>. </returns>
		public static double Interpolate(double First, double Second, double Alpha) {
			Alpha = Alpha < 0 ? 0 : Alpha > 1 ? 1 : Alpha;//[0..1]
			//return First * (Alpha / 255) + Second * (1 - (Alpha / 255));
			return First * Alpha + Second * (1.0 - Alpha);
		}

		/// <summary> Метод проверяет число <b>NUM</b> на соответствие диапазону <b>[MIN..MAX],</b> <br/> в случае выхода из диапазона, число приводится к верхней или нижней границе. </summary>
		/// <value>
		///		<b><paramref name="MIN"/>/<paramref name="MAX"/>:</b> нижняя/верхняя граница диапазона. <br/>
		///		<b><paramref name="NUM"/>:</b> проверяемое число. <br/>
		/// </value>
		/// <returns> Возвращает число <b>NUM</b> (изменённое при необходимости). </returns>
		public static float NumToMinMax(float MIN, float MAX, float NUM) {
            return System.Math.Min(MAX, System.Math.Max(MIN, NUM));//величина NUM в диапазоне [MIN..MAX]
		}

		/// <summary> Метод конвертирует цветовую модель <b>RGB</b> (Color) в цветовую модель <b>HSV.</b> </summary>
		/// <returns> Возвращает преобразованную цветовую модель <b>HSV.</b> </returns>
		public static HSV ColorToHSV(this Color color) {
			int max = System.Math.Max(color.R, System.Math.Max(color.G, color.B));
			int min = System.Math.Min(color.R, System.Math.Min(color.G, color.B));
			double hue = color.GetHue();
			double saturation = (max == 0) ? 0 : 1.0 - (1.0 * min / max);
			double value = max / 255.0;
			return new HSV(hue, saturation, value);
		}
		/// <summary> Метод конвертирует цветовую модель <b>HSV</b> в цветовую модель <b>RGB</b> (Color). </summary>
		/// <returns> Возвращает преобразованную цветовую модель <b>RGB</b> (Color). </returns>
		public static Color HSVToColor(this HSV hsv) {
			int hi = Convert.ToInt32(System.Math.Floor(hsv.Hue / 60)) % 6;
			double f = hsv.Hue / 60 - System.Math.Floor(hsv.Hue / 60);
			double value = hsv.Value * 255;
			int v = Convert.ToInt32(value);
			int p = Convert.ToInt32(value * (1 - hsv.Saturation));
			int q = Convert.ToInt32(value * (1 - f * hsv.Saturation));
			int t = Convert.ToInt32(value * (1 - (1 - f) * hsv.Saturation));
			return (hi == 0) ? Color.FromArgb(255, v, t, p) : (hi == 1) ? Color.FromArgb(255, q, v, p) :
				(hi == 2) ? Color.FromArgb(255, p, v, t) : (hi == 3) ? Color.FromArgb(255, p, q, v) :
					(hi == 4) ? Color.FromArgb(255, t, p, v) : Color.FromArgb(255, v, p, q);
		}

		/// <summary> Метод конвертирует целое число в римскую систему исчисления в диапазоне [0..3'999] = 4000. </summary>
		/// <remarks> Выдаёт исключение <b><paramref name="ArgumentException"/></b> если <b>num</b> вне диапазона допустимых значений. </remarks>
		/// <returns> Возвращает строковой эквивалент записи числа <b>num</b> в римской системе исчисления. </returns>
		public static string IntToRoman(int num) {
			if (num < 0 || num > 3999) throw new ArgumentException("Ошибка. Значение должно быть в диапазоне [0..3'999].");
			if (num == 0) return "N";
			int[] values = new int[] { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
			string[] numerals = new string[] { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };
			StringBuilder result = new StringBuilder();
			for (int i = 0; i < 13; i++) { while (num >= values[i]) { num -= values[i]; result.Append(numerals[i]); }}
			return result.ToString();
		}

		/// <summary> Метод конвертирует строку <b>str</b> в последовательность байтов в кодировке <b>ASCII</b>, <br/> каждый ASCII код разделён символом <b>separator</b>. </summary>
		/// <returns> Возвращает эквивалент строки <b>str</b> как набор байтов <b>ASCII</b>. </returns>
		public static string ToASCII(string str, string separator) { 
			return string.Join(separator, Encoding.Default.GetBytes(str));
		}

		/// <summary> Метод изменяет каждый пиксель изображения <b>originalImage</b> умножая его оттенки на <b>newColor</b>. <br/> Прозрачные пиксели (100%) - не трогаются! </summary>
		/// <value>	<b><paramref name="originalImage"/>:</b> изображение, которое требуется изменить. <br/>
		/// <b><paramref name="newColor"/>:</b> цвет, который будет умножатсья на каждый пиксель изображения <b>originalImage</b>. <br/>
		/// </value>
		/// <returns> Возвращает изменённый <b>Bitmap</b> переданного оригинала изображения <b>originalImage</b>. </returns>
		public static Bitmap ChangeImageColor(Bitmap originalImage, Color newColor)	{
			Bitmap newImage = new Bitmap(originalImage.Width, originalImage.Height);//новое изображение такого же размера
			Rectangle rect = new Rectangle(0, 0, originalImage.Width, originalImage.Height);
			BitmapData originalData = originalImage.LockBits(rect, ImageLockMode.ReadOnly, originalImage.PixelFormat);
			BitmapData newData = newImage.LockBits(rect, ImageLockMode.WriteOnly, newImage.PixelFormat);
			int bytesPerPixel = Image.GetPixelFormatSize(originalImage.PixelFormat) / 8;
			int byteCount = originalData.Stride * originalImage.Height;
			// Создаем буферы для хранения данных изображения
			byte[] originalBuffer = new byte[byteCount];
			byte[] newBuffer = new byte[byteCount];
			Marshal.Copy(originalData.Scan0, originalBuffer, 0, byteCount);
			// Обрабатываем каждый пиксель изображения
			Parallel.For(0, rect.Height, y => {
				for (int x = 0; x < rect.Width; x++) {
					int index = y * originalData.Stride + x * bytesPerPixel;
					if (originalBuffer[index + 3] == 0) continue;//прозрачный пиксель не трогаем
					// Извлекаем цвет пикселя
					byte blue = originalBuffer[index];
					byte green = originalBuffer[index + 1];
					byte red = originalBuffer[index + 2];
					byte alpha = originalBuffer[index + 3];
					// Вычисляем новый цвет пикселя
					byte newBlue = (byte)((blue + newColor.B) / 2);
					byte newGreen = (byte)((green + newColor.G) / 2);
					byte newRed = (byte)((red + newColor.R) / 2);
					byte newAlpha = (byte)((alpha + newColor.A) / 2);
					// Записываем новый цвет в буфер нового изображения
					newBuffer[index] = newBlue;
					newBuffer[index + 1] = newGreen;
					newBuffer[index + 2] = newRed;
					newBuffer[index + 3] = newAlpha;
				}
			});
			Marshal.Copy(newBuffer, 0, newData.Scan0, byteCount);//Копируем данные из буфера нового изображения обратно в изображение
			originalImage.UnlockBits(originalData); newImage.UnlockBits(newData);//Разблокируем изображения
			return newImage;
		}
	}
}