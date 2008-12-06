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
//	Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;
using SWF = System.Windows.Forms;

using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace Mono.UIAutomation.Winforms.Behaviors.TreeView
{
	internal class SelectionItemProviderBehavior : ProviderBehavior, ISelectionItemProvider
	{
		#region Private Members

		private TreeNodeProvider nodeProvider;

		#endregion
		
		#region Constructors
		
		public SelectionItemProviderBehavior (TreeNodeProvider nodeProvider) :
			base (nodeProvider)
		{
			this.nodeProvider = nodeProvider;
		}

		#endregion
		
		#region ISelectionItemProvider implementation 
		
		public void AddToSelection ()
		{
			IRawElementProviderSimple treeProvider =
				ProviderFactory.FindProvider (nodeProvider.TreeNode.TreeView);
			if (treeProvider == null)
				return;
			ISelectionProvider treeSelectionProvider = (ISelectionProvider)
				treeProvider.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id);
			if (treeSelectionProvider == null)
				return;

			if (!treeSelectionProvider.CanSelectMultiple &&
			    treeSelectionProvider.GetSelection ().Length > 0)
				throw new InvalidOperationException ("Cannot select multiple nodes.");

			Select ();
		}
		
		public void RemoveFromSelection ()
		{
			if (!IsSelected)
				return;
			throw new InvalidOperationException ("Cannot de-select node; selection required.");
		}
		
		public void Select ()
		{
			SWF.TreeView treeView = nodeProvider.TreeNode.TreeView;
			if (treeView.InvokeRequired) {
				treeView.BeginInvoke (new SWF.MethodInvoker (Select));
				return;
			}
			treeView.SelectedNode = nodeProvider.TreeNode;
		}
		
		public bool IsSelected {
			get {
				return nodeProvider.TreeNode.IsSelected;
			}
		}
		
		public IRawElementProviderSimple SelectionContainer {
			get {
				return ProviderFactory.FindProvider (nodeProvider.TreeNode.TreeView);
			}
		}
		
		#endregion 
		
		#region ProviderBehavior Overrides

		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == SelectionItemPatternIdentifiers.IsSelectedProperty.Id)
				return IsSelected;
			else if (propertyId == SelectionItemPatternIdentifiers.SelectionContainerProperty.Id)
				return SelectionContainer;
			return base.GetPropertyValue(propertyId);
		}

		public override void Connect ()
		{
			// TODO
		}

		public override void Disconnect ()
		{
			// TODO
		}

		public override AutomationPattern ProviderPattern {
			get {
				return SelectionItemPatternIdentifiers.Pattern;
			}
		}

		#endregion
	}
}
