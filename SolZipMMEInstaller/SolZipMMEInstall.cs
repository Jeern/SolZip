using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Configuration.Install;
using Microsoft.Win32;
using System.IO;
using System.Windows.Forms;
using System.Collections;

namespace SolZipMMEInstaller
{
    [RunInstaller(true)]
    public class SolZipMMEInstall : Installer
    {

        private const string AddInRegKey = @"Software\Jern\MME";
        private const string AddInRegKey64 = @"Software\Wow6432Node\Jern\MME";
        private const string AddInRegValueKey = "AddInMainDirectory";
        private const string TargetAssemblyKey = "assemblypath";
        private const string SearchString = "*.dll";
        private const string ExcludeString1 = "Installer.dll";
        private const string ArrayKey = "NewFiles";

        protected override void OnAfterInstall(IDictionary savedState)
        {
            base.OnAfterInstall(savedState);
            if (!string.IsNullOrEmpty(AddInDirectory) && Context.Parameters.ContainsKey(TargetAssemblyKey)
                && !string.IsNullOrEmpty(Context.Parameters[TargetAssemblyKey]))
            {
                string copyTo = AddInDirectory;
                if (!Directory.Exists(copyTo))
                {
                    Directory.CreateDirectory(copyTo);
                }
                string targetAssembly = Context.Parameters[TargetAssemblyKey];
                if (!string.IsNullOrEmpty(targetAssembly))
                {
                    string copyFrom = Path.GetDirectoryName(targetAssembly);
                    if (Directory.Exists(copyFrom))
                    {
                        string[] files =
                            Directory.GetFiles(copyFrom, SearchString, SearchOption.TopDirectoryOnly)
                                .Where(f => (!f.EndsWith(ExcludeString1))).ToArray();

                        string[] newFiles = new string[files.Length];
                        for (int i = 0; i < files.Length; i++)
                        {
                            try
                            {
                                string newFile = Path.Combine(copyTo, Path.GetFileName(files[i]));
                                File.Copy(files[i], newFile, true);
                                newFiles[i] = newFile;
                            }
                            catch { }
                        }
                        savedState.Add(ArrayKey, newFiles);
                    }
                }
            }
        }

        protected override void OnBeforeUninstall(System.Collections.IDictionary savedState)
        {
            base.OnBeforeUninstall(savedState);
            if (savedState.Contains(ArrayKey))
            {
                string[] files = savedState[ArrayKey] as string[];
                if (files != null)
                {
                    foreach (string file in files)
                    {
                        try
                        {
                            File.Delete(file);
                        }
                        catch { }
                    }
                }
            }

        }

        /// <summary>
        /// This method returns the Main directory of the AddIn from the registry
        /// returns empty string if it does not exists.
        /// </summary>
        /// <returns></returns>
        public static string AddInDirectory
        {
            get
            {
                try
                {
                    RegistryKey key = Registry.LocalMachine.OpenSubKey(AddInRegKey);
                    string path = Convert.ToString(key.GetValue(AddInRegValueKey));
                    if (!Directory.Exists(path))
                        return string.Empty;
                    return path;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

    }
}
