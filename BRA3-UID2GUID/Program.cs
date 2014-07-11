using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BRA3_UID2GUID
{
    class Program
    {
        private static string bansFile;
        private static string banReason;
        static void Main(string[] args)
        {
            Console.Title = "[Steam64] UID2GUID (ArmA3) :: @Pwnoz0r";
            if (args.Length != 1)
            {
                Console.WriteLine("Please drag a bans (text) file on this executable.");
                finished();
            }
            else
            {
                bansFile = args[0];
                Console.WriteLine("Please remove all duplicates in the original bans file.\n");
                Console.WriteLine("Reason for the ban. (eg. Banned by Battle Royale Games)\n");
                banReason = Console.ReadLine();
                Console.WriteLine("\n");
                parseBans();
            }
        }

        private static void parseBans()
        {
            int counter = 0;
            string line;

            try
            {
                StreamReader sr = new StreamReader(bansFile);
                while ((line = sr.ReadLine()) != null)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        string newLine = line.Replace("//", ":").Split(':')[0].Trim().Replace("\r", "").Replace("\n", "").Replace("\r\n", "");
                        if (!string.IsNullOrWhiteSpace(newLine))
                        {
                            Int64 newLine64;
                            Int64.TryParse(newLine, out newLine64);
                            Console.WriteLine(convertUID(newLine64, banReason).Replace("\n", ""));
                        }
                        counter++;
                    }
                }
                sr.Close();
                finished();
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); finished(); }
        }

        private static string convertUID(Int64 uid, string reason)
        {
            Int64 steamID = uid;
            byte[] parts = { 0x42, 0x45, 0, 0, 0, 0, 0, 0, 0, 0 };
            byte counter = 2;

            do
            {
                parts[counter++] = (byte)(steamID & 0xFF);
            } while ((steamID >>= 8) > 0);

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] beHash = md5.ComputeHash(parts);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < beHash.Length; i++)
            {
                sb.Append(beHash[i].ToString("x2"));
            }

            string genString = sb.ToString() + " -1 : " + reason + "\n";
            logBans(genString);

            return genString;
        }

        private static int tryTest = 0;

        private static string logBans(string ban)
        {
            string newLog = bansFile.Replace(".txt", "") + "-guid-" + getTime() + ".txt";
            if (tryTest == 0)
            {
                tryTest = 1;
                if (File.Exists(newLog))
                {
                    File.Delete(newLog);
                }
            }
            else
            {
                using (StreamWriter sw = new StreamWriter(newLog, true))
                {
                    sw.Write(ban);
                }
            }
            return ban;
        }

        private static string getTime()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        private static void finished()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}
