using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32;

// http://stackoverflow.com/questions/1061678/change-desktop-wallpaper-using-code-in-net
public sealed class Wallpaper
{
    Wallpaper() { }

    const int SPI_SETDESKWALLPAPER = 20;
    const int SPIF_UPDATEINIFILE = 0x01;
    const int SPIF_SENDWININICHANGE = 0x02;

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
    public enum Style : int
    {
        Tiled,
        Centered,
        Stretched
    }
    
    // http://www.neowin.net/forum/topic/232217-c-get-current-wallpaper-location/
    static private string GetCurrentWallpaper()
    {
        RegistryKey theCurrentMachine = Registry.CurrentUser;
        RegistryKey theControlPanel = theCurrentMachine.OpenSubKey ("Control Panel");
        RegistryKey theDesktop = theControlPanel.OpenSubKey ("Desktop");
        return Convert.ToString(theDesktop.GetValue("Wallpaper"));
    }

    public static void Set(string path, Style style)
    {
        string _;
        Style __;
        Set(path, style, out _, out __);
    }

    public static void Set(string path, Style style, out string prevPath, out Style prevStyle)
    {
        // System.IO.Stream s = new System.Net.WebClient().OpenRead(uri.ToString());

        prevPath = GetCurrentWallpaper();
        
        //System.Drawing.Image img = System.Drawing.Image.FromFile(path);
        //string tempPath = Path.GetTempFileName();
        //img.Save(tempPath, System.Drawing.Imaging.ImageFormat.Bmp);

        RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
        
        var ws = key.GetValue(@"WallpaperStyle");
        if((ws ?? ws).ToString() == "2") 
        {
            prevStyle = Style.Stretched;
        }
        else // if(ws.ToString() == "1")
        {
            var tw = key.GetValue("@TileWallpaper");
            if((tw ?? "").ToString() == "1")
            {
                prevStyle = Style.Tiled;
            }
            else // if(tw.ToString() == "0")
            {
                prevStyle = Style.Centered;
            }
        }
            
        
        if (style == Style.Stretched)
        {
            key.SetValue(@"WallpaperStyle", 2.ToString());
            key.SetValue(@"TileWallpaper", 0.ToString());
        }

        if (style == Style.Centered)
        {
            key.SetValue(@"WallpaperStyle", 1.ToString());
            key.SetValue(@"TileWallpaper", 0.ToString());
        }

        if (style == Style.Tiled)
        {
            key.SetValue(@"WallpaperStyle", 1.ToString());
            key.SetValue(@"TileWallpaper", 1.ToString());
        }

        SystemParametersInfo(SPI_SETDESKWALLPAPER,
            0,
            path,
            SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
    }
}