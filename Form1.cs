using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Windows.Forms;

namespace UserLoginApplication
{
    public partial class Form1 : Form
    {
        
        private enum TextBoxName
        {
            UserName,
            Password,
            DateOfCreateAccount
        }
        public struct ElementSize(int width, int height)
        {
            public int Height = height;
            public int Width = width;
        };

        public struct ElementPosition(int xPosition, int yPosition)
        {
            public int X = xPosition;
            public int Y = yPosition;

        };

        ElementPosition position;
        ElementSize elementSize;
        ElementSize buttonSize;
        ElementSize dataGridViewSize;

        private const int NumberOfGridViewColumns = 3;
        private int currentY;
        private bool isTextBox;

        private DataGridView? userDataGridView;
        private Button? saveButton;
        private Dictionary<TextBoxName, TextBox> _textBoxes = [];

        public Form1()
        {
            InitializeComponent();
            position = new ElementPosition(30, 190);
            elementSize = new ElementSize(270, 30);
            buttonSize = new ElementSize(270, 50);
            dataGridViewSize = new ElementSize(800, 180);

            CreateElements();

            saveButton!.Click += SaveButton_Click;

            this.Text = "User Login Application";
            this.Icon = Resource1.userLoginApp;
        }

        private void CreateElements()
        {
            isTextBox = false;
            currentY = 20;

            userDataGridView = CreateDataGridView(dataGridViewSize, Color.WhiteSmoke);
            AddControl(userDataGridView);

            //user name
            AddControl(CreateLabel("User Name"));
            _textBoxes[TextBoxName.UserName] = CreateTextBox();
            AddControl(_textBoxes[TextBoxName.UserName], isTextBox = true);

            //password
            AddControl(CreateLabel("Password"));

            _textBoxes[TextBoxName.Password] = CreateTextBox();
            _textBoxes[TextBoxName.Password].UseSystemPasswordChar = true;
            _textBoxes[TextBoxName.Password].PasswordChar = '*';
            AddControl(_textBoxes[TextBoxName.Password], isTextBox = true);

            //date of create account
            AddControl(CreateLabel("Date Of Create Account (dd, mm, yyyy)"));

            _textBoxes[TextBoxName.DateOfCreateAccount] = CreateTextBox();
            AddControl(_textBoxes[TextBoxName.DateOfCreateAccount], isTextBox = true);

            //save the data button
            saveButton = CreateButton("Save", Color.LightCyan);
            AddControl(saveButton);
        }

        private void AddControl(Control control, bool isTextBox = false, int spacing = 4)
        {
            control.Location = new Point(position.X, currentY);
            Controls.Add(control);

            currentY += control.Height + spacing;
            if (isTextBox)
            {
                currentY += 10;
            }
        }

        public DataGridView CreateDataGridView(ElementSize dataGridViewSize, Color backColor)
        {
            int i;

            DataGridView dataGridView = new()
            {
                Dock = DockStyle.Top,
                Size = new Size(dataGridViewSize.Width, dataGridViewSize.Height),
                CellBorderStyle = DataGridViewCellBorderStyle.Single,
                BackgroundColor = backColor,
                ScrollBars = ScrollBars.Both,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
            };

            dataGridView.Columns.Add("userName", "User Name");
            dataGridView.Columns.Add("password", "Password");
            dataGridView.Columns.Add("dateOfCreateAccount", "Date Of Create Account");

            for (i = 0; i < NumberOfGridViewColumns; i++)
            {
                dataGridView.Columns[i].DefaultCellStyle.Font = new Font("Times New Roman", 11F);

                dataGridView.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView.Columns[i].Width = (int)(200 * (i + 0.5));
            }
            return dataGridView;
        }

        public Button CreateButton(string text, Color color)
        {
            Button button = new()
            {
                Text = text,
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                Size = new Size(buttonSize.Width, buttonSize.Height),
                BackColor = color
            };
            return button;
        }

        public Label CreateLabel(string text)
        {
            Label label = new()
            {
                Text = text,
                Font = new Font("Segoe UI", 11F),
                Size = new Size(elementSize.Width, elementSize.Height),
                AutoSize = false
            };
            return label;
        }

        public TextBox CreateTextBox()
        {
            TextBox textBox = new()
            {
                Font = new Font("Segoe UI", 12F),
                Size = new Size(elementSize.Width, elementSize.Height),
            };
            return textBox;
        }

        private void SaveButton_Click(object? sender, EventArgs e) //object? because it can be null in C# 8.0 
        {
            if(IsValidTextInputs(_textBoxes))
            {
                userDataGridView!.Rows.Add(_textBoxes[TextBoxName.UserName].Text,
                                          _textBoxes[TextBoxName.Password].Text,
                                          _textBoxes[TextBoxName.DateOfCreateAccount].Text);
                ClearTextBoxes();
                ShowInfoMessage("Data saved saved successfully!");
            }
        }

        private bool IsValidTextInputs(Dictionary<TextBoxName, TextBox> textBoxes)
        {
            string userName = textBoxes[TextBoxName.UserName].Text.Trim();
            string password = textBoxes[TextBoxName.Password].Text.Trim();
            string dateOfCreateAccount = textBoxes[TextBoxName.DateOfCreateAccount].Text.Trim();

            if (string.IsNullOrWhiteSpace(userName) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(dateOfCreateAccount))
            {
                ShowErrorMessage("Please fill in all fields.");
                return false;
            }

            if (!IsValidDate(dateOfCreateAccount))
            {
                ShowErrorMessage("Please enter a valid date in the format dd, mm, yyyy.");
                return false;
            }

            if (!IsValidPassword(password))
            {
                ShowErrorMessage("Password must be at least 8 characters long and include at least one uppercase letter, one lowercase letter, and one digit.");
                return false;
            }
            return true;
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ShowInfoMessage(string message)
        {
            MessageBox.Show(message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool IsValidDate(string date)
        {
            return DateTime.TryParseExact(date, 
                new[] { "dd, MM, yyyy", "dd,MM,yyyy", "dd/MM/yyyy", "dd-MM-yyyy" }, 
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _);
        }

        private bool IsValidPassword(string password)
        {
            return password.Length >= 8 &&
                   password.Any(char.IsUpper) &&
                   password.Any(char.IsLower) &&
                   password.Any(char.IsDigit);
        }

        private void ClearTextBoxes()
        {
            foreach (var textBox in _textBoxes.Values)
            {
                textBox.Clear();
            }
        }
    }
}
