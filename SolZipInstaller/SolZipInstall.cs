using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Windows.Forms;
using System.Collections;

namespace SolZip
{
    [RunInstaller(true)]
    public class SolZipInstall : Installer
    {
        protected override void OnAfterInstall(System.Collections.IDictionary savedState)
        {
            base.OnAfterInstall(savedState);
            if (Context.Parameters.ContainsKey("assemblypath"))
            {
                AppendToEnvironmentPath(Path.GetDirectoryName(Context.Parameters["assemblypath"]));
            }
        }

        private void AppendToEnvironmentPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            EnvironmentVariableTarget target;
            if (Context.Parameters.ContainsKey("level") && Context.Parameters["level"] == "User")
            {
                target = EnvironmentVariableTarget.User;
            }
            else
            {
                target = EnvironmentVariableTarget.Machine;
            }

            string existingPath = Environment.GetEnvironmentVariable("Path", target);
            if (!string.IsNullOrEmpty(existingPath) && !string.IsNullOrEmpty(path) && !existingPath.Contains(path))
            {
                path = existingPath + ";" + path;
            }
            Environment.SetEnvironmentVariable("Path", path, target);
        }
    }
}
