using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Exoskeleton.Classes
{
    public class ExoAppLaunchInfo
    {
        public string SettingsPath { get; set; } = "";
        public string ShortName { get; set; } = "";
        public string Description { get; set; } = "";
        public string IconPath { get; set; } = "";
        public DateTime LastRun { get; set; } = DateTime.MinValue;
    }

    public class GlobalSettings
    {
        public bool EnableLaunchHistory { get; set; } = true;

        public List<ExoAppLaunchInfo> AppHistory { get; set; } = new List<ExoAppLaunchInfo>();

        public void Save(string filename)
        {
            XmlSerializer s = new XmlSerializer(this.GetType());

            using (TextWriter w = new StreamWriter(filename))
            {
                s.Serialize(w, this);
                w.Close();
            }
        }

        public static GlobalSettings Load(string filename)
        {
            GlobalSettings globalSettings = new GlobalSettings();

            if (File.Exists(filename))
            {
                XmlSerializer s = new XmlSerializer(typeof(GlobalSettings));
                TextReader r = new StreamReader(filename);
                try
                {
                    globalSettings = (GlobalSettings)s.Deserialize(r);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error Loading global settings", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                }
                finally
                {
                    r.Close();
                }
            }
            else
            {
                try
                {
                    globalSettings.Save(filename);
                }
                catch { }
            }

            return globalSettings;
        }

        public void AddLaunchHistory(string settingsPath, string shortName, string description, string iconPath)
        {
            if (!this.EnableLaunchHistory) return;

            ExoAppLaunchInfo ali = (from li in AppHistory where li.SettingsPath == settingsPath select li).FirstOrDefault();

            if (ali != null)
            {
                ali.ShortName = shortName;
                ali.Description = description;
                ali.IconPath = iconPath;
                ali.LastRun = DateTime.Now;

                return;
            }

            AppHistory.Add(
                new ExoAppLaunchInfo()
                {
                    SettingsPath = settingsPath,
                    ShortName = shortName,
                    Description = description,
                    IconPath = iconPath,
                    LastRun = DateTime.Now
                }
            );
        }
    }
}
