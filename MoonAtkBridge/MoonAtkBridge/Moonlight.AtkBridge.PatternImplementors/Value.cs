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
//      Andrés G. Aragoneses <aaragoneses@novell.com>
//      Mario Carrion <mcarrion@novell.com>
//      Brad Taylor <brad@getcoded.net>
//

using Atk;

using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

using Moonlight.AtkBridge;

namespace Moonlight.AtkBridge.PatternImplementors
{
	[ImplementsPattern (PatternInterface.Value)]
	public sealed class Value : BasePatternImplementor,
	                            Atk.TextImplementor,
	                            Atk.EditableTextImplementor
	{
#region Public Properties
		public int CaretOffset {
			get { return caretOffset; }
		}

		public int CharacterCount {
			get { return Text.Length; }
		}

		public Atk.Attribute[] DefaultAttributes {
			get { return new Atk.Attribute [0]; }
		}

		public int NSelections {
			get { return 0; }
		}

		public string TextContents {
			set {
				if (valueProvider.IsReadOnly)
					return;

				try {
					valueProvider.SetValue (value);
				} catch (ElementNotEnabledException) {
					return;
				}
			}
		}

#endregion

#region Public Methods
		public Value (Adapter adapter, AutomationPeer peer)
			: base (adapter, peer)
		{
			this.valueProvider = (IValueProvider) peer.GetPattern (
				PatternInterface.Value);

			adapter.AutomationPropertyChanged
				+= new EventHandler<AutomationPropertyChangedEventArgs> (
					OnAutomationPropertyChanged);
		}

		public override void OnRefStateSet (ref Atk.StateSet states)
		{
			if (valueProvider.IsReadOnly)
				states.RemoveState (Atk.StateType.Editable);
			else
				states.AddState (Atk.StateType.Editable);
		}

#region Atk.TextImplementor Methods
		public bool AddSelection (int startOffset, int endOffset)
		{
			selectionStartOffset = 0;
			selectionEndOffset = 0;
			return false;
		}

		public TextRange GetBoundedRanges (TextRectangle rect,
		                                   CoordType coordType,
		                                   TextClipType xClipType,
		                                   TextClipType yClipType)
		{
			Log.Warn ("GetBoundedRanges not implemented");
			return new Atk.TextRange ();
		}

		public char GetCharacterAtOffset (int offset)
		{
			string text = Text;
			if ((offset >= text.Length) || (offset < 0) || (String.IsNullOrEmpty (text)))
				return '\0';
			return text [offset];
		}

		public void GetCharacterExtents (int offset,
		                                 out int x, out int y,
		                                 out int width, out int height,
		                                 CoordType coords)
		{
			Atk.TextRectangle rect;
			GetRangeExtents (offset, offset, coords, out rect);

			x = rect.X;
			y = rect.Y;
			width = rect.Width;
			height = rect.Height;
		}

		public int GetOffsetAtPoint (int x, int y, CoordType coords)
		{
			Log.Warn ("GetOffsetAtPoint not implemented");
			return -1;
		}

		public void GetRangeExtents (int startOffset, int endOffset,
		                             CoordType coordType,
		                             out TextRectangle rect)
		{
			int length = Text.Length;
			System.Windows.Rect bounds = peer.GetBoundingRectangle ();

			rect.X = (int) (bounds.X + (bounds.Width * startOffset) / length);
			rect.Y = (int) bounds.Y;
			rect.Height = (int) bounds.Height;
			rect.Width = (int) (bounds.Width * (endOffset - startOffset)) / length;

/* TODO:
			if (coordType == Atk.CoordType.Window)
				resource.ConvertCoords (ref rect.X, ref rect.Y, false);
*/
		}

		public Atk.Attribute[] GetRunAttributes (int offset,
		                                         out int startOffset,
		                                         out int endOffset)
		{
			startOffset = endOffset = 0;
			return new Atk.Attribute [0];
		}

		public string GetSelection (int selectionNum,
		                            out int startOffset,
		                            out int endOffset)
		{
			startOffset = selectionStartOffset;
			endOffset = selectionEndOffset;
			return null;
		}

		public string GetText (int startOffset, int endOffset)
		{
			string text = (deleteHack != null) ? deleteHack : Text;
			if ((endOffset == -1) || (endOffset > text.Length))
				endOffset = text.Length;

			if (endOffset < startOffset)
				return String.Empty;

			return text.Substring (startOffset, endOffset - startOffset);
		}

		public string GetTextAfterOffset (int offset,
		                                  TextBoundary boundaryType,
		                                  out int startOffset,
		                                  out int endOffset)
		{
			string text = Text;

			startOffset = 0;
			endOffset = 0;
			try {
				switch (boundaryType) {
				case Atk.TextBoundary.Char:

					endOffset = startOffset = offset + 1;
					if (startOffset > text.Length)
						endOffset = startOffset = text.Length;
					else if (endOffset + 1 <= text.Length)
						endOffset++;

					return ReturnTextWrtOffset (startOffset, endOffset);

				case Atk.TextBoundary.LineEnd:
					ForwardToNextSeparator (newLineSeparators, offset, out startOffset, out endOffset);
					endOffset = ForwardToNextSeparator (newLineSeparators, endOffset, true);
					return ReturnTextWrtOffset (startOffset, endOffset);

				case Atk.TextBoundary.LineStart:
					startOffset = ForwardToNextSeparator (newLineSeparators, offset, false);
					endOffset = ForwardToNextSeparator (newLineSeparators, startOffset, false);
					return ReturnTextWrtOffset (startOffset, endOffset);

				case Atk.TextBoundary.WordEnd:
					ForwardToNextSeparator (wordSeparators, offset, out startOffset, out endOffset);
					endOffset = ForwardToNextSeparator (wordSeparators, endOffset, true);
					return ReturnTextWrtOffset (startOffset, endOffset);

				case Atk.TextBoundary.WordStart:
					startOffset = ForwardToNextSeparator (wordSeparators, offset, false);
					endOffset = ForwardToNextSeparator (wordSeparators, startOffset, false);
					return ReturnTextWrtOffset (startOffset, endOffset);

				case Atk.TextBoundary.SentenceEnd:
					ForwardToNextSeparator (sentenceSeparators, offset, out startOffset, out endOffset, true);
					endOffset = ForwardToNextSeparator (sentenceSeparators, endOffset, true);
					int testStartOffset, nextStartOffset, testEndOffset, nextEndOffset;
					ForwardToNextSeparator(softSentenceSeparators, startOffset, out testStartOffset, out nextStartOffset);
					if (testStartOffset == startOffset)
						startOffset = nextStartOffset;
					ForwardToNextSeparator(softSentenceSeparators, startOffset, out testEndOffset, out nextEndOffset);
					if (testEndOffset == endOffset)
						endOffset = nextEndOffset;

					return ReturnTextWrtOffset (startOffset, endOffset);

				case Atk.TextBoundary.SentenceStart:
					startOffset = ForwardToNextSeparator (sentenceSeparators, offset, false);
					endOffset = ForwardToNextSeparator (sentenceSeparators, startOffset, false);

					endOffset = ForwardToNextSeparator (wordSeparators, endOffset, false);

					return ReturnTextWrtOffset (startOffset, endOffset);

				default:
					throw GetNotSupportedBoundary (boundaryType);
				}
			} catch (System.IndexOutOfRangeException) {
				return String.Empty;
			} finally {
				selectionStartOffset = startOffset;
				selectionEndOffset = endOffset;
			}
		}

		public string GetTextAtOffset (int offset,
		                               TextBoundary boundaryType,
		                               out int startOffset,
		                               out int endOffset)
		{
			string text = Text;

			startOffset = 0;
			endOffset = 0;
			try {
				switch (boundaryType) {
				case Atk.TextBoundary.WordEnd:
					startOffset = BackwardToNextSeparator (wordSeparators, offset, false);
					endOffset = ForwardToNextSeparator (wordSeparators, offset, true);
					return ReturnTextWrtOffset (startOffset, endOffset);

				case Atk.TextBoundary.WordStart:
					startOffset = BackwardToNextSeparator (wordSeparators, offset, true);
					endOffset = ForwardToNextSeparator (wordSeparators, offset, false);
					return ReturnTextWrtOffset (startOffset, endOffset);

				case Atk.TextBoundary.LineEnd:
					startOffset = BackwardToNextSeparator (newLineSeparators, offset, false);
					endOffset = ForwardToNextSeparator (newLineSeparators, offset, true);
					return ReturnTextWrtOffset (startOffset, endOffset);

				case Atk.TextBoundary.LineStart:
					startOffset = BackwardToNextSeparator (newLineSeparators, offset, true);
					endOffset = ForwardToNextSeparator (newLineSeparators, offset, false);
					return ReturnTextWrtOffset (startOffset, endOffset);

				case Atk.TextBoundary.SentenceEnd:
					startOffset = BackwardToNextSeparator (sentenceSeparators, offset, false);
					endOffset = ForwardToNextSeparator (sentenceSeparators, offset, true);

					int testEndOffset, nextEndOffset;
					ForwardToNextSeparator(softSentenceSeparators, startOffset, out testEndOffset, out nextEndOffset);
					if (testEndOffset == endOffset)
						endOffset = nextEndOffset;

					return ReturnTextWrtOffset (startOffset, endOffset);

				case Atk.TextBoundary.SentenceStart:
					startOffset = BackwardToNextSeparator (sentenceSeparators, offset, true);
					endOffset = ForwardToNextSeparator (sentenceSeparators, offset, false);
					return ReturnTextWrtOffset (startOffset, endOffset);

				case Atk.TextBoundary.Char:
					startOffset = offset;
					if (startOffset < 0) {
						startOffset = text.Length;
						endOffset = text.Length;
					}
					else if (offset >= text.Length)
						startOffset = endOffset = text.Length;
					else
						endOffset = offset + 1;
					return ReturnTextWrtOffset (startOffset, endOffset);

				default:
					throw GetNotSupportedBoundary (boundaryType);
				}
			} catch (System.IndexOutOfRangeException) {
				return String.Empty;
			} finally {
				selectionStartOffset = startOffset;
				selectionEndOffset = endOffset;
			}
		}

		public string GetTextBeforeOffset (int offset,
		                                   TextBoundary boundaryType,
		                                   out int startOffset,
		                                   out int endOffset)
		{
			startOffset = 0;
			endOffset = 0;
			try {
				switch (boundaryType) {
				case Atk.TextBoundary.WordEnd:
					endOffset = BackwardToNextSeparator (wordSeparators, offset, false);
					startOffset = BackwardToNextSeparator (wordSeparators, endOffset, false);
					return ReturnTextWrtOffset (startOffset, endOffset);

				case Atk.TextBoundary.WordStart:
					BackwardToNextSeparator (wordSeparators, offset, out endOffset, out startOffset);
					startOffset = BackwardToNextSeparator (wordSeparators, startOffset, true);
					return ReturnTextWrtOffset (startOffset, endOffset);

				case Atk.TextBoundary.LineEnd:
					endOffset = BackwardToNextSeparator (newLineSeparators, offset, false);
					startOffset = BackwardToNextSeparator (newLineSeparators, endOffset, false);
					return ReturnTextWrtOffset (startOffset, endOffset);

				case Atk.TextBoundary.LineStart:
					BackwardToNextSeparator (newLineSeparators, offset, out endOffset, out startOffset);
					startOffset = BackwardToNextSeparator (newLineSeparators, startOffset, true);
					return ReturnTextWrtOffset (startOffset, endOffset);

				case Atk.TextBoundary.Char:
					offset = Clamp (offset, 0, Text.Length);
					startOffset = Clamp (offset - 1, 0, Text.Length);
					endOffset = Clamp (offset, 0, Text.Length);
					return ReturnTextWrtOffset (startOffset, endOffset);

				case Atk.TextBoundary.SentenceEnd:
					endOffset = BackwardToNextSeparator (sentenceSeparators, offset, false);
					startOffset = BackwardToNextSeparator (sentenceSeparators, endOffset, false);
					return ReturnTextWrtOffset (startOffset, endOffset);

				case Atk.TextBoundary.SentenceStart:
					BackwardToNextSeparator (sentenceSeparators, offset, out endOffset, out startOffset);
					startOffset = BackwardToNextSeparator (sentenceSeparators, startOffset, true);
					return ReturnTextWrtOffset (startOffset, endOffset);

				default:
					throw GetNotSupportedBoundary (boundaryType);
				}
			} catch (System.IndexOutOfRangeException) {
				return String.Empty;
			} finally {
				selectionStartOffset = startOffset;
				selectionEndOffset = endOffset;
			}
		}

		public bool RemoveSelection (int selectionNum)
		{
			return false;
		}

		public bool SetCaretOffset (int offset)
		{
			caretOffset = offset;
			return true;
		}

		public bool SetSelection (int selectionNum,
		                          int startOffset, int endOffset)
		{
			return false;
		}
#endregion

#region Atk.TextImplementor Methods
		public bool SetRunAttributes (GLib.SList attribSet, int startOffset, int endOffset)
		{
			// ValuePattern doesn't support text attributes
			return false;
		}

		public void InsertText (string str, ref int position)
		{
			if (valueProvider.IsReadOnly)
				return;

			string text = Text;
			if (position < 0 || position > text.Length)
				position = text.Length;   // gail

			TextContents = text.Substring (0, position)
				+ str + text.Substring (position);
		}

		public void CopyText (int startPos, int endPos)
		{
			Log.Warn ("ValuePattern doesn't support clipboard actions");
		}

		public void CutText (int startPos, int endPos)
		{
			Log.Warn ("ValuePattern doesn't support clipboard actions");
		}

		public void DeleteText (int startPos, int endPos)
		{
			if (valueProvider.IsReadOnly)
                                return;

			string text = Text;
                        if (String.IsNullOrEmpty (text))
                                return;

                        if (startPos < 0)
                                startPos = 0;
                        if (endPos < 0 || endPos > text.Length)
                                endPos = text.Length;
                        if (startPos > endPos)
                                startPos = endPos;

                        TextContents = text.Remove (startPos,
			                            endPos - startPos);
		}

		public void PasteText (int position)
		{
			Log.Warn ("ValuePattern doesn't support clipboard actions");
		}
#endregion

#endregion

#region Private Methods
		private void OnAutomationPropertyChanged (object o, AutomationPropertyChangedEventArgs args)
		{
			if (args.Property == ValuePatternIdentifiers.ValueProperty) {
				AutomationControlType controlType
					= peer.GetAutomationControlType ();
				if (controlType == AutomationControlType.Edit
				    || controlType == AutomationControlType.Document) {
					TextAdapter textAdapter
						= new TextAdapter ((TextImplementor) adapter);

					string oldValue = (string) args.OldValue;
					string newValue = (string) args.NewValue;

					if (oldValue == newValue)
						return;

					if (newValue.StartsWith (oldValue)) {
						textAdapter.EmitTextChanged (Atk.TextChangedDetail.Insert,
									     oldValue.Length,
						                             newValue.Length - oldValue.Length);
					} else if (oldValue.StartsWith (newValue)) {
						textAdapter.EmitTextChanged (Atk.TextChangedDetail.Delete,
									     newValue.Length,
						                             oldValue.Length - newValue.Length);
					} else {
						// XXX: Improve this algorithm so that
						// it diffs the two values (maybe using
						// a modified Levenshtein alg?) and
						// send just what is deleted or
						// inserted to avoid thrashing ATs.
						textAdapter.EmitTextChanged (Atk.TextChangedDetail.Delete,
									     0, oldValue.Length);

						textAdapter.EmitTextChanged (Atk.TextChangedDetail.Insert,
									     0, newValue.Length);
					}
				} else {
					// Mirror Gtk+'s behavior here somewhat
					// and only fire visible_data_changed
					// in the case of label and label-like
					// elements.
					adapter.EmitSignal ("visible_data_changed");
				}
			}
		}

		private string ReturnTextWrtOffset (int startOffset, int endOffset)
		{
			//TODO: optimize?
			return Text.Substring (startOffset, endOffset - startOffset);
		}

		private void ForwardToNextSeparator (char[] seps,
		                                     int startOffset,
		                                     out int stopEarlyOffset,
		                                     out int stopLateOffset)
		{
			ForwardToNextSeparator (seps, startOffset,
			                        out stopEarlyOffset,
			                        out stopLateOffset, false);
		}

		private void ForwardToNextSeparator (char[] seps, int startOffset,
		                                     out int stopEarlyOffset,
		                                     out int stopLateOffset,
		                                     bool findNonSeparators)
		{
			string text = Text;
			int retOffset = startOffset;
			bool anyNonSeparator = false;
			while (retOffset < text.Length) {
				bool isSep = CharEqualsAny (text [retOffset], seps);
				if (!isSep)
					anyNonSeparator = true;

				if (!isSep || (findNonSeparators && !anyNonSeparator))
					retOffset++;
				else
					break;
			}

			stopEarlyOffset = retOffset;
			while (retOffset < text.Length && CharEqualsAny (text [retOffset], seps))
				retOffset++;
			stopLateOffset = retOffset;
		}

		private int ForwardToNextSeparator (char[] seps,
		                                    int startOffset,
		                                    bool stopEarly)
		{
			string text = Text;
			int retOffset = startOffset;
			if (retOffset >= text.Length)
				return text.Length;

			while (!CharEqualsAny (text [retOffset], seps)) {
				if (retOffset == text.Length - 1)
					return text.Length;
				retOffset++;
			}

			if (stopEarly)
				return retOffset;

			while ((retOffset < text.Length) && CharEqualsAny (text [retOffset], seps))
				retOffset++;
			return retOffset;
		}

		private void BackwardToNextSeparator (char[] seps,
		                                      int startOffset,
		                                      out int stopEarlyOffset,
		                                      out int stopLateOffset)
		{
			if (startOffset <= 1) {
				stopEarlyOffset = 0;
				stopLateOffset = 0;
				return;
			}

			int retOffset = startOffset - 1;
			string text = Text;

			while (!CharEqualsAny (text [retOffset], seps)) {
				retOffset--;
				if (retOffset < 0)
					break;
			}

			stopEarlyOffset = retOffset + 1;
			if (retOffset < 0) {
				stopLateOffset = 0;
				return;
			}

			while (CharEqualsAny (text [retOffset], seps)) {
				retOffset--;
				if (retOffset < 0)
					break;
			}

			stopLateOffset = retOffset + 1;
			return;
		}

		private int BackwardToNextSeparator (char[] seps,
		                                     int startOffset,
		                                     bool stopEarly)
		{
			if (startOffset <= 1)
				return 0;

			int retOffset = startOffset - 1;
			string text = Text;

			while (!CharEqualsAny (text [retOffset], seps)) {
				retOffset--;
				if (retOffset < 0)
					break;
			}

			if (stopEarly)
				return retOffset + 1;
			else if (retOffset < 0)
				return 0;

			while (CharEqualsAny (text [retOffset], seps)) {
				retOffset--;
				if (retOffset < 0)
					break;
			}

			return retOffset + 1;
		}

		private bool CharEqualsAny (char boilerPlate, char [] candidates)
		{
			foreach (char candidate in candidates)
				if (boilerPlate == candidate)
					return true;
			return false;
		}

		private NotSupportedException GetNotSupportedBoundary (Atk.TextBoundary bType)
		{
			return new NotSupportedException (
				String.Format ("The value {0} is not supported as a Atk.TextBoundary type.",
					bType));
		}

		private bool IsAddition (string super, string sub, out int offset)
		{
			int startOff = 0;
			int subLength = sub.Length;

			while (startOff < subLength && super [startOff] == sub [startOff])
				startOff++;

			if (startOff == subLength) {
				offset = startOff;
				return true;
			}

			int endOff = subLength - 1;
			int diff = super.Length - subLength;
			while (endOff >= startOff && sub [endOff] == super [endOff + diff])
				endOff--;
			offset = startOff;

			return (startOff > endOff);
		}

		private int Clamp (int val, int min, int max)
		{
			return Math.Min (Math.Max (val, min), max);
		}
#endregion

#region Private Properties
		private string Text {
			get {
				try {
					return valueProvider.Value;
				} catch (InvalidOperationException) {
					return String.Empty;
				}
			}
		}
#endregion

#region Private Fields
		//TODO: use regexp?
		private static char [] wordSeparators = new char [] { ' ', '\n', '\r', '.', '!', ',', '?', '\t' };
		private static char [] newLineSeparators = new char [] { '\n', '\r' };
		private static char [] sentenceSeparators = new char [] { '\n', '\r', '.' };
		private static char [] softSentenceSeparators = new char [] { '.', ':'};

		private IValueProvider valueProvider;
		private int caretOffset;
		private string deleteHack = null;

		private int selectionStartOffset = 0;
		private int selectionEndOffset = 0;
#endregion
	}
}
