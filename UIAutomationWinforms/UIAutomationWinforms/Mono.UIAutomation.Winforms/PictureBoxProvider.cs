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
//	Mario Carrion <mcarrion@novell.com>
// 
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Forms;
using Mono.Unix;
using Mono.UIAutomation.Bridge;
using Mono.UIAutomation.Winforms.Behaviors.PictureBox;

namespace Mono.UIAutomation.Winforms
{

	[MapsComponent (typeof (PictureBox))]
	internal class PictureBoxProvider : FragmentRootControlProvider
	{

		#region Constructors

		public PictureBoxProvider (PictureBox pictureBox) : base (pictureBox)
		{
		}

		#endregion

		#region SimpleControlProvider: Specialization

		public override void Initialize ()
		{
			base.Initialize ();

			SetBehavior (EmbeddedImagePatternIdentifiers.Pattern,
			             new EmbeddedImageProviderBehavior (this));
		}

		#endregion

		#region SimpleControlProvider specialization

		// TODO: In Vista the PictureBox is defined as Pane Provider
		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.Image.Id;
			else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
				return false;
			else
				return base.GetProviderPropertyValue (propertyId);
		}
		#endregion
	}
}
