﻿using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using TSDI = Tekla.Structures.Dialog;

namespace Jungle_WPF_Plugins_DrawingTools.FragmentSymbol
{
    /// <summary>
    /// Логика взаимодействия для MainWindow_FragmentSymbol.xaml
    /// </summary>
    public partial class MainWindow_FragmentSymbol : TSDI.PluginWindowBase
    {
        public MainWindowViewModel dataViewModel { get; set; }
        public MainWindow_FragmentSymbol(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            this.dataViewModel = viewModel;
        }


        private void WpfOkApplyModifyGetOnOffCancel_ApplyClicked(object sender, EventArgs e)
        {
            this.Apply();
        }

        private void WpfOkApplyModifyGetOnOffCancel_CancelClicked(object sender, EventArgs e)
        {
            this.Close();
        }

        private void WpfOkApplyModifyGetOnOffCancel_GetClicked(object sender, EventArgs e)
        {
            this.Get();
        }

        private void WpfOkApplyModifyGetOnOffCancel_ModifyClicked(object sender, EventArgs e)
        {
            this.Modify();
        }

        private void WpfOkApplyModifyGetOnOffCancel_OkClicked(object sender, EventArgs e)
        {
            this.Apply();
            this.Close();
            
        }

        private void WpfOkApplyModifyGetOnOffCancel_OnOffClicked(object sender, EventArgs e)
        {
            this.ToggleSelection();
        }



        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }



        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }


        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_Reverse_Click(object sender, RoutedEventArgs e)
        {
            int currentDir = dataViewModel.Direction;
            if(currentDir == 0)
            {
                dataViewModel.Direction = 1;
            }
            else
                dataViewModel.Direction= 0;

            this.Modify();
            this.Apply();
        }
    }
}