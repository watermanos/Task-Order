using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;
public static class Program
{
    public class TaskItem
    {
        public string Title { get; set; }
        public DateTime Deadline { get; set; }
        public string Status { get; set; } // To Do / Doing / Done
        public bool NotifyByEmail { get; set; }
    }

    public class KanbanForm : Form
    {
        private FlowLayoutPanel panelToDo, panelDoing, panelDone;
        private TextBox txtTaskTitle, txtUserEmail;
        private DateTimePicker dtpDeadline;
        private CheckBox chkNotify;
        private Button btnAddTask;
        private List<TaskItem> tasks = new List<TaskItem>();

        public KanbanForm()
        {
            BuildUI();
        }

        private void BuildUI()
        {
            this.Text = "Kanban Board";
            this.Size = new Size(1000, 600);

            // ===== Top bar για Email και Task input =====
            Label lblEmail = new Label { Text = "Your Email:", AutoSize = true, Left = 10, Top = 15 };
            txtUserEmail = new TextBox { Left = 100, Top = 10, Width = 200 };

            txtTaskTitle = new TextBox { Left = 320, Top = 10, Width = 200, Text = "Task Title" };
            dtpDeadline = new DateTimePicker { Left = 530, Top = 10, Width = 150 };
            chkNotify = new CheckBox { Text = "Notify by Email", Left = 690, Top = 12 };
            btnAddTask = new Button { Text = "Add Task", Left = 820, Top = 10 };

            btnAddTask.Click += BtnAddTask_Click;

            this.Controls.Add(lblEmail);
            this.Controls.Add(txtUserEmail);
            this.Controls.Add(txtTaskTitle);
            this.Controls.Add(dtpDeadline);
            this.Controls.Add(chkNotify);
            this.Controls.Add(btnAddTask);

            // ===== Columns =====
            panelToDo = CreateColumn("To Do", 10);
            panelDoing = CreateColumn("Doing", 330);
            panelDone = CreateColumn("Done", 650);

            this.Controls.Add(panelToDo);
            this.Controls.Add(panelDoing);
            this.Controls.Add(panelDone);
        }

        private FlowLayoutPanel CreateColumn(string title, int left)
        {
            Label lbl = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 30
            };

            FlowLayoutPanel panel = new FlowLayoutPanel
            {
                Left = left,
                Top = 50,
                Width = 300,
                Height = 500,
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = true,
                AllowDrop = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };

            panel.Controls.Add(lbl);

            panel.DragEnter += (s, e) => { e.Effect = DragDropEffects.Move; };
            panel.DragDrop += (s, e) =>
            {
                Control card = (Control)e.Data.GetData(typeof(Control));
                panel.Controls.Add(card);
            };

            return panel;
        }

        private void BtnAddTask_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTaskTitle.Text)) return;

            var task = new TaskItem
            {
                Title = txtTaskTitle.Text,
                Deadline = dtpDeadline.Value,
                Status = "To Do",
                NotifyByEmail = chkNotify.Checked
            };
            tasks.Add(task);

            Panel card = new Panel
            {
                Width = 250,
                Height = 60,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lbl = new Label
            {
                Text = $"{task.Title}\nDue: {task.Deadline.ToShortDateString()}",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            card.Controls.Add(lbl);

            // Drag and Drop για κάρτες
            card.MouseDown += (s, ev) =>
            {
                card.DoDragDrop(card, DragDropEffects.Move);
            };

            panelToDo.Controls.Add(card);

            txtTaskTitle.Clear();
        }

        // ===== Αποστολή Email (για reminders) =====
        private void SendEmail(string to, string subject, string body)
        {
            try
            {
                var mail = new MailMessage();
                mail.From = new MailAddress("your.email@gmail.com");
                mail.To.Add(to);
                mail.Subject = subject;
                mail.Body = body;

                var smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("your.email@gmail.com", "your-app-password"),
                    EnableSsl = true
                };
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Email error: " + ex.Message);
            }
        }
    }


}
