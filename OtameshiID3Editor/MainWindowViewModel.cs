using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Aldentea.ID3Editor.Otameshi
{
	using Mvvm;

	public class MainWindowViewModel : INotifyPropertyChanged
	{
		public MainWindowViewModel()
		{
			_loadCommand = new DelegateCommand(Load_Executed);
			_updateCommand = new DelegateCommand(Update_Executed, Update_CanExecute);
			Tag = null;
		}

		#region プロパティ

		#region *Titleプロパティ
		public string Title
		{
			get
			{
				return Tag != null ? Tag.Title : string.Empty;
			}
			set
			{
				if (!CannotEdit && Title != value)
				{
					Tag.Title = value;
					NotifyPropertyChanged();
				}
			}
		}
		#endregion

		#region *Artistプロパティ
		public string Artist
		{
			get
			{
				return Tag != null ? Tag.Artist : string.Empty;
			}
			set
			{
				if (!CannotEdit && Artist != value)
				{
					Tag.Artist = value;
					NotifyPropertyChanged();
				}
			}
		}
		IID3Tag _tag;
		#endregion

		IID3Tag Tag
		{
			get
			{
				return _tag;
			}
			set
			{
				_tag = value;
				NotifyPropertyChanged("Title");
				NotifyPropertyChanged("Artist");
				NotifyPropertyChanged("CannotEdit");
			}
		}

		public bool CannotEdit
		{
			get
			{
				return _tag == null;
			}
		}


		#region *CurrentFileNameプロパティ
		public string CurrentFileName
		{
			get
			{
				return _currentFileName;
			}
			set
			{
				if (CurrentFileName != value)
				{
					_currentFileName = value;
					NotifyPropertyChanged();
					((DelegateCommand)UpdateCommand).RaiseCanExecuteChanged();
				}
			}
		}
		string _currentFileName = string.Empty;
		#endregion

		#endregion

		#region コマンド

		public ICommand LoadCommand
		{
			get
			{
				return _loadCommand;
			}
		}
		ICommand _loadCommand;

		void Load_Executed(object parameter)
		{
			var dialog = new Microsoft.Win32.OpenFileDialog();

			if (dialog.ShowDialog() == true)
			{
				Tag = ID3Editor.ReadFile(dialog.FileName);
				this.CurrentFileName = dialog.FileName;
			}
		}

		public ICommand UpdateCommand
		{
			get
			{
				return _updateCommand;
			}
		}
		ICommand _updateCommand;

		async void Update_Executed(object parameter)
		{
			await ID3Editor.UpdateAsync(CurrentFileName, Tag);
		
		}

		bool Update_CanExecute(object parameter)
		{
			return !string.IsNullOrEmpty(this.CurrentFileName);
		}

		#endregion

		#region INotifyPropertyChanged実装

		public event PropertyChangedEventHandler PropertyChanged = delegate { };

		protected void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

	}
}
