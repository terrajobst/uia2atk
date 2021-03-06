// Permission is hereby granted, free of charge, to any person obtaining 
// a copy of this software and associated documentation files (the 
// "Software"), to deal in the Software without restriction, including 
// without limitation the rights to use, copy, modify, merge, publish, 
// distribute, sublicense, and/or sell copies of the Software, and to 
// permit persons to whom the Software is furnished to do so, subject to 
// the following conditions: 
//  
// The above copyright notice and this permission notice shall be 
// included in all copies or substantial portions of the Software. 
//  
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION 
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// 
// Copyright (c) 2008 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//	Brad Taylor <brad@getcoded.net>
// 

using System;
using System.Windows.Automation;
using Mono.UIAutomation.Winforms;
using SWF = System.Windows.Forms;
using System.Windows.Automation.Provider;

namespace Mono.UIAutomation.Winforms.Events
{
	internal class PartListItemAutomationHasKeyboardFocusPropertyEvent 
		: BaseAutomationPropertyEvent
	{
#region Constructors
		public PartListItemAutomationHasKeyboardFocusPropertyEvent (
			ListItemProvider provider,
			DateTimePickerProvider.DateTimePickerListPartProvider listProvider)
			: base (provider, 
			        AutomationElementIdentifiers.HasKeyboardFocusProperty)
		{
			this.listProvider = listProvider;
			this.listItemProvider = provider;
			
			isSelected = listProvider.IsItemSelected (listItemProvider);
		}
#endregion
		
#region IConnectable Overrides		 
		public override void Connect ()
		{
			((SWF.DateTimePicker) listProvider.Control)
				.ValueChanged += new EventHandler (OnValueChanged);
		}

		public override void Disconnect ()
		{
			((SWF.DateTimePicker) listProvider.Control)
				.ValueChanged -= new EventHandler (OnValueChanged);
		}
#endregion
		
#region Private Methods
		private void OnValueChanged (object sender, EventArgs args)
		{
			bool sel = listProvider.IsItemSelected (listItemProvider);
			if (sel != isSelected) {
				RaiseAutomationPropertyChangedEvent ();
				isSelected = sel;
			}
		}
#endregion 

#region Private Fields
		private bool isSelected = false;
		private ListItemProvider listItemProvider;
		private DateTimePickerProvider.DateTimePickerListPartProvider listProvider;
#endregion
	}
}
