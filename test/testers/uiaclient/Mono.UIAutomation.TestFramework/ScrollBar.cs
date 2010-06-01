// ScrollBar.cs: ScrollBar control class wrapper.
//
// This program is free software; you can redistribute it and/or modify it under
// the terms of the GNU General Public License version 2 as published by the
// Free Software Foundation.
//
// This program is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more
// details.
//
// You should have received a copy of the GNU General Public License along with
// this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
//
// Copyright (c) 2010 Novell, Inc (http://www.novell.com)
//
// Authors:
//	Ray Wang  (rawang@novell.com)
//	Felicia Mu  (fxmu@novell.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Automation;

namespace Mono.UIAutomation.TestFramework
{
	public class ScrollBar : Element
	{
		public static readonly ControlType UIAType = ControlType.ScrollBar;
		
		//List the patterns that the control must support
		string[] SupportedPatterns = new string[] {};

		public ScrollBar (AutomationElement elm)
			: base (elm)
		{
		}

		#region RangeValue Pattern
		public void SetValue (double value)
		{
			SetValue (value, true);
		}

		public void SetValue (double value, bool log)
		{
			if (log)
				procedureLogger.Action (string.Format ("Set {0} to {1}.", value, this.NameAndType));

			RangeValuePattern rvp = (RangeValuePattern) element.GetCurrentPattern (RangeValuePattern.Pattern);
			rvp.SetValue (value);
		}

		public bool IsReadOnly {
			get { return (bool) element.GetCurrentPropertyValue (RangeValuePattern.IsReadOnlyProperty, true); }
		}

		public double LargeChange {
			get { return (double) element.GetCurrentPropertyValue (RangeValuePattern.LargeChangeProperty, true); }
		}

		public double SmallChange {
			get { return (double) element.GetCurrentPropertyValue (RangeValuePattern.SmallChangeProperty, true); }
		}

		public double Maximum {
			get { return (double) element.GetCurrentPropertyValue (RangeValuePattern.MaximumProperty, true); }
		}

		public double Minimum {
			get { return (double) element.GetCurrentPropertyValue (RangeValuePattern.MinimumProperty, true); }
		}

		public double Value {
			get { return (double) element.GetCurrentPropertyValue (RangeValuePattern.ValueProperty, true); }
		}
		#endregion

		#region Scroll Pattern
		public void Scroll (ScrollAmount horizontalAmount, ScrollAmount verticalAmount)
		{
			Scroll (horizontalAmount, verticalAmount, true);
		}

		public void Scroll (ScrollAmount horizontalAmount, ScrollAmount verticalAmount, bool log)
		{
			if (log)
				procedureLogger.Action (string.Format ("Scroll {0} horizontally and {1} vertically.",
				                                       horizontalAmount.ToString (),
				                                       verticalAmount.ToString ()));

			ScrollPattern sp = (ScrollPattern) element.GetCurrentPattern (ScrollPattern.Pattern);
			sp.Scroll (horizontalAmount, verticalAmount);
		}

		public void SetScrollPercent (double horizontalPercent, double verticalPercent)
		{
			SetScrollPercent (horizontalPercent, verticalPercent, true);
		}

		public void SetScrollPercent (double horizontalPercent, double verticalPercent, bool log)
		{
			if (log)
				procedureLogger.Action (string.Format ("Set {0} {1}% horizontally and {2}% vertically.",
				                                       this.NameAndType, horizontalPercent, verticalPercent));

			ScrollPattern sp = (ScrollPattern) element.GetCurrentPattern (ScrollPattern.Pattern);
			sp.SetScrollPercent (horizontalPercent, verticalPercent);
		}
		#endregion
	}
}