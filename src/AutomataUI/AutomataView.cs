﻿using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaTextRenderer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace AutomataUI
{
    public class AutomataView
    {
        public AutomataModel AutomataData;
        //this.ResizeEnd += Window_Resize;

        public SkiaSharp.Views.Desktop.SKGLControl skiaView;
        public SKPoint worldOffset;
        public float worldScale = 1;

        //colors
        SKPaint stateInitPaint;
        SKPaint stateDefaultPaint;
        SKPaint textWhitePaint;
        SKPaint textHelpPaint;
        SKPaint activePaint;
        SKPaint transitionPaint;
        SKPaint activeTransitionPaint;

        SkiaTextRenderer.Font font;

        //needed for new transitions
        public State startTransitionState;
        public State endTransitionState;

        public SKPoint mousePosition;

        //Initialize
        public AutomataView(AutomataModel AutomataDataInput)
        {
            AutomataData = AutomataDataInput; // reference to parent class data
            AutomataData.Redraw += AutomataDataRedraw;


            skiaView = new SkiaSharp.Views.Desktop.SKGLControl();
            skiaView.Dock = System.Windows.Forms.DockStyle.Fill;
            skiaView.Location = new System.Drawing.Point(0, 0);
            skiaView.Name = "skiaView";
            skiaView.Size = new System.Drawing.Size(774, 529);
            skiaView.TabIndex = 0;
            skiaView.Text = "skControl1";


            skiaView.PaintSurface += new System.EventHandler<SkiaSharp.Views.Desktop.SKPaintGLSurfaceEventArgs>(UpdateSkiaView);

            SetupPaints();
            font = new SkiaTextRenderer.Font(SKTypeface.Default, 15);

        }

        public void AutomataDataRedraw()
        {
            Debug.WriteLine("redraw UI");
            this.skiaView.Invalidate();
        }

        public void SetupPaints()
        {

            stateInitPaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Color = SKColor.Parse("#00ffea")
            };

            stateDefaultPaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Color = SKColor.Parse("#323232")
            };

            activePaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = SKColor.Parse("#ffa500"),
                StrokeWidth = 5
            };

            transitionPaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.StrokeAndFill,
                Color = SKColors.Cyan,
                StrokeWidth = 3,
            };

            activeTransitionPaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.StrokeAndFill,
                Color = SKColor.Parse("#ffa500"),
                StrokeWidth = 3,
            };

            textWhitePaint = new SKPaint
            {
                Color = SKColors.White,
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                TextAlign = SKTextAlign.Center,
                Typeface = SKTypeface.FromFamilyName("Arial"),
                TextSize = 15,
                IsStroke = false,
                FilterQuality = SKFilterQuality.High
            };

            textHelpPaint = new SKPaint
            {
                Color = SKColor.Parse("#555555"),
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                TextAlign = SKTextAlign.Left,
                Typeface = SKTypeface.FromFamilyName("Arial"),
                TextSize = 12,
                IsStroke = false,
                FilterQuality = SKFilterQuality.High
            };


        }
        public void DrawStates(SKCanvas canvas)
        {

            if (AutomataData != null)
            {
                foreach (var item in AutomataData.states)
                {

                    //draw active state
                    if (item == AutomataData.activeState)
                    {
                        canvas.DrawCircle(item.Bounds.MidX, item.Bounds.MidY, 52, activePaint);
                    }

                    //draw init state
                    if (item.Name == "Init")
                    {
                        canvas.DrawCircle(item.Bounds.MidX, item.Bounds.MidY, 50, stateInitPaint);
                        DrawStateText(canvas, item.Name, SKColors.Black, new SKPoint(item.Bounds.MidX, item.Bounds.MidY));
                    }
                    //draw states
                    else
                    {
                        canvas.DrawCircle(item.Bounds.MidX, item.Bounds.MidY, 50, stateDefaultPaint);
                        DrawStateText(canvas, item.Name, SKColors.White, new SKPoint(item.Bounds.MidX, item.Bounds.MidY));
                    }
                }
            }

        }
        private void UpdateSkiaView(object sender, SKPaintGLSurfaceEventArgs e)
        {
           

            var canvas = e.Surface.Canvas;
            // make sure the canvas is blank
            canvas.Clear(SKColor.Parse("#141414"));

            //help text
            canvas.DrawText("Create/Edit State (DblClick) Create Transition (RClick -> LClick) Force State/Transition (Ctrl + LClick) ", 10, skiaView.Height-10, textHelpPaint);

            // scale and translate world aka canvas
            canvas.Scale(worldScale);
            canvas.Translate(worldOffset);

            DrawTransitions(canvas);

            DrawNewTransition(canvas);

            DrawStates(canvas);

            //debug mouse method
            //canvas.DrawCircle(Tools.ToWorldSpace(mousePos, worldOffset, worldScale), 10, stateDefaultPaint);
        }
        private void DrawStateText(SKCanvas canvas, string name, SKColor textColor, SKPoint pos)
        {

            //Antialiased Text
            //using (var paint = new SKPaint())
            //{
            //    paint.TextSize = 64.0f;
            //    paint.IsAntialias = true;
            //    paint.Color = new SKColor(0x42, 0x81, 0xA4);
            //    paint.IsStroke = false;

            //    canvas.DrawText("MegaState Haha", pos.X, pos.Y+4, textWhitePaint);
            // }

            TextRendererSk.DrawText(canvas,
                                                name,
                                                font,
                                                SKRect.Create(pos.X - 40, pos.Y - 40, 80, 80),
                                                textColor,
                                                SkiaTextRenderer.TextFormatFlags.WordBreak |
                                                SkiaTextRenderer.TextFormatFlags.VerticalCenter |
                                                SkiaTextRenderer.TextFormatFlags.HorizontalCenter);

        }
        private void DrawTransitionText(SKCanvas canvas, string name, SKColor textColor, SKPoint pos)
        {
            TextRendererSk.DrawText(canvas,
                                                        name,
                                                        font,
                                                        SKRect.Create(pos.X - 40, pos.Y - 40, 80, 80),
                                                        textColor,
                                                        SkiaTextRenderer.TextFormatFlags.WordBreak |
                                                        SkiaTextRenderer.TextFormatFlags.VerticalCenter |
                                                        SkiaTextRenderer.TextFormatFlags.HorizontalCenter);

        }
        private void DrawTransitions(SKCanvas canvas)
        {
            if (AutomataData != null)
            {
                float angle = 0.0f;

                foreach (var transition in AutomataData.transitions)
                {
                    var start = new SKPoint(transition.StartState.Bounds.MidX, transition.StartState.Bounds.MidY);
                    var end = new SKPoint(transition.EndState.Bounds.MidX, transition.EndState.Bounds.MidY);

                    // check if there is a return transition to draw double connections correctly
                    foreach (Transition subtransition in AutomataData.transitions) // check if there is a return transition to draw double connections correctly
                    {
                        if (subtransition.StartState.ID == transition.EndState.ID && subtransition.EndState.ID == transition.StartState.ID)
                        {
                            angle = 0.4f;
                            break;
                        }
                        else angle = 0.0f;
                    }

                    // get transitions points with gap, center of transition and angle for arrow
                    Tools.EdgePoints edgepoints = Tools.GetEdgePoints(start, end, 55, angle);

                    //draw active transition
                    if (AutomataData.activeTransition != null && transition == AutomataData.activeTransition)
                    {
                        canvas.DrawLine(edgepoints.A, edgepoints.B, activeTransitionPaint);
                        DrawArrow(canvas, new SKPoint(edgepoints.B.X, edgepoints.B.Y), edgepoints.Angle, SKColor.Parse("#ffa500"));
                    }
                    //draw inactive transition
                    else
                    {
                        canvas.DrawLine(edgepoints.A, edgepoints.B, transitionPaint);
                        DrawArrow(canvas, new SKPoint(edgepoints.B.X, edgepoints.B.Y), edgepoints.Angle, SKColors.Cyan);
                    }

                    SKPoint center = Tools.CenterPoints(edgepoints.A, edgepoints.B);

                    SKPath linePath = new SKPath();
                    linePath.MoveTo(center.X - 60, center.Y + 5);
                    linePath.LineTo(center.X + 60, center.Y + 5);

                    SKSize s = TextRendererSk.MeasureText(transition.Name, font);

                    SKRect textBounds = new SKRect(center.X - s.Width / 2, center.Y - s.Height / 2, center.X + s.Width / 2, center.Y + s.Height / 2);

                    ////////////////add bounds to transition
                    transition.Bounds = textBounds;
                    ////////////////add bounds to transition

                    
                    canvas.DrawRect(center.X - s.Width / 2, center.Y - s.Height / 2, s.Width, s.Height, stateDefaultPaint);

                    //i have to draw onto a path to avoid jitter...weirdo
                    canvas.DrawTextOnPath(transition.Name, linePath, 0, 0, textWhitePaint);

                }
            }


        }
        public void DrawNewTransition(SKCanvas canvas)
        {
            if (startTransitionState != null)
            {
                var transitionPaint = new SKPaint
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.StrokeAndFill,
                    Color = SKColors.Red,
                    StrokeWidth = 5
                };

                SKPoint start = new SKPoint(startTransitionState.Bounds.MidX, startTransitionState.Bounds.MidY);
                SKPoint end = Tools.ToWorldSpace(mousePosition, worldOffset, worldScale);
                Tools.EdgePoints edgepoints = Tools.GetEdgePoints(start, end, 55, 0);

                if (endTransitionState == null)
                {
                    canvas.DrawLine(edgepoints.A, end, transitionPaint);
                    DrawArrow(canvas, new SKPoint(end.X, end.Y), edgepoints.Angle, SKColors.Red);
                }
                else
                {
                    edgepoints = Tools.GetEdgePoints(start, new SKPoint(endTransitionState.Bounds.MidX, endTransitionState.Bounds.MidY), 55, 0);

                    //do the drawing
                    canvas.DrawLine(edgepoints.A, edgepoints.B, transitionPaint);
                    DrawArrow(canvas, new SKPoint(edgepoints.B.X, edgepoints.B.Y), edgepoints.Angle, SKColors.Red);
                }
            }
        }
        private void DrawArrow(SKCanvas canvas, SKPoint pos, float angle, SKColor color)
        {

            var pathStroke2 = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.StrokeAndFill,
                Color = color,
                StrokeWidth = 5
            };

            var path2 = new SKPath { FillType = SKPathFillType.EvenOdd };
            path2.MoveTo(0, 3);
            path2.LineTo(5, 10);
            path2.LineTo(-5, 10);
            path2.LineTo(0, 3);
            path2.Close();

            path2.Transform(SKMatrix.CreateRotationDegrees(360 - angle + 180));
            path2.Transform(SKMatrix.CreateTranslation(pos.X, pos.Y));
            canvas.DrawPath(path2, pathStroke2);
        }

        //public void DrawActiveState(SKCanvas canvas)
        //{
        //    if (AutomataData.activeState != null)
        //    {
        //        //canvas.DrawCircle(AutomataData.activeState.Bounds.MidX, AutomataData.activeState.Bounds.MidY, 55, stateActivePaint);

        //        var outerPaint = new SKPaint
        //        {
        //            IsAntialias = true,
        //            Style = SKPaintStyle.Stroke, //stroke so that it traces the outline
        //            Color = SKColor.Parse("#ffa500"), //make it the color red
        //            StrokeWidth = 10
        //        };


        //        SKPath skPath = new SKPath();
        //        skPath.AddArc(AutomataData.activeState.Bounds, 0, 360);
        //        canvas.DrawPath(skPath, outerPaint);

        //    }

        //}
    }
}
