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
            //foreach (string s in Context.Parameters.Keys)
            //{
            //    MessageBox.Show(s + ": " + Context.Parameters[s]);
            //}
            //foreach (object o in savedState.Keys)
            //{
            //    MessageBox.Show(o.ToString() + ": " + savedState[o].ToString() );
            //}
            //foreach (IDictionary d in (IDictionary[])savedState["_reserved_nestedSavedStates"])
            //{
            //    foreach (object o in d.Keys)
            //    {
            //        MessageBox.Show(o.ToString() + ": " + d[o].ToString());
            //    }
            //}

            base.OnAfterInstall(savedState);
            if (Context.Parameters.ContainsKey("assemblypath"))
            {
                AppendToEnvironmentPath(Path.GetDirectoryName(Context.Parameters["assemblypath"]));
            }
        }

        //public override void Install(IDictionary stateSaver)
        //{
        //    foreach (object o in stateSaver.Keys)
        //    {
        //        MessageBox.Show(o.ToString() + ": " + stateSaver[o].ToString());
        //    }
        //    base.Install(stateSaver);
        //    foreach (object o in stateSaver.Keys)
        //    {
        //        MessageBox.Show(o.ToString() + ": " + stateSaver[o].ToString());
        //    }
        //}

        private void AppendToEnvironmentPath(string path)
        {
            //MessageBox.Show("Called: " + path);
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
                //MessageBox.Show("1: " + path);
            }
            Environment.SetEnvironmentVariable("Path", path, target);
            //MessageBox.Show("2: " + path);
        }
    }
}
