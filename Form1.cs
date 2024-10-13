using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SkiaSharp.Views.Desktop;
using UFO;
using UFO.Logic;
using UFO.Math;
using static UFO.Enums_Structs;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using static UFO.cSIMULATION;
using System.IO;
using System.Linq;

namespace Particle_Simulation {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.Opaque, true);

            //окно Window OpenGL
            //MessageBox.Show(GL.GetString(StringName.Version));
            GL_WINDOW.MODE = Enums_Structs.DrawMode.Default;

            CreateData_pnl_Settings();
            GL_WINDOW.pnl_Settings = pnl_Settings;
            Form1_Load(null, null);
            //Run() - последней строкой!
            GL_WINDOW.Run();//входит в игровую петлю с максимальной частотой обновления кадров FPS, если VSync = On
            Close();
        }

        /// <summary> Хранит поле, связанное с окном <b>OpenGL</b>. </summary>
        GL_Window GL_WINDOW = new GL_Window() {
            Size = new Size(Screen.PrimaryScreen.WorkingArea.Size.Width, Screen.PrimaryScreen.WorkingArea.Size.Height),
            Location = new Point(0, 0), WindowBorder = WindowBorder.Resizable, WindowState = OpenTK.WindowState.Maximized,
            VSync = VSyncMode.On, Visible = true,
        };

        /// <summary> Свой кастомный генератор случайных чисел. </summary>
        public RANDOM Random = new RANDOM(RANDOM.INIT.RandomNext);
        /// <summary> Вектор для расчётов. </summary>
        public Vector2Df VECTOR = new Vector2Df();
        /// <summary> Хранит объект симуляции. </summary>
        public cSIMULATION SIMULATION = null;
        /// <summary> Содержит поля и методы отрисовки кадра. </summary>
        public DRAW Draw = null;
        /// <summary> Потоковый итератор. Каждый итератор массива имеет свой диапазон частиц. В сумме все итераторы охватывают каждую частицу списка. </summary>
        public Iterator[] _Iterator;

        /// <summary> Таймер в общем потоке. Управляет фронтендом: анимацией, визуальными контролами, интерфейсом и т.д. </summary>
        //public System.Windows.Forms.Timer Event_Timer_Frontend = null;
        /// <summary> Таймер в отдельном фоновом потоке. Управляет бэкендом: взаимодействием частиц, физикой и т.д. </summary>
        public System.Threading.Timer[] Event_Thread_Timer_Backend = null;
        /// <summary> Секундомер для отсчёта 1000 мс., по истечению которого на панельке <b>ss_Info</b> обновляется информация. </summary>
        System.Diagnostics.Stopwatch stopwatch_Paint = System.Diagnostics.Stopwatch.StartNew();

        /// <summary> Метод срабатывает 1 раз во время загрузки формы. </summary>
        private void Form1_Load(object sender, EventArgs e) {
            // Инициализация и настройка OpenGL контекста
            //GL.ClearColor(Color.Black);
            //GL.Enable(EnableCap.DepthTest);
            //// Включение ускорения GPU, если поддерживается
            //if (GL.GetString(StringName.Vendor).Contains("NVIDIA") || GL.GetString(StringName.Vendor).Contains("AMD"))
            //    { GL.Enable(EnableCap.SampleShading/*GpuShader4*/); }

            Text = "Симуляция частиц / C# / Window: " + Width + "x" + Height;
            skgl_Holst.ContextMenuStrip = cms_ContextMenu;//ассоциируем контекстное меню с текстовым полем

            var ScreenWidth = Screen.PrimaryScreen.Bounds.Size.Width;/*разрешение экрана по горизонтали*/
            var ScreenHeight = Screen.PrimaryScreen.Bounds.Size.Height;/*разрешение экрана по вертикали*/
            var AreaW = Screen.PrimaryScreen.WorkingArea.Size.Width;/*разрешение рабочей области без пуска и рочего по горизонтали*/
            var AreaH = Screen.PrimaryScreen.WorkingArea.Size.Height;/*разрешение рабочей области без пуска и рочего по вертикали*/

            //размер и позиция SKGLControl холста
            skgl_Holst.Size = new Size(AreaW, AreaH - cms_ContextMenu.Height);
            skgl_Holst.Location = new System.Drawing.Point(0, 0);

            pnl_Settings.Parent = skgl_Holst;
            //pnl_Settings.Size = new Size(400, (int)(AreaH / 1.8));
            pnl_Settings.Location = new Point(skgl_Holst.Width - 5, 5);

            ss_Info.Items.Add("FPS: ");/*0*/        ss_Info.Items.Add("Средний fps: ");/*1*/
            ss_Info.Items.Add("Частиц: ");/*2*/     ss_Info.Items.Add("Сумма импульсов: ");/*3*/
            ss_Info.Items.Add("Потоков: ");/*4*/    ss_Info.Items.Add("Счётчик потоков: ");/*5*/

            Size gc = new Size(40, 20);//размер сетки Grid в яччейках      Size(20, 10)
            Draw = new DRAW(GL_WINDOW.ClientSize.Width, GL_WINDOW.ClientSize.Height,
                GL_WINDOW.ClientSize.Width / gc.Width, GL_WINDOW.ClientSize.Height / gc.Height
            );
            //создание объекта - симуляция
            SIMULATION = new cSIMULATION {
                Size = new Size(GL_WINDOW.ClientSize.Width, GL_WINDOW.ClientSize.Height),//2D размер симуляции
                G = 0.6, KGd = 3000.0,//ГРАВИТАЦИЯ              0.2
                Kp = 1.0, KEd = 3000.0,//МАГНЕТИЗМ              0.4
                Fw = 0.75, KSd = 20.0,//СИЛЬНОЕ ВЗАИМОДЕЙСТВИЕ  0.5
                Density = 1.22, Viscosity = 0.182, Elasticity = 0.9, g = 0.981, KFС = 0.00025, Acceleration = 1.0,
                SizeGrid = new Size(gc.Width, gc.Height),//числа подбирать так, чтобы результат деления в Draw.GridPixels был без остатка
                SizeCellsPixels = new Size(GL_WINDOW.ClientSize.Width / gc.Width, GL_WINDOW.ClientSize.Height / gc.Height),
                //Draw = new DRAW(skgl_Holst.Size.Width, skgl_Holst.Size.Height)
             //   Draw = new DRAW(GL_WINDOW.ClientSize.Width, GL_WINDOW.ClientSize.Height)
            };
            GL_WINDOW.Link_SIMULATION = SIMULATION;
            GL_WINDOW.Link_Draw = Draw;

            //результат деления должен быть без остатка
            //SIMULATION.Draw.GridPixels = new Size(skgl_Holst.Size.Width / gc.Width, skgl_Holst.Size.Height / gc.Height);
            //Draw.GridPixels = new Size(GL_WINDOW.ClientSize.Width / gc.Width, GL_WINDOW.ClientSize.Height / gc.Height);
            skgl_Holst.BackColor = Draw.bgGroundColor;

            btn_RND_BackGround.Text = $"Случайный фон {GL_WINDOW.textureID}/{Directory.GetFiles("RESOURCES").Length}";
            btn_NEXT_BackGround.Text = $">> {GL_WINDOW.textureID}/{Directory.GetFiles("RESOURCES").Length}";
            btn_PREV_BackGround.Text = $"<< {GL_WINDOW.textureID}/{Directory.GetFiles("RESOURCES").Length}";

            //ТЕСТЫ
            //добавление объекта в лист green.Add(tmp) происходит с копированием при любом раскладе
            //если удалить объект в "part[2]", его копия в green[0] останется
            //var part = new List<PARTICLE>(); var green = new List<PARTICLE>();
            //for (int i = 0; i < 6; i++) { var tmp = new PARTICLE();
            //    if (i == 2 | i == 4) tmp.pColor = ColorType.Green;
            //    part.Add(tmp); if (tmp.pColor == ColorType.Green) green.Add(tmp);
            //} part[2] = null;

            btn_STOP.BackColor = btn_START.BackColor = Color.FromArgb(153, 180, 209);
            btn_STOP.ForeColor = btn_START.ForeColor = Color.Silver;

            //НАСТРОЙКИ ГРАФИКИ
            UD_circle_segments.ValueChanged += (s, a) => { GL_WINDOW.Circle_NumSegments = (int)UD_circle_segments.Value; };
            chb_ContourCircle.CheckedChanged += (s, a) => { GL_WINDOW.FlagContourCircle = chb_ContourCircle.Checked; };
            UD_circle_thickness.ValueChanged += (s, a) => { GL_WINDOW.ThicknessContour = (float)UD_circle_thickness.Value; };
            chb_AlphaColorCircle.CheckedChanged += (s, a) => { GL_WINDOW.AlphaColorCircle = chb_AlphaColorCircle.Checked; };

            //ФИЗИКА СИМУЛЯЦИИ
            rb_MoveCheck.CheckedChanged += (s, a) => { SIMULATION.MovingCheck = MoveCheck.MoveCheck; };
            rb_MoveCheckGrid.CheckedChanged += (s, a) => { SIMULATION.MovingCheck = MoveCheck.MoveCheckGrid; };
            rb_MoveCheckGridList.CheckedChanged += (s, a) => { SIMULATION.MovingCheck = MoveCheck.MoveCheckGridList; };

            //ЧАСТИЦА
            btn_Property_Particle.Click += (s, a) => { WinDlg_Property(); };

            //НАСТРОЙКИ СИМУЛЯЦИИ
            btn_CREATE.Click += (s, a) => {
                pnl_Settings.Cursor = Cursors.WaitCursor;
                сгенерироватьСимуляциюToolStripMenuItem_Click(null, null);
                if (SIMULATION.Particles != null && SIMULATION.Particles.Count > 0) {
                    btn_START.Enabled = true;
                    btn_START.BackColor = Color.Gainsboro; btn_START.ForeColor = Color.Black;
                }
                pnl_Settings.Cursor = Cursors.Default;
            };
            btn_START.Click += (s, a) => {
                запуститьСимуляциюToolStripMenuItem_Click(null, null);
                //btn_CREATE.Enabled = btn_START.Enabled = false; btn_STOP.Enabled = true;
                btn_CREATE.BackColor = btn_START.BackColor = Color.FromArgb(153, 180, 209);
                btn_CREATE.ForeColor = btn_START.ForeColor = Color.Silver;
                btn_STOP.BackColor = Color.Gainsboro; btn_STOP.ForeColor = Color.Black;
            };
            btn_STOP.Click += (s, a) => { //остановка симуляции
                SIMULATION.Close(); SIMULATION.Stop();
                for (int i = 0; i < Event_Thread_Timer_Backend.Length; i++)
                    Event_Thread_Timer_Backend[i].Change(0, Timeout.Infinite);
                btn_STOP.Enabled = false; btn_START.Enabled = btn_CREATE.Enabled = true;
                btn_STOP.BackColor = Color.FromArgb(153, 180, 209); btn_STOP.ForeColor = Color.Silver;
                btn_CREATE.BackColor = btn_START.BackColor = Color.Gainsboro;
                btn_CREATE.ForeColor = btn_START.ForeColor = Color.Black;
            };

            //кнопки со значениями по умолчанию
            button1.Click += (s, a) => { tb_Gravity.Value = 30; tb_Gravity_Scroll(tb_Gravity, new EventArgs()); };
            button2.Click += (s, a) => { tb_Gravity_Dist.Value = 5000; tb_Gravity_Dist_Scroll(tb_Gravity_Dist, new EventArgs()); };
            button3.Click += (s, a) => { tb_Electromagnetism.Value = 50; tb_Electromagnetism_Scroll(tb_Electromagnetism, new EventArgs()); };
            button4.Click += (s, a) => { tb_Electromagnetism_Dist.Value = 5000; tb_Electromagnetism_Dist_Scroll(tb_Electromagnetism_Dist, new EventArgs()); };
            button5.Click += (s, a) => { tb_StrongInteraction.Value = 37; tb_StrongInteraction_Scroll(tb_StrongInteraction, new EventArgs()); };
            button6.Click += (s, a) => { tb_StrongInteraction_Dist.Value = 20; tb_StrongInteraction_Dist_Scroll(tb_StrongInteraction_Dist, new EventArgs()); };
            button7.Click += (s, a) => { tb_Atmosphere.Value = 500; tb_Atmosphere_Scroll(tb_Atmosphere, new EventArgs()); };
            button8.Click += (s, a) => { tb_afg.Value = 981; tb_afg_Scroll(tb_afg, new EventArgs()); };
            button9.Click += (s, a) => { tb_Acceleration.Value = 50; tb_Acceleration_Scroll(tb_Acceleration, new EventArgs()); };
            button10.Click += (s, a) => { tb_KFC.Value = 1000; tb_KFC_Scroll(tb_KFC, new EventArgs()); };

            //всплывающие подсказки
            ToolTip[] Hint = new ToolTip[11];
            string T = "Кнопка", txt = "Выставить ползунок трекбара\nв значение - по умолчанию";
            Hint[0] = UFO.Extensions.CreateHint(button1, 0, 35000, T, txt, Color.Black, Color.MintCream, ToolTipIcon.Info, false);
            Hint[1] = UFO.Extensions.CreateHint(button2, 0, 35000, T, txt, Color.Black, Color.MintCream, ToolTipIcon.Info, false);
            Hint[2] = UFO.Extensions.CreateHint(button3, 0, 35000, T, txt, Color.Black, Color.MintCream, ToolTipIcon.Info, false);
            Hint[3] = UFO.Extensions.CreateHint(button4, 0, 35000, T, txt, Color.Black, Color.MintCream, ToolTipIcon.Info, false);
            Hint[4] = UFO.Extensions.CreateHint(button5, 0, 35000, T, txt, Color.Black, Color.MintCream, ToolTipIcon.Info, false);
            Hint[5] = UFO.Extensions.CreateHint(button6, 0, 35000, T, txt, Color.Black, Color.MintCream, ToolTipIcon.Info, false);
            Hint[6] = UFO.Extensions.CreateHint(button7, 0, 35000, T, txt, Color.Black, Color.MintCream, ToolTipIcon.Info, false);
            Hint[7] = UFO.Extensions.CreateHint(button8, 0, 35000, T, txt, Color.Black, Color.MintCream, ToolTipIcon.Info, false);
            Hint[8] = UFO.Extensions.CreateHint(button9, 0, 35000, T, txt, Color.Black, Color.MintCream, ToolTipIcon.Info, false);
            Hint[9] = UFO.Extensions.CreateHint(button10, 0, 35000, T, txt, Color.Black, Color.MintCream, ToolTipIcon.Info, false);

            Hint[10] = UFO.Extensions.CreateHint(tb_Acceleration, 0, 35000, "Трекбар",
                "Величина акселерации частицы (скорость изменения скорости):\n" +
                "[1.0] = нулевая скорость;\n[0.0] = нулевое трение;\nПромежуточные варианты - ускорение или замеление.",
                Color.Black, Color.MintCream, ToolTipIcon.Info, false);

            //Event_Timer_Frontend = new System.Windows.Forms.Timer { Interval = 1, Enabled = false };
            //Event_Timer_Frontend.Tick += _Frontend;
            Event_Thread_Timer_Backend = new System.Threading.Timer[1];
            for (int i = 0; i < Event_Thread_Timer_Backend.Length; i++)
                Event_Thread_Timer_Backend[i] = new System.Threading.Timer(_Backend, i, Timeout.Infinite, 0);
        }

        /// <summary> <b>ПЕТЛЯ</b>. Метод обрабатывает отрисовку графики в фоне и управляет контролами. </summary>
        //private void _Frontend(object sender, EventArgs e) {
        //    SIMULATION.Draw.CreatePathCircles(SIMULATION.Particles);
        //}

        /// <summary> Переменная блокировки/разблокировки ресурсов потока. </summary>
        static object block = new object();
        /// <summary> <b>ПЕТЛЯ</b>. Потоковый метод обрабатывающий симуляцию. <br/>
        /// Петля с фундаментальными взаимодействиями - физика частиц. </summary>
        private void _Backend(object state) {
            if (!SIMULATION.IsOpen || !SIMULATION.IsWork) return;
            //lock хорошо синхронизирует потоки, но падает скорость вычислений и частицы двигаются рывками
            lock (block) 
            { // Блокируем общий объект во имя синхронизации
                var count = SIMULATION.Particles.Count; int it = (int)state;

                //ЧАСТИЦА 1 [i]
                for (int i = _Iterator[it].Begin; i < _Iterator[it].End; i++) {
                    //Monitor.Enter(block);//блокировка потока. альтернативный способ
                    PARTICLE pi; if (i < _Iterator[it].End) pi = SIMULATION.Particles[i]; else break;
                    if (pi == null || pi.Delete) continue;

                    SIMULATION.setForce3In1(pi);
                    PARTICLE _pj = null;//ссылка на круг-препятствие
                    bool IsCollision = false;
                    if (SIMULATION.MovingCheck == MoveCheck.MoveCheckGridList ) {
                        if (pi.MoveCheckGridList(SIMULATION, out _pj)) { IsCollision = true; }//СТОЛКНОВЕНИЕ БЫЛО!
                    } else if (SIMULATION.MovingCheck == MoveCheck.MoveCheckGrid) {
                        if (pi.MoveCheckGrid(SIMULATION, out _pj)) { IsCollision = true; }//СТОЛКНОВЕНИЕ БЫЛО!
                    }

                    var velocity = pi.Velocity2D;//фактически пройденный путь
                    //ЧАСТИЦА 2 [j]
                    for (int j = 0; j < SIMULATION.Particles.Count; j++) {
                        var pj = SIMULATION.Particles[j];
                        if (i == j && i != SIMULATION.Particles.Count - 1) continue;//запрещаем сравнение частицы с собой
                        if (pj == null || pj.Delete) continue;

                        if (SIMULATION.MovingCheck == MoveCheck.MoveCheck) {
                            if (pi.MoveCheck(i, j, SIMULATION, ref _pj, ref velocity)) { //СТОЛКНОВЕНИЕ БЫЛО!
                               IsCollision = true;
                            }
                        }
                        if (i == j) continue;//запрещаем сравнение частицы с собой

                        //4 фундаментальных взаимодействий частиц в порядке убывания по силе
                        /*1 СИЛЬНОЕ ВЗАИМОДЕЙСТВИЕ*/
                        //Physics2D.StrongInteraction(pi, pj, SIMULATION);
                        /*2 ЭЛЕКТРОМАГНИТНОЕ ВЗАИМОДЕЙСТВИЕ*/
                        //Physics2D.Electromagnetism(pi, pj, SIMULATION);
                        //*3 СЛАБОЕ ВЗАИМОДЕЙСТВИЕ*/Physics2D.WeakInteraction(pi, pj);//НЕ АКТУАЛЬНО ДЛЯ ЭТОЙ СИМУЛЯЦИИ
                        /*4 ГРАВИТАЦИЯ*/
                        //Physics2D.Gravity(pi, pj, SIMULATION);

                        //растаскиваем 2 частицы если они столкнулись на данном тике цикла
                        if (Physics2D.Collision(pi, pj)) { 
                            //if (pi.pColor != pj.pColor)
                            Physics2D.Demarcation(pi, pj, Demarcation.Left);
                        }
                    }
                    //for (int L = 0; L < pi.LinkPart.Length; L++)
                    //	if (pi.LinkPart[L] != null) Physics2D.MooveCoupling(pi, ref pi.LinkPart[L], SIMULATION.KFС);

                    if (IsCollision) { //СТОЛКНОВЕНИЕ БЫЛО!
                        //if (pi.pColor == ColorType.Aqua || _pj.pColor == ColorType.Aqua)
                            //Physics2D.Coupling(pi, _pj);
                        //if (pi.pColor == _Type.White && _pj.pColor == _Type.White)
                        //Physics2D.Real_Impact(pi, _pj);/*отскок*/
                        //else if (pi.pColor != _Type.Violet && _pj.pColor != _Type.Violet)
                        Physics2D.ElasticImpact5(pi, _pj);/*отскок*/
                        //else Physics2D.InelasticImpact(pi, _pj);/*слипание*/

                        //реверс
                        //if (_pj.pColor == _Type.Black) { pi.Velocity.X = -pi.Velocity.X; pi.Velocity.Y = -pi.Velocity.Y; }
                        if (_pj.pColor == ColorType.White) { pi.Velocity2D.X -= 1; pi.Velocity2D.Y -= 1; }
                        //сменить цвет (тип)
                        if (pi.pColor != ColorType.White && _pj.pColor == ColorType.Magenta)
                            pi.pColor = (ColorType)Random.RND(2, (int)ColorType.Black);
                        if (_pj.pColor == ColorType.White && !pi.Delete && !_pj.Delete &&
                            (pi.pColor == ColorType.Red || pi.pColor == ColorType.Orange)) {
                            //*ПОЛНОЕ-СЛАБОЕ ПОГЛОЩЕНИЕ*/Physics2D.Collapse(pi, _pj, pi.TYPE); count--; 
                        }
                    }
                    SIMULATION.Border_Processing(pi, BorderWindow.REVERSE);
                    
                    //Monitor.Exit(block);//отмена блокировки потока. альтернативный способ
                }
                //создание нового списка за вычетом удалённых кругов
                if (count < SIMULATION.Particles.Count) {
                    List<PARTICLE> NewParticles = new List<PARTICLE>();
                    foreach (var part in SIMULATION.Particles) { 
                        if (!part.Delete) NewParticles.Add(part); else {
                            //отцепляем все перекрытия круга [i] в сетке grid по старым координатам Old
                            for (int n = 0; n < part.Grid.Item2.Count; n++) {
                                SIMULATION.Grid[part.Grid.Item2[n].X, part.Grid.Item2[n].Y].Remove(part);
                            }
                        }
                    }
                    for (int k = 0; k < _Iterator.Length; k++) {
                        _Iterator[k].Begin = (int)((float)NewParticles.Count / _Iterator.Length * k);
                        _Iterator[k].End = (int)((float)NewParticles.Count / _Iterator.Length * (k + 1));
                    }
                    SIMULATION.Particles = NewParticles;
                }
            }
            SIMULATION.Stop();//тест
        }

        int Count_Threads = 0;
        /// <summary> <b>ПЕТЛЯ</b>. Потоковый метод обрабатывающий симуляцию. <br/>
        /// Петля с произвольными правилами взаимодействия частиц. </summary>
        public /*async*/ void _Backend2(object state) {
            if (!SIMULATION.IsOpen || !SIMULATION.IsWork) return;
            //await Task.Run(() => { 
            //int it = (int)state;
            //ЧАСТИЦА 1 [i]
            for (int i = 0; i < SIMULATION.Particles.Count; i++) { 
            //for (int i = _Iterator[it].Begin; i < _Iterator[it].End; i++) {
                PARTICLE pi; //if (i < _Iterator[it].End) pi = SIMULATION.Particles[i]; else break;
                if (i < SIMULATION.Particles.Count) pi = SIMULATION.Particles[i]; else break;
                if (pi == null || pi.Delete) continue;

                int XX1 = pi.Grid.Item1.X - 3; int XX2 = pi.Grid.Item1.X + 3;//+ 2
                XX1 = XX1 < 0 ? 0 : XX1; XX2 = XX2 >= SIMULATION.SizeGrid.Width ? SIMULATION.SizeGrid.Width - 1 : XX2;
                int YY1 = pi.Grid.Item1.Y - 3; int YY2 = pi.Grid.Item1.Y + 3;//+ 2
                YY1 = YY1 < 0 ? 0 : YY1; YY2 = YY2 >= SIMULATION.SizeGrid.Height ? SIMULATION.SizeGrid.Height - 1 : YY2;
                //ЧАСТИЦА 2 [j]
                for (int y = YY1; y < YY2 + 1; y++) for (int x = XX1; x < XX2 + 1; x++) for (int j = 0; j < SIMULATION.Grid[x, y].Count; j++) {
                    PARTICLE pj; int index;
                    lock (block) {
                        if (j < SIMULATION.Grid[x, y].Count) pj = SIMULATION.Grid[x, y][j]; else break;
                        //if (index < SIMULATION.Particles.Count) pj = SIMULATION.Particles[index]; else break;
                        if (pj == null || pj.Delete) continue;
                        if (pi == pj) continue;
                    }
                    
                    if (Physics2D.Collision(pi, pj)) { Physics2D.Demarcation(pi, pj, Demarcation.Left); 
                        //if (pi.pColor == _Type.White && pj.pColor == _Type.White) 
                                     //Physics2D.Real_Impact(pi, pj);/*отскок*/
                        //else if (pi.pColor != _Type.Purple && pj.pColor != _Type.Purple)
                            //Physics2D.ElasticImpact5(pi, pj);/*отскок*/
                        //else
                        //Physics2D.InelasticImpact(pi, pj);/*слипание*/
                    }
                    //GREEN
                    Physics2D.Rule(pi, pj, ColorType.Green, ColorType.Green, 0.032f, 100);
                    Physics2D.Rule(pi, pj, ColorType.Green, ColorType.Green, -0.064f, 25);
                    Physics2D.Rule(pi, pj, ColorType.Yellow, ColorType.Green, 0.02f, 100);
                    Physics2D.Rule(pi, pj, ColorType.Red, ColorType.Green, 0.034f, 100);
                    ////hysics2D.Rule_For(White, Green, -0.01f, 60);
                    ////RED
                    Physics2D.Rule(pi, pj, ColorType.Red, ColorType.Red, -0.01f, 100);
                    Physics2D.Rule(pi, pj, ColorType.Green, ColorType.Red, -0.017f, 100);
                    Physics2D.Rule(pi, pj, ColorType.Aqua, ColorType.Red, 0.0013f, 150);
                    Physics2D.Rule(pi, pj, ColorType.Aqua, ColorType.Red, -0.013f, 50);
                    ////Physics2D.Rule_For(White, Red, -0.005f, 60);
                    ////YELLOW
                    Physics2D.Rule(pi, pj, ColorType.Yellow, ColorType.Yellow, -0.015f, 100);
                    Physics2D.Rule(pi, pj, ColorType.Green, ColorType.Yellow, 0.034f, 100);
                    Physics2D.Rule(pi, pj, ColorType.Red, ColorType.Yellow, -0.002f, 25);
                    //Physics2D.Rule_For(White, Yellow, -0.0015f, 60);
                    //AQUA
                    Physics2D.Rule(pi, pj, ColorType.Aqua, ColorType.Aqua, -0.08f, 30);
                    Physics2D.Rule(pi, pj, ColorType.Green, ColorType.Aqua, -0.0015f, 70);
                    Physics2D.Rule(pi, pj, ColorType.Red, ColorType.Aqua, -0.01f, 70);
                    //Physics2D.Rule_For(Yellow, Aqua, -0.005f, 70);
                    //Physics2D.Rule_For(White, Aqua, 0.01f, 200);
                    //Physics2D.Rule_For(White, Aqua, -0.015f, 80);
                    ////WHITE
                    //Physics2D.Rule_For(White, White, -0.5f, 20);
                    //Physics2D.Rule_For(Green, White, -0.05f, 50);
                    //Physics2D.Rule_For(Red, White, -0.012f, 100);
                    //Physics2D.Rule_For(Yellow, White, -0.015f, 50);
                    //Physics2D.Rule_For(Aqua, White, -0.025f, 100);
                    
                    var dist = Physics2D.Fast_Distance(pi.Position2D, pj.Position2D);
                    if (dist < 20*2 && dist > 0) { 
                       	if (pi.pColor != ColorType.White && //pi.pColor != _Type.Aqua &&
                            pj.pColor == ColorType.Aqua && pj.CountCoupling < 1) Physics2D.Coupling(pi, pj);
                    }
                }
                lock (block) {
                    //обработка связанных частиц
                    for (int L = 0; L < pi.LinkPart.Length; L++) { 
                        if (pi.LinkPart[L] != null) {
                            Physics2D.MooveCoupling(pi, ref pi.LinkPart[L], SIMULATION.KFС);
                        }
                    }

                    pi.Move();
                    SIMULATION.Border_Processing(pi, BorderWindow.REVERSE);
                    //перецепка частицы в другую ячейку после смены позиции, если это необходимо
                    if (!pi.Delete) SIMULATION.Remove_And_Add_Particle_In_AllCellGrid(pi);
                }
            }
            //Count_Threads++;
            //lock (block) SIMULATION.GridMarkup();
            //});
        }

        /// <summary> Метод генерирует новую симуляцию. </summary>
        private void сгенерироватьСимуляциюToolStripMenuItem_Click(object sender, EventArgs e) {
            Cursor = Cursors.WaitCursor;
            SIMULATION.Close(); SIMULATION.Stop();
            //остановка таймеров
            //Event_Timer_Frontend.Stop();
            for (int i = 0; i < Event_Thread_Timer_Backend.Length; i++)
                Event_Thread_Timer_Backend[i].Change(0, Timeout.Infinite);
            Thread.Sleep(200);

            int min = 500; int max = 500; double MAX_START_MASS = 2500;
            SIMULATION.Clear_Particles();
            goto lb1;
            //for (int i = 0; i < 50; i++)
              //  SIMULATION.Add_Particles(_Type.White, Random.RND(1, 200)/*Massa*/, MAX_START_MASS, Random.RND(0, 5)/*speed*/);
            for (int i = 0; i < 250; i++)
                SIMULATION.Add_Particles(ColorType.Green, Random.RND(min, max)/*Massa*/, MAX_START_MASS, Random.RND(0, 10)/*speed*/);
            for (int i = 0; i < 250; i++)
                SIMULATION.Add_Particles(ColorType.Red, Random.RND(min, max)/*Massa*/, MAX_START_MASS, Random.RND(0, 100)/*speed*/);
            for (int i = 0; i < 300; i++)
                SIMULATION.Add_Particles(ColorType.Yellow, Random.RND(min, max)/*Massa*/, MAX_START_MASS, Random.RND(0, 100)/*speed*/);
            for (int i = 0; i < 200; i++)
                SIMULATION.Add_Particles(ColorType.Aqua, Random.RND(min, max)/*Massa*/, MAX_START_MASS, Random.RND(0, 150)/*speed*/);

            lb1:
            SIMULATION.Add_Particles(ColorType.Red, 1200, MAX_START_MASS, 105);
            SIMULATION.Particles[0].Position2D.X = 50; SIMULATION.Particles[0].Position2D.Y = 510;
            SIMULATION.Particles[0].Velocity2D.SetXY(Math.Cos(0), Math.Sin(0));//180 0 <<     0 0 >>
            SIMULATION.Particles[0].Velocity2D = SIMULATION.Particles[0].GetVelocityFromSpeed(105*0);

            SIMULATION.Add_Particles(ColorType.Purple, 1100, MAX_START_MASS, 0);
            SIMULATION.Particles[1].Position2D.X = 250; SIMULATION.Particles[1].Position2D.Y = 500;
            SIMULATION.Particles[1].Velocity2D.SetXY(Math.Cos(0), Math.Sin(0));
            SIMULATION.Particles[1].Velocity2D = SIMULATION.Particles[1].GetVelocityFromSpeed(0);

            SIMULATION.Add_Particles(ColorType.Blue, 1450, MAX_START_MASS, 0);
            SIMULATION.Particles[2].Position2D.X = 550; SIMULATION.Particles[2].Position2D.Y = 500;
            SIMULATION.Particles[2].Velocity2D.SetXY(Math.Cos(0), Math.Sin(0));
            SIMULATION.Particles[2].Velocity2D = SIMULATION.Particles[2].GetVelocityFromSpeed(0);

            SIMULATION.Add_Particles(ColorType.Purple, 350, MAX_START_MASS, 0);
            SIMULATION.Particles[3].Position2D.X = 300; SIMULATION.Particles[3].Position2D.Y = 525;
            SIMULATION.Particles[3].Velocity2D.SetXY(Math.Cos(0), Math.Sin(0));
            SIMULATION.Particles[3].Velocity2D = SIMULATION.Particles[3].GetVelocityFromSpeed(0);

            SIMULATION.Add_Particles(ColorType.Aqua, 450, MAX_START_MASS, 50);
            SIMULATION.Particles[4].Position2D.X = 150; SIMULATION.Particles[4].Position2D.Y = 200;
            SIMULATION.Particles[4].Velocity2D.SetXY(Math.Cos(120), Math.Sin(120));
            SIMULATION.Particles[4].Velocity2D = SIMULATION.Particles[4].GetVelocityFromSpeed(50*0);

            SIMULATION.Add_Particles(ColorType.Purple, 400, MAX_START_MASS, 0);
            SIMULATION.Particles[5].Position2D.X = 210; SIMULATION.Particles[5].Position2D.Y = 500;
            SIMULATION.Particles[5].Velocity2D.SetXY(Math.Cos(0), Math.Sin(0));
            SIMULATION.Particles[5].Velocity2D = SIMULATION.Particles[5].GetVelocityFromSpeed(0);

            SIMULATION.Add_Particles(ColorType.Blue, 1450, MAX_START_MASS, 0);
            SIMULATION.Particles[6].Position2D.X = 550; SIMULATION.Particles[6].Position2D.Y = 565;
            SIMULATION.Particles[6].Velocity2D.SetXY(Math.Cos(0), Math.Sin(0));
            SIMULATION.Particles[6].Velocity2D = SIMULATION.Particles[6].GetVelocityFromSpeed(0);

            SIMULATION.Add_Particles(ColorType.Green, 1700, MAX_START_MASS, 50);
            SIMULATION.Particles[7].Position2D.X = 1200; SIMULATION.Particles[7].Position2D.Y = 510;
            SIMULATION.Particles[7].Velocity2D.SetXY(Math.Cos(180), Math.Sin(0));
            SIMULATION.Particles[7].Velocity2D = SIMULATION.Particles[7].GetVelocityFromSpeed(50*0);

            SIMULATION.Add_Particles(ColorType.Purple, 2000, MAX_START_MASS, 0);
            SIMULATION.Particles[8].Position2D.X = 150; SIMULATION.Particles[8].Position2D.Y = 555;
            SIMULATION.Particles[8].Velocity2D.SetXY(Math.Cos(0), Math.Sin(0));
            SIMULATION.Particles[8].Velocity2D = SIMULATION.Particles[8].GetVelocityFromSpeed(0);

            //кладём каждую частицу в свою ячейку таблицы Grid учитывая расположение этой частицы в симуляции
            SIMULATION.GridMarkup();
            //этот код должен быть здесь, т.к. лист Particles создаётся тоже здесь, а минилисты берут инфу из него
            Draw.ColorList.Green = SIMULATION.CreateMiniListParticles(ColorType.Green);
            Draw.ColorList.Red = SIMULATION.CreateMiniListParticles(ColorType.Red);
            Draw.ColorList.Yellow = SIMULATION.CreateMiniListParticles(ColorType.Yellow);
            Draw.ColorList.Aqua = SIMULATION.CreateMiniListParticles(ColorType.Aqua);

            //растаскиваем частицы чтобы они не перекрывали друг друга
            //возможен бесконечный цикл, если суммарная площадь частиц больше площади пространства
            //так же код зацикливается при множестве кругов больших радиусов 30+
            //bool D = true; while (D) { D = false;
            //    for (int i = 0; i < SIMULATION.Particles.Count; i++) for (int j = 0; j < SIMULATION.Particles.Count; j++) {
            //        if (i == j) continue;
            //        if (Physics2D.Collision(SIMULATION.Particles[i], SIMULATION.Particles[j])) {
            //            Physics2D.Demarcation(SIMULATION.Particles[i], SIMULATION.Particles[j]); D = true;
            //}}}

            _Iterator = new Iterator[Event_Thread_Timer_Backend.Length];
            for (int i = 0; i < _Iterator.Length; i++) _Iterator[i] = new
                Iterator((int)((float)SIMULATION.Particles.Count / _Iterator.Length * i),
                         (int)((float)SIMULATION.Particles.Count / _Iterator.Length * (i + 1)));

            //GL_WINDOW.Link_SIMULATION = SIMULATION;
            //GAME.LinkParticles = SIMULATION.Particles;
            //GAME.LinkGrid = SIMULATION.Grid;
            GL_WINDOW.Count_Thread_Timer_Backend = Event_Thread_Timer_Backend.Length;

            SIMULATION.Open();
            Cursor = Cursors.Default;
        }

        /// <summary> Метод запускает симуляцию. </summary>
        private void запуститьСимуляциюToolStripMenuItem_Click(object sender, EventArgs e) {
            if (SIMULATION.IsWork == false) {
                SIMULATION.Open(); SIMULATION.Strat();
                //Event_Timer_Frontend.Start();//запуск таймера
                for (int i = 0; i < Event_Thread_Timer_Backend.Length; i++)
                    Event_Thread_Timer_Backend[i].Change(0, 1);//запуск без перезапуска таймера
            }
        }

        //int Count_fps = 0; int Summ_fps = 0; int Summ_threads = 0;
        /// <summary> Control: skgl_Holst. Окрашиваемая поверхность. <br/> Вызывается всякий раз при перерисовке поверхности. </summary>
        private void skgl_Holst_PaintSurface(object sender, SKPaintGLSurfaceEventArgs e) {
//            return;
//            System.Diagnostics.Stopwatch t = System.Diagnostics.Stopwatch.StartNew();
//            //Clear(SKColors.LightSeaGreen);

//            //ПРОИЗВОДИТЕЛЬНОСТЬ РИСОВАНИЯ:
//            //1. SKBitmap/SKCanvas + КОПИРОВАНИЕ SKBitmap НА SKGLControl МЕТОДОМ e.Surface.Canvas.DrawBitmap()
//            //   ПРОИСХОДИТ НА ПРОЦЕССОРЕ CPU. СКОРОСТЬ ОТРИСОВКИ: 90 FPS на 800 частиц (с выключенной физикой)
//            //2. РИСОВАНИЕ НА e.Surface.Canvas КОНТРОЛА SKGLControl В ЕГО ОБРАБОТЧИКЕ СОБЫТИЯ PaintSurface
//            //   ПРОИСХОДИТ НА ВИДЕОКАРТЕ GPU. СКОРОСТЬ ОТРИСОВКИ: 570 FPS на 800 частиц (с выключенной физикой)
//            // 1. запуск без физики и графики на 1000 частиц, 1 поток - 3500 FPS
//            // 2. с выключенной отрисовкой на 1000 частиц, 1 поток - 2500 FPS
//            // 3. с выключенной физикой на 1000 частиц, 1 поток - 450 FPS
//            // 4. запуск "всё включено" на 1000 частиц, 1 поток - 250 FPS
////            SIMULATION.Draw.CreateFrame(SIMULATION.Particles, e.Surface.Canvas, SIMULATION.Grid);
//            //e.Surface.Canvas.DrawBitmap(SIMULATION.Draw.sk_BMP, new SKPoint(0, 0));

//            //if (SIMULATION.Draw.TestInfo != "0")  Text = "FRAME time = " + SIMULATION.Draw.TestInfo + " ms";
//            //Summ_fps += (int)SIMULATION.Draw.FPS.ShowFPS(); Count_fps++;
//            Summ_fps += (int)GL_WINDOW.FPS.ShowFPS(); Count_fps++;
//            Summ_threads += Count_Threads;
//            if (Count_fps >= 1000) { ss_Info.Items[1].Text = $"Средний fps на 1000 кадров: [{Summ_fps / Count_fps}]";
//                Count_fps = Summ_fps = 0;
//                ss_Info.Items[5].Text = $"Среднее кол-во отработанных потоков на 1000 кадров: [{Summ_threads / System.Math.Max(Count_Threads, 1)}]";
//                Count_Threads = Summ_threads = 0;
//            }
//            if (stopwatch_Paint.ElapsedMilliseconds > 1000) {
//                ss_Info.Items[0].Text = $"FPS: {/*SIMULATION.Draw.*/GL_WINDOW.FPS.ShowFPS()}";
//                //ss_Info.Items[0].Text = $"FPS: {SIMULATION.Draw.FPS.ShowFPS()} | FPS2: {SIMULATION.Draw.FPS2.Show}";
//                ss_Info.Items[2].Text = $"Частиц: {SIMULATION.Particles.Count}";
//                double impuls = 0; foreach (var part in SIMULATION.Particles) { impuls += part.GetSpeed(part.Velocity2D) * part.Massa; }
//                ss_Info.Items[3].Text = $"Сумма импульсов: {impuls:0.##}";
//                ss_Info.Items[4].Text = $"Потоков: {Event_Thread_Timer_Backend.Length}";
//                ss_Info.Refresh(); stopwatch_Paint.Restart();
//            }
//            GL_WINDOW.Title = $"{ss_Info.Items[0].Text} | {ss_Info.Items[1].Text} | {ss_Info.Items[2].Text} | {ss_Info.Items[3].Text} | {ss_Info.Items[4].Text} | {ss_Info.Items[5].Text}";
//            if (SIMULATION.IsOpen) {
//                //_Backend2(null);
//                t.Stop(); Text = t.ElapsedMilliseconds.ToString();
//                skgl_Holst.Invalidate();
//            }
        }

        /// <summary> Control: skgl_Holst. Перерисовка поверхности. </summary>
        private void skgl_Holst_Paint(object sender, PaintEventArgs e) {
            //skgl_Holst.Invalidate();
        }

//========================================== МЕТОДЫ ИНТЕРФЕЙСА И КОНТРОЛОВ ==========================================
        /// <summary> Метод обрабатывает движение мыши по skgl_Holst. </summary>
        private void skgl_Holst_MouseMove(object sender, MouseEventArgs e) {
            if (e.X >= skgl_Holst.Width - 20) {
                //await System.Threading.Tasks.Task.Run(() => { 
                    var p = pnl_Settings; int div = p.Width / 2;
                    while (p.Right > p.Parent.Right - 5) {
                        //p.BeginInvoke((MethodInvoker)(() => { 
                            p.Left -= p.Left / div; div += 4; pnl_Settings.Update();
                        //}));
                        Thread.Sleep(1);
                    }
                //});
                pnl_Settings.Left = pnl_Settings.Parent.Right - pnl_Settings.Width - 5;
            } else { pnl_Settings.Left = pnl_Settings.Parent.Width - 5; }
        }

        /// <summary> Метод обрабатывает изменение величины ползунка "Гравитация". </summary>
        private void tb_Gravity_Scroll(object sender, EventArgs e) {
            SIMULATION.G = tb_Gravity.Value / 50.0;
            label1.Text = $"Притяжение [{SIMULATION.G:0.##}]:"; pnl_Settings.Update();
        }
        /// <summary> Метод обрабатывает изменение величины ползунка дистанции действия гравитации. </summary>
        private void tb_Gravity_Dist_Scroll(object sender, EventArgs e) {
            SIMULATION.KGd = tb_Gravity_Dist.Value;
            label3.Text = $"Дистанция [{tb_Gravity_Dist.Value}]:"; pnl_Settings.Update();
        }

        /// <summary> Метод обрабатывает изменение величины ползунка "Электромагнетизм". </summary>
        private void tb_Electromagnetism_Scroll(object sender, EventArgs e) {
            SIMULATION.Kp = tb_Electromagnetism.Value / 50.0;
            label2.Text = $"Электромагнетизм [{SIMULATION.Kp:0.##}]:"; pnl_Settings.Update();
        }
        /// <summary> Метод обрабатывает изменение величины ползунка дистанции действия электромагнетизма. </summary>
        private void tb_Electromagnetism_Dist_Scroll(object sender, EventArgs e) {
            SIMULATION.KEd = tb_Electromagnetism_Dist.Value;
            label4.Text = $"Дистанция [{tb_Electromagnetism_Dist.Value}]:"; pnl_Settings.Update();
        }

        /// <summary> Метод обрабатывает изменение величины ползунка "Сильное взаимодействие". </summary>
        private void tb_StrongInteraction_Scroll(object sender, EventArgs e) {
            SIMULATION.Fw = tb_StrongInteraction.Value / 50.0;
            label5.Text = $"Сильное взаимодействие [{SIMULATION.Fw:0.##}]:"; pnl_Settings.Update();
        }
        /// <summary> Метод обрабатывает изменение величины ползунка дистанции действия сильного взаимодействия. </summary>
        private void tb_StrongInteraction_Dist_Scroll(object sender, EventArgs e) {
            SIMULATION.KSd = tb_StrongInteraction_Dist.Value;
            label6.Text = $"Дистанция [{tb_StrongInteraction_Dist.Value}]:"; pnl_Settings.Update();
        }

        /// <summary> Метод обрабатывает изменение величины ползунка плотности атмосферы. </summary>
        private void tb_Atmosphere_Scroll(object sender, EventArgs e) {
            SIMULATION.Density = tb_Atmosphere.Value;
            label7.Text = $"Плотность атмосферы [{tb_Atmosphere.Value}]:"; pnl_Settings.Update();
        }

        /// <summary> Метод обрабатывает изменение величины ползунка ускорения свободного падения (силы тяжести). </summary>
        private void tb_afg_Scroll(object sender, EventArgs e) {
            SIMULATION.g = (double)(tb_afg.Value) / tb_afg.Maximum;
            label8.Text = $"Ускор.св.пад [{SIMULATION.g:0.###}]:"; pnl_Settings.Update();
        }

        /// <summary> Метод обрабатывает изменение величины ползунка силы трения для каждой частицы. </summary>
        private void tb_Acceleration_Scroll(object sender, EventArgs e) {
            double NUM = tb_Acceleration.Maximum - tb_Acceleration.Value;
            var num = NUM / tb_Acceleration.Maximum;
            SIMULATION.Acceleration = num;
            foreach (var Particle in SIMULATION.Particles) Particle.Acceleration = num;
            label9.Text = $"Акселерация [{1 - num:0.###}]:"; pnl_Settings.Update();
        }

        /// <summary> Метод обрабатывает изменение величины ползунка коэффициента силы связи частиц. </summary>
        private void tb_KFC_Scroll(object sender, EventArgs e) {
            var num = tb_KFC.Value / 1_000_000.0; SIMULATION.KFС = num;
            label10.Text = $"Сила связи [{num:0.####}]:"; pnl_Settings.Update();
        }

        /// <summary> Метод обрабатывает изменение радио кнопок переключателей режимов отрисовки симуляции. </summary>
        private void rb_MODE_CheckedChanged(object sender, EventArgs e) {
            var rb = sender as RadioButton;
            //SIMULATION.Draw.MODE = (Enums_Structs.DrawMode)System.Convert.ToInt32(rb.Tag);
            GL_WINDOW.MODE = (Enums_Structs.DrawMode)System.Convert.ToInt32(rb.Tag);
            //MessageBox.Show("Выбран режим: " + GAME.MODE);
        }

        /// <summary> Метод меняет фоновую картинку симуляции на случайную. </summary>
        private void btn_RND_BackGround_Click(object sender, EventArgs e) {
            if (GL_WINDOW.bg_textureArray.Length <= 1) return;
            var index = GL_WINDOW.textureID;//SIMULATION.Draw.bgGroundImage_Index;
            while (index == GL_WINDOW.textureID) {
                GL_WINDOW.textureID = Random.RND(0, GL_WINDOW.bg_textureArray.Length - 1);
                skgl_Holst.Refresh();
            }
            btn_RND_BackGround.Text = $"Случайный фон {GL_WINDOW.textureID}/{GL_WINDOW.bg_textureArray.Length}";
            btn_NEXT_BackGround.Text = $">> {GL_WINDOW.textureID}/{GL_WINDOW.bg_textureArray.Length}";
            btn_PREV_BackGround.Text = $"<< {GL_WINDOW.textureID}/{GL_WINDOW.bg_textureArray.Length}";
            btn_PREV_BackGround.Left = btn_RND_BackGround.Right + 5;
            btn_NEXT_BackGround.Left = btn_PREV_BackGround.Right + 5;
        }

        /// <summary> Метод меняет фоновую картинку на предыдущую. </summary>
        private void btn_PREV_BackGround_Click(object sender, EventArgs e) {
            GL_WINDOW.textureID -= GL_WINDOW.textureID > 1 ? 1 : -(GL_WINDOW.bg_textureArray.Length - 1);
            skgl_Holst.Refresh();
            btn_RND_BackGround.Text = $"Случайный фон {GL_WINDOW.textureID}/{GL_WINDOW.bg_textureArray.Length}";
            btn_PREV_BackGround.Text = $"<< {GL_WINDOW.textureID}/{GL_WINDOW.bg_textureArray.Length}";
            btn_NEXT_BackGround.Text = $">> {GL_WINDOW.textureID}/{GL_WINDOW.bg_textureArray.Length}";
            btn_PREV_BackGround.Left = btn_RND_BackGround.Right + 5;
            btn_NEXT_BackGround.Left = btn_PREV_BackGround.Right + 5;
        }

        /// <summary> Метод меняет фоновую картинку на следующую. </summary>
        private void btn_NEXT_BackGround_Click(object sender, EventArgs e) {
            GL_WINDOW.textureID += GL_WINDOW.textureID < GL_WINDOW.bg_textureArray.Length ? 1
                : -(GL_WINDOW.bg_textureArray.Length - 1);
            skgl_Holst.Refresh();
            btn_NEXT_BackGround.Text = $">> {GL_WINDOW.textureID}/{GL_WINDOW.bg_textureArray.Length}";
            btn_PREV_BackGround.Text = $"<< {GL_WINDOW.textureID}/{GL_WINDOW.bg_textureArray.Length}";
            btn_RND_BackGround.Text = $"Случайный фон {GL_WINDOW.textureID}/{GL_WINDOW.bg_textureArray.Length}";
            btn_PREV_BackGround.Left = btn_RND_BackGround.Right + 5;
            btn_NEXT_BackGround.Left = btn_PREV_BackGround.Right + 5;
        }
    }
}
