﻿using System;
using System.Windows.Controls;
using System.Windows.Forms.Integration;

namespace AcHelper.WPF.Palettes
{
    public class WpfPalette : ElementHost, IPalette
    {
        #region Fields ...
        private UserControl _view = null;
        private string _name;

        private VisibleState _visible_state = VisibleState.Show;
        private WpfPaletteSet _parent = null;
        private bool _closed;
        #endregion

        #region Ctor ...
        public WpfPalette(UserControl view, string name)
        {
            _view = view;
            _name = name;

            Dock = System.Windows.Forms.DockStyle.Fill;
            AutoSize = true;
            SetAutoSizeMode(System.Windows.Forms.AutoSizeMode.GrowAndShrink);
            Child = _view;

            _closed = false;
        }
        #endregion

        #region Properties ...
        /// <summary>
        /// Sets and gets the VisibleState.
        /// </summary>
        public VisibleState VisibleState
        {
            get { return _visible_state; }
            set 
            {
                if (_visible_state != value)
                {
                    _visible_state = value; 
                    OnVisibleStateChanged(value);
                }
            }
        }
        /// <summary>
        /// Sets and gets the Palette name.
        /// </summary>
        public string PaletteName
        {
            get => _name;
            set => _name = value;
        }
        /// <summary>
        ///  Sets and gets the parent paletteset.
        /// </summary>
        public WpfPaletteSet PaletteSet
        {
            get => _parent;
            set => _parent = value;
        }
        #endregion

        #region Methods ...
        public void ClosePaletteSet()
        {
            if (_parent != null)
            {
                _parent.Visible = false;
            }
        }
        public void Close()
        {
            OnPaletteClosing(PaletteName);

            VisibleStateChanged = null;
            VisibleState = VisibleState.Hide;
            _closed = true;

            OnPaletteClosed(PaletteName);
        }
        #endregion

        #region Events ...
        public event WpfPaletteClosingEventHandler WpfPaletteClosing;
        public event WpfPaletteVisibleStateChangedEventHandler VisibleStateChanged;
        public event WpfPaletteClosedEventHandler WpfPaletteClosed;
        
        protected virtual void OnPaletteClosing(string paletteName)
        {
            WpfPaletteClosing?.Invoke(this, new WpfPaletteClosingEventArgs(paletteName));
        }
        protected virtual void OnVisibleStateChanged(VisibleState newVisibleState)
        {
            VisibleStateChanged?.Invoke(this, new WpfPaletteVisibleStateChangedEventArgs(VisibleState, newVisibleState));
        }
        protected virtual void OnPaletteClosed(string paletteName)
        {
            WpfPaletteClosed?.Invoke(this, new WpfPaletteClosedEventArgs(paletteName));
        }
        #endregion

        #region Dispose ...
        private bool _disposed = false;
        public new void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                if (!_closed)
                {
                    Close();
                }
                _view = null;
                Parent = null;
            }

            _disposed = true;
        }
        #endregion
    }
}