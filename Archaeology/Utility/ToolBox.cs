using System.Text;

namespace AutoPartyadak.Utility
{
    public class ToolBox
    {
        public static void SaveLog(object log)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{log.ToString()}");
            sb.AppendLine(DateTime.Now.ToShortTimeString());
            sb.AppendLine($"--------------------------------");
            System.IO.File.AppendAllText(Path.Combine(Directory.GetCurrentDirectory(),
                "wwwroot",
                "log.txt"), sb.ToString());
        }
    }
}