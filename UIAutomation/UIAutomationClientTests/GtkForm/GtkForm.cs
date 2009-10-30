using System;

using Gtk;

namespace GtkForm
{
	public class DemoMain
	{
		private Gtk.Window window;
		private Gtk.Label label1;
		private Gtk.Entry textBox1;
		private Gtk.TextView textBox3;
		private Gtk.TreeView treeView1;
		private Gtk.TreeView treeView2;
		private Gtk.ScaleButton scaleButton1;
		private Gtk.HBox hboxPanel;
		private Gtk.Entry textBoxExtra;
		private Gtk.Entry txtCommand;
		private Gtk.Button button1;

		public static void Main (string[] args)
		{
			Application.Init ();
			new DemoMain ();
			Application.Run ();
		}

		public DemoMain ()
		{
			window = new Gtk.Window ("Sample GTK form");
			Gtk.HBox hbox = new Gtk.HBox (false, 0);
			Gtk.HBox hbox1 = new Gtk.HBox (false, 0);
			Gtk.HBox hbox2 = new Gtk.HBox (false, 0);
			Gtk.HBox hbox3 = new Gtk.HBox (false, 0);
			hbox.Add (hbox1);
			window.SetDefaultSize (600, 400);
			window.DeleteEvent += new DeleteEventHandler (WindowDelete);

			button1 = new Gtk.Button ("button1");
			button1.Clicked += Button1Clicked;
			Gtk.Button button2 = new Gtk.Button ("button2");
			Gtk.Button button3 = new Gtk.Button ("button3");
			Gtk.Button button4 = new Gtk.Button ("button4");
			button4.Clicked += Button4Clicked;
			Gtk.Button button5 = new Gtk.Button ("button5");
			Gtk.Button button6 = new Gtk.Button ("button6");
			Gtk.Button button7 = new Gtk.Button ("button7");
			button7.Sensitive = false;

			scaleButton1 = new Gtk.ScaleButton (0, 0, 100, 10, new string [0]);

			hbox1.Add (hbox3);
			hbox1.Add (hbox2);
			hbox1.Add (button3);
			hbox1.Add (button2);

			button3.Accessible.Description = "help text 3";
			button3.Sensitive = false;

			label1 = new Gtk.Label ("label1");

			textBox1 = new Gtk.Entry ();
			Gtk.Entry textBox2 = new Gtk.Entry ();
			textBox2.Visibility = false;
			textBox2.Sensitive = false;
			textBox2.IsEditable = false;
			textBox3 = new Gtk.TextView ();
			// TODO: scrollbars
			Gtk.CheckButton checkbox1 = new Gtk.CheckButton ("checkbox1");
			Gtk.CheckButton checkbox2 = new Gtk.CheckButton ("checkbox2");
			checkbox2.Sensitive = false;

			Gtk.TreeStore store = new Gtk.TreeStore (typeof (string));
			Gtk.TreeIter [] iters = new Gtk.TreeIter [2];
			iters [0] = store.AppendNode ();
			store.SetValue (iters [0], 0, "item 1");
			iters [1] = store.AppendNode (iters [0]);
			store.SetValue (iters [1], 0, "item 1a");
			iters [0] = store.AppendNode ();
			store.SetValue (iters [0], 0, "item 2");
			iters [1] = store.AppendNode (iters [0]);
			store.SetValue (iters [1], 0, "item 2a");
			iters [1] = store.AppendNode (iters [0]);
			store.SetValue (iters [1], 0, "item 2b");
			treeView1 = new Gtk.TreeView (store);
			AddTreeViewColumn (treeView1, 0, "column 1");
			treeView1.ExpandAll ();

			store = new Gtk.TreeStore (typeof (string), typeof (string));
			iters [0] = store.AppendNode ();
			store.SetValue (iters [0], 0, "item1");
			store.SetValue (iters [0], 1, "item2");
			iters [0] = store.AppendNode ();
			store.SetValue (iters [0], 0, "item3");
			store.SetValue (iters [0], 1, "item4");
			treeView2 = new Gtk.TreeView (store);
			AddTreeViewColumn (treeView2, 0, "column1");
			AddTreeViewColumn (treeView2, 1, "column2");
			treeView2.Accessible.Name = "dataGridView1";

			hboxPanel = new Gtk.HBox ();
			Gtk.Button btnRemoveTextBox = new Gtk.Button ("Remove");
			btnRemoveTextBox.Clicked += RemoveTextBoxClicked;
			Gtk.Button btnAddTextBox = new Gtk.Button ("Add");
			btnAddTextBox.Clicked += AddTextBoxClicked;
			btnAddTextBox.Clicked += AddTextBoxClicked;
			txtCommand = new Gtk.Entry ();
			txtCommand.Accessible.Name = "txtCommand";
			Gtk.Button btnRun = new Gtk.Button ("Run");
			btnRun.Clicked += btnRunClicked;
			hboxPanel.Add (btnRemoveTextBox);
			hboxPanel.Add (btnAddTextBox);

			hbox2.Add (button5);
			hbox2.Add (checkbox1);
			hbox2.Add (checkbox2);
			hbox2.Add (button4);

			hbox3.Add (button7);
			hbox3.Add (button6);
			hbox3.Sensitive = false;

			hbox.Add (textBox3);
			hbox.Add (textBox2);
			hbox.Add (textBox1);
			hbox.Add (label1);
			hbox.Add (button1);
			hbox.Add (treeView1);
			hbox.Add (treeView2);
			hbox.Add (txtCommand);
			hbox.Add (btnRun);
			hbox.Add (hboxPanel);
			hbox.Add (scaleButton1);

			window.Add (hbox);
			window.ShowAll ();
		}

		private void AddTreeViewColumn (Gtk.TreeView treeView, int i, string name)
		{
			Gtk.TreeViewColumn col = new Gtk.TreeViewColumn ();
			col.Title = name;
			treeView.AppendColumn (col);
			Gtk.CellRendererText cell = new Gtk.CellRendererText ();
			col.PackStart (cell, true);
			col.AddAttribute (cell, "text", i);
		}

		private void Button1Clicked (object o, EventArgs args)
		{
			textBox1.Text = "button1_click";
			label1.Text = "button1_click";
		}

		private void Button4Clicked (object o, EventArgs args)
		{
			treeView1.Sensitive = !treeView1.Sensitive;
			treeView2.Sensitive = !treeView2.Sensitive;
			scaleButton1.Sensitive = !scaleButton1.Sensitive;
		}

		private void RemoveTextBoxClicked (object o, EventArgs args)
		{
			if (textBoxExtra == null)
				throw new Exception ("No textBox to remove");
			hboxPanel.Remove (textBoxExtra);
			textBoxExtra = null;
		}

		private void AddTextBoxClicked (object o, EventArgs args)
		{
			if (textBoxExtra != null)
				throw new Exception ("Adding more than one TextBox not supported");
			textBoxExtra = new Gtk.Entry ();
			hboxPanel.Add (textBoxExtra);
		}

		private void btnRunClicked (object o, EventArgs args)
		{
			const string sampleText = "Lorem ipsum dolor sit amet";

			string cmd = txtCommand.Text;
			if (cmd == "click button1")
				button1.Activate ();
			else if (cmd == "set textbox3 text")
				textBox3.Buffer.Text = sampleText;
			else if (cmd == "select textbox3") {
				if (textBox3.Buffer.Text.Length < 4)
					textBox3.Buffer.Text = sampleText;
				Gtk.TextIter startIter, endIter;
				textBox3.Buffer.GetSelectionBounds (out startIter, out endIter);
				int start, end;
				start = startIter.Offset;
				end = endIter.Offset;
				end = (end-start == 3? 4: 3);
				start = 0;
				startIter = textBox3.Buffer.GetIterAtOffset (start);
				endIter = textBox3.Buffer.GetIterAtOffset (end);
				textBox3.Buffer.MoveMark ("selection_bound", startIter);
				textBox3.Buffer.MoveMark ("insert", endIter);
			}
		}

		private void WindowDelete (object o, DeleteEventArgs args)
		{
			Application.Quit ();
			args.RetVal = true;
		}
	}
}