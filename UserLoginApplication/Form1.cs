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
        public struct ElementSize
        {
            public int Width;
            public int Height;
        };

        public struct ElementPosition
        {
            public int X;
            public int Y;

        };
        ElementPosition position;
        ElementSize elementSize;
        ElementSize buttonSize;
        ElementSize dataGridViewSize;

        private const int NumberOfGridViewColumns = 3;
        private const int _margin = 30;

        public DataGridView userDataGridView;
        private Dictionary<TextBoxName, TextBox> _textBoxes = [];

        public Form1()
        {
            InitializeComponent();
            position.X = 30;
            position.Y = 200;
            elementSize.Width = 270;
            elementSize.Height = 30;
            buttonSize.Width = elementSize.Width;
            buttonSize.Height = 50;
            dataGridViewSize.Width = 800;
            dataGridViewSize.Height = 180;

            saveButton = new Button
            {
                Text = "Save",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Location = new Point(50, 390),
                Size = new Size(elementSize.Width, elementSize.ButtonHeight),
                BackColor = Color.LightCyan,
            };

            userDataGridView = CreateDataGridView(Color.Red, dataGridViewSize);
            CreateElements();

            saveButton.Click += SaveButton_Click;
        }

        private void CreateElements()
        {
            int i = 0;

            Controls.Add(userDataGridView);

            //user name
            Controls.Add(CreateLabel("User Name",
                                     position.X,
                                     position.Y + (i++) * (_margin)));
            _textBoxes[TextBoxName.UserName] = CreateTextBox(position.X, position.Y + (i++) * (_margin));
            Controls.Add(_textBoxes[TextBoxName.UserName]);

            //password
            Controls.Add(CreateLabel("Password",
                                     position.X,
                                     position.Y + (i++) * (_margin)));
            _textBoxes[TextBoxName.Password] = CreateTextBox(position.X, position.Y + (i++) * (_margin));
            _textBoxes[TextBoxName.Password].UseSystemPasswordChar = true;
            _textBoxes[TextBoxName.Password].PasswordChar = '*';
            Controls.Add(_textBoxes[TextBoxName.Password]);

            //date of create account
            Controls.Add(CreateLabel("Date Of Create Account (dd, mm, yyyy)",
                                     position.X,
                                     position.Y + (i++) * (_margin)));
            _textBoxes[TextBoxName.DateOfCreateAccount] = CreateTextBox(position.X, position.Y + (i++) * (_margin));
            Controls.Add(_textBoxes[TextBoxName.DateOfCreateAccount]);

            Controls.Add(saveButton);
        }

        public DataGridView CreateDataGridView(Color backColor, ElementSize dataGridViewSize)
        {
            int i;

            DataGridView dataGridView = new DataGridView
            {
                Dock = DockStyle.Top,
                Size = new Size(dataGridViewSize.Width, dataGridViewSize.Height),
                CellBorderStyle = DataGridViewCellBorderStyle.Single,
                GridColor = backColor,
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

        public Label CreateLabel(string text, int xPosition, int yPosition)
        {
            Label label = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 11F),
                Size = new Size(elementSize.Width, elementSize.Height),
                Location = new Point(xPosition, yPosition),
                AutoSize = false
            };
            return label;
        }

        public TextBox CreateTextBox(int xPosition, int yPosition)
        {
            TextBox textBox = new TextBox
            {
                Font = new Font("Segoe UI", 12F),
                Location = new Point(xPosition, yPosition),
                Size = new Size(elementSize.Width, elementSize.Height),
                Margin = new Padding(100)
            };
            return textBox;
        }

        public Button saveButton;

        private void SaveButton_Click(object? sender, EventArgs e) //object? because it can be null in C# 8.0 
        {
            if(IsValidTextInputs(_textBoxes))
            {
                userDataGridView.Rows.Add(_textBoxes[TextBoxName.UserName].Text,
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
