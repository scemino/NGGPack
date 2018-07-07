//
// GGPackExplorerWindow.cs
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
using System.Linq;
using Terminal.Gui;

namespace NGGPack.Console
{
    internal class GGPackExplorerWindow : Window
    {
        private ListView _listViewEntries;
        private GGPack _pack;
        private TextView _detailView;

        public GGPackExplorerWindow(GGPack pack)
            : base("GGPack Explorer")
        {
            Width = Dim.Fill();
            Y = 1;
            Height = Dim.Fill();

            _pack = pack;
            base.WantMousePositionReports = true;

            var entries = _pack.Entries.Select(e => e.Name).ToList();
            _listViewEntries = new ListView(entries)
            {
                Width = Dim.Percent(50),
                Height = Dim.Fill()
            };
            _detailView = new TextView
            {
                X = Pos.Percent(50),
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                Text = string.Empty,
            };
            _listViewEntries.SelectedChanged += OnSelectedChanged;
            Add(_listViewEntries, _detailView);
            OnSelectedChanged();
        }

        private void OnSelectedChanged()
        {
            var entry = _pack.Entries[_listViewEntries.SelectedItem];
            if (string.Equals(Path.GetExtension(entry.Name), ".png", StringComparison.OrdinalIgnoreCase))
            {
                _detailView.Text = "no preview";
                return;
            }
            if (string.Equals(Path.GetExtension(entry.Name), ".ogg", StringComparison.OrdinalIgnoreCase))
            {
                _detailView.Text = "no preview";
                return;
            }
            if (string.Equals(Path.GetExtension(entry.Name), ".wav", StringComparison.OrdinalIgnoreCase))
            {
                _detailView.Text = "no preview";
                return;
            }

            if (string.Equals(Path.GetExtension(entry.Name), ".wimpy", StringComparison.OrdinalIgnoreCase))
            {
                var hash = new GGPackReader().ReadHash(_pack.GetEntryStream(entry.Name));
                _detailView.Text = hash.ToString();
                return;
            }

            using (var reader = new StreamReader(_pack.GetEntryStream(entry.Name)))
            {
                _detailView.Text = reader.ReadToEnd();
            }
        }
    }
}
