using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Threading;
using Microsoft.Win32;

class Program
{
    static void Main(string[] args)
    {
        string prev = null;
        Wallpaper.Style prevStyle = 0;

        Func<IEnumerable<DriveInfo>> usbDrives = () => DriveInfo.GetDrives()
                      .Where(drive => drive.IsReady && drive.DriveType == DriveType.Removable);

        var myDriveLetter = Assembly.GetExecutingAssembly().Location.ToUpper()[0];
        var onUsb = myDriveLetter != 'C';
            // usbDrives().Any(drive => drive.RootDirectory.ToString().ToUpper()[0] == myDriveLetter);


        if (onUsb)
        {
            if (args.Length == 0)
                CopyAndRun();
            return;
        }

        bool wallpaperSet = false;
        while (true)
        {
            if (usbDrives().Count() >= 1)
            {
                if (!wallpaperSet)
                {
                    Wallpaper.Set("base.jpg", Wallpaper.Style.Centered, out prev, out prevStyle);
                    wallpaperSet = true;
                }
            }
            else
            {
                if (wallpaperSet)
                {
                    Wallpaper.Set(prev, prevStyle);
                    wallpaperSet = false;
                }
            }

            Thread.Sleep(1000);

        }
    }

    static void CopyAndRun()
    {
        var dir = Path.GetTempPath() + "\\usbGuard";
        Directory.CreateDirectory(dir);
        File.Copy("usbguard.exe", dir + "\\usbguard.exe", false);
        File.Copy("base.jpg", dir + "\\usbguard.jpg", false);
        var proc = new Process();
        var psi = proc.StartInfo;
        psi.FileName = dir + "\\usbguard.exe";

        proc.Start();

    }
}

