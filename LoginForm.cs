using System;
using System.Windows.Forms;

public partial class LoginForm : Form
{
    private TextBox txtEmail;
    private TextBox txtPassword;
    private Button btnLogin;
    private CheckBox chkHasPartners;
    private TextBox txtPartners;

    public LoginForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()


    {
        chkHasPartners = new CheckBox { Text = "I have partners", Left = 20, Top = 110, Width = 200 };
        chkHasPartners.CheckedChanged += (s, e) =>
        {
            txtPartners.Visible = chkHasPartners.Checked;
        };

        txtPartners = new TextBox { Left = 20, Top = 140, Width = 230, Height = 60, Multiline = true, Visible = false };

        this.Controls.Add(chkHasPartners);
        this.Controls.Add(txtPartners);

        this.Text = "Login";
        this.ClientSize = new System.Drawing.Size(300, 200);

        Label lblEmail = new Label { Text = "Email:", Left = 20, Top = 30, Width = 80 };
        txtEmail = new TextBox { Left = 100, Top = 30, Width = 150 };

        Label lblPassword = new Label { Text = "App Password:", Left = 20, Top = 70, Width = 80 };
        txtPassword = new TextBox { Left = 100, Top = 70, Width = 150, UseSystemPasswordChar = true };

        btnLogin = new Button { Text = "Login", Left = 100, Top = 120, Width = 100 };
        btnLogin.Click += BtnLogin_Click;

        this.Controls.Add(lblEmail);
        this.Controls.Add(txtEmail);
        this.Controls.Add(lblPassword);
        this.Controls.Add(txtPassword);
        this.Controls.Add(btnLogin);
    }

    private void BtnLogin_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
        {
            MessageBox.Show("Please enter both email and password.");
            return;
        }

        
        Appsettings.Current = new Appsettings
        {
            SenderEmail = txtEmail.Text,
            SenderPassword = txtPassword.Text,
            RecipientEmail = txtEmail.Text, // ο ίδιος θα λαμβάνει
            PartnerEmails = chkHasPartners.Checked
                ? txtPartners.Text.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                : null
        };

     
        Appsettings.Save();

        this.DialogResult = DialogResult.OK;
        this.Close();
    }

}
