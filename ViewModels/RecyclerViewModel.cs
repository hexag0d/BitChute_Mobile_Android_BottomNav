using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.ViewModels;
using System;
using System.Windows.Input;


/// <summary>
/// the idea here is to convert the app over to mvvm eventually
/// I might exclude this but leaving for now.  this file is a sandbox, essentially
/// </summary>
namespace TextExample.Core.ViewModels
{
    public class TextViewModel : MvxViewModel
    {
        private string editText = String.Empty;
        private string copyText = String.Empty;

        #region Constructor
        public TextViewModel() { }
        #endregion

        #region Properties
        public string EditText
        {
            get { return this.editText; }
            set
            {
                this.editText = value;
                this.CopyText = this.editText;
                this.RaisePropertyChanged(() => EditText);
            }
        }

        public string CopyText
        {
            get { return this.copyText; }
            set
            {
                this.copyText = value;
                this.RaisePropertyChanged(() => CopyText);
            }
        }
        #endregion

        #region Methods
        private void ClearTextValue()
        {
            this.EditText = String.Empty;
        }
        #endregion

        #region Commands
        public ICommand ResetTextCommand
        {
            get { return new MvxCommand(() => ClearTextValue()); }
        }
        #endregion
    }
}