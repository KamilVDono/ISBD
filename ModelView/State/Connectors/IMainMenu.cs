using System;
using System.Collections.Generic;
using ISBD.Model;

namespace ISBD.ModelView.State
{
	public interface IMainMenu
	{
		void RegisterForSelectedUserChange(Action<OsobaModel> selectionAction);

		void UnregisterForSelectedUserChange(Action<OsobaModel> selectionAction);

		List<TransakcjaModel> Transactions { set; }

		List<OsobaModel> ValidUsers { set; }

	}
}