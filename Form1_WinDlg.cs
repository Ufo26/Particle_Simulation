using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UFO;
using static UFO.Enums_Structs;

namespace Particle_Simulation {
    public partial class Form1 : Form {
        /// <summary> Метод создаёт ячейку <b>Row.Cell</b> для таблицы <b>DataGridView</b>. </summary>
        /// <value> 
        ///     <b><paramref name="value"/>:</b> значение для <b>Cell.Value</b>. <br/>
        ///     <b><paramref name="type"/>:</b> тип ячейки для <b>Cell.ValueType</b>. <br/>
        ///     <b><paramref name="style"/>:</b> стиль ячейки для <b>Cell.Style</b>. <br/>
        ///     <b><paramref name="items"/>:</b> массив строк для <b>Cell.Items</b>, если тип ячейки - <b>DataGridViewComboBoxCell</b>, для остальных типов items = null. <br/>
        ///     <b><paramref name="fs"/>:</b> внешний вид элемента для <b>Cell.FlatStyle</b>, если тип ячейки - <b>DataGridViewComboBoxCell</b>, для остальных типов fs = FlatStyle.System. <br/>
        ///     <b><paramref name="TCG"/>:</b> перечисление сложных типов для таблицы, хранит информацию о типе размещаемого контрола в ячейке. <br/>
        /// </value>
        /// <returns> Возвращяет ячейку <b>Cell</b> для таблицы. </returns>
        private object CreateCell(object value, Type type, DataGridViewCellStyle style,
            string[] items, FlatStyle fs, TypeCellGrid TCG)
        {
            switch (TCG) {
                case TypeCellGrid.TextBoxCell:
                    return new DataGridViewTextBoxCell { Value = value, ValueType = type, Style = style, };
                    break;
                case TypeCellGrid.ComboBoxCell:
                    var tmp = new DataGridViewComboBoxCell { Value = value, ValueType = type, Style = style, FlatStyle = fs, };
                    tmp.Items.AddRange(items);
                    return tmp;
                    break;
                default: return null; break;
            }
        }
        /// <summary> Метод создаёт строку <b>Row</b>, заполненную ячейками <b>Cell</b> для таблицы <b>DataGridView</b>. </summary>
        /// <value> 
        ///     <b><paramref name="value"/>:</b> массив значений для <b>Cell[].Value</b>, равный количеству ячеек в строке. <br/>
        ///     <b><paramref name="type"/>:</b> массив типов ячеек для <b>Cell[].ValueType</b>, равный количеству ячеек в строке. <br/>
        ///     <b><paramref name="style"/>:</b> массив стилей ячейки для <b>Cell[].Style</b>, равный количеству ячеек в строке. <br/>
        ///     <b><paramref name="items"/>:</b> массив строк для <b>Cell.Items</b>, если тип ячейки - <b>DataGridViewComboBoxCell</b>, для остальных типов items = null. <br/>
        ///     <b><paramref name="fs"/>:</b> внешний вид элемента для <b>Cell.FlatStyle</b>, если тип ячейки - <b>DataGridViewComboBoxCell</b>, для остальных типов fs = FlatStyle.System. <br/>
        ///     <b><paramref name="TCG"/>:</b> перечисление сложных типов для таблицы, хранит информацию о типе размещаемого контрола в ячейке. <br/>
        /// </value>
        /// <returns> Возвращяет строку <b>Row</b>, заполненную ячейками <b>Cell</b> для таблицы. </returns>
        private DataGridViewRow CreateRow(object[] value, Type[] type, DataGridViewCellStyle[] style,
            string[] items, FlatStyle fs, TypeCellGrid[] TCG)
        {
            DataGridViewRow row = new DataGridViewRow {};
            for (int i = 0; i < value.Length; i++) {
                switch (TCG[i]) {
                    case TypeCellGrid.TextBoxCell:
                        row.Cells.Add((DataGridViewTextBoxCell)CreateCell(value[i], type[i], style[i], items, fs, TCG[i]));
                        break;
                    case TypeCellGrid.ComboBoxCell:
                        row.Cells.Add((DataGridViewComboBoxCell)CreateCell(value[i], type[i], style[i], items, fs, TCG[i]));
                        break;
                }
            }
            return row;
        }

        /// <summary> Главная панель, на которой расположены все данные настроек и свойств симуляции. </summary>
        private Panel pnl_Settings_MainPanel = null;
        /// <summary> Таблица отображения и ввода коэффициентов для свойств симуляции. </summary>
        private DataGridView pnl_Settings_gridView = null;
        /// <summary> Панель информации о выбранной строке на панели свойств симуляции. </summary>
        private Panel pnl_Settings_PanelInformation = null;
        /// <summary> Текст названия ячейки на панели информации о выбранной строке на панели свойств симуляции. </summary>
        private Label pnl_Settings_TextNameInformation = null;
        /// <summary> Текст описания на панели информации о выбранной строке на панели свойств симуляции. </summary>
        private Label pnl_Settings_TextInformation = null;
        /// <summary> Хранит текст ячейки первого столбца на панели свойств симуляции, в котором произошла ошибка ввода данных. <br/> Если pnl_Settings_SaveCellText = "", значит ошибки ввода нет. </summary>
        private string pnl_Settings_SaveCellText = "";
        /// <summary> Хранит массив строк - описаний каждого свойства, выводимого в <b>pnl_Settings_TextInformation</b>. <br/> Кадому элементу массива соответствует такая же по индексу строка в таблице Grid. </summary>
        private string[] pnl_Settings_ArrayInformation = null;

        /// <summary> Метод создаёт данные свойств на панели pnl_Settings для окна симуляции. </summary>
        public void CreateData_pnl_Settings() {
            return;
            if (pnl_Settings_MainPanel == null) {
                float FSize = 10; string FName = "Arial";
                Color Light = Color.FromArgb(66, 66, 72); Color Dark = Color.FromArgb(37, 37, 38);
                pnl_Settings_MainPanel = new Panel { Parent = pnl_Settings, BackColor = Light, AutoSize = true,
                    Size = new Size(500, 800), BorderStyle = BorderStyle.FixedSingle, Location = new Point(0, 0),
                };
                pnl_Settings_MainPanel.BringToFront();
                btn_RND_BackGround.Parent = btn_PREV_BackGround.Parent = btn_NEXT_BackGround.Parent = pnl_Settings_MainPanel;

                pnl_Settings_gridView = Extensions.CreateGrid(pnl_Settings_MainPanel, "pnl_Settings_gridView", new Font(FName, FSize, FontStyle.Bold),
                    Location: new Point(5, 5), BackgroundColor: Dark, GridColor: Light, BorderStyle: BorderStyle.FixedSingle, Enabled: true);
                var _ = pnl_Settings_gridView;
                _.SuspendLayout();
                _.Columns.Add("Text", "Свойство"); _.Columns.Add("Number", "Значение");
                _.DefaultCellStyle.BackColor = Dark; _.DefaultCellStyle.ForeColor = Color.White;
                _.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                for (int i = 0; i < _.Columns.Count; i++) {
                    if (i == 0) {
                        _.Columns[i].DefaultCellStyle.Font = new Font(FName, FSize, FontStyle.Bold);
                        _.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                        _.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        _.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        _.Columns[i].ReadOnly = true;      
                    } else {
                        _.Columns[i].DefaultCellStyle.Font = new Font(FName, FSize, FontStyle.Regular);
                        _.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                        _.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        //_.Columns[i].ValueType = typeof(double);
                    }
                }
                Font Bold = new Font(FName, FSize, FontStyle.Bold);
                Font Regular = new Font(FName, FSize, FontStyle.Regular);
                var StyleHead = new DataGridViewCellStyle { Font = Bold, BackColor = Light, ForeColor = Color.Black };
                var StyleProperty = new DataGridViewCellStyle { Font = Regular, BackColor = Dark, ForeColor = Color.White };
                DataGridViewRow[] rows = {
                    new DataGridViewRow { Cells = {
                        new DataGridViewTextBoxCell { Value = " ГРАФИКА ", ValueType = typeof(string), Style = StyleHead },
                        new DataGridViewTextBoxCell { Value = DBNull.Value, ValueType = typeof(string), Style = StyleHead } }
                    },
                    CreateRow(new object[] { "   Режим отрисовки (Mode)  ", "Default" }, new Type[] { typeof(string), typeof(string) }, new DataGridViewCellStyle[] { StyleProperty, StyleProperty }, new string[] { "Default", "Grid", "TestGrid", "OneCellOneCircle" }, FlatStyle.Popup, new TypeCellGrid[] { TypeCellGrid.TextBoxCell, TypeCellGrid.ComboBoxCell } ),
                    CreateRow(new object[] { "   Грани круга  ", 6 }, new Type[] { typeof(string), typeof(int) }, new DataGridViewCellStyle[] { StyleProperty, StyleProperty }, null, FlatStyle.System, new TypeCellGrid[] { TypeCellGrid.TextBoxCell, TypeCellGrid.TextBoxCell } ),
                    CreateRow(new object[] { "   Флаг контура круга  ", "True" }, new Type[] { typeof(string), typeof(string) }, new DataGridViewCellStyle[] { StyleProperty, StyleProperty }, new string[] { "True", "False" }, FlatStyle.Popup, new TypeCellGrid[] { TypeCellGrid.TextBoxCell, TypeCellGrid.ComboBoxCell } ),
                    CreateRow(new object[] { "     Толщина контура круга  ", 2 }, new Type[] { typeof(string), typeof(int) }, new DataGridViewCellStyle[] { StyleProperty, StyleProperty }, null, FlatStyle.System, new TypeCellGrid[] { TypeCellGrid.TextBoxCell, TypeCellGrid.TextBoxCell } ),
                    CreateRow(new object[] { "   Флаг Прозрачности круга  ", "True" }, new Type[] { typeof(string), typeof(string) }, new DataGridViewCellStyle[] { StyleProperty, StyleProperty }, new string[] { "True", "False" }, FlatStyle.Popup, new TypeCellGrid[] { TypeCellGrid.TextBoxCell, TypeCellGrid.ComboBoxCell } ),

                    CreateRow(new object[] { " ФИЗИКА СИМУЛЯЦИИ ", DBNull.Value }, new Type[] { typeof(string), typeof(string) }, new DataGridViewCellStyle[] { StyleHead, StyleHead }, null, FlatStyle.System, new TypeCellGrid[] { TypeCellGrid.TextBoxCell, TypeCellGrid.TextBoxCell } ),
                };
                _.Rows.AddRange(rows);
                for (int i = 0; i < _.Rows.Count; i++) if (_[1, i].Value == DBNull.Value) _[1, i].ReadOnly = true;//запрещаем редактирование заголовков
                pnl_Settings_ArrayInformation = new string[] { 
                    "Под заголовком ГРАФИКА, расположены Свойства связанные с манипуляцией графики симуляции.",
                    "Свойство задаёт способ вывода графики.\nDefault: стандартная отрисовка симуляции, каждая частица берётся из листа Particles[i];\nGrid: отрисовка в режиме таблицы Grid[x, y][index], каждая частица берётся из листа индексов каждой ячейки таблицы;\nTestGrid: тоже самое что и предыдущий режим + отображение покрываемых ячеек кругом и вектора скорости;\nOneCellOneCircle: в центре каждой ячейки Grid отображается лишь один круг с усреднённым радиусом находящихся там кругов;",
                    "Свойство задаёт количество граней круга в диапазоне от 3 до 360.",
                    "Свойство задаёт видимость контура круга.\nTrue = отображать контур; False - нет.",
                    "Свойство задаёт толщину контура круга в пределах от 1 до 25.",
                    "Свойство разрешает прозрачность круга, степень прозрачности зависит от скорости: чем медленнее двигается частица - тем прозрачнее она становится.\nTrue = разрешить прозрачность; False - запретить.",
                        
                    "Под заголовком ФИЗИКА СИМУЛЯЦИИ, расположены Свойства связанные с манипуляцией общей физики симуляции.",
                };

                ////_.Rows.Add("  Плотность среды:  ", 1.22);
                //_.Rows.Add("  Вязкость среды:  ", 0.182);
                //_.Rows.Add("  Масштаб высоты:  ", 0.2);
                int Height = (_.Rows.Count + 2) * _.Font.Height; Height = Height <= 800 ? Height : 800;
                _.Size = new Size(500, Height);
                _.ResumeLayout();

                _.DataError += (s, a) => { //обработчик ошибки ввода в ячейку
                    pnl_Settings_SaveCellText = (string)_[0, a.RowIndex].Value;
                    _[0, a.RowIndex].Value = "Ошибка ввода!";
                    _[0, a.RowIndex].Style.ForeColor = Color.Red; _[0, a.RowIndex].Selected = false;
                };
                _.CellEndEdit += (s, a) => { //обработчик остановки или завершения ввода данных в ячейку
                    if (pnl_Settings_SaveCellText != "") { //была ошибка ввода
                        _[0, a.RowIndex].Value = pnl_Settings_SaveCellText; pnl_Settings_SaveCellText = "";
                        _[0, a.RowIndex].Style.ForeColor = Color.White;
                    } else {
                        if (_[1, a.RowIndex].Value == DBNull.Value) { _[1, a.RowIndex].Value = 0; }//пустой ввод
                        else if (a.RowIndex == 2) { //коррекция диапазона значений в ячейке граней круга
                            if ((int)_[1, a.RowIndex].Value < 3) _[1, a.RowIndex].Value = 3;
                            else if ((int)_[1, a.RowIndex].Value > 360) _[1, a.RowIndex].Value = 360;
                        } else if (a.RowIndex == 4) { //коррекция диапазона значений в ячейке толщины контура круга
                            if ((int)_[1, a.RowIndex].Value < 0) _[1, a.RowIndex].Value = 0;
                            else if ((int)_[1, a.RowIndex].Value > 25) _[1, a.RowIndex].Value = 25;
                        }
                    }
                };
                _.Click += (s, a) => { if (_.SelectedCells.Count > 0) { _.ClearSelection(); } };
                _.CellClick += (s, a) => {
                    if (pnl_Settings_gridView[a.ColumnIndex, a.RowIndex].Value == DBNull.Value) _.ClearSelection();
                    else _[a.ColumnIndex, a.RowIndex].Selected = true;
                    pnl_Settings_TextNameInformation.Text = ((string)_[0, a.RowIndex].Value).TrimStart();
                    pnl_Settings_TextInformation.Text = pnl_Settings_ArrayInformation[a.RowIndex];

                    pnl_Settings_TextInformation.Height = TextRenderer.MeasureText(pnl_Settings_TextInformation.Text, pnl_Settings_TextInformation.Font,
                        pnl_Settings_TextInformation.ClientSize, TextFormatFlags.WordBreak).Height + 10;
                    btn_RND_BackGround.Top = btn_PREV_BackGround.Top = btn_NEXT_BackGround.Top = pnl_Settings_PanelInformation.Bottom + 5;
                };

                pnl_Settings_PanelInformation = new Panel { Parent = pnl_Settings_MainPanel, 
                    Size = new Size(_.Width, 50), BorderStyle = BorderStyle.FixedSingle, 
                    BackColor = _.GridColor, AutoSize = true, Location = new Point(_.Left, _.Bottom + 5),
                };
                pnl_Settings_TextNameInformation = new Label { Parent = pnl_Settings_PanelInformation, Text = "",
                    Font = new Font(FName, FSize, FontStyle.Bold), AutoSize = true, 
                    ForeColor = Color.SkyBlue, BackColor = pnl_Settings_PanelInformation.BackColor,
                    Location = new Point(10, 10),
                };
                pnl_Settings_TextInformation = new Label { Parent = pnl_Settings_PanelInformation, Text = "",
                    Font = new Font(FName, FSize - 0, FontStyle.Regular),
                    ForeColor = Color.White, BackColor = pnl_Settings_PanelInformation.BackColor,
                    Size = new Size(pnl_Settings_PanelInformation.Width - 20, pnl_Settings_PanelInformation.Height - pnl_Settings_TextNameInformation.Height - 20),
                    Location = new Point(10, pnl_Settings_TextNameInformation.Bottom + 5),
                };
                btn_RND_BackGround.Left = 5; btn_PREV_BackGround.Left = btn_RND_BackGround.Right + 5; btn_NEXT_BackGround.Left = btn_PREV_BackGround.Right + 5;
                btn_RND_BackGround.Top = btn_PREV_BackGround.Top = btn_NEXT_BackGround.Top = pnl_Settings_PanelInformation.Bottom + 5;

                pnl_Settings_MainPanel.Click += (s, a) => { if (_.SelectedCells.Count > 0) { _[0, 0].Selected = true; _.ClearSelection(); } };
                pnl_Settings_PanelInformation.Click += (s, a) => { if (_.SelectedCells.Count > 0) { _[0, 0].Selected = true; _.ClearSelection(); } };
                pnl_Settings_TextNameInformation.Click += (s, a) => { if (_.SelectedCells.Count > 0) { _[0, 0].Selected = true; _.ClearSelection(); } };
            }
            pnl_Settings_gridView[0, 0].Selected = true; pnl_Settings_gridView.ClearSelection();
        }



        /// <summary> Окно настроек свойств <b>Particles[].Property</b> частицы. </summary>
        private Form Form_Property = null;
        /// <summary> Таблица отображения и ввода коэффициентов для полей свойства <b>Particles[].Property</b> частциы. </summary>
        private DataGridView Property_gridView = null;
        /// <summary> Массив кнопок: [0] "применить"; [1] "Значения по умолчанию" в окне <b>Form_Property</b>. </summary>
        private Button[] Property_Btn = null;
        /// <summary> Панель информации о выбранной строке в окне <b>Form_Property</b>. </summary>
        private Panel Property_PanelInformation = null;
        /// <summary> Текст названия ячейки на панели информации о выбранной строке в окне <b>Form_Property</b>. </summary>
        private Label Property_TextNameInformation = null;
        /// <summary> Текст описания на панели информации о выбранной строке в окне <b>Form_Property</b>. </summary>
        private Label Property_TextInformation = null;
        /// <summary> Хранит текст ячейки первого столбца в окне <b>Form_Property</b>, в котором произошла ошибка ввода данных. <br/> Если Property_SaveCellText = "", значит ошибки ввода нет. </summary>
        private string Property_SaveCellText = "";

        /// <summary> Метод создаёт и открывает диалоговое окно с настройками <b>Particles[].Property</b> частицы. </summary>
        public void WinDlg_Property() {
            if (Form_Property == null) {
                float FSize = 10; string FName = "Arial";
                Form_Property = Extensions.CreateForm("Form_Property", null, new Font(FName, FSize, FontStyle.Bold),
                    StartPosition: FormStartPosition.CenterScreen, FormBorderStyle: FormBorderStyle.FixedSingle,
                    Padding: new Padding(0, 0, 0, 10), ControlBox: true, KeyPreview: true, AutoSize: true);
                Form_Property.BackColor = Color.FromArgb(37, 37, 38);
                Form_Property.KeyUp += (s, a) => { if (a.KeyCode == Keys.Escape) { ((Form)s).Close(); } };
                Form_Property.FormClosing += (s, a) => { if (a.CloseReason == CloseReason.UserClosing) { a.Cancel = true; Form_Property.Hide(); } };

                Property_gridView = Extensions.CreateGrid(Form_Property, "Property_gridView", new Font(FName, FSize, FontStyle.Bold),
                    Location: new Point(5, 5), BackgroundColor: Form_Property.BackColor, GridColor: Color.FromArgb(60, 60, 66),
                    BorderStyle: BorderStyle.FixedSingle, Enabled: true);
                Property_gridView.Columns.Add("Text", "Свойство"); Property_gridView.Columns.Add("Number", "Значение");
                Property_gridView.DefaultCellStyle.BackColor = Form_Property.BackColor;
                Property_gridView.DefaultCellStyle.ForeColor = Color.White;
                Property_gridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                for (int i = 0; i < Property_gridView.Columns.Count; i++) {
                    if (i == 0) {
                        Property_gridView.Columns[i].DefaultCellStyle.Font = new Font(FName, FSize, FontStyle.Bold);
                        Property_gridView.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                        Property_gridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        Property_gridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        Property_gridView.Columns[i].ReadOnly = true;      
                    } else {
                        Property_gridView.Columns[i].DefaultCellStyle.Font = new Font(FName, FSize, FontStyle.Regular);
                        Property_gridView.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        Property_gridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        Property_gridView.Columns[i].ValueType = typeof(double);
                    }
                }
                Property_gridView.Rows.Add("  Лобовое сопротивление:  ", 0.4);
                Property_gridView.Rows.Add("  Толщина круга:  ", 0.3);
                Property_gridView.Rows.Add("  Коэффициент радиуса:  ", 0.005);
                //gridView.Rows.Add("  Плотность среды:  ", 1.22);
                //gridView.Rows.Add("  Вязкость среды:  ", 0.182);
                //gridView.Rows.Add("  Масштаб высоты:  ", 0.2);
                Property_gridView.Size = new Size(450, (Property_gridView.Rows.Count + 1) * Property_gridView.Font.Height);
                Property_gridView.DataError += (s, a) => {
                    Property_SaveCellText = (string)Property_gridView[0, a.RowIndex].Value;
                    Property_gridView[0, a.RowIndex].Value = "Ошибка ввода!";
                    Property_gridView[0, a.RowIndex].Style.ForeColor = Color.Red;
                    Property_gridView[0, a.RowIndex].Selected = false;
                };
                Property_gridView.CellEndEdit += (s, a) => {
                    if (Property_SaveCellText != "") { //была ошибка ввода
                        Property_gridView[0, a.RowIndex].Value = Property_SaveCellText; Property_SaveCellText = "";
                        Property_gridView[0, a.RowIndex].Style.ForeColor = Color.White;
                    } else { if (Property_gridView[1, a.RowIndex].Value == DBNull.Value) { Property_gridView[1, a.RowIndex].Value = 0; } }
                };
                Property_gridView.CellClick += (s, a) => {
                    Property_TextNameInformation.Text = (string)Property_gridView[0, a.RowIndex].Value;
                    switch (a.RowIndex) {
                        case 0: Property_TextInformation.Text = "Коэффициент лобового сопротивления формы частицы. " +
                            "Используется для расчётов инерционного сопротивления среды в симуляции.\n" +
                            "Капля = 0.04;  Круг = 0.4;  Полусфера = 0.42;  Шар = 0.47;  Конус = 0.5;" +
                            "  Куб = 1.05;  Цилиндр = 1.15;  диск = 1.18;  парашют = 1.28;";
                            break;
                        case 1: Property_TextInformation.Text = "Коэффициент высоты (толщины) круга.\n" +
                            "Используется для вычисления силы Архимеда в симуляции среды.";
                            break;
                        case 2: Property_TextInformation.Text = "Коэффициент радиуса круга.\n" +
                            "Используется для коррекции действительного радиуса круга в псевдо-радиус для расчётов симуляции среды.";
                            break;
                        default: Property_TextNameInformation.Text = Property_TextInformation.Text = ""; break;
                    }
                    Property_TextInformation.Height = TextRenderer.MeasureText(Property_TextInformation.Text, Property_TextInformation.Font,
                        Property_TextInformation.ClientSize, TextFormatFlags.WordBreak).Height + 10;
                    Property_Btn[0].Top = Property_Btn[1].Top = Property_PanelInformation.Bottom + 5;
                };

                Property_PanelInformation = new Panel { Parent = Form_Property, 
                    Size = new Size(Property_gridView.Width, 100), BorderStyle = BorderStyle.FixedSingle, 
                    Location = new Point(Property_gridView.Left, Property_gridView.Bottom + 5),
                    BackColor = Property_gridView.GridColor, AutoSize = true,
                };
                Property_TextNameInformation = new Label { Parent = Property_PanelInformation, Text = "",
                    Font = new Font(FName, FSize, FontStyle.Bold), AutoSize = true,
                    ForeColor = Color.SkyBlue, BackColor = Property_PanelInformation.BackColor,
                    Location = new Point(0, 10),
                };
                Property_TextInformation = new Label { Parent = Property_PanelInformation, Text = "",
                    Font = new Font(FName, FSize - 0, FontStyle.Regular),
                    ForeColor = Color.White, BackColor = Property_PanelInformation.BackColor,
                    Size = new Size(Property_PanelInformation.Width - 20, Property_PanelInformation.Height - Property_TextNameInformation.Height - 20),
                    Location = new Point(10, Property_TextNameInformation.Bottom + 5),
                };

                Property_Btn = new Button[2];
                for (int i = 0; i < Property_Btn.Length; i++) {
                    Property_Btn[i] = new Button { Parent = Form_Property, Font = new Font(FName, FSize, FontStyle.Regular),
                        AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Padding = new Padding(10, 0, 10, 0),
                        ForeColor = Color.White, BackColor = Property_gridView.GridColor, FlatStyle = FlatStyle.Popup, 
                    };
                    if (i == 0) { Property_Btn[i].Text = "Применить";
                        Property_Btn[i].Location = new Point(Property_PanelInformation.Right - Property_Btn[i].Width, Property_PanelInformation.Bottom + 5);
                    } else {
                        Property_Btn[i].Text = "Значения по умолчанию";
                        Property_Btn[i].Location = new Point(5, Property_PanelInformation.Bottom + 5);
                    }
                }
                Property_Btn[0].Click += (s, a) => { //Применить
                    for (int i = 0; i < SIMULATION.Particles.Count; i++) {
                        SIMULATION.setParticleProperty(SIMULATION.Particles[i], (double)Property_gridView[1, 0].Value,
                            (double)Property_gridView[1, 1].Value, (double)Property_gridView[1, 2].Value);
                    }
                    MessageBox.Show($"Изменения вступили в силу для {SIMULATION.Particles.Count} кругов.");
                };
                Property_Btn[1].Click += (s, a) => { //Значения по умолчанию
                    for (int i = 0; i < SIMULATION.Particles.Count; i++) { SIMULATION.setParticleProperty(SIMULATION.Particles[i]); }
                    if (SIMULATION.Particles.Count > 0) {
                        Property_gridView[1, 0].Value = SIMULATION.Particles[0].Property.cd;
                        Property_gridView[1, 1].Value = SIMULATION.Particles[0].Property.h;
                        Property_gridView[1, 2].Value = SIMULATION.Particles[0].Property.Kr;
                    }
                };
            }

            Form_Property.Text = $"Окно настроек свойств Property частиц / {Form_Property.Width}x{Form_Property.Height} px";
            if (!Form_Property.Visible) Form_Property.Show(); else Form_Property.BringToFront();
            Property_gridView[0, 0].Selected = false;
        }
    }
}
