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
//      Brad Taylor <brad@getcoded.net>
//

using Atk;

using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

using Moonlight.AtkBridge;

namespace Moonlight.AtkBridge.ProviderImplementors
{
	[ImplementsPattern (PatternInterface.Value)]
	public class Value : Atk.TextImplementor
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

		IntPtr GLib.IWrapper.Handle {
			get { return IntPtr.Zero; }
		}
#endregion

#region Public Methods
		public Value (AutomationPeer peer)
		{
			this.peer = peer;
			this.valueProvider = (IValueProvider) peer.GetPattern (
				PatternInterface.Value);
		}

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

					return ReturnTextWrtOffset (startOffset);

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
						endOffset = offset;
					else
						endOffset = offset + 1;
					return ReturnTextWrtOffset (offset);

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
					startOffset = offset - 1;
					endOffset = offset;
					return ReturnTextWrtOffset (startOffset);

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

#region Protected Properties
		protected string Text {
			get { return valueProvider.Value; }
		}
#endregion

#region Protected Fields
		protected int selectionStartOffset = 0;
		protected int selectionEndOffset = 0;

		protected AutomationPeer peer;
#endregion

#region Private Methods
		private string ReturnTextWrtOffset (int startOffset, int endOffset)
		{
			//TODO: optimize?
			return Text.Substring (startOffset, endOffset - startOffset);
		}

		// endOffset == startOffset + 1
		private string ReturnTextWrtOffset (int startOffset)
		{
			//TODO: optimize?
			if ((startOffset < 0) || (startOffset > Text.Length))
				return String.Empty;
			return new String (new char[] { GetCharacterAtOffset (startOffset) });
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
			while (true) {
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
#endregion

#region Private Fields
		//TODO: use regexp?
		private static char [] wordSeparators = new char [] { ' ', '\n', '\r', '.', '\t' };
		private static char [] newLineSeparators = new char [] { '\n', '\r' };
		private static char [] sentenceSeparators = new char [] { '\n', '\r', '.' };
		private static char [] softSentenceSeparators = new char [] { '.', ':'};

		private IValueProvider valueProvider;
		private int caretOffset;
		private string deleteHack = null;
#endregion
	}
}
