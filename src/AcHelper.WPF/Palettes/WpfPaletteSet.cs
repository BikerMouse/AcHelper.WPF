﻿using Autodesk.AutoCAD.Windows;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AcHelper.WPF.Palettes
{
    /// <summary>
    /// Represents the method that handles the 
    /// WpfPalet5teSetVisibleStateChanged event of a WpfPaletteset.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="arg">A WpfPaletteSetVisibleStatChangedEventArgs that contains the event data.</param>
    public delegate void WpfPaletteSetVisibleStateChangedEventHandler(object sender, WpfPaletteSetVisibleStateChangedEventArgs arg);

    /// <summary>
    /// Paletteset containing (multiple) palettes; Optimized for WPF.
    /// </summary>
    public class WpfPaletteSet : PaletteSet
    {
        private string _name = string.Empty;
        private List<IPalette> _palettes = new List<IPalette>();
        private static Size _size, _minimumSize = Size.Empty;
        private bool _firstTimeVisible = true;
        private DockSides _dock = DockSides.None;

        /// <summary>
        /// Creates a WpfPaletteset.
        /// </summary>
        /// <param name="name">Name of the paletteset to create.</param>
        /// <param name="guid">Guid of the paletteset to create.</param>
        public WpfPaletteSet(string name, Guid guid)
            : this(name, guid, new Size(350,450), new Size(350,450))
        { }
        /// <summary>
        /// Creates a WpfPaletteset.
        /// </summary>
        /// <param name="name">Name of the paletteset to create.</param>
        /// <param name="guid">Guid of the paletteset to create.</param>
        /// <param name="size">Size of the paletteset to create.
        /// Default size is Width: 350, Height: 450.</param>
        /// <param name="minimumSize">Minimum size of the paletteset to create.
        /// Default size is Width: 350, Height: 450.</param>
        public WpfPaletteSet(string name, Guid guid
            , Size size
            , Size minimumSize)
            : this(name, guid, size, minimumSize, DockSides.Left, DockSides.None | DockSides.Left | DockSides.Right)
        { }
        /// <summary>
        /// Creates a WpfPaletteset.
        /// </summary>
        /// <param name="name">Name of the paletteset to create.</param>
        /// <param name="guid">Guid of the paletteset to create.</param>
        /// <param name="size">Size of the paletteset to create.
        /// Default size is Width: 350, Height: 450.</param>
        /// <param name="minimumSize">Minimum size of the paletteset to create.
        /// Default size is Width: 350, Height: 450.</param>
        /// <param name="dockside">Default side to dock the paletteset is Left.</param>
        /// <param name="docksideEnabled">Default enabled sides to dock the paletteset are Left, Right, None (floating).</param>
        public WpfPaletteSet(string name, Guid guid
            , Size size
            , Size minimumSize
            , DockSides dockside
            , DockSides docksideEnabled)
            : base(name, null, guid)
        {
            _name = name;
            
            _minimumSize = minimumSize;
            MinimumSize = minimumSize;

            _size = size;
            Size = size;

            _dock = dockside;
            Dock = dockside;
            DockEnabled = docksideEnabled;

            Style = PaletteSetStyles.ShowAutoHideButton
                | PaletteSetStyles.ShowCloseButton
                | PaletteSetStyles.Snappable;
        }

        #region Properties ...
        /// <summary>
        /// Represents the name of the paletteset.
        /// </summary>
        public string PaletteSetName
        {
            get { return _name; }
            set { _name = value; }
        }
        /// <summary>
        /// A list of all the palettes wich this paletteset contains.
        /// </summary>
        public List<IPalette> Palettes
        {
            get { return _palettes; }
            set { _palettes = value; }
        }
        /// <summary>
        /// Checks whether this paletteset contains a palette with the given name.
        /// </summary>
        /// <param name="name">Name of the palette</param>
        /// <returns>True if the paletteset contains a palette with the given name; Otherwise false.</returns>
        public bool HasPalette(string name)
        {
            foreach (IPalette item in _palettes)
            {
                if (string.Equals(item.PaletteName, name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Gets the palette with the given name.
        /// </summary>
        /// <param name="name">Name of the palette to return.</param>
        /// <returns></returns>
        public IPalette GetPalette(string name)
        {
            return _palettes.FirstOrDefault(p => string.Equals(p.PaletteName, name, StringComparison.InvariantCultureIgnoreCase));
        }
        /// <summary>
        /// Counts the amount of palettes assigned to this paletteset.
        /// </summary>
        public override int Count
        {
            get { return _palettes.Count; }
        }
        /// <summary>
        /// Gets the palette on the given index within this paletteset.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        new public IPalette this[int index]
        {
            get
            {
                if (Count > 0)
                {
                    return _palettes[index];
                }
                return null;
            }
        }
        /// <summary>
        /// Add palette to paletteset
        /// </summary>
        /// <param name="palette"></param>
        /// <exception cref="WpfPaletteSetException"/>
        public void AddPalette(IPalette palette)
        {
            string error_message = string.Format("Could not add palette '{0}' to the paletteset '{1}'", palette.PaletteName, PaletteSetName);

            if (!HasPalette(palette.PaletteName))
            {
                try
                {
                    Control view = palette as Control;
                    if (view != null)
                    {
                        Add(palette.PaletteName, view);
                        _palettes.Add(palette);
                        palette.PaletteSet = this;
                    }
                }
                catch (Exception ex)
                {
                    throw new WpfPaletteSetException(error_message, PaletteSetName, ex);
                }
            }
        }
        /// <summary>
        /// Removes palette from paletteset.
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="WpfPaletteSetException"/>
        public void RemovePalette(string name)
        {
            string error_message = string.Format("Something went wrong while removing palette: '{0}'", name);

            IPalette palette = _palettes.FirstOrDefault(p => string.Equals(p.PaletteName, name, StringComparison.InvariantCultureIgnoreCase));
            if (palette != null)
            {
                try
                {
                    int i = _palettes.IndexOf(palette);
                    Remove(i);
                    _palettes.RemoveAt(i);
                    palette.Dispose();

                    if (Count < 0)
                    {
                        Visible = false;
                    }
                }
                catch (Exception ex)
                {
                    throw new WpfPaletteSetException(error_message, PaletteSetName, ex);
                }
            }
        }
        #endregion

        #region Methods ...
        public void ActivatePaletteSet()
        {
            Visible = true;
            if (_firstTimeVisible)
            {
                Size = _size;
                Dock = _dock;
                _firstTimeVisible = false;
            }
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class WpfPaletteSetException : Exception
    {
        private string _paletteset_name;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="paletteSetName"></param>
        /// <param name="inner"></param>
        public WpfPaletteSetException(string message, string paletteSetName, Exception inner)
            : base(message, inner)
        {
            _paletteset_name = paletteSetName;
        }

        /// <summary>
        /// 
        /// </summary>
        public string PaletteSetName
        {
            get { return _paletteset_name; }
        }

        /// <summary>
        /// 
        /// </summary>
        public WpfPaletteSetException() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public WpfPaletteSetException(string message) : base(message) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public WpfPaletteSetException(string message, Exception inner) : base(message, inner) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected WpfPaletteSetException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
