using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BeyondVisionEngine.Components;
using BeyondVisionEngine.Enums;

namespace BeyondVisionEngine.EditorWindows
{
    /// <summary>
    /// Interaction logic for EditDialogEvent.xaml
    /// </summary>
    /// 

    public class DecisionInfo : INotifyPropertyChanged
    {
        private string _decisionInfoText;

        public string DecisionInfoText
        {
            get => _decisionInfoText;
            set
            {
                if (value == _decisionInfoText) 
                    return;

                _decisionInfoText = value;
                OnPropertyChanged("DecisionInfoText");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string value)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(value));
        }
    }

    public partial class EditDialogEvent : Window
    {
        private const int DialogStartingUpMargin = 10;
        private const int DialogStartingLeftMargin = 35;
        private const int DeleteButtonLeftMargin = 10;
        private const int LengthBetweenDialogs = 28;

        private string _selectedVoice = "";
        private int _dialogCounter = 0;
        private DialogGameEvent _dialogGameEvent;
        private bool _wereDecisionsAdded;

        public DecisionInfo DecisionInfo { get; set; }

        public EditDialogEvent(DialogGameEvent dialogGameEvent)
        {
            _dialogGameEvent = dialogGameEvent;
            InitializeComponent();

            DecisionInfo = new DecisionInfo();

            DataContext = DecisionInfo;
        }


        private void EditDialogEvent_OnLoaded(object sender, RoutedEventArgs e)
        {
            foreach (var dialog in _dialogGameEvent.Dialogs)
            {
                AddNewTextBox(dialog.Text, false);
            }

            foreach (var item in Enum.GetValues(typeof(DecisionResult)))
            {
                LeftDecisionType.Items.Add(item);
                RightDecisionType.Items.Add(item);
            }

            if (_dialogGameEvent.DecisionCount != 0)
            {
                LeftDecisionType.SelectedValue = _dialogGameEvent.LeftDecisionResult;
                RightDecisionType.SelectedValue = _dialogGameEvent.RightDecisionResult;
            }

        }

        private void AddNewTextBox(string text = "", bool isNew = true, string decisionType = "")
        {
            var topMargin = DialogStartingUpMargin + _dialogCounter * LengthBetweenDialogs;

            var newDialog = new TextBox
            {
                Margin = new Thickness(DialogStartingLeftMargin, topMargin, 0, 0),
                Text = text,
                Height = 23,
                Width = 328,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                TextWrapping = TextWrapping.Wrap,
            };

            _dialogCounter += 1;
            EditDialogEventGrid.Children.Add(newDialog);

            var dialogDeleteButton = new Button
            {
                Margin = new Thickness(DeleteButtonLeftMargin, topMargin, 0, 0),
                Width = 25,
                Height = 23,
                Content = "X",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };
            dialogDeleteButton.Click += DeleteDialog;
            EditDialogEventGrid.Children.Add(dialogDeleteButton);

            var newSingleDialog = new SingleDialog(text) { TextBox = newDialog, DeleteButton = dialogDeleteButton, Voice = _selectedVoice };

            if (decisionType == "right")
                newSingleDialog.IsRightDecision = true;
            else if (decisionType == "left")
                newSingleDialog.IsLeftDecision = true;


            if (isNew)
                _dialogGameEvent.Dialogs.Add(newSingleDialog);
        }

        protected void DeleteDialog(object sender, RoutedEventArgs e)
        {
            var clickedDeleteButton = (Button)sender;

            var dialog = _dialogGameEvent.Dialogs.First(x => x.DeleteButton == clickedDeleteButton);
            EditDialogEventGrid.Children.Remove(dialog.TextBox);
            EditDialogEventGrid.Children.Remove(clickedDeleteButton);
            _dialogGameEvent.Dialogs.Remove(dialog);

            _dialogCounter -= 1;
            UpdateView();
        }

        private void UpdateView()
        {
            var counter = 0;
            foreach (var dialog in _dialogGameEvent.Dialogs)
            {
                dialog.TextBox.Margin = new Thickness(DialogStartingLeftMargin, DialogStartingUpMargin + counter * LengthBetweenDialogs, 0, 0);
                dialog.DeleteButton.Margin = new Thickness(DeleteButtonLeftMargin, DialogStartingUpMargin + counter * LengthBetweenDialogs, 0, 0);
                counter += 1;
            }
        }

        private void AddDialogButton_Click_1(object sender, RoutedEventArgs e)
        {
            AddNewTextBox($"{_selectedVoice}");
        }

        private void EditDialogEvent_OnClosing(object sender, CancelEventArgs e)
        {
            foreach (var dialog in _dialogGameEvent.Dialogs)
            {
                dialog.Text = dialog.TextBox.Text;
            }
        }

        private void AddDecisionButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_wereDecisionsAdded)
            {
                MessageBox.Show("You can have only 2 decisions!");
                return;
            }

            AddNewTextBox($"Left decision", decisionType: "left");
            AddNewTextBox($"Right decision", decisionType: "right");
            _dialogGameEvent.DecisionCount = 2;
            _wereDecisionsAdded = true;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedVoice = (string)((ComboBoxItem)Voices.SelectedItem).Content;
        }

        private void RightDecisionResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_wereDecisionsAdded)
            {
                DecisionInfo.DecisionInfoText = "First add decisions!";
                return;
            }

            DecisionInfo.DecisionInfoText = "";
            _dialogGameEvent.RightDecisionResult = (DecisionResult)RightDecisionType.SelectedItem;
        }

        private void LeftDecisionResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_wereDecisionsAdded)
            {
                DecisionInfo.DecisionInfoText = "First add decisions!";
                return;
            }

            DecisionInfo.DecisionInfoText = "";
            _dialogGameEvent.LeftDecisionResult = (DecisionResult)LeftDecisionType.SelectedItem;
        }

    }
}
