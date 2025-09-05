using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace KanbanApp
{
    public class TaskItem
    {
        public string Title { get; set; }
        public string Status { get; set; } // To Do / Doing / Done
        public string Priority { get; set; } // High / Medium / Low
        public DateTime Deadline { get; set; }
        public bool NotifyByEmail { get; set; }
    }

    public class Form1 : Form
    {
        private List<TaskItem> tasks = new List<TaskItem>();
        private string saveFile = "tasks.json";

        private TextBox txtTitle;
        private DateTimePicker dtpDeadline;
        private ComboBox cmbPriority;
        private CheckBox chkNotify;
        private Button btnAdd, btnSave, btnLoad;

        private FlowLayoutPanel pnlToDoTasks;
        private FlowLayoutPanel pnlDoingTasks;
        private FlowLayoutPanel pnlDoneTasks;

        public Form1()
        {
            BuildUI();
            LoadTasks();
            RefreshBoard();
        }

        private void BuildUI()
        {
            this.Text = "Kanban Board";
            this.Size = new Size(1000, 600);

            // Top input controls
            txtTitle = new TextBox { Left = 10, Top = 10, Width = 200 };
            dtpDeadline = new DateTimePicker { Left = 220, Top = 10, Width = 150 };
            cmbPriority = new ComboBox { Left = 380, Top = 10, Width = 100 };
            cmbPriority.Items.AddRange(new string[] { "High", "Medium", "Low" });
            chkNotify = new CheckBox { Left = 490, Top = 12, Text = "Notify by Email" };
            btnAdd = new Button { Left = 600, Top = 10, Text = "Add Task" };
            btnAdd.Click += BtnAdd_Click;

            btnSave = new Button { Left = 700, Top = 10, Text = "Save" };
            btnSave.Click += BtnSave_Click;
            btnLoad = new Button { Left = 780, Top = 10, Text = "Load" };
            btnLoad.Click += BtnLoad_Click;

            this.Controls.AddRange(new Control[] { txtTitle, dtpDeadline, cmbPriority, chkNotify, btnAdd, btnSave, btnLoad });

            // ===== Columns =====
            CreateColumn("To Do", 10, out pnlToDoTasks);
            CreateColumn("Doing", 340, out pnlDoingTasks);
            CreateColumn("Done", 670, out pnlDoneTasks);
        }

        private void CreateColumn(string title, int left, out FlowLayoutPanel taskPanel)
        {
            Panel outer = new Panel { Left = left, Top = 50, Width = 300, Height = 500, BorderStyle = BorderStyle.FixedSingle };

            Label lbl = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Height = 30,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter
            };

            taskPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                AllowDrop = true
            };

            taskPanel.DragEnter += Panel_DragEnter;
            taskPanel.DragDrop += Panel_DragDrop;

            outer.Controls.Add(taskPanel);
            outer.Controls.Add(lbl);

            this.Controls.Add(outer);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text)) return;

            TaskItem task = new TaskItem
            {
                Title = txtTitle.Text,
                Deadline = dtpDeadline.Value,
                Priority = cmbPriority.Text,
                Status = "To Do",
                NotifyByEmail = chkNotify.Checked
            };
            tasks.Add(task);
            RefreshBoard();
        }

        private void RefreshBoard()
        {
            pnlToDoTasks.Controls.Clear();
            pnlDoingTasks.Controls.Clear();
            pnlDoneTasks.Controls.Clear();

            foreach (var task in tasks)
            {
                Button card = new Button
                {
                    Text = $"{task.Title}\nDue: {task.Deadline.ToShortDateString()}",
                    Width = 280,
                    Height = 60,
                    BackColor = GetPriorityColor(task.Priority),
                    Tag = task
                };
                card.MouseDown += Card_MouseDown;

                switch (task.Status)
                {
                    case "To Do": pnlToDoTasks.Controls.Add(card); break;
                    case "Doing": pnlDoingTasks.Controls.Add(card); break;
                    case "Done": pnlDoneTasks.Controls.Add(card); break;
                }
            }
        }

        /*  private Color GetPriorityColor(string priority) => priority switch
          {
              "High" => Color.LightCoral,
              "Medium" => Color.Khaki,
              "Low" => Color.LightGreen,
              _ => Color.LightGray,
          }; αυτο αλλαζει σε απο κατω */
        private Color GetPriorityColor(string priority)
        {
            switch (priority)
            {
                case "High":
                    return Color.LightCoral;
                case "Medium":
                    return Color.Khaki;
                case "Low":
                    return Color.LightGreen;
                default:
                    return Color.LightGray;
            }
        }

        private void Card_MouseDown(object sender, MouseEventArgs e)
        {
            Button card = sender as Button;
            if (card != null)
                DoDragDrop(card, DragDropEffects.Move);
        }

        private void Panel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Button)))
                e.Effect = DragDropEffects.Move;
        }

        private void Panel_DragDrop(object sender, DragEventArgs e)
        {
            Button card = e.Data.GetData(typeof(Button)) as Button;
            FlowLayoutPanel targetPanel = sender as FlowLayoutPanel;
            if (card == null || targetPanel == null) return;

            FlowLayoutPanel oldParent = card.Parent as FlowLayoutPanel;
            oldParent.Controls.Remove(card);
            targetPanel.Controls.Add(card);

            TaskItem task = card.Tag as TaskItem;
            if (task != null)
            {
                if (targetPanel == pnlToDoTasks) task.Status = "To Do";
                else if (targetPanel == pnlDoingTasks) task.Status = "Doing";
                else if (targetPanel == pnlDoneTasks) task.Status = "Done";
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            string json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(saveFile, json);
            MessageBox.Show("Tasks saved!");
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            LoadTasks();
            RefreshBoard();
        }

        private void LoadTasks()
        {
            if (File.Exists(saveFile))
            {
                string json = File.ReadAllText(saveFile);
                tasks = JsonSerializer.Deserialize<List<TaskItem>>(json);
            }
        }
    }
}
