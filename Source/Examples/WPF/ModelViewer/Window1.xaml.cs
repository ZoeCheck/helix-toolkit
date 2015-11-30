// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Window1.xaml.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Interaction logic for Window1.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using HelixToolkit.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
namespace ModelViewer
{
    /// <summary>
    /// Interaction logic for Window1.
    /// </summary>
    public partial class Window1
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Window1"/> class.
        /// </summary>
        public Window1()
        {
            this.InitializeComponent();
            //this.DataContext = new MainViewModel(new FileDialogService(), view1);
        }

        private string[] OpenDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择文件(可多选，最多3个)";
            openFileDialog.Filter = "3D model files (*.3ds;*.obj;*.lwo;*.stl)|*.3ds;*.obj;*.objz;*.lwo;*.stl";
            openFileDialog.FileName = string.Empty;
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Multiselect = true;
            openFileDialog.DefaultExt = ".3ds";
            if (openFileDialog.ShowDialog() == true)
            {
                if (openFileDialog.FileNames.Length > 0)
                {
                    return openFileDialog.FileNames;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private async void MenuItemOpen_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            rootSP.Children.Clear();
            string[] files = OpenDialog();
            if (files != null)
            {
                if (files.Length > 0)
                {
                    foreach (var item in files)
                    {
                        Model3D model = await this.LoadAsync(item, false);

                        HelixViewport3D viewPort = new HelixViewport3D();
                        viewPort.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                        viewPort.Height = 300;
                        viewPort.CameraRotationMode = CameraRotationMode.Trackball;
                        viewPort.ModelUpDirection = new Vector3D(0, 1, 0);
                        viewPort.ShowFrameRate = true;

                        ModelVisual3D visual3D = new ModelVisual3D();
                        viewPort.Children.Add(visual3D);
                        visual3D.Content = model;
                        DefaultLights deLight = new DefaultLights();
                        visual3D.Children.Add(deLight);

                        rootSP.Children.Add(viewPort);
                    }
                }
            }
        }

        private async Task<Model3DGroup> LoadAsync(string model3DPath, bool freeze)
        {
            return await Task.Factory.StartNew(() =>
            {
                var mi = new ModelImporter();

                if (freeze)
                {
                    // Alt 1. - freeze the model 
                    return mi.Load(model3DPath, null, true);
                }

                // Alt. 2 - create the model on the UI dispatcher
                return mi.Load(model3DPath, this.Dispatcher);
            });
        }

        private void MenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MessageBox.Show("3D性能测试程序", "关于", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}