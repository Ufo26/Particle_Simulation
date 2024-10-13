using System;
using System.Collections.Generic;
using System.Timers;

namespace UFO {
	/// <summary> Класс получает время прошедшее с 1 Января 1970 года в милисекундах. </summary>
	static class CurrentMillis {
		private static readonly DateTime Jan1St1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		/// <summary> Получает текущую временную метку в милисекундах. </summary>
		public static long Millisecond { get { return (long)(DateTime.UtcNow - Jan1St1970).TotalMilliseconds; } }
	}

	/// <summary> <b>Мой <paramref name="новый"/> класс для максимально простого подсчёта и вывода информации FPS.</b>  </summary>
	/// <remarks> 
	///		<paramref name="Подсчёт"/> <paramref name="FPS"/> <paramref name="происходит"/> <paramref name="с"/>
	///		<paramref name="помощью"/> <paramref name="таймера"/> <paramref name="работающего"/> <paramref name="в"/> <paramref name="фоне"/>. <br/>
	///		<b>Пример использования:</b> <br/> <code>
	///		FPS fps = FPS();//создание объекта
	///		fps.Start();//запуск
	///		
	///		while (...) { //бесконечный цикл симуляции
	///			string = fps.Show;//вывод FPS в любом месте цикла 
	///			fps.Frames++;//инкремент построенного кадра в конце цикла или в методе построения кадра
	///		}
	///	</code> </remarks>
	public class FPS_Counter_Timer {
		/// <summary> Хранит количество отсчитанных кадров. </summary>
		public int Frames = 0;
		/// <summary> Получает и задаёт количество FPS/сек. </summary>
		public int Show = 0;
		/// <summary> Получает и задаёт информацию о симуляции. <b>true</b> = симуляция на паузе, <b>false</b> = нет. </summary>
		public bool IsPaused = false;
		/// <summary> Хранит объект "секундомер". </summary>
		readonly Timer Swatch = new Timer( );

		/// <summary> Конструктор по умолчанию. </summary>
		public FPS_Counter_Timer() { Swatch.Interval = 1000; Swatch.Enabled = false;	Swatch.Elapsed += Timer_Elapsed; }
		/// <summary> Петля. Метод вызывается событием <b>Timer.Elapsed</b> по истечении интервала времени (1000 мс для FPS). </summary>
		private void Timer_Elapsed(object sender, ElapsedEventArgs e) { Show = Frames; Frames = 0; }

		/// <summary> Метод запускает ежесекундный подсчёт FPS. </summary>
		public void Start() { Swatch.Start(); }
		/// <summary> Метод останавливает ежесекундный подсчёт FPS и/или ставит его на паузу. </summary>
		public void Stop() { Swatch.Stop();	}
	}

	/// <summary> <b><paramref name="Старенький"/> класс из моей библиотеки C++ для подсчёта и вывода FPS.</b> </summary>
	///		<remarks> <paramref name="Подсчёт"/> <paramref name="FPS"/> <paramref name="происходит"/> <paramref name="с"/>
	///		<paramref name="помощью"/> <paramref name="разницы"/> <paramref name="системного"/> <paramref name="времени"/>. <br/>
	///		<b>Пример использования:</b> <br/> <code>
	///		sTime time_FPS = sTime();//создание объекта
	///			
	///		while (...) { //бесконечный цикл симуляции
	///			string = time_FPS.ShowFPS();//вывод FPS в любом месте цикла
	///			time_FPS.AddFrame();//увеличение счётчика кадров и обновление второго времени в конце цикла или в методе построения кадра
	///		}
	///	</code> </remarks>
	public class FPS_Counter {
		/// <summary> Хранит количество отсчитанных кадров. </summary>
		long Frames = 0;
		/// <summary> Хранит количество FPS/сек. </summary>
		long FPS = 0;
		/// <summary> Хранит первое время в милисекундах. </summary>
		long t1 = CurrentMillis.Millisecond;
		/// <summary> Хранит второе время в милисекундах. </summary>
		long t2 = CurrentMillis.Millisecond;
		/// <summary> Хранит время пройденное в симуляции в милисекундах. </summary>
		long StartTime = 0;
		/// <summary> Хранит время пройденное на паузе в милисекундах. </summary>
		long TimePause = 0;

		/// <summary> Метод засекает время в милисекундах на старте программы/процесса и не обнуляется до завершения программы. </summary>
		public void SetStartTime() { if (StartTime == 0) StartTime = CurrentMillis.Millisecond; else StartTime += CurrentMillis.Millisecond - TimePause; }
		/// <summary> Метод засекает время в милисекундах на паузе. </summary>
		public void SetTimePause() { TimePause = CurrentMillis.Millisecond; }
		/// <summary> Метод засекает время снаружи процесса (цикла) один раз для t1. </summary>
		//public void SetTime1() { t1 = CurrentMillis.Millisecond; }
		/// <summary> Метод увеличивает счётчик кадров на единицу и засекает новое время для t2. <br/> Рекомендуется вызывать в методе построения кадра. </summary>
		public void AddFrame() { t2 = CurrentMillis.Millisecond; Frames++; }
		/// <summary> Метод показывает текущий <b>FPS</b>, вычисляя его по формуле (по умолчанию раз в секунду). <br/> Параметр <b>elapsed</b> определяет как часто вычислять FPS, по умолчанию кол-во кадров в секунду вычисляется 1 раз в 1000 мс. </summary>
		public long ShowFPS(long elapsed = 1000) {
			if (t2 - t1 >= elapsed) { FPS = (long)(Frames / ((t2 - t1) / 1000.0)); Frames = 0; t1 = t2; } return FPS;
		}
		/// <summary> Метод показывает текущий <b>FPS/</b>, беря информацию из счётчика кадров раз в секунду. </summary>
		public long ShowFPS_FromFrames() { if (t2 - t1 >= 1000) { FPS = Frames; Frames = 0; t1 = t2; } return FPS; }
		/// <summary> Метод показывает время работы программы/процесса/симуляции в секундах. </summary>
		public long ShowTime() { if (t2 < StartTime) return 0; else return (t2 - StartTime) / 1000; }
	}
}
