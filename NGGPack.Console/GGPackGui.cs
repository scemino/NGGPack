//
// GGPackGui.cs
//
// Author:
//       scemino <scemino74@gmail.com>
//
// Copyright (c) 2018 
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.IO;
using Terminal.Gui;

namespace NGGPack.Console
{
    internal class GGPackGui
    {
        public GGPackGui()
        {
            Application.Init();

            Application.Top.Add(CreateMenu());

            OpenPack();
        }

        public void Show()
        {
            Application.Run();
        }

        private MenuBar CreateMenu()
        {
            return new MenuBar(new MenuBarItem[] {
                new MenuBarItem ("_File", new MenuItem [] {
                    new MenuItem ("_Open", "Open a pack", () => OpenPack()),
                    new MenuItem ("_Quit", string.Empty, () => { if (Quit ()) Application.Top.Running = false; })
                })});
        }

        private void OpenPack()
        {
            var dialog = new OpenDialog("Open", "Open a pack")
            {
                FilePath = "ThimbleweedPark.ggpack1",
                AllowedFileTypes = new string[] { ".ggpack1", ".ggpack2" }
            };
            Application.Run(dialog);
            var path = Path.Combine(dialog.DirectoryPath.ToString(), dialog.FilePath.ToString());
            OpenPack(path);
        }

        private void OpenPack(string path)
        {
            try
            {
                var fs = File.OpenRead(path);
                var pack = new GGPackReader().ReadPack(fs);
                var win = new GGPackExplorerWindow(pack);
                Application.Top.Add(win);
            }
            catch (Exception)
            {
                MessageBox.ErrorQuery(50, 10, "Error", "Invalid ggpack file", "OK");
            }
        }

        private bool Quit()
        {
            //var n = MessageBox.Query(50, 7, string.Empty, "Are you sure you want to quit?", "Yes", "No");
            //return n == 0;
            return true;
        }
    }
}
