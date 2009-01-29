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
//      Sandy Armstrong <sanfordarmstrong@gmail.com>
//      Andrés G. Aragoneses <aaragoneses@novell.com>
//      Calvin Gaisford <cgaisford@novell.com>
// 

using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{
	public class Button : ComponentAdapter, Atk.ActionImplementor, Atk.TextImplementor, Atk.ImageImplementor
	{
		private static string default_invoke_description = "Sends a request to activate a control and initiate its single, unambiguous action.";
		private static string default_invoke_name = "click";

		private IInvokeProvider				invokeProvider;
		private IToggleProvider				toggleProvider;
		private string						actionDescription = null;
		protected string					actionName = null;
		
		private ITextImplementor textExpert = null;
		private int selectionStartOffset = 0, selectionEndOffset = 0;
		
		// UI Automation Properties supported
		// AutomationElementIdentifiers.AcceleratorKeyProperty.Id
		// AutomationIdProperty() ?
		// AutomationElementIdentifiers.BoundingRectangleProperty.Id
		// AutomationElementIdentifiers.ClickablePointProperty.Id
		// AutomationElementIdentifiers.ControlTypeProperty.Id
		// AutomationElementIdentifiers.HelpTextProperty.Id
		// AutomationElementIdentifiers.IsContentElementProperty.Id
		// AutomationElementIdentifiers.IsControlElementProperty.Id
		// AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id
		// AutomationElementIdentifiers.LabeledByProperty.Id
		// AutomationElementIdentifiers.LocalizedControlTypeProperty.Id
		// AutomationElementIdentifiers.NameProperty.Id
		public Button (IRawElementProviderSimple provider) : base (provider)
		{
			invokeProvider = (IInvokeProvider)provider.GetPatternProvider(InvokePatternIdentifiers.Pattern.Id);
			imageImplementor = new ImageImplementorHelper (this);
			InitializeAdditionalProviders ();
			if (invokeProvider != null) {
				//it seems the default description should be null:
				//actionDescription = default_invoke_description;
				actionName = default_invoke_name;
				Role = Atk.Role.PushButton;
			}

			textExpert = TextImplementorFactory.GetImplementor (this, provider);
		}
		
		protected virtual void InitializeAdditionalProviders ()
		{
		}

		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();
			
			return states;
		}

		
		// Return the number of actions (Read-Only)
		// Both IInvokeProvider and IToggleProvider have only one action
		public int NActions
		{
			get {
				return 1;
			}
		}
		
		// Get a localized name for the specified action
		public string GetLocalizedName (int action)
		{
			if (action != 0)
				return null;

			// TODO: Localize the name?
			return actionName;
		}
		
		// Sets a description of the specified action
		public bool SetDescription (int action, string description)
		{
			if (action != 0)
				return false;
			
			actionDescription = description;
			return true;
		}
		
		// Get the key bindings for the specified action
		public string GetKeybinding (int action)
		{
			string keyBinding = null;
			
			if (action != 0)
				return keyBinding;

			keyBinding = (string) 
			  Provider.GetPropertyValue (AutomationElementIdentifiers.AccessKeyProperty.Id);

			if (!string.IsNullOrEmpty (keyBinding))
				keyBinding = keyBinding.Replace ("Alt+", "<Alt>");
			
			return keyBinding;
		}
		// Get the name of the specified action
		public virtual string GetName (int action)
		{
			if (action != 0)
				return null;

			return actionName;
		}
		
		// Get the description of the specified action
		public string GetDescription (int action)
		{
			if (action != 0)
				return null;

			return actionDescription;
		}

		// Perform the action specified
		public virtual bool DoAction (int action)
		{
			if (invokeProvider != null) {
				if (action != 0)
					return false;

				OnPressed ();
				try {
					invokeProvider.Invoke ();
				} catch (ElementNotEnabledException) {
					return false;
				}
				OnReleased ();
				
				return true;
			}
			return false;
		}
		
		public int CaretOffset {
			get {
				return 0;
			}
		}

		public Atk.Attribute [] DefaultAttributes {
			get {
				throw new NotImplementedException ();
			}
		}

		public int CharacterCount {
			get {
				return textExpert.Length;
			}
		}

		public int NSelections {
			get {
				return -1;
			}
		}
		
		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			if (eventId == InvokePatternIdentifiers.InvokedEvent) {
				// TODO: send signal to ATK
			} else if (eventId == AutomationElementIdentifiers.AutomationFocusChangedEvent) {
				// TODO: Handle AutomationFocusChangedEvent
			} else if (eventId == AutomationElementIdentifiers.StructureChangedEvent) {
				// TODO: Handle StructureChangedEvent
			}
			base.RaiseAutomationEvent (eventId, e);
		}

		public override void RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{
			if (e.Property == TogglePatternIdentifiers.ToggleStateProperty) {
				//if it's a toggle, it should not be a basic Button class, but CheckBox or other
				throw new NotSupportedException ("Toggle events should not land here (should not be reached)");
			} else if (e.Property == AutomationElementIdentifiers.BoundingRectangleProperty) {
				// TODO: Handle BoundingRectangleProperty change
			}
			else
				base.RaiseAutomationPropertyChangedEvent (e);
		}
		
		protected override void UpdateNameProperty (string newName)
		{
			base.UpdateNameProperty (newName);

			Atk.TextAdapter adapter = new Atk.TextAdapter (this);

			// First delete all text, then insert the new text
			adapter.EmitTextChanged (Atk.TextChangedDetail.Delete, 0, textExpert.Length);

			adapter.EmitTextChanged (Atk.TextChangedDetail.Insert, 0,
						 newName == null ? 0 : newName.Length);
			EmitVisibleDataChanged ();
		}
		
		// TODO: although UIA doesn't cover press and release actions, figure out if maybe it's useful to
		// notify the state change, regardless of its actual non-effect
		private void OnPressed ()
		{
			NotifyStateChange (Atk.StateType.Armed, true);
		}
		private void OnReleased ()
		{
			NotifyStateChange (Atk.StateType.Armed, false);
		}
		
		public string GetText (int startOffset, int endOffset)
		{
			return textExpert.GetText (startOffset, endOffset);
		}

		public string GetTextAfterOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			string ret = textExpert.GetTextAfterOffset (offset, boundaryType, out startOffset, out endOffset);
			selectionStartOffset = startOffset;
			selectionEndOffset = endOffset;
			return ret;
		}

		public string GetTextAtOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			string ret = textExpert.GetTextAtOffset (offset, boundaryType, out startOffset, out endOffset);
			selectionStartOffset = startOffset;
			selectionEndOffset = endOffset;
			return ret;
		}

		public string GetTextBeforeOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			string ret = textExpert.GetTextBeforeOffset (offset, boundaryType, out startOffset, out endOffset);
			selectionStartOffset = startOffset;
			selectionEndOffset = endOffset;
			return ret;
		}
		
		public char GetCharacterAtOffset (int offset)
		{
			return textExpert.GetCharacterAtOffset (offset);
		}

		public Atk.Attribute [] GetRunAttributes (int offset, out int startOffset, out int endOffset)
		{
			return textExpert.GetRunAttributes (offset, out startOffset, out endOffset);
		}

		public void GetCharacterExtents (int offset, out int x, out int y, out int width, out int height, Atk.CoordType coords)
		{
			textExpert.GetCharacterExtents (offset, out x, out y, out width, out height, coords);
		}

		public int GetOffsetAtPoint (int x, int y, Atk.CoordType coords)
		{
			throw new NotImplementedException ();
		}

		public string GetSelection (int selectionNum, out int startOffset, out int endOffset)
		{
			startOffset = selectionStartOffset;
			endOffset = selectionEndOffset;
			return null;
		}

		public bool AddSelection (int startOffset, int endOffset)
		{
			throw new NotImplementedException ();
		}

		public bool RemoveSelection (int selectionNum)
		{
			return false;
		}

		public bool SetSelection (int selectionNum, int startOffset, int endOffset)
		{
			return false;
		}

		public bool SetCaretOffset (int offset)
		{
			return false;
		}

		public void GetRangeExtents (int startOffset, int endOffset, Atk.CoordType coordType, out Atk.TextRectangle rect)
		{
			textExpert.GetRangeExtents (startOffset, endOffset, coordType, out rect);
		}

		public Atk.TextRange GetBoundedRanges (Atk.TextRectangle rect, Atk.CoordType coordType, Atk.TextClipType xClipType, Atk.TextClipType yClipType)
		{
			throw new NotImplementedException ();
		}


#region ImageImplementor implementation 

		protected ImageImplementorHelper imageImplementor;
		
		public string ImageDescription
		{
			get { return imageImplementor.ImageDescription; }
		}
		
		public void GetImageSize (out int width, out int height)
		{
			imageImplementor.GetImageSize (out width, out height);
		}
		
		public void GetImagePosition (out int x, out int y, Atk.CoordType coordType)
		{
			imageImplementor.GetImagePosition (out x, out y, coordType);
		}
		
		public bool SetImageDescription (string description)
		{
			return imageImplementor.SetImageDescription (description);
		}
		
		public string ImageLocale 
		{
			get { return imageImplementor.ImageLocale; }
		}
		
#endregion


	}
}
