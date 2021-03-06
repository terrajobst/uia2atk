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
// Copyright (c) 2009 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//      Mike Gorse <mgorse@novell.com>
// 

using System;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms;
using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	[TestFixture]
	public class TrackBarProviderTest : BaseProviderTest
	{
		#region Test
		
		[Test]
		public void BasicPropertiesTest ()
		{
			TrackBar trackBar = new TrackBar ();
			IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (trackBar);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.Slider.Id);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "slider");

			trackBar.Orientation = Orientation.Vertical;
			TestProperty (provider,
			              AutomationElementIdentifiers.OrientationProperty,
			              OrientationType.Vertical);
			trackBar.Orientation = Orientation.Horizontal;
			TestProperty (provider,
			              AutomationElementIdentifiers.OrientationProperty,
			              OrientationType.Horizontal);
		}
		
		[Test]
		public void ProviderPatternTest ()
		{
			TrackBar trackBar = new TrackBar ();
			IRawElementProviderSimple provider = 
				ProviderFactory.GetProvider (trackBar);
			
			object rangeValueProvider =
				provider.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (rangeValueProvider,
			                  "Not returning RangeValuePatternIdentifiers.");
			Assert.IsTrue (rangeValueProvider is IRangeValueProvider,
			               "Not returning RangeValuePatternIdentifiers.");
		}
		
		#endregion
		
		#region IRangeValueProvider Test
		
		[Test]
		public void IRangeValueProviderIsReadOnlyTest ()
		{
			TrackBar trackBar = new TrackBar ();
			IRawElementProviderSimple provider = 
				ProviderFactory.GetProvider (trackBar);
			
			IRangeValueProvider rangeValueProvider = (IRangeValueProvider)
				provider.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (rangeValueProvider,
			                  "Not returning RangeValuePatternIdentifiers.");
			
			Assert.AreNotEqual (trackBar.Enabled && trackBar.Visible,
			                 rangeValueProvider.IsReadOnly,
			                 "IsReadOnly value");
		}
		
		[Test]
		public void IRangeValueProviderMinimumTest ()
		{
			TrackBar trackBar = new TrackBar ();
			IRawElementProviderSimple provider = 
				ProviderFactory.GetProvider (trackBar);
			
			IRangeValueProvider rangeValueProvider = (IRangeValueProvider)
				provider.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (rangeValueProvider,
			                  "Not returning RangeValuePatternIdentifiers.");
			
			Assert.AreEqual ((double) trackBar.Minimum,
			                 rangeValueProvider.Minimum,
			                 "Minimum value");
		}
		
		[Test]
		public void IRangeValueProviderMaximumTest ()
		{
			TrackBar trackBar = new TrackBar ();
			IRawElementProviderSimple provider = 
				ProviderFactory.GetProvider (trackBar);
			
			IRangeValueProvider rangeValueProvider = (IRangeValueProvider)
				provider.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (rangeValueProvider,
			                  "Not returning RangeValuePatternIdentifiers.");
			
			Assert.AreEqual ((double) trackBar.Maximum,
			                 rangeValueProvider.Maximum,
			                 "Maximum value");
		}
		
		[Test]
		public void IRangeValueProviderLargeChangeTest ()
		{
			TrackBar trackBar = new TrackBar ();
			IRawElementProviderSimple provider = 
				ProviderFactory.GetProvider (trackBar);
			
			IRangeValueProvider rangeValueProvider = (IRangeValueProvider)
				provider.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (rangeValueProvider,
			                  "Not returning RangeValuePatternIdentifiers.");
			
			Assert.AreEqual ((double) trackBar.LargeChange,
			                 rangeValueProvider.LargeChange,
			                 "LargeChange value");
		}
		
		[Test]
		public void IRangeValueProviderSmallChangeTest ()
		{
			TrackBar trackBar = new TrackBar ();
			IRawElementProviderSimple provider = 
				ProviderFactory.GetProvider (trackBar);
			
			IRangeValueProvider rangeValueProvider = (IRangeValueProvider)
				provider.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (rangeValueProvider,
			                  "Not returning RangeValuePatternIdentifiers.");
			
			Assert.AreEqual ((double) trackBar.SmallChange,
			                 rangeValueProvider.SmallChange,
			                 "SmallChange value");
		}
		
		[Test]
		public void IRangeValueProviderValueTest ()
		{
			TrackBar trackBar = new TrackBar ();
			IRawElementProviderSimple provider = 
				ProviderFactory.GetProvider (trackBar);
			
			IRangeValueProvider rangeValueProvider = (IRangeValueProvider)
				provider.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id);
			Assert.IsNotNull (rangeValueProvider,
			                  "Not returning RangeValuePatternIdentifiers.");
			
			Assert.AreEqual ((double) trackBar.Value,
			                 rangeValueProvider.Value,
			                 "Value value");
		}
		
		#endregion
		
		#region Navigation Test

		[Test]
		public void NavigationTest ()
		{
			TrackBar trackBar = (TrackBar) GetControlInstance ();
			IRawElementProviderFragmentRoot rootProvider;
			IRawElementProviderFragment forwardButtonProvider;
			IRawElementProviderFragment thumbProvider;
			IRawElementProviderFragment backwardButtonProvider;
			
			rootProvider = (IRawElementProviderFragmentRoot) GetProviderFromControl (trackBar);
			
			forwardButtonProvider = rootProvider.Navigate (NavigateDirection.FirstChild);
			Assert.IsNotNull (forwardButtonProvider,
			                  "ForwardButton shouldn't be null.");
			Assert.IsNull (forwardButtonProvider.Navigate (NavigateDirection.PreviousSibling),
			               "ForwardButton should be the first child.");
			
			thumbProvider = forwardButtonProvider.Navigate (NavigateDirection.NextSibling);
			Assert.IsNotNull (thumbProvider,
			                  "Thumb shouldn't be null.");

			backwardButtonProvider = thumbProvider.Navigate (NavigateDirection.NextSibling);
			Assert.IsNotNull (backwardButtonProvider,
			                  "BackwardButton shouldn't be null.");
			Assert.IsNull (backwardButtonProvider.Navigate (NavigateDirection.NextSibling),
			               "BackwardButton should be the last child.");
			
			Assert.AreEqual (rootProvider,
			                 forwardButtonProvider.Navigate (NavigateDirection.Parent),
			                 "ForwardButton with different parent");
			Assert.AreEqual (rootProvider,
			                 forwardButtonProvider.FragmentRoot,
			                 "ForwardButton with different FragmentRoot");
			Assert.AreEqual (rootProvider,
			                 thumbProvider.Navigate (NavigateDirection.Parent),
			                 "Thumb with different parent");
			Assert.AreEqual (rootProvider,
			                 thumbProvider.FragmentRoot,
			                 "Thumb with different FragmentRoot");
			Assert.AreEqual (rootProvider,
			                 backwardButtonProvider.Navigate (NavigateDirection.Parent),
			                 "BackwardButton with different parent");
			Assert.AreEqual (rootProvider,
			                 backwardButtonProvider.FragmentRoot,
			                 "BackwardButton with different FragmentRoot");
			
			TestProperty (forwardButtonProvider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.Button.Id);
			TestProperty (forwardButtonProvider,
			              AutomationElementIdentifiers.IsContentElementProperty,
			              false);
			TestProperty (forwardButtonProvider,
			              AutomationElementIdentifiers.IsKeyboardFocusableProperty,
			              false);
			TestProperty (thumbProvider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.Thumb.Id);
			TestProperty (thumbProvider,
			              AutomationElementIdentifiers.IsContentElementProperty,
			              false);
			TestProperty (thumbProvider,
			              AutomationElementIdentifiers.IsKeyboardFocusableProperty,
			              false);
		}
		
		#endregion
		
		#region BaseProviderTest Overrides
		
		protected override Control GetControlInstance ()
		{
			return new TrackBar ();
		}
		
		#endregion
	}
}
