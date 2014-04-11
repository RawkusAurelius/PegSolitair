﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

//TO-DO: Implement the AI (MiniMax / AB-Pruning), Add Player Transitions, Add Win States

namespace ChineseCheckers
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (Renderable render = new Renderable())
            {
                render.Run(30.0);
            }
        }
    }
    class Renderable : GameWindow
    {
        private int currentX;
        private int currentY;
        private double[] projectionMatrix = new double[16];
        private double[] modelViewMatrix = new double[16];
        private int[] viewport = new int[4];

        public Renderable()
        {
            currentX = 0;
            currentY = 0;
        }
        protected override void OnLoad(EventArgs eventargs)
        {
           
            base.OnLoad(eventargs);
            base.Title = "Chinese Checkers";
            base.VSync = VSyncMode.On;
            this.Mouse.ButtonDown += new EventHandler<MouseButtonEventArgs>(OnMouseDownHandler);
            GL.ClearColor(0f, 0f, 0f, 0f);
            GL.Enable(EnableCap.ColorMaterial);
            PegSolitaire.InitializeGameBoard();
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();         
            GL.Ortho(0, 100.0, 100.0, 0.0, 0.0, 4.0);
            GL.GetDouble(GetPName.ProjectionMatrix, projectionMatrix);
            
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.GetDouble(GetPName.ModelviewMatrix, modelViewMatrix);
            GL.GetInteger(GetPName.Viewport, viewport);
            PegSolitaire.DrawBoard();
            
            SwapBuffers();
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 64.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
            
        }
        protected void OnMouseDownHandler(object sender, OpenTK.Input.MouseButtonEventArgs e)
        {
            if (e.IsPressed && (currentX != e.X && currentY != e.Y))
                UpdateMouse(e.X, e.Y);       
        }
        private void UpdateMouse(int x, int y)
        {
            if (currentX != x && currentY != y)
            {
                WindowToClient(x, ref currentX, y, ref currentY);
                PegSolitaire.SelectPiece(currentX, currentY);
            }
        }
        private void WindowToClient(int x, ref int currentX, int y, ref int currentY)
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(projectionMatrix);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(modelViewMatrix);

            var box = (float)0.0;
            GL.ReadPixels(x, viewport[3] - y, 1, 1, OpenTK.Graphics.OpenGL.PixelFormat.DepthComponent, PixelType.Float, ref box);
            Vector3 results;
            OpenTK.Graphics.Glu.UnProject(new Vector3(x, viewport[3] - y, 0.0f), modelViewMatrix, projectionMatrix, viewport, out results);
            currentX = (int)results.X;
            currentY = (int)results.Y;
        } 
    }
}
    