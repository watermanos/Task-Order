# 📝 Task Order Board in C# WinForms

This project is a simple desktop **Kanban Board** built with **C# (WinForms)**.  
It allows you to create, organize, and save tasks in 3 columns:

- **To Do**
- **Doing**
- **Done**

Each task supports:
- Title and description
- Deadline
- Priority (High / Medium / Low)
- Optional email reminder

---

## ⚙️ Features

- Add tasks with title, description, deadline, and priority.
- Drag & Drop tasks between columns.
- Save all tasks into a local file (`tasks.json`).
- Automatically load tasks when the application starts.
- Send email reminders for tasks with deadlines.

---

## 📧 Email Configuration

Email reminders are sent through **Gmail SMTP**.  
⚠️ The sender and password must be configured directly in the code (`EmailHelper.cs`):

```csharp
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
