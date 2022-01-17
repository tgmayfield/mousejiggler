#region header

// MouseJiggler - MainForm.cs
// 
// Created by: Alistair J R Young (avatar) at 2021/01/24 1:57 AM.

#endregion

#region using

using System;
using System.Windows.Forms;

using ArkaneSystems.MouseJiggler.Properties;

#endregion

namespace ArkaneSystems.MouseJiggler
{
    public partial class MainForm : Form
    {
        /// <summary>
        ///     Constructor for use by the form designer.
        /// </summary>
        public MainForm ()
            : this (jiggleOnStartup: false, clickOnStartup:false, minimizeOnStartup: false, zenJiggleEnabled: false, jigglePeriod: 1, clickPeriod: 1)
        { }

        public MainForm (bool jiggleOnStartup, bool clickOnStartup, bool minimizeOnStartup, bool zenJiggleEnabled, int jigglePeriod, int clickPeriod)
        {
            this.InitializeComponent ();

            // Jiggling on startup?
            this.JiggleOnStartup = jiggleOnStartup;
            this.clickOnStartup = clickOnStartup;

            // Set settings properties
            // We do this by setting the controls, and letting them set the properties.

            this.cbMinimize.Checked = minimizeOnStartup;
            this.cbZen.Checked      = zenJiggleEnabled;
            this.tbJigglePeriod.Value     = jigglePeriod;
            this.tbClickPeriod.Value     = clickPeriod;
        }

        public bool JiggleOnStartup { get; }

        public bool clickOnStartup { get; }

        private void MainForm_Load (object sender, EventArgs e)
        {
            if (this.JiggleOnStartup)
                this.cbJiggling.Checked = true;

            if (this.clickOnStartup)
                this.cbClicking.Checked = true;    
        }

        private void UpdateNotificationAreaText ()
        {
            if (!this.cbJiggling.Checked)
            {
                this.niTray.Text = "Not jiggling the mouse.";
            }
            else
            {
                string? ww = this.ZenJiggleEnabled ? "with" : "without";
                this.niTray.Text = $"Jiggling mouse every {this.JigglePeriod} s, {ww} Zen.";
            }
        }

        private void cmdAbout_Click (object sender, EventArgs e)
        {
            new AboutBox ().ShowDialog (owner: this);
        }

        #region Property synchronization

        private void cbSettings_CheckedChanged (object sender, EventArgs e)
        {
            this.panelSettings.Visible = this.cbSettings.Checked;
        }

        private void cbMinimize_CheckedChanged (object sender, EventArgs e)
        {
            this.MinimizeOnStartup = this.cbMinimize.Checked;
        }

        private void cbZen_CheckedChanged (object sender, EventArgs e)
        {
            this.ZenJiggleEnabled = this.cbZen.Checked;
        }

        private void tbJigglePeriod_ValueChanged (object sender, EventArgs e)
        {
            this.JigglePeriod = this.tbJigglePeriod.Value;
        }

        private void tbClickPeriod_ValueChanged (object sender, EventArgs e)
        {
            this.ClickPeriod = this.tbClickPeriod.Value;
        }
        #endregion Property synchronization

        #region Do the Jiggle!

        protected bool Zig = true;

        private void cbJiggling_CheckedChanged (object sender, EventArgs e)
        {
            this.jiggleTimer.Enabled = this.cbJiggling.Checked;
        }

        private void cbClicking_CheckedChanged (object sender, EventArgs e)
        {
            this.clickTimer.Enabled = this.cbClicking.Checked;
        }

        private void jiggleTimer_Tick (object sender, EventArgs e)
        {
            if (this.ZenJiggleEnabled)
                Helpers.Jiggle (delta: 0);
            else if (this.Zig)
                Helpers.Jiggle (delta: 4);
            else //zag
                Helpers.Jiggle (delta: -4);

            this.Zig = !this.Zig;
        }
        private void clickTimer_Tick (object sender, EventArgs e)
        {
            Helpers.Click ();
        }
        #endregion Do the Jiggle!

        #region Minimize and restore

        private void cmdTrayify_Click (object sender, EventArgs e)
        {
            this.MinimizeToTray ();
        }

        private void niTray_DoubleClick (object sender, EventArgs e)
        {
            this.RestoreFromTray ();
        }

        private void MinimizeToTray ()
        {
            this.Visible        = false;
            this.ShowInTaskbar  = false;
            this.niTray.Visible = true;

            this.UpdateNotificationAreaText ();
        }

        private void RestoreFromTray ()
        {
            this.Visible        = true;
            this.ShowInTaskbar  = true;
            this.niTray.Visible = false;
        }

        #endregion Minimize and restore

        #region Settings property backing fields

        private int jigglePeriod;

        private int clickPeriod;

        private bool minimizeOnStartup;

        private bool zenJiggleEnabled;

        private bool clickEnabled;

        #endregion Settings property backing fields

        #region Settings properties

        public bool MinimizeOnStartup
        {
            get => this.minimizeOnStartup;
            set
            {
                this.minimizeOnStartup             = value;
                Settings.Default.MinimizeOnStartup = value;
                Settings.Default.Save ();
            }
        }

        public bool ClickEnabled
        {
            get => this.clickEnabled;
            set
            {
                this.clickEnabled      = value;
                Settings.Default.Click = value;
                Settings.Default.Save ();
            }
        }
        public bool ZenJiggleEnabled
        {
            get => this.zenJiggleEnabled;
            set
            {
                this.zenJiggleEnabled      = value;
                Settings.Default.ZenJiggle = value;
                Settings.Default.Save ();
            }
        }

        public int JigglePeriod
        {
            get => this.jigglePeriod;
            set
            {
                this.jigglePeriod             = value;
                Settings.Default.JigglePeriod = value;
                Settings.Default.Save ();

                this.jiggleTimer.Interval = value * 1000;
                this.lbJigglePeriod.Text        = $"{value} s";
            }
        }

        public int ClickPeriod
        {
            get => this.clickPeriod;
            set
            {
                this.clickPeriod             = value;
                Settings.Default.ClickPeriod = value;
                Settings.Default.Save ();

                this.jiggleTimer.Interval = value * 1000;
                this.lbClickPeriod.Text        = $"{value} s";
            }
        }
        #endregion Settings properties

        #region Minimize on start

        private bool firstShown = true;

        private void MainForm_Shown (object sender, EventArgs e)
        {
            if (this.firstShown && this.MinimizeOnStartup)
                this.MinimizeToTray ();

            this.firstShown = false;
        }

        #endregion
    }
}
