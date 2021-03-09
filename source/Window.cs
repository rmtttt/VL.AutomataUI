﻿using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutomataUI
{

/// <summary>
/// Main window providing home for automata view rendering, data and control
/// </summary>

    public class Window : Form
    {
        AutomataView View;
        Interaction Interaction;
        AutomataModel Model;
        Dialogs Dialogs;
        
        public Window()
        {
            InitializeAutomata();
        } 
         
        private void InitializeAutomata()
        {          
            //
            // 
            //
            View = new AutomataView();
            Model = new AutomataModel(true); 
            Dialogs = new Dialogs(); 
            // create mousekeyboard control for drawing
            Interaction = new Interaction(View,Model,Dialogs);

            this.SuspendLayout();
            AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            ClientSize = new System.Drawing.Size(774, 529);
            Controls.Add(View.skiaView);
            Name = "AutomataUI";
            Text = "AutomataUI";
            ResumeLayout(false);
        }
    }
}