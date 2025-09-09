using System;
using System.IO;
using System.Text.Json;

public class Appsettings
{
    public static Appsettings Current { get; set; }

    public string SenderEmail { get; set; }
    public string SenderPassword { get; set; }
    public string RecipientEmail { get; set; }
    public string[] PartnerEmails { get; set; }

   
    private static string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logins");
    private static string filePath = Path.Combine(folderPath, "settings.json");

    public static void Save()
    {
        if (Current != null)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath); 
            }

            string json = JsonSerializer.Serialize(Current, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }
    }

    public static void Load()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            Current = JsonSerializer.Deserialize<Appsettings>(json);
        }
        else
        {
            Current = new Appsettings();
        }
    }
}
