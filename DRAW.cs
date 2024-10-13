//using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using UFO.Logic;
using UFO.Math;
using static UFO.Enums_Structs;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Linq;
using OpenTK.Input;
using static System.Math;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace UFO {
    /// <summary> Класс отрисовки графики частиц симуляции. Содержит: <br/>
    /// 1. DRAW.<b>GL_Window</b> - окно OpenGL; <br/>
    /// 2. DRAW.<b>GL_DRAW_2D</b> - набор статичных методов для рисования геометрических примитивов: <br/>
    /// ....a) DRAW.GL_DRAW_2D.<b>BeginEnd</b> - самый простой и устаревший способ рисования в OpenGL; <br/>
    /// ....b) DRAW.GL_DRAW_2D.<b>DrawArrays</b> - более быстрый способ рисования в OpenGL; <br/>
    /// ....с) VBO, Шейдеры - ещё более быстрые способы рисования, но пока не реализованы в данном проекте; <br/>
    /// 3. Рендер встроен в класс окна <b>GL_Window</b>. </summary>
    public class DRAW {
        /// <summary> Свой кастомный генератор случайных чисел. </summary>
        //public RANDOM Random = new RANDOM(RANDOM.INIT.RandomNext);

        /// <summary> Размер 2D холста области рисования. </summary>
        public Size Size;
        /// <summary> Хранит масштаб. Scale(1) = масштаб 100%, Scale(0.5) = масштаб 50%, Scale(1.6) = масштаб 160%. </summary>
        public double Scale;
        /// <summary> Хранит шаг масштаба. Scale изменяется на эту величину. </summary>
        public double Step;
        /// <summary> Хранит цвет фона холста (PictureBox). </summary>
        public Color bgGroundColor;
        /// <summary> Хранит цвет линии связи. </summary>
        //public SKColor LineCoupling;
        /// <summary> Размер ячейки таблицы в пикселях. </summary>
        public Size GridPixels = new Size();
        /// <summary> Режим отрисовки симуляции. </summary>
        //public DrawMode MODE = DrawMode.Default;


        /// <summary> Хранит структуру листов частиц, сгруппированных по цвету. </summary>
        public MiniListColor ColorList = new MiniListColor(true);

        /// <summary> Хранит текстовую тестовую информацию. </summary>
        public string TestInfo = "";
        public bool Lock = false;

        /// <summary> Конструктор класса <b>DRAW</b>. Инициализирует поля класса необходимыми значениями. </summary>
        public DRAW(int Width, int Height, int CellWidth, int CellHeight) {
	        Scale = 1;//масштаб. Scale(1) => 100%. Scale(1.5) = 1.5 * 100 = 150%. Scale(0.3) = 0.3 * 100 = 30%
	        Step = 0.1;//шаг масштаба. Scale изменяется на эту величину
            bgGroundColor = Color.FromArgb(40, 40, 40);
            //LineCoupling = new SKColor(255, 255, 255, 120);
            Size = new Size(Width, Height);
            GridPixels = new Size(CellWidth, CellHeight);
        }
    }

    //=======================================================================================================================
    //----------------------------------------- КЛАСС ДЛЯ МАНИПУЛЯЦИЙ С ОКНОМ OpenGL ----------------------------------------
    //=======================================================================================================================
    /// <summary> Класс создаёт окно Window OpenGL унаследованное от OpenTK.GameWindow и рисует на нём всю графику. </summary>
    public class GL_Window : GameWindow {
        /// <summary> Свой кастомный генератор случайных чисел. </summary>
        public RANDOM Random = new RANDOM(RANDOM.INIT.RandomNext);
        /// <summary> Хранит объект считающий кол-во кадров в секунду с помощью разницы системного времени. </summary>
        public FPS_Counter FPS = new FPS_Counter();
        /// <summary> Хранит FPS, посчитанный средствами OpenGL. </summary>
        private int fps = 0;
        /// <summary> Хранит время, за которое построился 1 кадр графики OpenGL. </summary>
        private double frameTime = 0;
        /// <summary> Хранит ссылку на симуляцию. </summary>
        public cSIMULATION Link_SIMULATION;
        /// <summary> Хранит ссылку на рендер симуляции. </summary>
        public DRAW Link_Draw;
        /// <summary> Режим отрисовки симуляции. </summary>
        public DrawMode MODE;

        /// <summary> Хранит цвет фона окна OpenGL. Диапазон 0.0f - 1.0f. </summary>
        public Color4 bgColor;
        /// <summary> Хранит цвет линии связи. Диапазон 0.0f - 1.0f. </summary>
        public Color4 LineCouplingColor;

        /// <summary> Хранит флаг альфа канала цвета круга. <br/> <b>true</b> = круг может иметь прозрачный цвет фона при остановке движения; <br/> <b>false</b> = круг не может иметь прозрачный и полупрозрачный цвет фона. </summary>
        public bool AlphaColorCircle = true;

        /// <summary> Хранит флаг контура круга. <br/> <b>true</b> = есть контур; <br/> <b>false</b> = нет. </summary>
        public bool FlagContourCircle = true;
        /// <summary> Хранит толщину контура круга. </summary>
        public float ThicknessContour = 2;

        /// <summary> Хранит индекс текущей отображаемой текстуры-картинки в OpenGL окне. </summary>
        public int textureID = 3;
        /// <summary> Хранит массив загруженных фоновых текстур в память видеокарты, диапазон: [0..N], count = N + 1; </summary>
        public int[] bg_textureArray;

        int circle_num_segments = 6;
        /// <summary> Хранит кол-во граней круга. <br/> Количество сегментов (numSegments) определяет гладкость круга: <br/>
        ///     numSegments: 360 = гладкий круг, 6 = гексагон (пчелиные соты), 4 = квад, 3 = треугольник. <br/> допустимый диапазон [3..360].  </summary>
        public int Circle_NumSegments {
            get { return circle_num_segments; }
            set { circle_num_segments = value < 3 ? 3 : value > 360 ? 360 : value; } 
        }
        /// <summary> Хранит массив вершин XY для всех имеющихся кругов. <br/>
        ///     Пример: <br/> при NumSegments = 6, размер массива для 1000 кругов = 2 х 6 х 1000 = 12'000 элементов, 1 круг будет использовать 2 х 6 = 12 элементов массива. </summary>
        /// <remarks> При изменении кол-ва частиц в цикле физики симуляции, так же следует изменить размер этого массива согласно новому кол-ву частиц. </remarks>
        double[] circle_vertices2D;
        /// <summary> Хранит массив цветов в формате RGBA для каждой вершины XY, для всех имеющихся кругов. <br/>
        ///     Пример: <br/> при NumSegments = 6, размер массива для 1000 кругов = 4 х 6 х 1000 = 24'000 элементов, 1 круг будет использовать 4 х 6 = 24 элемента массива. </summary>
        /// <remarks> <inheritdoc cref="circle_vertices2D"/> </remarks>
        float[] circle_colors4;
        /// <summary> Хранит массив вершин XY для всех имеющихся линий связи. <br/>
        ///     Пример: <br/> линия имеет 4 координаты (x1y1 - x2y2). <br/> По дефолту 1 связь на 1 частицу: размер массива для 1000 кругов = 4 х 1 х 1000 = 4'000 элементов, 1 линия связи будет использовать 4 х 1 = 4 элементов массива; <br/>
        ///     10 связей на 1 частицу: размер массива для 1000 кругов = 4 х 10 х 1000 = 40'000 элементов, 10 линий связи будут использовать 4 х 10 = 40 элементов массива. </summary>
        /// <remarks> <inheritdoc cref="circle_vertices2D"/> </remarks>
        double[] line_vertices2D;
        /// <summary> Хранит массив цветов в формате RGBA для каждой вершины XY, для всех имеющихся линий связи. <br/>
        ///     Пример: <br/> По дефолту 1 связь на 1 частицу: размер массива для 1000 кругов = 4 х 2 х 1 х 1000 = 8'000 элементов, 1 линия связи будет использовать 4 х 2 х 1 = 8 элементов массива; <br/>
        ///     10 связей на 1 частицу: размер массива для 1000 кругов = 4 х 2 х 10 х 1000 = 80'000 элементов, 10 линий связи будут использовать 4 х 2 х 10 = 80 элементов массива. </summary>
        /// <remarks> <inheritdoc cref="circle_vertices2D"/> </remarks>
        float[] line_colors4;

        GL_DRAW_2D.VBO meshVbo;
        GL_DRAW_2D.VAO meshVao;
        GL_DRAW_2D.ShaderProgram shaderProgram;

        /// <summary> Класс создаёт и хранит массив фоновых текстур. </summary>
        public static class BackGroundTextures {
            /// <summary> Метод создаёт массив фоновых текстур из папки с ресурсами. </summary>
            /// <value> <b><paramref name="paths"/>:</b> массив путей к файлам с изображениями. <br/> </value>
            /// <returns> Возвращает массив фоновых картинок (null - если картинок нет). </returns>
            public static int[] CreateTexturesFromFiles(string[] paths) {
                //var path = Directory.GetFiles("RESOURCES");
                if (paths.Length <= 0) return null; 
                int[] ID = new int[paths.Length];
                for (int i = 0; i < paths.Length; i++) ID[i] = LoadTexture(paths[i]);
                return ID;
            }

            /// <summary> Метод загружает картинку в память видеокарты с помощью функций GL. </summary>
            /// <value> <b><paramref name="path"/>:</b> путь к файлу с изображением. <br/> </value>
            /// <returns> Возвращает номер созданной текстуры в памяти видеокарты. </returns>
            public static int LoadTexture(string path) {
                Bitmap bitmap = new Bitmap(path);
                int bitmap_width = bitmap.Width;
                GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
                GL.GenTextures(1, out int tex); GL.BindTexture(TextureTarget.Texture2D, tex);
                BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                //разворачиваем цвета пикселей из формата Format32bppArgb в Rgba
                Parallel.ForEach(Enumerable.Range(0, bitmap.Height), y => {
                    unsafe { byte* p = (byte*)data.Scan0 + y * data.Stride;
                        for (int x = 0; x < bitmap_width; x++) {
                            byte A = p[x * 4 + 3], R = p[x * 4 + 2], G = p[x * 4 + 1], B = p[x * 4];
                            p[x * 4] = R; p[x * 4 + 1] = G; p[x * 4 + 2] = B; p[x * 4 + 3] = A;
                    }}
                });

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, data.Scan0);
                bitmap.UnlockBits(data);
                return tex;
            }

            /// <summary> Метод рисует картинку из памяти видеокарты в окне GameWindow. </summary>
            /// <value> 
            ///     <b><paramref name="numImage"/>:</b> номер изображениея в памяти видеокарты. <br/> 
            ///     <b><paramref name="window"/>:</b> размер окна OpenGL. <br/> 
            /// </value>
            public static void DrawBackGroundImage(int numImage, Size window) {
                GL.MatrixMode(MatrixMode.Projection);
                GL.PushMatrix(); 
                    GL.LoadIdentity(); GL.Ortho(0, window.Width, window.Height, 0, -1, 1);
                    GL.MatrixMode(MatrixMode.Modelview);
                    GL.PushMatrix();
                        GL.LoadIdentity();
                        //GL.Disable(EnableCap.Lighting);
                        GL.Enable(EnableCap.Texture2D);
                        GL.BindTexture(TextureTarget.Texture2D, numImage); 
                        GL.Begin(PrimitiveType.Quads); //натягиваем текстуру на окно
                            GL.TexCoord2(0, 0); GL.Vertex2(0, 0);
                            GL.TexCoord2(1, 0); GL.Vertex2(window.Width, 0);
                            GL.TexCoord2(1, 1); GL.Vertex2(window.Width, window.Height);
                            GL.TexCoord2(0, 1); GL.Vertex2(0, window.Height);
                        GL.End();
                        GL.Disable(EnableCap.Texture2D); 
                    GL.PopMatrix();
                    GL.MatrixMode(MatrixMode.Projection);
                GL.PopMatrix();
                GL.MatrixMode(MatrixMode.Modelview);
            }
        }

        /// <summary> Ссылка на панельку с настройками. Нужно чтобы пробросить её в отдельное окно WinForms поверх окна OpenGL - GameWindow. </summary>
        public System.Windows.Forms.Panel pnl_Settings;

        /// <summary> <b>[0] ИНИЦИАЛИЗАЦИЯ OpenGL</b> (разовый вызов перед главной петлёй) <br/>
        ///     Метод устанавливает цвет фона и настраивает матрицу проекции OpenGL. </summary>
        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            bgColor = new Color4(255/255f, 255/255f, 255/255f, 255/255f);
            LineCouplingColor = new Color4(1f, 1f, 1f, 0.4706f);//(255, 255, 255, 120);

            GL.ClearColor(bgColor);
            //GL.Enable(EnableCap.DepthTest);//проверка разрешения фигур (впереди стоящая закрывает фигуру за ней) - 3D
            //GL.DepthFunc(DepthFunction.Equal);//тип проверки глубины - 3D
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(0, BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            //GL.Enable(EnableCap.CullFace);
            //GL.CullFace(CullFaceMode.Front);

            meshVbo = new GL_DRAW_2D.VBO();
            meshVbo.SetData(new[] {
                new GL_DRAW_2D.VBO.Vertex(-0.5f, -0.5f, new Color4(1f, 0f, 0f, 1f)),
                new GL_DRAW_2D.VBO.Vertex(0.5f, -0.5f, new Color4(0f, 1f, 0f, 1f)),
                new GL_DRAW_2D.VBO.Vertex(0.0f, 0.5f, new Color4(0f, 0f, 1f, 1f))
            });
            meshVao = new GL_DRAW_2D.VAO(3); // 3 вершины
            meshVao.AttachVBO(0, meshVbo, 2, VertexAttribPointerType.Float, 6 * sizeof(float), 0); // Нулевой атрибут вершины – позиция, у неё 2 компонента типа float
            meshVao.AttachVBO(1, meshVbo, 4, VertexAttribPointerType.Float, 6 * sizeof(float), 2 * sizeof(float)); // Первый атрибут вершины – цвет, у него 3 компонента типа float

            shaderProgram = new GL_DRAW_2D.ShaderProgram();
            using (var vertexShader = new GL_DRAW_2D.Shader(ShaderType.VertexShader))
            using (var fragmentShader = new GL_DRAW_2D.Shader(ShaderType.FragmentShader)) {
                vertexShader.Compile(@"#version 330 core
                    layout (location = 0) in vec2 aPosition;
                    layout (location = 1) in vec3 aColor;
                    out vec4 vertexColor;
                    void main() {
                        gl_Position = vec4(aPosition, 0.0, 1.0);//aPosition = (X, Y); gl_Position = X, Y, Z, Depth;
                        vertexColor = vec4(aColor, 1.);
                    }
                ");
                fragmentShader.Compile(@"#version 330 core
                    out vec4 FragColor; in vec4 vertexColor;
                    void main() { FragColor = vertexColor; }
                ");

                shaderProgram.AttachShader(vertexShader);
                shaderProgram.AttachShader(fragmentShader);
                shaderProgram.Link();
            }


            // Создаем WinForms форму для панельки с настройками
            float FSize = 10;//размер шрифта текста
            string FName = "Arial";//имя шрифта текста
            var form = new System.Windows.Forms.Form {
                Font = new Font(FName, FSize, FontStyle.Bold), //Padding = new System.Windows.Forms.Padding(0, 0, 0, 10),
                StartPosition = System.Windows.Forms.FormStartPosition.CenterParent, 
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle,
                ControlBox = true, KeyPreview = true, //AutoSize = true,
                Width = pnl_Settings.Width + 20, Height = pnl_Settings.Height + 40,// Top = 50,
            };
            pnl_Settings.Parent = form; pnl_Settings.Left = pnl_Settings.Top = 0;
            form.Show();// Отображаем форму как дочернее окно GameWindow
            form.Text = $"Окно интерфейса / Размер окна: {form.Width}x{form.Height}";

            //ВКЛ/ВЫКЛ вертикальную синхронизацию
            VSync = VSyncMode.Off;//отключение даёт макс FPS и повышенную нагрузку на GPU/CPU
            //создание массива фоновых картинок для симуляции
            bg_textureArray = BackGroundTextures.CreateTexturesFromFiles(Directory.GetFiles("RESOURCES"));

            //инициализация данных отрисовки и массивов вершин
            //circle_vertices2D = new double[2 * Circle_NumSegments * Link_SIMULATION.Particles.Count];//2 = XY
            //circle_colors4 = new float[4 * Circle_NumSegments * Link_SIMULATION.Particles.Count];// 4 = RGBA
            //line_vertices2D = new double
            //    [4 * Link_SIMULATION.Particles[0].LinkPart.Length * Link_SIMULATION.Particles.Count];//4 = x1y1 - x2y2
            //line_colors4 = new float
            //    [8 * Link_SIMULATION.Particles[0].LinkPart.Length * Link_SIMULATION.Particles.Count];// 8 = RGBA для каждой вершины XY
        }


        /// <summary> <b>[1] ЦИКЛИЧЕСКАЯ ПРОВЕРКА ИЗМЕНЕНИЯ РАЗМЕРА ОКНА OpenGL</b> <br/> Метод обрабатывает изменение размера окна. </summary>
        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            //код для 2D сцены
            GL.Viewport(ClientRectangle);
                //ортогональная проекция (объекты с удалением по оси Z не уменьшаются в размерах)
                //точка [0, 0] слева вверху как у окон windows
                Matrix4 ortho = Matrix4.CreateOrthographicOffCenter(0, ClientRectangle.Width, ClientRectangle.Height, 0, -1, 1);
                GL.MatrixMode(MatrixMode.Projection); //GL.PushMatrix(); 
                GL.LoadMatrix(ref ortho);
  //              GL.LoadIdentity();
                //перспективная проекция (объекты с удалением по оси Z уменьшаются в размерах)
                //GL.Frustum(0, 1, 1, 0, 0.1, Link_SIMULATION.Size.Depth);
        }

        int Count_fps = 0; int Summ_fps = 0;
        public int Count_Thread_Timer_Backend;
        string s0 = "", s1 = "", s2 = "", s3 = "", s4 = "";
        /// <summary> <b>[2] ЦИКЛИЧЕСКОЕ ОБНОВЛЕНИЕ ЛОГИКИ КАДРА ОКНА OpenGL</b> <br/> Метод в фоновом цикле обрабатывает обновление логики игры. </summary>
        protected override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);
            if (Link_SIMULATION == null || Link_SIMULATION.Particles == null || Link_SIMULATION.Particles.Count <= 0) { return; }

            if (circle_vertices2D == null || circle_colors4 == null || line_vertices2D == null || line_colors4 == null) { 
                //инициализация данных отрисовки и массивов вершин
                circle_vertices2D = new double[2 * Circle_NumSegments * Link_SIMULATION.Particles.Count];//2 = XY
                circle_colors4 = new float[4 * Circle_NumSegments * Link_SIMULATION.Particles.Count];// 4 = RGBA
                line_vertices2D = new double
                    [4 * Link_SIMULATION.Particles[0].LinkPart.Length * Link_SIMULATION.Particles.Count];//4 = x1y1 - x2y2
                line_colors4 = new float
                    [8 * Link_SIMULATION.Particles[0].LinkPart.Length * Link_SIMULATION.Particles.Count];// 8 = RGBA для каждой вершины XY
            }

            //переаллоцирование памяти для массивов вершин кругов и линий, если кол-во частиц изменилось
            if (circle_vertices2D.Length / (Circle_NumSegments * 2) != Link_SIMULATION.Particles.Count) {
                circle_vertices2D = new double[2 * Circle_NumSegments * Link_SIMULATION.Particles.Count];//2 = XY
                circle_colors4 = new float[4 * Circle_NumSegments * Link_SIMULATION.Particles.Count];// 4 = RGBA
            }
            if (line_vertices2D.Length / (4 * Link_SIMULATION.Particles[0].LinkPart.Length) != Link_SIMULATION.Particles.Count) {
                line_vertices2D = new double [4 * Link_SIMULATION.Particles[0].LinkPart.Length * Link_SIMULATION.Particles.Count];//4 = x1y1 - x2y2
                line_colors4 = new float[8 * Link_SIMULATION.Particles[0].LinkPart.Length * Link_SIMULATION.Particles.Count];// 8 = RGBA для каждой вершины XY
            }

            Summ_fps += (int)FPS.ShowFPS(); Count_fps++;
            if (Count_fps >= 5000) { s1 = $"Средний fps на 5000 кадров: [{Summ_fps / Count_fps}]"; Count_fps = Summ_fps = 0; }
            //расчёт FPS средствами OpenGL
            frameTime += e.Time; fps++;
            if (frameTime >= 1) {
                s0 = $"FPS: {FPS.ShowFPS()} | GL.FPS: {fps}";
                s2 = $"Частиц: {Link_SIMULATION.Particles.Count}";
                double impuls = 0; foreach (var part in Link_SIMULATION.Particles) { impuls += part.GetSpeed(part.Velocity2D) * part.Mass; }
                s3 = $"Сумма импульсов: {impuls:0.##}";
                s4 = $"Потоков: {Count_Thread_Timer_Backend}";
                Title = $"{s0} | {s1} | {s2} | {s3} | {s4} | Window: {ClientSize.Width}x{ClientSize.Height} | SIMULATION: {Link_SIMULATION.Size.Width}x{Link_SIMULATION.Size.Height}";
                frameTime = fps = 0;
            }

            if (Keyboard.GetState().IsKeyDown(Key.Escape)) {
                Exit();
            }
        }

        /// <summary> <b>[3] ЦИКЛИЧЕСКОЕ ОБНОВЛЕНИЕ ОТРИСОВКИ КАДРА ОКНА OpenGL</b> <br/> Метод строит кадр изображения в окне OpenGL. </summary>
        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);
            GL.ClearColor(bgColor);
            //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);//3D
            GL.Clear(ClearBufferMask.ColorBufferBit);//2D
            BackGroundTextures.DrawBackGroundImage(textureID, ClientSize);//Рисование фоновой текстуры
            if (Link_SIMULATION == null || Link_SIMULATION.Particles == null || Link_SIMULATION.Particles.Count <= 0) { SwapBuffers(); return; }

            var parts = Link_SIMULATION.Particles; var Grid = Link_SIMULATION.Grid;
            bool DrawGrid = false;//true = нарисовать оконную сетку

            shaderProgram.Use();
            meshVao.Draw(PrimitiveType.Triangles);


            switch (MODE) {
                case DrawMode.Default: //отрисовка симуляции по умолчанию с помощью множества вызовов DrawCircle
                    //скорость отрисовки: FPS 250
                    //for (var i = 0; i < parts.Count; i++) {
                    //    //parts[i].FillColor4.A = (float)parts[i].GetSpeed() + 0.15f;
                    //    if (parts[i].Radius >= 2) {
                    //    //    GL_DRAW.BeginEnd.Circle(parts[i].Position3D.X, parts[i].Position3D.Y, -parts[i].Position3D.Z, parts[i].Radius, Circle_NumSegments, parts[i].FillColor4, PrimitiveType.TriangleFan);
                    //        //GL_DRAW.BeginEnd.Circle(parts[i].Position3D.X, parts[i].Position3D.Y, -parts[i].Position3D.Z, parts[i].Radius, Circle_NumSegments, parts[i].FillColor4, PrimitiveType.LineLoop);
                    //        GL_DRAW_2D.DrawArrays.Circle(parts[i].Position3D.X, parts[i].Position3D.Y, parts[i].Radius, Circle_NumSegments, parts[i].FillColor4, PrimitiveType.TriangleFan);
                    //        GL_DRAW_2D.DrawArrays.Circle(parts[i].Position3D.X, parts[i].Position3D.Y, parts[i].Radius, Circle_NumSegments, parts[i].OutlineColor4, PrimitiveType.LineLoop);
                    //    }
                    //    else GL_DRAW_2D.BeginEnd.DrawPoint(parts[i].Position3D.X, parts[i].Position3D.Y, parts[i].FillColor4, parts[i].Radius);
                    //    if (parts[i].LinkPart != null) GL_DRAW_2D.DrawLineCoupling(parts[i], LineCouplingColor);
                    //}

                    //отрисовка всех кругов разом
                    //for (var i = 0; i < parts.Count*0+1; i++)
                    //    GL_DRAW_2D.VBO.Circle(parts[i].Position2D.X, parts[i].Position2D.Y, parts[i].Radius, Circle_NumSegments, parts[i].FillColor4, PrimitiveType.TriangleFan);
               //     GL_DRAW_2D.DrawArrays.AllCircles(parts, Circle_NumSegments, circle_vertices2D, circle_colors4, PrimitiveType.TriangleFan, AlphaColorCircle);
                    //отрисовка всех контуров разом
               //     if (FlagContourCircle) GL_DRAW_2D.DrawArrays.AllCircles(parts, Circle_NumSegments, circle_vertices2D, circle_colors4, PrimitiveType.LineLoop, AlphaColorCircle, ThicknessContour);
                    //отрисовка всех линий связи разом - fps 420
                    GL_DRAW_2D.DrawArrays.AllLines(parts, line_vertices2D, line_colors4, LineCouplingColor);
                    //отрисовка линий связи в цикле поштучно - fps 350
                    //for (var i = 0; i < parts.Count; i++) if (parts[i].LinkPart != null) GL_DRAW_2D.DrawLineCoupling(parts[i], LineCouplingColor);
                    break;

                case DrawMode.Grid: //отрисовка симуляции в режиме таблицы Grid
                                    //каждая частица берётся из листа индексов каждой ячейки таблицы Grid[x, y][index]
                    if (Grid == null) return;
                    var cx = Grid.GetLength(0); var cy = Grid.GetLength(1);
                    var K = 360.0 / (cx + ((cy - 1) * cy));
                    //for (var i = 0; i < parts.Count; i++)
                    for (var y = 0; y < cx; y++) for (var x = 0; x < cy; x++) for (var i = 0; i < Grid[x, y].Count; i++) {
                        if (i >= Grid[x, y].Count) break; PARTICLE p = Grid[x, y][i];
                        HSV h = new HSV(0, 1, 1) {
                            Hue = (p.Grid.Item1.X + p.Grid.Item1.Y * cy) * K, Saturation = 1, Value = 1.0
                        }; 
                        Color tmp = h.HSVToColor();
                        Color4 cl = new Color4(tmp.R, tmp.G, tmp.B, tmp.A);
                        GL_DRAW_2D.DrawArrays.Circle(p.Position2D.X, p.Position2D.Y, 6, Circle_NumSegments, cl, PrimitiveType.TriangleFan);
                        //GL_DRAW_2D.BeginEnd.DrawCircle(p.Position2D.X, p.Position2D.Y, 6, Circle_NumSegments, cl, PrimitiveType.TriangleFan);
                    }
                    DrawGrid = true;//сетка
                    break;

                case DrawMode.TestGrid: //отрисовка симуляции в режиме таблицы Grid
                                        //каждая частица берётся из листа индексов каждой ячейки таблицы Grid[x, y][index]
                    if (Grid == null) return;
                    var CX = Grid.GetLength(0); var CY = Grid.GetLength(1);
                    for (var y = 0; y < CY; y++) for (var x = 0; x < CX; x++) {
                        //средний фоновый цвет перекрытия в ячейке
                        float R = 0; float G = 0; float B = 0;
                        for (var i = 0; i < Grid[x, y].Count; i++) { R += Grid[x, y][i].FillColor4.R; G += Grid[x, y][i].FillColor4.G; B += Grid[x, y][i].FillColor4.B;  }
                        Color4 bgGrid = new Color4(R / Grid[x, y].Count, G / Grid[x, y].Count, B / Grid[x, y].Count, 0.45f);

                        for (var i = 0; i < Grid[x, y].Count; i++) { 
                            if (i >= Grid[x, y].Count) break; PARTICLE p = Grid[x, y][i];
                            Size sz = Link_Draw.GridPixels;
                            //закраска фона. красим все ячейки SIMULATION.Grid которые перекрывает круг
                            int xx1 = x * sz.Width; int yy1 = y * sz.Height;//левый верх
                            int xx2 = xx1 + sz.Width; int yy2 = yy1;//правый верх
                            int xx3 = xx2; int yy3 = yy1 + sz.Height;//правый низ
                            int xx4 = xx1; int yy4 = yy3;//левый низ
                            GL_DRAW_2D.BeginEnd.Quads(xx1, yy1, xx2, yy2, xx3, yy3, xx4, yy4, 1, bgGrid);

                            //закраска фона. красим все ячейки Particles[i].Grid которые перекрывает круг В ДВИЖЕНИИ!
                            for (int n = 0; n < p.Grid.Item3.Count; n++) {
                                int cellX = p.Grid.Item3[n].X; int cellY = p.Grid.Item3[n].Y;
                                int cx1 = cellX * sz.Width; int cy1 = cellY * sz.Height;//левый верх
                                int cx2 = cx1 + sz.Width; int cy2 = cy1;//правый верх
                                int cx3 = cx2; int cy3 = cy1 + sz.Height;//правый низ
                                int cx4 = cx1; int cy4 = cy3;//левый низ
                                GL_DRAW_2D.DrawArrays.Line(cx1 + 2, cy1 + 2, cx2 - 2, cy2 + 2, 2, p.FillColor4);
                                GL_DRAW_2D.DrawArrays.Line(cx2 - 2, cy2 + 2, cx3 - 2, cy3 - 2, 2, p.FillColor4);
                                GL_DRAW_2D.DrawArrays.Line(cx3 - 2, cy3 - 2, cx4 + 2, cy4 - 2, 2, p.FillColor4);
                                GL_DRAW_2D.DrawArrays.Line(cx4 + 2, cy4 - 2, cx1 + 2, cy1 + 2, 2, p.FillColor4);
                            }

                            GL_DRAW_2D.BeginEnd.Circle(p.Position2D.X, p.Position2D.Y, p.Radius, Circle_NumSegments, p.FillColor4, PrimitiveType.TriangleFan);

                            //отображаем вектор скорости
                            Vector2Df start = p.Position2D; Vector2Df end = p.Position2D + p.Velocity2D;
                            GL_DRAW_2D.DrawArrays.Line(start.X, start.Y, end.X, end.Y, 2, new Color4(255, 255, 255, 200));
                        }
                    }
                    DrawGrid = true;//сетка
                    break;

                case DrawMode.OneCellOneCircle: //в каждой ячейке Grid хранится 1 усреднённый круг по радиусу и координатам
                    if (Grid == null) return;
                    var _CX = Grid.GetLength(0); var _CY = Grid.GetLength(1);
                    for (var y = 0; y < _CY; y++) for (var x = 0; x < _CX; x++) {
                        var count = Grid[x, y].Count;
                        if (count > 0) { 
                            double _x = 0, _y = 0; float R = 0, G = 0, B = 0;
                            for (var i = 0; i < count; i++) {
                                try { PARTICLE p = Grid[x, y][i]; 
                                    _x += p.Position2D.X; _y += p.Position2D.Y;
                                    R += p.FillColor4.R; G += p.FillColor4.G; B += p.FillColor4.B;
                                } catch { break; }
                            }
                            _x /= count; _y /= count; R /= count; G /= count; B /= count;
                            if (count >= 2) GL_DRAW_2D.BeginEnd.Circle(_x, _y, count, Circle_NumSegments, new Color4(R, G, B, 1f), PrimitiveType.TriangleFan);
                            else GL_DRAW_2D.BeginEnd.Point(_x, _y, new Color4(R, G, B, 1f), count);
                        }
                    }
                    DrawGrid = true;//сетка
                    break;
            }

            if (DrawGrid) { //сетка
                Color4 Cl = new Color4(255, 255, 255, 50); Size sz = Link_Draw.GridPixels;
                for (var y = 0; y < Grid.GetLength(1); y++) 
                    GL_DRAW_2D.BeginEnd.Line(0, y * sz.Height, Size.Width, y * sz.Height, 1, Cl);
                for (var x = 0; x < Grid.GetLength(0); x++) 
                    GL_DRAW_2D.BeginEnd.Line(x * sz.Width, 0, x * sz.Width, Size.Height, 1, Cl);
            }

            GL.Color4(bgColor);//задаём фоновый цвет, чтобы не искажались цвета фоновой текстуры при наложении геометрии
            //GL.PopMatrix();

            FPS.AddFrame();
            //GL.Flush(); // Гарантирует выполнение всех команд OpenGL перед отображением изображения
            SwapBuffers();
        }

        /// <summary> <b>[4] МЕТОД УДАЛЕНИЯ ЗАГРУЖЕННЫХ РЕСУРСОВ</b> (разовый вызов если IsRun = false) <br/> Метод удаляет все загруженные ресурсы из памяти на этапе инициализации (загрузки) в методе OnLoad(). </summary>
        protected override void OnUnload(EventArgs e) {
            base.OnUnload(e);
        }

        /// <summary> Метод принимает входные координаты (и их глубину в пространстве) в клиентской области окна OpenGL <br/> и преобразует их в 3D координаты OpenGL с учетом видового экрана. </summary>
        /// <value> 
        ///     <b><paramref name="clientX"/>:</b> горизонтальная координата в клиентской области окна OpenGL по оси <b>X</b>, отсчитываемая от левого края окна. <br/> 
        ///     <b><paramref name="clientY"/>:</b> вертикальная координата в клиентской области окна OpenGL по оси <b>Y</b>, отсчитываемая от верхнего края окна. <br/> 
        ///     <b><paramref name="z"/>:</b> глубина точки в трехмерном пространстве OpenGL, меньшие значения z означают, что точка находится ближе к наблюдателю. <br/> 
        ///     <b><paramref name="viewportWidth"/>:</b> ширина видового экрана (viewport) в контексте OpenGL в пикселях. <br/> 
        ///     <b><paramref name="viewportHeight"/>:</b> высота видового экрана (viewport) в контексте OpenGL в пикселях. <br/> 
        /// </value>
        /// <remarks> Видовой экран - это область в окне, в которой отображается графика OpenGL. </remarks>
        /// <returns> Возвращает 3D координаты OpenGL с учетом видового экрана. </returns>
        //public Vector3 ClientToOpenGL(int clientX, int clientY, float z, int viewportWidth, int viewportHeight) {
        //    // Получаем текущую матрицу проекции и модельно-видовой матрицы
        //    Matrix4 projectionMatrix = _ProjectionMatrix; // Замените на актуальную матрицу проекции
        //    Matrix4 modelViewMatrix = _ModelviewMatrix; // Замените на актуальную модельно-видовую матрицу
        //    // Инвертируем матрицу проекции и модельно-видовую матрицу
        //    Matrix4.Invert(ref projectionMatrix, out Matrix4 invertedProjectionMatrix);
        //    Matrix4.Invert(ref modelViewMatrix, out Matrix4 invertedModelViewMatrix);
        //    // Создаем вектор с экранными координатами (x, y, z)
        //    Vector3 screenCoord = new Vector3(clientX, viewportHeight - clientY, z);
        //    // Преобразуем экранные координаты в мировые координаты
        //    Vector4 worldCoord = new Vector4(screenCoord.X / viewportWidth * 2 - 1, (screenCoord.Y / viewportHeight * 2 - 1) * -1, screenCoord.Z * 2 - 1, 1);
        //    // Умножаем координаты на обратные матрицы
        //    worldCoord = Vector4.Transform(worldCoord, invertedProjectionMatrix * invertedModelViewMatrix);
        //    worldCoord /= worldCoord.W;// Нормализуем координаты
        //    return new Vector3(worldCoord.X, worldCoord.Y, worldCoord.Z);
        //    //return Vector3.Zero; // В случае ошибки
        //}
    }

    //=======================================================================================================================
    //--------------------------------------------- ВЫВОД ГРАФОНИЯ В ОКНЕ OpenGL --------------------------------------------
    //=======================================================================================================================
    /// <summary> Список всех существующих методов класса <b>GL_DRAW</b>. <br/> Служит для удобства восприятия: что уже реализовано, а что нет. </summary>
    public interface IOpenGLDraw {
        ///<summary> <inheritdoc cref="GL_DRAW_2D.DrawLineCoupling"/> </summary>
        void DrawLineCoupling(PARTICLE Part, Color4 LineCouplingColor);
    }
    /// <summary> Список всех существующих методов класса <b>GL_DRAW.BeginEnd</b>. <br/> Служит для удобства восприятия: что уже реализовано, а что нет. </summary>
    public interface IOpenGLDraw_BeginEnd {
        ///<summary> <inheritdoc cref="GL_DRAW_2D.BeginEnd.Point"/> </summary>
        void Point(double x, double y, Color4 color, float th);
        ///<summary> <inheritdoc cref="GL_DRAW_2D.BeginEnd.Line_Quads"/> </summary>
        void Line_Quads(double x, double y, double x2, double y2, float th, Color4 color);
        ///<summary> <inheritdoc cref="GL_DRAW_2D.BeginEnd.Line"/> </summary>
        void Line(double x, double y, double x2, double y2, float th, Color4 fill_color);
        ///<summary> <inheritdoc cref="GL_DRAW_2D.BeginEnd.Circle"/> </summary>
        void Circle(double cx, double cy, double radius, int Circle_NumSegments, Color4 color, PrimitiveType pType, float th = 1);
    }
    /// <summary> Список всех существующих методов класса <b>GL_DRAW.DrawArrays</b>. <br/> Служит для удобства восприятия: что уже реализовано, а что нет. </summary>
    public interface IOpenGLDraw_DrawArrays {
        ///<summary> <inheritdoc cref="GL_DRAW_2D.DrawArrays.Line"/> </summary>
        void Line(double x1, double y1, double x2, double y2, float th, Color4 color);
        ///<summary> <inheritdoc cref="GL_DRAW_2D.DrawArrays.AllLines"/> </summary>
        void AllLines(List<PARTICLE> parts, double[] line_vertices2D, float[] line_colors4, Color4 LineCouplingColor);
        ///<summary> <inheritdoc cref="GL_DRAW_2D.DrawArrays.Circle"/> </summary>
        void Circle(double cx, double cy, double radius, int Circle_NumSegments, Color4 color, PrimitiveType pType, float th = 1);
        ///<summary> <inheritdoc cref="GL_DRAW_2D.DrawArrays.AllCircles"/> </summary>
        void AllCircles(List<PARTICLE> parts, int Circle_NumSegments, double[] circle_vertices2D, float[] circle_colors4, PrimitiveType pType, float th = 1);
    }

    /// <summary> Статический класс. Реализует все методы, рисующие графику в памяти видеокарты. </summary>
    public class GL_DRAW_2D {
        /// <summary> Статический класс. Реализует методы, рисующие графику в память видеокарты с помощью <b>Begin()/End()</b>. </summary>
        public static class BeginEnd {
            /// <summary> Метод рисует точку в указанных координатах с заданным цветом <b>color</b> и толщиной <b>th</b>, методом <b>Begin()/End()</b>. </summary>
            public static void Point(double x, double y, Color4 color, float th) {
                GL.PointSize(th); GL.Color4(color); 
                GL.Begin(PrimitiveType.Points); 
                    GL.Vertex2(x, y);
                GL.End(); //GL.PointSize(1);
            }
            /// <summary> Метод рисует линию от сих до сих с заданной толщиной <b>th</b> и цветом <b>color</b>, методом <b>Begin()/End()</b>. </summary>
            /// <remarks> На удивление <b>DrawLine_BeginEnd()</b> на 50 fps быстрее, чем <b>DrawLine_DrawArrays()</b>. </remarks>
            public static void Line(double x, double y, double x2, double y2, float th, Color4 fill_color) {
                GL.LineWidth(th); GL.Color4(fill_color);
                GL.Begin(PrimitiveType.Lines); 
                    GL.Vertex2(x, y); GL.Vertex2(x2, y2);
                GL.End(); //GL.LineWidth(1);
            }
            /// <summary> Метод рисует линию от сих до сих с заданной толщиной <b>th</b> и цветом <b>color</b>, методом <b>Begin()/End()</b>. </summary>
            /// <remarks> Из 2 переданных вершин, вычисляются 4 вершины и отрисовываются как квад соединённых точек. <br/> Такой способ эмулирует <b>ЛИНИЮ</b> заданной толщины. </remarks>
            public static void Line_Quads(double x, double y, double x2, double y2, double th, Color4 color) {
                double dx = x2 - x, dy = y2 - y;//дельты
                double length = Sqrt(dx * dx + dy * dy);//вектор направления
                dx /= length; dy /= length;//нормализация вектора направления
                double nx = -dy * th / 2.0, ny = dx * th / 2.0;//векторы, перпендикулярные к направлению линии для толщины
                //вершины квадрата для отрисовки линии с толщиной
                double X0 = x - nx,   Y0 = y - ny,
                       X1 = x + nx,   Y1 = y + ny,
                       X2 = x2 + nx,  Y2 = y2 + ny,
                       X3 = x2 - nx,  Y3 = y2 - ny;
                GL.Color4(color);
                GL.Begin(PrimitiveType.Quads);
                    //отрисовываем квад как линию с заданной толщиной
                    GL.Vertex2(X0, Y0); GL.Vertex2(X1, Y1);
                    GL.Vertex2(X2, Y2); GL.Vertex2(X3, Y3);
                GL.End();
            }

            /// <summary> Метод рисует квад из 4 вершин с заданной толщиной <b>th</b> и цветом <b>color</b>, методом <b>Begin()/End()</b>. </summary>
            /// <remarks> Четыре вершины должны идти строго по периметру по часовой или против часовой стрелке, не пересекая центр квада диагональной линией! <br/>
            /// Соблюдая эти простые правила, вы получите замкнутые линии в <b>квад</b>, который в зависимости от расположения точек, может быть: <br/>
            /// ромбом, параллелограмом, квадратом, прямоугольником и т.д. </remarks>
            public static void Quads(double x1, double y1, double x2, double y2, 
                                         double x3, double y3, double x4, double y4, float th, Color4 color)
            {
                GL.LineWidth(th); GL.Color4(color);
                GL.Begin(PrimitiveType.Quads);
                    //отрисовываем квад как линию с заданной толщиной
                    GL.Vertex2(x1, y1); GL.Vertex2(x2, y2);
                    GL.Vertex2(x3, y3); GL.Vertex2(x4, y4);
                GL.End();
                //GL.LineWidth(1);
            }

            /// <summary> Метод отрисовывает 2D круг, рассчитывая ко-ры точек для построения граней вокруг центра окружности. <br/>
            ///     <b>cx,cy</b> - центр окружности; <br/> <b>radius</b> - радиус; <br/> <b>Circle_NumSegments</b> - кол-во рёбер круга (3 - треугольник, 4 - квад, 6 гексагон (пчелиные соты), 360 гладкий круг); <br/> <b>color</b> - цвет фона или контура круга; <br/> <b>pType</b> - тип примитива (TriangleFan/Polygon - закрашенный круг, LineLoop - контур круга); <br/> <b>th</b> - толщина контура круга. <br/>
            ///     1. метод рисования: Begin()/End(); <br/> 2. <b>400 FPS</b> на 1000 кругов (поштучная отрисовка). </summary>
            public static void Circle(double cx, double cy, double radius, int Circle_NumSegments, Color4 color,
                PrimitiveType pType, float th = 1)
            {
                if (pType == PrimitiveType.LineLoop || pType == PrimitiveType.Lines) GL.LineWidth(th);
                GL.Color4(color);//цвет круга
                GL.Begin(pType);
                    double angle;
                    for (int i = 0; i < Circle_NumSegments; i++) {
                        angle = 2 * PI * i / Circle_NumSegments;
                        GL.Vertex2(cx + radius * Cos(angle), cy + radius * Sin(angle));
                    }
                GL.End();
                //if (pType == PrimitiveType.LineLoop || pType == PrimitiveType.Lines) GL.LineWidth(1);
            }
        }

        /// <summary> Статический класс. Реализует методы, рисующие графику в память видеокарты с помощью <b>DrawArrays()</b>. </summary>
        public static class DrawArrays {
            /// <summary> Метод рисует линию от сих до сих с заданной толщиной <b>th</b> и цветом <b>color</b>, методом <b>DrawArrays</b>. </summary>
            /// <remarks> На удивление <b>DrawLine_DrawArrays()</b> на 50 fps медленнее, чем <b>DrawLine_BeginEnd()</b>. </remarks>
            public static void Line(double x1, double y1, double x2, double y2, float th, Color4 color) {
                double[] vertices = new double[] { x1, y1, x2, y2 };
                float[] colors = new float[] { color.R, color.G, color.B, color.A, color.R, color.G, color.B, color.A };
                GL.EnableClientState(ArrayCap.VertexArray); GL.EnableClientState(ArrayCap.ColorArray);
                GL.VertexPointer(2, VertexPointerType.Double, 0, vertices);
                GL.ColorPointer(4, ColorPointerType.Float, 0, colors);
                GL.LineWidth(th);
                GL.DrawArrays(PrimitiveType.Lines, 0, 2);
                GL.DisableClientState(ArrayCap.VertexArray); GL.DisableClientState(ArrayCap.ColorArray);
                //GL.LineWidth(1);
            }

            /// <summary> Метод отрисовывает 1 круг, рассчитывая ко-ры точек для построения граней вокруг центра окружности. 
            ///     <br/> <b>cx,cy</b> - центр окружности; <br/> <b>radius</b> - радиус; <br/> <b>Circle_NumSegments</b> - кол-во рёбер круга (3 - треугольник, 4 - квад, 6 гексагон (пчелиные соты), 360 гладкий круг); <br/> <b>color</b> - цвет фона или контура круга; <br/> <b>pType</b> - тип примитива (TriangleFan/Polygon - закрашенный круг, LineLoop - контур круга); <br/> <b>th</b> - толщина контура круга. <br/>
            ///     1. метод рисования: массив вершин XYZ, массив цветов RGBA, метод DrawArrays(); <br/> 2. <b>550 FPS</b> на 1000 кругов (поштучная отрисовка). </summary>
            public static void Circle(double cx, double cy, double radius, int Circle_NumSegments, Color4 color,
                                          PrimitiveType pType, float th = 1)
            {
                double[] vertices = new double[Circle_NumSegments * 2];
                float[] colors = new float[Circle_NumSegments * 4];
                double angle;
                for (int i = 0; i < Circle_NumSegments; i++) {
                    angle = 2 * PI * i / Circle_NumSegments;
                        vertices[i * 2] = (cx + radius * Cos(angle));     //X
                        vertices[i * 2 + 1] = (cy + radius * Sin(angle)); //Y
                    colors[i * 4] = color.R; colors[i * 4 + 1] = color.G;
                    colors[i * 4 + 2] = color.B; colors[i * 4 + 3] = color.A;
                }
                if (pType == PrimitiveType.LineLoop || pType == PrimitiveType.Lines) GL.LineWidth(th);
                GL.EnableClientState(ArrayCap.VertexArray); GL.EnableClientState(ArrayCap.ColorArray);
                GL.VertexPointer(2, VertexPointerType.Double, 0, vertices);
                GL.ColorPointer(4, ColorPointerType.Float, 0, colors);
                GL.DrawArrays(pType, 0, Circle_NumSegments);
                GL.DisableClientState(ArrayCap.VertexArray); GL.DisableClientState(ArrayCap.ColorArray);
                //if (pType == PrimitiveType.LineLoop || pType == PrimitiveType.Lines) GL.LineWidth(1);
            }

            /// <summary> Метод отрисовывает весь список линий связи, всех частиц. <br/>
            ///     <b>parts</b> - лист частиц. <br/> <b>line_vertices2D</b> - массив вершин XY для линий. <br/> <b>line_colors4</b> - массив цветов для вершин линий. <br/> <b>LineCouplingColor</b> - цвет линии связи. <br/> Метод рисования: массив вершин XY, массив цветов RGBA, метод DrawArrays(). </summary>
            public static void AllLines(List<PARTICLE> parts, double[] line_vertices2D, float[] line_colors4, Color4 LineCouplingColor) {
                GL.EnableClientState(ArrayCap.VertexArray); GL.EnableClientState(ArrayCap.ColorArray);
                GL.VertexPointer(2, VertexPointerType.Double, 0, line_vertices2D);
                GL.ColorPointer(4, ColorPointerType.Float, 0, line_colors4);
                double x, y; int counter = 0;
                for (int it = 0; it < parts.Count; it++) {
                    x = parts[it]?.Position2D.X ?? 0; y = parts[it]?.Position2D.Y ?? 0;
                    for (int i = 0; i < parts[it].LinkPart.Length; i++) {
                        //линия связи
                        if (parts[it]?.LinkPart[i] != null) { 
                            var x2 = parts[it]?.LinkPart[i]?.Position2D.X ?? x; 
                            var y2 = parts[it]?.LinkPart[i]?.Position2D.Y ?? y;
                            var dist = Physics2D.Distance(parts[it]?.Position2D ?? new Vector2Df(), parts[it]?.LinkPart[i]?.Position2D ?? parts[it]?.Position2D ?? new Vector2Df());
                            if (dist > 0) {
                                LineCouplingColor.A = (255 - (int)(dist * 4.0)) / 255f;
                                if (LineCouplingColor.A <= 0) LineCouplingColor.A = 0.01f;
                                int j = counter * 2 * 2; int j2 = counter * 2 * 4;
                                //x1y1
                                line_vertices2D[j] = x; 
                                line_vertices2D[j + 1] = y;
                                //x2y2
                                line_vertices2D[j + 2] = x2;
                                line_vertices2D[j + 3] = y2; 
                                //color x1y1
                                line_colors4[j2] = LineCouplingColor.R;
                                line_colors4[j2 + 1] = LineCouplingColor.G;
                                line_colors4[j2 + 2] = LineCouplingColor.B;
                                line_colors4[j2 + 3] = LineCouplingColor.A;
                                //color x2y2
                                line_colors4[j2 + 4] = LineCouplingColor.R;
                                line_colors4[j2 + 5] = LineCouplingColor.G;
                                line_colors4[j2 + 6] = LineCouplingColor.B;
                                line_colors4[j2 + 7] = LineCouplingColor.A;

                                var th = (float)(parts[it].ValueCoupling / dist * 1.5);//толщина линии в 2D

                                GL.LineWidth(th);
                                GL.DrawArrays(PrimitiveType.Lines, counter * 2, 2);
                                counter++;
                            }
                        }
                    }
                }
                GL.DisableClientState(ArrayCap.VertexArray); GL.DisableClientState(ArrayCap.ColorArray);
                //GL.LineWidth(1);
            }

            /// <summary> Метод отрисовывает весь список кругов, рассчитывая ко-ры точек для построения граней вокруг центра окружности. <br/>
            ///     <b>parts</b> - лист частиц. <br/> <b>Circle_NumSegments</b> - кол-во рёбер круга (3 - треугольник, 4 - квад, 6 гексагон (пчелиные соты), 360 гладкий круг); <br/> <b>circle_vertices3D</b> - массив вершин для кругов. <br/> <b>circle_colors4</b> - массив цветов для вершин кругов. <br/> <b>pType</b> - тип примитива (TriangleFan/Polygon - закрашенный круг, LineLoop - контур круга); <br/> <b>AlphaColorCircle</b> - true: круг может быть прозрачным в зависимости от скорости, false: круг не может быть прозрачным. <br/> <b>th</b> - толщина контура круга. <br/> 1. метод рисования: массив вершин XYZ, массив цветов RGBA, метод DrawArrays(); <br/> 2. <b>800 FPS</b> на 1000 кругов (отрисовка всех кругов за 1 вызов метода). </summary>
            public static void AllCircles(List<PARTICLE> parts, int Circle_NumSegments, double[] circle_vertices2D, 
                                          float[] circle_colors4, PrimitiveType pType, bool AlphaColorCircle, float th = 1)
            {
                GL.EnableClientState(ArrayCap.VertexArray); GL.EnableClientState(ArrayCap.ColorArray);
                GL.VertexPointer(2, VertexPointerType.Double, 0, circle_vertices2D);
                GL.ColorPointer(4, ColorPointerType.Float, 0, circle_colors4);
                Color4 fc; double angle, cx, cy;
                for (int it = 0; it < parts.Count; it++) {
                    if (pType != PrimitiveType.LineLoop && pType != PrimitiveType.Lines) { 
                        fc = parts[it].FillColor4;//закраска
                        if (AlphaColorCircle) fc.A = (float)parts[it].GetSpeed() + 0.1f;//чем медленнее, тем прозрачнее
                    } else { GL.LineWidth(th); fc = parts[it].OutlineColor4; }//контур
                    cx = parts[it].Position2D.X; cy = parts[it].Position2D.Y; 

                    if (parts[it].Radius > 2) {
                        for (int i = 0; i < Circle_NumSegments; i++) { 
                            angle = 2 * PI * i / Circle_NumSegments;
                            int j = it * Circle_NumSegments * 2 + i * 2;
                            int j2 = it * Circle_NumSegments * 4 + i * 4;
                            circle_vertices2D[j] = cx + parts[it].Radius * Cos(angle);     //X
                            circle_vertices2D[j + 1] = cy + parts[it].Radius * Sin(angle); //Y
                            circle_colors4[j2] = fc.R; circle_colors4[j2 + 1] = fc.G;
                            circle_colors4[j2 + 2] = fc.B; circle_colors4[j2 + 3] = fc.A;//color
                        }
                        GL.DrawArrays(pType, it * Circle_NumSegments, Circle_NumSegments);
                    } else BeginEnd.Point(cx, cy, fc, parts[it].Radius);
                }
                GL.DisableClientState(ArrayCap.VertexArray); GL.DisableClientState(ArrayCap.ColorArray);
            }
        }

        /// <summary> Статический класс. Реализует методы, рисующие графику в память видеокарты с помощью буфера вершин <b>VBO</b>. </summary>
        public static class VBO2 {
            /// <summary> Хранит идентификатор буферного объекта VBO. </summary>
            private static uint vboID;
            /// <summary> Хранит идентификатор буферного объекта VAO. </summary>
            private static uint vaoID;
            /// <summary> Хранит массив вершин. </summary>
            private static vertices[] Vertices;
            private static string vertexShader;
            private static string fragmentShader;
            private static int shader;
            private struct vertices { public float X, Y, R, G, B, A; public const int SizeInBytes = (6 + 1) * sizeof(float); }

            /// <summary> Метод создаёт вершинный буфер VBO во время инициализации (разовый метод, перед циклической отрисовкой геометрии). <br/>
            /// BufferUsageHint.StaticDraw – данные будут очень редко обновляться; <br/>
            /// BufferUsageHint.DynamicDraw – данные будут обновляться, но не каждый кадр; <br/>
            /// BufferUsageHint.StreamDraw – данные будут обновляться каждый кадр; <br/>
            ///</summary>
            private static uint CreateVBO(vertices[] data) {
                //vertexShader = @"#version 330 core
                //                 layout (location = 0) in vec2 aPosition;
                //                 layout (location = 1) in vec4 aColor;
                //                 out vec4 vertexColor;
                //                 void main() {
                //                     vertexColor = aColor;
                //                     gl_Position = vec4(aPosition, 1.0);
                //                 }";

                //fragmentShader = @"#version 330 core
                //                   out vec4 FragColor; in vec4 vertexColor;
                //                   void main() { FragColor = vertexColor; }";
                //int vs = GL.CreateShader(ShaderType.VertexShader); GL.ShaderSource(vs, vertexShader); GL.CompileShader(vs);
                //int fs = GL.CreateShader(ShaderType.FragmentShader); GL.ShaderSource(fs, fragmentShader); GL.CompileShader(fs);
                //shader = GL.CreateProgram(); GL.AttachShader(shader, vs); GL.AttachShader(shader, fs);
                //GL.LinkProgram(shader);

                GL.GenBuffers(1, out uint vbo);//выдать свободный идентификатор в vbo
                //GL.GenVertexArrays(1, out vaoId);//
                //GL.BindVertexArray(vaoId);//
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);//делаем буфер текущим
                //GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(data.Length * sizeof(float)), data, BufferUsageHint.DynamicDraw);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(data.Length * vertices.SizeInBytes), data, BufferUsageHint.DynamicDraw);
                //GL.VertexAttribPointer(0, 6, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);//
                //GL.EnableVertexAttribArray(0);//
                //GL.InterleavedArrays(InterleavedArrayFormat.C4ubV2f, 0, IntPtr.Zero);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);//полная деактивация буфера VBO
                return vbo;
            }
            public static void InitVBO(int Circle_NumSegments) {
                //vertices = new float[(Circle_NumSegments) * 2 + Circle_NumSegments * 4]; // Массив вершин и цветов
                //vertices = new float[Circle_NumSegments * 2]; // Массив вершин
                //colors = new float[Circle_NumSegments * 4]; // Массив цветов
                //verts_colors = new float[Circle_NumSegments * 2 + Circle_NumSegments * 4]; // Массив вершин и цветов
                Vertices = new vertices[Circle_NumSegments]; // Массив вершин
                //vbo_vertex_ID = CreateVBO(vertices);
                //vbo_color_ID = CreateVBO(colors);
                vboID = CreateVBO(Vertices);
            }

            /// <summary> Метод отрисовывает 1 круг, рассчитывая ко-ры точек для построения граней вокруг центра окружности. 
            ///     <br/> <b>cx,cy</b> - центр окружности; <br/> <b>radius</b> - радиус; <br/> <b>Circle_NumSegments</b> - кол-во рёбер круга (3 - треугольник, 4 - квад, 6 гексагон (пчелиные соты), 360 гладкий круг); <br/> <b>color</b> - цвет фона или контура круга; <br/> <b>pType</b> - тип примитива (TriangleFan/Polygon - закрашенный круг, LineLoop - контур круга); <br/> <b>th</b> - толщина контура круга. <br/>
            ///     1. метод рисования: массив вершин XYZ, массив цветов RGBA, метод DrawArrays(); <br/> 2. <b>550 FPS</b> на 1000 кругов (поштучная отрисовка). </summary>
            public static void Circle(double cx, double cy, double radius, int Circle_NumSegments, Color4 color,
                                      PrimitiveType pType, float th = 1)
            {
                double angle; int it = 0;
                for (int i = 0; i < Circle_NumSegments; i++) {
                    angle = 2 * PI * i / Circle_NumSegments;
                    Vertices[it].X = (float)(cx + radius * Cos(angle));//x
                    Vertices[it].Y = (float)(cy + radius * Sin(angle));//y
                    Vertices[it].R = color.R; // Красный цвет для сегментов круга
                    Vertices[it].G = color.G; // Зеленый цвет для сегментов круга
                    Vertices[it].B = color.B; // Синий цвет для сегментов круга
                    Vertices[it].A = color.A; // Альфа-канал для сегментов круга
                    it++;
                }

                GL.EnableClientState(ArrayCap.VertexArray);//включить состояние OpenGL на использование вершинных буферов
                GL.EnableClientState(ArrayCap.ColorArray);//включить состояние OpenGL на использование цветовых буферов

                GL.BindBuffer(BufferTarget.ArrayBuffer, vboID);//подключаем буфер вершин
                    GL.VertexPointer(2, VertexPointerType.Float, vertices.SizeInBytes * 2, IntPtr.Zero);
                    GL.VertexPointer(4, VertexPointerType.Float, vertices.SizeInBytes * 2, vertices.SizeInBytes);
                    //меняем все вершины vertices в массиве и перезаписываем в видеокарту
                    //GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)(sizeof(float) * vertices.Length), sizeof(float) * vertices.Length, vertices);
                //GL.EnableVertexAttribArray(0); // Включаем атрибут вершины
                //GL.VertexAttribPointer(0, 1, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);
                //GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 6 * sizeof(float), IntPtr.Zero);
                //GL.BindBuffer(BufferTarget.ArrayBuffer, 0);//полная деактивация буфера VBO

                //GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_color_ID);//подключаем буфер цвета
                //                  GL.VertexPointer(4, VertexPointerType.Float, 0, IntPtr.Zero);
                //меняем все вершины vertices в массиве и перезаписываем в видеокарту
                //GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)(sizeof(float) * vertices.Length), sizeof(float) * vertices.Length, vertices);
                //GL.EnableVertexAttribArray(1); // Включаем атрибут цвета
                  //  GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 6 * sizeof(float), (IntPtr)(2 * sizeof(float)));
                //GL.BindBuffer(BufferTarget.ArrayBuffer, 0);//полная деактивация буфера VBO

     //           GL.LineWidth(th);

                //GL.UseProgram(shader);
                    GL.Color4(color);
                    GL.DrawArrays(pType, 0, Vertices.Length / 2);
                GL.DisableClientState(ArrayCap.VertexArray);//восстанавливаем состояние OpenGL
                GL.DisableClientState(ArrayCap.ColorArray);//восстанавливаем состояние OpenGL
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);//полная деактивация буфера VBO
                //GL.BindVertexArray(0);
            }
        }

        /// <summary> Класс <b>VBO</b> - Vertex Buffer Object. <br/> Реализует создание/удаление объекта VBO и хранение буфера вершин vertices в видео памяти видеокарты. </summary>
        public sealed class VBO : IDisposable {
            /// <summary> Структура для буфера вершин. <br/> Хранит координаты X/Y и 4 компоненты цвета RGBA. </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct Vertex {
                public float X, Y; public float R, G, B, A;
                public Vertex(float x, float y, Color4 color) { X = x; Y = y; R = color.R; G = color.G; B = color.B; A = color.A; }
            }
            /// <summary> Константа ошибки идентификатора VBO. </summary>
            private const int InvalidHandle = -1;
            /// <summary> Идентификатор <b>VBO</b>. </summary>
            public int Handle { get; private set; }
            /// <summary> Тип целевого буфера <b>VBO</b>. </summary>
            public BufferTarget Type { get; private set; }

            /// <summary> Конструктор <b>VBO</b>. Задаёт тип целевого буфера и создаёт новый VBO. </summary>
            public VBO(BufferTarget type = BufferTarget.ArrayBuffer) { Type = type; AcquireHandle(); }
            /// <summary> Создаёт новый VBO и сохраняет его идентификатор в свойство Handle. </summary>
            private void AcquireHandle() { Handle = GL.GenBuffer(); }
            /// <summary> Делает данный VBO текущим. </summary>
            public void Use() { GL.BindBuffer(Type, Handle); }
            /// <summary> Заполняет VBO массивом data (вершинами struct Vertex). </summary>
            public void SetData<T>(T[] data) where T : struct {
                if (data.Length == 0) throw new ArgumentException("Массив должен содержать хотя бы один элемент", "data");
                Use();
                GL.BufferData(Type, (IntPtr)(data.Length * Marshal.SizeOf(typeof(T))), data, BufferUsageHint.DynamicDraw);
            }
            /// <summary> Освобождает занятые данным VBO ресурсы. </summary>
            private void ReleaseHandle() { if (Handle == InvalidHandle) return; GL.DeleteBuffer(Handle); Handle = InvalidHandle; }
            public void Dispose() { ReleaseHandle(); GC.SuppressFinalize(this); }

            ~VBO() {
                // При вызове финализатора контекст OpenGL может уже не существовать и попытка выполнить GL.DeleteBuffer приведёт к ошибке
                if (GraphicsContext.CurrentContext != null && !GraphicsContext.CurrentContext.IsDisposed) ReleaseHandle();
            }
        }

        /// <summary> Класс <b>VAO</b> - Vertex Array Object. <br/> Реализует создание/удаление объекта VAO и отрисовку буфера вершин VBO на экране. </summary>
        public sealed class VAO : IDisposable {
            private const int InvalidHandle = -1;
            public int Handle { get; private set; }
            public int VertexCount { get; private set; } // Число вершин для отрисовки

            public VAO(int vertexCount) { VertexCount = vertexCount; AcquireHandle(); }

            private void AcquireHandle() { Handle = GL.GenVertexArray(); }
            public void Use() { GL.BindVertexArray(Handle); }
            public void AttachVBO(int index, VBO vbo, int elementsPerVertex, 
                VertexAttribPointerType pointerType, int stride, int offset)
            {
                Use(); vbo.Use();
                GL.EnableVertexAttribArray(index);
                GL.VertexAttribPointer(index, elementsPerVertex, pointerType, false, stride, offset);
            }
            public void Draw(PrimitiveType type) { Use(); GL.DrawArrays(type, 0, VertexCount); }
            private void ReleaseHandle() { if (Handle == InvalidHandle) return; GL.DeleteVertexArray(Handle); Handle = InvalidHandle; }
            public void Dispose() { ReleaseHandle(); GC.SuppressFinalize(this); }

            ~VAO() { if (GraphicsContext.CurrentContext != null && !GraphicsContext.CurrentContext.IsDisposed) ReleaseHandle(); }
        }

        public sealed class Shader : IDisposable {
            private const int InvalidHandle = -1;
            public int Handle { get; private set; }
            public ShaderType Type { get; private set; }

            public Shader(ShaderType type) { Type = type; AcquireHandle(); }

            private void AcquireHandle() { Handle = GL.CreateShader(Type); }
            public void Compile(string source) {
                GL.ShaderSource(Handle, source);
                GL.CompileShader(Handle);
                int compileStatus;
                GL.GetShader(Handle, ShaderParameter.CompileStatus, out compileStatus);
                // Если произошла ошибка, выведем сообщение
                if (compileStatus == 0) System.Windows.Forms.MessageBox.Show(GL.GetShaderInfoLog(Handle));
            }
            private void ReleaseHandle() { if (Handle == InvalidHandle) return; GL.DeleteShader(Handle); Handle = InvalidHandle; }
            public void Dispose() { ReleaseHandle(); GC.SuppressFinalize(this); }

            ~Shader() { if (GraphicsContext.CurrentContext != null && !GraphicsContext.CurrentContext.IsDisposed) ReleaseHandle(); }
        }

        public sealed class ShaderProgram : IDisposable {
            private const int InvalidHandle = -1;
            public int Handle { get; private set; }

            public ShaderProgram() { AcquireHandle(); }
            private void AcquireHandle() { Handle = GL.CreateProgram(); }
            public void AttachShader(Shader shader) { GL.AttachShader(Handle, shader.Handle); }
            public void Link() {
                GL.LinkProgram(Handle);
                int linkStatus;
                GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out linkStatus);
                if (linkStatus == 0) System.Windows.Forms.MessageBox.Show(GL.GetProgramInfoLog(Handle));
            }
            public void Use() { GL.UseProgram(Handle); }
            private void ReleaseHandle() { if (Handle == InvalidHandle) return; GL.DeleteProgram(Handle); Handle = InvalidHandle; }
            public void Dispose() { ReleaseHandle(); GC.SuppressFinalize(this); }

            ~ShaderProgram() { if (GraphicsContext.CurrentContext != null && !GraphicsContext.CurrentContext.IsDisposed) ReleaseHandle(); }
        }



        /// <summary> Метод рисует все линии связи между двумя частицами: <b>Part</b> и <b>Part.LinkPart[]</b>. </summary>
        public static void DrawLineCoupling(PARTICLE Part, Color4 LineCouplingColor) {
            var x = (float)(Part?.Position2D.X ?? 0); var y = (float)(Part?.Position2D.Y ?? 0); 
            for (int i = 0; i < Part.LinkPart.Length; i++) {
                if (Part?.LinkPart[i] != null) { //линия связи
                    var x2 = (float)(Part?.LinkPart[i]?.Position2D.X ?? x);
                    var y2 = (float)(Part?.LinkPart[i]?.Position2D.Y ?? y);
                    var dist = Physics2D.Distance(Part?.Position2D ?? new Vector2Df(), Part?.LinkPart[i]?.Position2D ?? Part?.Position2D ?? new Vector2Df());
                    if (dist > 0) { LineCouplingColor.A = (255 - (int)(dist * 4)) / 255f;
                        if (LineCouplingColor.A <= 0) LineCouplingColor.A = 0.01f;

                        var th = (float)(Part.ValueCoupling / dist * 1.5);//толщина линии в 2D
                        BeginEnd.Line(x, y, x2, y2, th, LineCouplingColor);
                        //DrawArrays.DrawLine(x, y, x2, y2, th, LineCouplingColor);
                    }
                }
            }
        }
    }
    //=======================================================================================================================
}