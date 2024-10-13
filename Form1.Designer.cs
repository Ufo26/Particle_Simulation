
namespace Particle_Simulation
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cms_ContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.сгенерироватьСимуляциюToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.запуститьСимуляциюToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ss_Info = new System.Windows.Forms.StatusStrip();
            this.skgl_Holst = new SkiaSharp.Views.Desktop.SKGLControl();
            this.pnl_Settings = new System.Windows.Forms.Panel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btn_STOP = new System.Windows.Forms.Button();
            this.btn_CREATE = new System.Windows.Forms.Button();
            this.btn_START = new System.Windows.Forms.Button();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btn_Property_Particle = new System.Windows.Forms.Button();
            this.rb_MoveCheckGridList = new System.Windows.Forms.RadioButton();
            this.rb_MoveCheckGrid = new System.Windows.Forms.RadioButton();
            this.rb_MoveCheck = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chb_AlphaColorCircle = new System.Windows.Forms.CheckBox();
            this.label14 = new System.Windows.Forms.Label();
            this.UD_circle_thickness = new System.Windows.Forms.NumericUpDown();
            this.chb_ContourCircle = new System.Windows.Forms.CheckBox();
            this.btn_PREV_BackGround = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.UD_circle_segments = new System.Windows.Forms.NumericUpDown();
            this.rb_MODE_4 = new System.Windows.Forms.RadioButton();
            this.label12 = new System.Windows.Forms.Label();
            this.rb_MODE_1 = new System.Windows.Forms.RadioButton();
            this.btn_NEXT_BackGround = new System.Windows.Forms.Button();
            this.rb_MODE_3 = new System.Windows.Forms.RadioButton();
            this.btn_RND_BackGround = new System.Windows.Forms.Button();
            this.rb_MODE_2 = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button10 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.tb_KFC = new System.Windows.Forms.TrackBar();
            this.label10 = new System.Windows.Forms.Label();
            this.tb_Acceleration = new System.Windows.Forms.TrackBar();
            this.label9 = new System.Windows.Forms.Label();
            this.tb_afg = new System.Windows.Forms.TrackBar();
            this.label8 = new System.Windows.Forms.Label();
            this.tb_Atmosphere = new System.Windows.Forms.TrackBar();
            this.label7 = new System.Windows.Forms.Label();
            this.tb_StrongInteraction_Dist = new System.Windows.Forms.TrackBar();
            this.label6 = new System.Windows.Forms.Label();
            this.tb_StrongInteraction = new System.Windows.Forms.TrackBar();
            this.label5 = new System.Windows.Forms.Label();
            this.tb_Electromagnetism_Dist = new System.Windows.Forms.TrackBar();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tb_Gravity_Dist = new System.Windows.Forms.TrackBar();
            this.tb_Electromagnetism = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_Gravity = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.cms_ContextMenu.SuspendLayout();
            this.pnl_Settings.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UD_circle_thickness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UD_circle_segments)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tb_KFC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_Acceleration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_afg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_Atmosphere)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_StrongInteraction_Dist)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_StrongInteraction)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_Electromagnetism_Dist)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_Gravity_Dist)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_Electromagnetism)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_Gravity)).BeginInit();
            this.SuspendLayout();
            // 
            // cms_ContextMenu
            // 
            this.cms_ContextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.cms_ContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.сгенерироватьСимуляциюToolStripMenuItem,
            this.запуститьСимуляциюToolStripMenuItem});
            this.cms_ContextMenu.Name = "contextMenuStrip1";
            this.cms_ContextMenu.Size = new System.Drawing.Size(318, 52);
            this.cms_ContextMenu.Text = "1212";
            // 
            // сгенерироватьСимуляциюToolStripMenuItem
            // 
            this.сгенерироватьСимуляциюToolStripMenuItem.Name = "сгенерироватьСимуляциюToolStripMenuItem";
            this.сгенерироватьСимуляциюToolStripMenuItem.Size = new System.Drawing.Size(317, 24);
            this.сгенерироватьСимуляциюToolStripMenuItem.Text = "Сгенерировать новую симуляцию";
            this.сгенерироватьСимуляциюToolStripMenuItem.Click += new System.EventHandler(this.сгенерироватьСимуляциюToolStripMenuItem_Click);
            // 
            // запуститьСимуляциюToolStripMenuItem
            // 
            this.запуститьСимуляциюToolStripMenuItem.Name = "запуститьСимуляциюToolStripMenuItem";
            this.запуститьСимуляциюToolStripMenuItem.Size = new System.Drawing.Size(317, 24);
            this.запуститьСимуляциюToolStripMenuItem.Text = "Запустить симуляцию";
            this.запуститьСимуляциюToolStripMenuItem.Click += new System.EventHandler(this.запуститьСимуляциюToolStripMenuItem_Click);
            // 
            // ss_Info
            // 
            this.ss_Info.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ss_Info.Location = new System.Drawing.Point(0, 636);
            this.ss_Info.Name = "ss_Info";
            this.ss_Info.Size = new System.Drawing.Size(1741, 22);
            this.ss_Info.TabIndex = 1;
            this.ss_Info.Text = "statusStrip1";
            // 
            // skgl_Holst
            // 
            this.skgl_Holst.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.skgl_Holst.Location = new System.Drawing.Point(13, 13);
            this.skgl_Holst.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.skgl_Holst.Name = "skgl_Holst";
            this.skgl_Holst.Size = new System.Drawing.Size(1706, 335);
            this.skgl_Holst.TabIndex = 2;
            this.skgl_Holst.VSync = false;
            this.skgl_Holst.PaintSurface += new System.EventHandler<SkiaSharp.Views.Desktop.SKPaintGLSurfaceEventArgs>(this.skgl_Holst_PaintSurface);
            this.skgl_Holst.Paint += new System.Windows.Forms.PaintEventHandler(this.skgl_Holst_Paint);
            this.skgl_Holst.MouseMove += new System.Windows.Forms.MouseEventHandler(this.skgl_Holst_MouseMove);
            // 
            // pnl_Settings
            // 
            this.pnl_Settings.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pnl_Settings.Controls.Add(this.groupBox4);
            this.pnl_Settings.Controls.Add(this.groupBox3);
            this.pnl_Settings.Controls.Add(this.groupBox2);
            this.pnl_Settings.Controls.Add(this.groupBox1);
            this.pnl_Settings.Location = new System.Drawing.Point(737, 12);
            this.pnl_Settings.Name = "pnl_Settings";
            this.pnl_Settings.Size = new System.Drawing.Size(779, 570);
            this.pnl_Settings.TabIndex = 3;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btn_STOP);
            this.groupBox4.Controls.Add(this.btn_CREATE);
            this.groupBox4.Controls.Add(this.btn_START);
            this.groupBox4.Controls.Add(this.radioButton2);
            this.groupBox4.Controls.Add(this.radioButton1);
            this.groupBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox4.Location = new System.Drawing.Point(6, 436);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(770, 127);
            this.groupBox4.TabIndex = 38;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "НАСТРОЙКИ СИМУЛЯЦИИ:";
            // 
            // btn_STOP
            // 
            this.btn_STOP.AutoSize = true;
            this.btn_STOP.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_STOP.Enabled = false;
            this.btn_STOP.Location = new System.Drawing.Point(195, 80);
            this.btn_STOP.Name = "btn_STOP";
            this.btn_STOP.Size = new System.Drawing.Size(65, 30);
            this.btn_STOP.TabIndex = 4;
            this.btn_STOP.Text = "Стоп";
            this.btn_STOP.UseVisualStyleBackColor = true;
            // 
            // btn_CREATE
            // 
            this.btn_CREATE.AutoSize = true;
            this.btn_CREATE.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_CREATE.BackColor = System.Drawing.Color.Gainsboro;
            this.btn_CREATE.Location = new System.Drawing.Point(8, 80);
            this.btn_CREATE.Name = "btn_CREATE";
            this.btn_CREATE.Size = new System.Drawing.Size(99, 30);
            this.btn_CREATE.TabIndex = 3;
            this.btn_CREATE.Text = "Создать";
            this.btn_CREATE.UseVisualStyleBackColor = false;
            // 
            // btn_START
            // 
            this.btn_START.AutoSize = true;
            this.btn_START.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_START.Enabled = false;
            this.btn_START.Location = new System.Drawing.Point(109, 80);
            this.btn_START.Name = "btn_START";
            this.btn_START.Size = new System.Drawing.Size(83, 30);
            this.btn_START.TabIndex = 2;
            this.btn_START.Text = "Запуск";
            this.btn_START.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Checked = true;
            this.radioButton2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.radioButton2.Location = new System.Drawing.Point(10, 25);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(162, 24);
            this.radioButton2.TabIndex = 1;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "Законы Физики";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.radioButton1.Location = new System.Drawing.Point(10, 50);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(229, 24);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.Text = "Произвольные правила";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btn_Property_Particle);
            this.groupBox3.Controls.Add(this.rb_MoveCheckGridList);
            this.groupBox3.Controls.Add(this.rb_MoveCheckGrid);
            this.groupBox3.Controls.Add(this.rb_MoveCheck);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox3.Location = new System.Drawing.Point(399, 159);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(374, 271);
            this.groupBox3.TabIndex = 37;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "ЧАСТИЦА:";
            // 
            // btn_Property_Particle
            // 
            this.btn_Property_Particle.Location = new System.Drawing.Point(322, 15);
            this.btn_Property_Particle.Name = "btn_Property_Particle";
            this.btn_Property_Particle.Size = new System.Drawing.Size(35, 35);
            this.btn_Property_Particle.TabIndex = 50;
            this.btn_Property_Particle.Text = "P";
            this.btn_Property_Particle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btn_Property_Particle.UseVisualStyleBackColor = true;
            // 
            // rb_MoveCheckGridList
            // 
            this.rb_MoveCheckGridList.AutoSize = true;
            this.rb_MoveCheckGridList.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rb_MoveCheckGridList.Location = new System.Drawing.Point(15, 86);
            this.rb_MoveCheckGridList.Name = "rb_MoveCheckGridList";
            this.rb_MoveCheckGridList.Size = new System.Drawing.Size(177, 24);
            this.rb_MoveCheckGridList.TabIndex = 49;
            this.rb_MoveCheckGridList.Tag = "1";
            this.rb_MoveCheckGridList.Text = "MoveCheckGridList";
            this.rb_MoveCheckGridList.UseVisualStyleBackColor = true;
            // 
            // rb_MoveCheckGrid
            // 
            this.rb_MoveCheckGrid.AutoSize = true;
            this.rb_MoveCheckGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rb_MoveCheckGrid.Location = new System.Drawing.Point(15, 56);
            this.rb_MoveCheckGrid.Name = "rb_MoveCheckGrid";
            this.rb_MoveCheckGrid.Size = new System.Drawing.Size(149, 24);
            this.rb_MoveCheckGrid.TabIndex = 48;
            this.rb_MoveCheckGrid.Tag = "1";
            this.rb_MoveCheckGrid.Text = "MoveCheckGrid";
            this.rb_MoveCheckGrid.UseVisualStyleBackColor = true;
            // 
            // rb_MoveCheck
            // 
            this.rb_MoveCheck.AutoSize = true;
            this.rb_MoveCheck.Checked = true;
            this.rb_MoveCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rb_MoveCheck.Location = new System.Drawing.Point(15, 32);
            this.rb_MoveCheck.Name = "rb_MoveCheck";
            this.rb_MoveCheck.Size = new System.Drawing.Size(117, 24);
            this.rb_MoveCheck.TabIndex = 47;
            this.rb_MoveCheck.TabStop = true;
            this.rb_MoveCheck.Tag = "1";
            this.rb_MoveCheck.Text = "MoveCheck";
            this.rb_MoveCheck.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chb_AlphaColorCircle);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.UD_circle_thickness);
            this.groupBox2.Controls.Add(this.chb_ContourCircle);
            this.groupBox2.Controls.Add(this.btn_PREV_BackGround);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.UD_circle_segments);
            this.groupBox2.Controls.Add(this.rb_MODE_4);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.rb_MODE_1);
            this.groupBox2.Controls.Add(this.btn_NEXT_BackGround);
            this.groupBox2.Controls.Add(this.rb_MODE_3);
            this.groupBox2.Controls.Add(this.btn_RND_BackGround);
            this.groupBox2.Controls.Add(this.rb_MODE_2);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(770, 150);
            this.groupBox2.TabIndex = 36;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "ГРАФИКА:";
            // 
            // chb_AlphaColorCircle
            // 
            this.chb_AlphaColorCircle.AutoSize = true;
            this.chb_AlphaColorCircle.Checked = true;
            this.chb_AlphaColorCircle.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chb_AlphaColorCircle.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.chb_AlphaColorCircle.Location = new System.Drawing.Point(15, 82);
            this.chb_AlphaColorCircle.Name = "chb_AlphaColorCircle";
            this.chb_AlphaColorCircle.Size = new System.Drawing.Size(332, 24);
            this.chb_AlphaColorCircle.TabIndex = 44;
            this.chb_AlphaColorCircle.Text = "Прозрачность зависит от скорости";
            this.chb_AlphaColorCircle.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label14.Location = new System.Drawing.Point(232, 58);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(133, 20);
            this.label14.TabIndex = 43;
            this.label14.Text = "толщина круга";
            // 
            // UD_circle_thickness
            // 
            this.UD_circle_thickness.BackColor = System.Drawing.SystemColors.Window;
            this.UD_circle_thickness.Location = new System.Drawing.Point(161, 54);
            this.UD_circle_thickness.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.UD_circle_thickness.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.UD_circle_thickness.Name = "UD_circle_thickness";
            this.UD_circle_thickness.Size = new System.Drawing.Size(65, 26);
            this.UD_circle_thickness.TabIndex = 42;
            this.UD_circle_thickness.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // chb_ContourCircle
            // 
            this.chb_ContourCircle.AutoSize = true;
            this.chb_ContourCircle.Checked = true;
            this.chb_ContourCircle.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chb_ContourCircle.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.chb_ContourCircle.Location = new System.Drawing.Point(15, 57);
            this.chb_ContourCircle.Name = "chb_ContourCircle";
            this.chb_ContourCircle.Size = new System.Drawing.Size(140, 24);
            this.chb_ContourCircle.TabIndex = 41;
            this.chb_ContourCircle.Text = "Контур круга";
            this.chb_ContourCircle.UseVisualStyleBackColor = true;
            // 
            // btn_PREV_BackGround
            // 
            this.btn_PREV_BackGround.AutoSize = true;
            this.btn_PREV_BackGround.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_PREV_BackGround.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_PREV_BackGround.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_PREV_BackGround.Location = new System.Drawing.Point(196, 106);
            this.btn_PREV_BackGround.Name = "btn_PREV_BackGround";
            this.btn_PREV_BackGround.Size = new System.Drawing.Size(75, 29);
            this.btn_PREV_BackGround.TabIndex = 40;
            this.btn_PREV_BackGround.Text = "<< 0/20";
            this.btn_PREV_BackGround.UseVisualStyleBackColor = true;
            this.btn_PREV_BackGround.Click += new System.EventHandler(this.btn_PREV_BackGround_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label13.Location = new System.Drawing.Point(11, 31);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(243, 20);
            this.label13.TabIndex = 39;
            this.label13.Text = "Кол-во граней круга [3..360]:";
            // 
            // UD_circle_segments
            // 
            this.UD_circle_segments.Location = new System.Drawing.Point(260, 29);
            this.UD_circle_segments.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.UD_circle_segments.Minimum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.UD_circle_segments.Name = "UD_circle_segments";
            this.UD_circle_segments.Size = new System.Drawing.Size(65, 26);
            this.UD_circle_segments.TabIndex = 38;
            this.UD_circle_segments.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            // 
            // rb_MODE_4
            // 
            this.rb_MODE_4.AutoSize = true;
            this.rb_MODE_4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rb_MODE_4.Location = new System.Drawing.Point(550, 42);
            this.rb_MODE_4.Name = "rb_MODE_4";
            this.rb_MODE_4.Size = new System.Drawing.Size(203, 24);
            this.rb_MODE_4.TabIndex = 37;
            this.rb_MODE_4.TabStop = true;
            this.rb_MODE_4.Tag = "3";
            this.rb_MODE_4.Text = "Grid: 1 Cell => 1 Circle";
            this.rb_MODE_4.UseVisualStyleBackColor = true;
            this.rb_MODE_4.CheckedChanged += new System.EventHandler(this.rb_MODE_CheckedChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(387, 22);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(243, 20);
            this.label12.TabIndex = 36;
            this.label12.Text = "Режим отрисовки (Mode):";
            // 
            // rb_MODE_1
            // 
            this.rb_MODE_1.AutoSize = true;
            this.rb_MODE_1.Checked = true;
            this.rb_MODE_1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rb_MODE_1.Location = new System.Drawing.Point(387, 42);
            this.rb_MODE_1.Name = "rb_MODE_1";
            this.rb_MODE_1.Size = new System.Drawing.Size(151, 24);
            this.rb_MODE_1.TabIndex = 31;
            this.rb_MODE_1.TabStop = true;
            this.rb_MODE_1.Tag = "0";
            this.rb_MODE_1.Text = "По умолчанию";
            this.rb_MODE_1.UseVisualStyleBackColor = true;
            this.rb_MODE_1.CheckedChanged += new System.EventHandler(this.rb_MODE_CheckedChanged);
            // 
            // btn_NEXT_BackGround
            // 
            this.btn_NEXT_BackGround.AutoSize = true;
            this.btn_NEXT_BackGround.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_NEXT_BackGround.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_NEXT_BackGround.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_NEXT_BackGround.Location = new System.Drawing.Point(277, 106);
            this.btn_NEXT_BackGround.Name = "btn_NEXT_BackGround";
            this.btn_NEXT_BackGround.Size = new System.Drawing.Size(75, 29);
            this.btn_NEXT_BackGround.TabIndex = 34;
            this.btn_NEXT_BackGround.Text = ">> 0/20";
            this.btn_NEXT_BackGround.UseVisualStyleBackColor = true;
            this.btn_NEXT_BackGround.Click += new System.EventHandler(this.btn_NEXT_BackGround_Click);
            // 
            // rb_MODE_3
            // 
            this.rb_MODE_3.AutoSize = true;
            this.rb_MODE_3.BackColor = System.Drawing.Color.Transparent;
            this.rb_MODE_3.Location = new System.Drawing.Point(550, 67);
            this.rb_MODE_3.Name = "rb_MODE_3";
            this.rb_MODE_3.Size = new System.Drawing.Size(161, 24);
            this.rb_MODE_3.TabIndex = 35;
            this.rb_MODE_3.TabStop = true;
            this.rb_MODE_3.Tag = "2";
            this.rb_MODE_3.Text = "TestGrid[x, y][i]";
            this.rb_MODE_3.UseVisualStyleBackColor = false;
            this.rb_MODE_3.CheckedChanged += new System.EventHandler(this.rb_MODE_CheckedChanged);
            // 
            // btn_RND_BackGround
            // 
            this.btn_RND_BackGround.AutoSize = true;
            this.btn_RND_BackGround.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_RND_BackGround.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_RND_BackGround.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_RND_BackGround.Location = new System.Drawing.Point(15, 106);
            this.btn_RND_BackGround.Name = "btn_RND_BackGround";
            this.btn_RND_BackGround.Size = new System.Drawing.Size(175, 29);
            this.btn_RND_BackGround.TabIndex = 33;
            this.btn_RND_BackGround.Text = "Случайный фон 0/20";
            this.btn_RND_BackGround.UseVisualStyleBackColor = true;
            this.btn_RND_BackGround.Click += new System.EventHandler(this.btn_RND_BackGround_Click);
            // 
            // rb_MODE_2
            // 
            this.rb_MODE_2.AutoSize = true;
            this.rb_MODE_2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rb_MODE_2.Location = new System.Drawing.Point(387, 67);
            this.rb_MODE_2.Name = "rb_MODE_2";
            this.rb_MODE_2.Size = new System.Drawing.Size(140, 24);
            this.rb_MODE_2.TabIndex = 32;
            this.rb_MODE_2.Tag = "1";
            this.rb_MODE_2.Text = "Индексы Grid";
            this.rb_MODE_2.UseVisualStyleBackColor = true;
            this.rb_MODE_2.CheckedChanged += new System.EventHandler(this.rb_MODE_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button10);
            this.groupBox1.Controls.Add(this.button9);
            this.groupBox1.Controls.Add(this.button8);
            this.groupBox1.Controls.Add(this.button7);
            this.groupBox1.Controls.Add(this.button6);
            this.groupBox1.Controls.Add(this.button5);
            this.groupBox1.Controls.Add(this.button4);
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.tb_KFC);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.tb_Acceleration);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.tb_afg);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.tb_Atmosphere);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.tb_StrongInteraction_Dist);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.tb_StrongInteraction);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.tb_Electromagnetism_Dist);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.tb_Gravity_Dist);
            this.groupBox1.Controls.Add(this.tb_Electromagnetism);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.tb_Gravity);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox1.Location = new System.Drawing.Point(3, 159);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(390, 271);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ФИЗИКА СИМУЛЯЦИИ:";
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(195, 230);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(25, 25);
            this.button10.TabIndex = 30;
            this.button10.Text = ".";
            this.button10.UseVisualStyleBackColor = true;
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(170, 230);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(25, 25);
            this.button9.TabIndex = 29;
            this.button9.Text = ".";
            this.button9.UseVisualStyleBackColor = true;
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(195, 185);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(25, 25);
            this.button8.TabIndex = 28;
            this.button8.Text = ".";
            this.button8.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(170, 185);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(25, 25);
            this.button7.TabIndex = 27;
            this.button7.Text = ".";
            this.button7.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(195, 140);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(25, 25);
            this.button6.TabIndex = 26;
            this.button6.Text = ".";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(170, 140);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(25, 25);
            this.button5.TabIndex = 25;
            this.button5.Text = ".";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(195, 95);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(25, 25);
            this.button4.TabIndex = 24;
            this.button4.Text = ".";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(170, 95);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(25, 25);
            this.button3.TabIndex = 23;
            this.button3.Text = ".";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(195, 50);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(25, 25);
            this.button2.TabIndex = 22;
            this.button2.Text = ".";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(170, 50);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(25, 25);
            this.button1.TabIndex = 21;
            this.button1.Text = ".";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // tb_KFC
            // 
            this.tb_KFC.AutoSize = false;
            this.tb_KFC.Location = new System.Drawing.Point(210, 230);
            this.tb_KFC.Maximum = 10000;
            this.tb_KFC.Name = "tb_KFC";
            this.tb_KFC.Size = new System.Drawing.Size(175, 30);
            this.tb_KFC.TabIndex = 19;
            this.tb_KFC.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tb_KFC.Value = 1000;
            this.tb_KFC.Scroll += new System.EventHandler(this.tb_KFC_Scroll);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label10.Location = new System.Drawing.Point(220, 210);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(164, 20);
            this.label10.TabIndex = 18;
            this.label10.Text = "Сила связи [0.001]:";
            // 
            // tb_Acceleration
            // 
            this.tb_Acceleration.AutoSize = false;
            this.tb_Acceleration.LargeChange = 1;
            this.tb_Acceleration.Location = new System.Drawing.Point(5, 230);
            this.tb_Acceleration.Maximum = 1000;
            this.tb_Acceleration.Name = "tb_Acceleration";
            this.tb_Acceleration.Size = new System.Drawing.Size(175, 30);
            this.tb_Acceleration.TabIndex = 17;
            this.tb_Acceleration.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tb_Acceleration.Scroll += new System.EventHandler(this.tb_Acceleration_Scroll);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label9.Location = new System.Drawing.Point(10, 210);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(170, 20);
            this.label9.TabIndex = 16;
            this.label9.Text = "Акселерация [0.00]:";
            // 
            // tb_afg
            // 
            this.tb_afg.AutoSize = false;
            this.tb_afg.Location = new System.Drawing.Point(210, 185);
            this.tb_afg.Maximum = 1000;
            this.tb_afg.Name = "tb_afg";
            this.tb_afg.Size = new System.Drawing.Size(175, 30);
            this.tb_afg.TabIndex = 15;
            this.tb_afg.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tb_afg.Value = 981;
            this.tb_afg.Scroll += new System.EventHandler(this.tb_afg_Scroll);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label8.Location = new System.Drawing.Point(210, 165);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(177, 20);
            this.label8.TabIndex = 14;
            this.label8.Text = "Ускор.св.пад [0.981]:";
            // 
            // tb_Atmosphere
            // 
            this.tb_Atmosphere.AutoSize = false;
            this.tb_Atmosphere.Location = new System.Drawing.Point(5, 185);
            this.tb_Atmosphere.Maximum = 5000;
            this.tb_Atmosphere.Name = "tb_Atmosphere";
            this.tb_Atmosphere.Size = new System.Drawing.Size(175, 30);
            this.tb_Atmosphere.TabIndex = 13;
            this.tb_Atmosphere.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tb_Atmosphere.Value = 500;
            this.tb_Atmosphere.Scroll += new System.EventHandler(this.tb_Atmosphere_Scroll);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label7.Location = new System.Drawing.Point(5, 165);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(198, 17);
            this.label7.TabIndex = 12;
            this.label7.Text = "Плотность атмосферы [500]:";
            // 
            // tb_StrongInteraction_Dist
            // 
            this.tb_StrongInteraction_Dist.AutoSize = false;
            this.tb_StrongInteraction_Dist.Location = new System.Drawing.Point(210, 140);
            this.tb_StrongInteraction_Dist.Maximum = 1000;
            this.tb_StrongInteraction_Dist.Name = "tb_StrongInteraction_Dist";
            this.tb_StrongInteraction_Dist.Size = new System.Drawing.Size(175, 30);
            this.tb_StrongInteraction_Dist.TabIndex = 11;
            this.tb_StrongInteraction_Dist.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tb_StrongInteraction_Dist.Value = 20;
            this.tb_StrongInteraction_Dist.Scroll += new System.EventHandler(this.tb_StrongInteraction_Dist_Scroll);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.Location = new System.Drawing.Point(230, 120);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(139, 20);
            this.label6.TabIndex = 10;
            this.label6.Text = "Дистанция [20]:";
            // 
            // tb_StrongInteraction
            // 
            this.tb_StrongInteraction.AutoSize = false;
            this.tb_StrongInteraction.Location = new System.Drawing.Point(5, 140);
            this.tb_StrongInteraction.Maximum = 200;
            this.tb_StrongInteraction.Name = "tb_StrongInteraction";
            this.tb_StrongInteraction.Size = new System.Drawing.Size(175, 30);
            this.tb_StrongInteraction.TabIndex = 9;
            this.tb_StrongInteraction.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tb_StrongInteraction.Value = 37;
            this.tb_StrongInteraction.Scroll += new System.EventHandler(this.tb_StrongInteraction_Scroll);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(5, 120);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(220, 17);
            this.label5.TabIndex = 8;
            this.label5.Text = "Сильное взаимодействие [0.74]:";
            // 
            // tb_Electromagnetism_Dist
            // 
            this.tb_Electromagnetism_Dist.AutoSize = false;
            this.tb_Electromagnetism_Dist.Location = new System.Drawing.Point(210, 95);
            this.tb_Electromagnetism_Dist.Maximum = 5000;
            this.tb_Electromagnetism_Dist.Name = "tb_Electromagnetism_Dist";
            this.tb_Electromagnetism_Dist.Size = new System.Drawing.Size(174, 30);
            this.tb_Electromagnetism_Dist.TabIndex = 7;
            this.tb_Electromagnetism_Dist.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tb_Electromagnetism_Dist.Value = 5000;
            this.tb_Electromagnetism_Dist.Scroll += new System.EventHandler(this.tb_Electromagnetism_Dist_Scroll);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(220, 75);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(157, 20);
            this.label4.TabIndex = 6;
            this.label4.Text = "Дистанция [5000]:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(220, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(157, 20);
            this.label3.TabIndex = 5;
            this.label3.Text = "Дистанция [5000]:";
            // 
            // tb_Gravity_Dist
            // 
            this.tb_Gravity_Dist.AutoSize = false;
            this.tb_Gravity_Dist.Location = new System.Drawing.Point(210, 50);
            this.tb_Gravity_Dist.Maximum = 5000;
            this.tb_Gravity_Dist.Name = "tb_Gravity_Dist";
            this.tb_Gravity_Dist.Size = new System.Drawing.Size(175, 30);
            this.tb_Gravity_Dist.TabIndex = 4;
            this.tb_Gravity_Dist.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tb_Gravity_Dist.Value = 5000;
            this.tb_Gravity_Dist.Scroll += new System.EventHandler(this.tb_Gravity_Dist_Scroll);
            // 
            // tb_Electromagnetism
            // 
            this.tb_Electromagnetism.AutoSize = false;
            this.tb_Electromagnetism.Location = new System.Drawing.Point(5, 95);
            this.tb_Electromagnetism.Maximum = 200;
            this.tb_Electromagnetism.Name = "tb_Electromagnetism";
            this.tb_Electromagnetism.Size = new System.Drawing.Size(175, 30);
            this.tb_Electromagnetism.TabIndex = 3;
            this.tb_Electromagnetism.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tb_Electromagnetism.Value = 50;
            this.tb_Electromagnetism.Scroll += new System.EventHandler(this.tb_Electromagnetism_Scroll);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(10, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(187, 18);
            this.label2.TabIndex = 2;
            this.label2.Text = "Электромагнетизм [1.00]:";
            // 
            // tb_Gravity
            // 
            this.tb_Gravity.AutoSize = false;
            this.tb_Gravity.Location = new System.Drawing.Point(5, 50);
            this.tb_Gravity.Maximum = 200;
            this.tb_Gravity.Name = "tb_Gravity";
            this.tb_Gravity.Size = new System.Drawing.Size(175, 30);
            this.tb_Gravity.TabIndex = 1;
            this.tb_Gravity.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tb_Gravity.Value = 30;
            this.tb_Gravity.Scroll += new System.EventHandler(this.tb_Gravity_Scroll);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(10, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(156, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Притяжение [0.6]:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1741, 658);
            this.Controls.Add(this.pnl_Settings);
            this.Controls.Add(this.skgl_Holst);
            this.Controls.Add(this.ss_Info);
            this.DoubleBuffered = true;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.cms_ContextMenu.ResumeLayout(false);
            this.pnl_Settings.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UD_circle_thickness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UD_circle_segments)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tb_KFC)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_Acceleration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_afg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_Atmosphere)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_StrongInteraction_Dist)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_StrongInteraction)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_Electromagnetism_Dist)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_Gravity_Dist)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_Electromagnetism)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_Gravity)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip cms_ContextMenu;
        private System.Windows.Forms.ToolStripMenuItem запуститьСимуляциюToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem сгенерироватьСимуляциюToolStripMenuItem;
        private System.Windows.Forms.StatusStrip ss_Info;
        private SkiaSharp.Views.Desktop.SKGLControl skgl_Holst;
        private System.Windows.Forms.Panel pnl_Settings;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar tb_Gravity;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar tb_Electromagnetism;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TrackBar tb_Gravity_Dist;
        private System.Windows.Forms.TrackBar tb_Electromagnetism_Dist;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TrackBar tb_StrongInteraction;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TrackBar tb_StrongInteraction_Dist;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TrackBar tb_Atmosphere;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TrackBar tb_afg;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TrackBar tb_Acceleration;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TrackBar tb_KFC;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.RadioButton rb_MODE_2;
        private System.Windows.Forms.RadioButton rb_MODE_1;
        private System.Windows.Forms.Button btn_NEXT_BackGround;
        private System.Windows.Forms.Button btn_RND_BackGround;
        private System.Windows.Forms.RadioButton rb_MODE_3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rb_MODE_4;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown UD_circle_segments;
        private System.Windows.Forms.Button btn_PREV_BackGround;
        private System.Windows.Forms.CheckBox chb_ContourCircle;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.NumericUpDown UD_circle_thickness;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Button btn_START;
        private System.Windows.Forms.Button btn_CREATE;
        private System.Windows.Forms.Button btn_STOP;
        private System.Windows.Forms.CheckBox chb_AlphaColorCircle;
        private System.Windows.Forms.RadioButton rb_MoveCheckGrid;
        private System.Windows.Forms.RadioButton rb_MoveCheck;
        private System.Windows.Forms.RadioButton rb_MoveCheckGridList;
        private System.Windows.Forms.Button btn_Property_Particle;
    }
}

