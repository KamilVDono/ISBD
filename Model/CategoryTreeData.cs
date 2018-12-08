using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISBD.Model
{
	public class TreeData : INotifyPropertyChanged
	{
		public Action<TreeData> OnSelectionChange { get; set; }

		public TreeData Parent { get; set; }

		public ObservableCollection<CategoryTreeData> Children { get; set; }
		private bool _Selected = true;

		public bool Selected
		{
			get => _Selected;
			set
			{
				_Selected = value;
				OnSelectionChange?.Invoke(this);
				this.NotifyPropertyChanged("Selected");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void NotifyPropertyChanged(string propName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
		}
	}
	public class CategoryTreeData: TreeData
	{
		public KategoriaModel Category { get; set; }
	}

	public class MainTreeCategoryData: TreeData
	{
		public string Name { get; set; }
	}
}
