#!/usr/bin/env ipy

##############################################################################
# Written by:  Calen Chen <cachen@novell.com>
# Date:        06/02/2009
# Description: the sample for winforms control:
#              ToolBarButton
##############################################################################

# The docstring below is used in the generated log file
"""
This sample will show "ToolBarButton" controls in the form.It can be 
used for Autotest tools(e.g. Strongwind) to test the behaviors of controls.
"""

# imports
import os
import System
from sys import path
from os.path import exists


import clr
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Windows.Forms import *
from System.Drawing import *

harness_dir = path[0]
i = harness_dir.rfind("/")
uiaqa_path = harness_dir[:i]

class ToolBarSample(Form):
    """ToolBar control class"""

    def __init__(self):
        """ToolBarSample class init function."""

        # setup form
        self.Text = "ToolBar control"
        self.Height = 200 
        self.Width = 400

        # setup label
        self.label = Label()
        self.label.Text = "ToolBar and ToolBarButton example"
        self.label.AutoSize = True
        self.label.Height = 200 
        self.label.Width = self.Width - 10
        self.label.Location = Point (10, 80)

        # Create and initialize the ToolBar and ToolBarButton controls.
        self.toolbar = ToolBar()
        self.toolbar.ButtonClick += self.on_click

        # image list
        self.imagelist = ImageList()
        self.imagelist.ColorDepth = ColorDepth.Depth32Bit;
        self.imagelist.ImageSize = Size(32, 32)

        # small images
        names = [
                "abiword_48.png",
                "bmp.png",
                "disks.png",
                "evolution.png"
            ]

        for i in names:
            self.imagelist.Images.Add (Image.FromFile("%s/samples/listview-items-icons/32x32/" % uiaqa_path + i))

        self.toolbar.ImageList = self.imagelist

        # setup toolbarbuttons
        self.count = 0
        self.toolbar_btn1 = ToolBarButton()
        self.toolbar_btn1.Text = "PushButton"
        self.toolbar_btn1.ImageIndex = 0
        self.toolbar_btn1.Tag = 0

        self.toolbar_btn2 = ToolBarButton()
        self.toolbar_btn2.Text = "DropDownButton"
        self.toolbar_btn2.ImageIndex = 1
        self.toolbar_btn2.Style = ToolBarButtonStyle.DropDownButton
        self.menu = ContextMenu()
        self.toolbar_btn2.DropDownMenu = self.menu
        self.mi0 = self.menu.MenuItems.Add("Red")
        self.mi0.Click += self.cc
        self.mi1 = self.menu.MenuItems.Add("Blue")
        self.mi1.Click += self.cc

        self.toolbar_btn3 = ToolBarButton()
        self.toolbar_btn3.Text = "Toggle"
        self.toolbar_btn3.ImageIndex = 2
        self.toolbar_btn3.Style = ToolBarButtonStyle.ToggleButton

        self.toolbar_btn4 = ToolBarButton()
        self.toolbar_btn4.Text = "nop"
        self.toolbar_btn4.ImageIndex = 3
        self.toolbar_btn4.Enabled = False

        self.toolbar_btn5 = ToolBarButton()
        self.toolbar_btn5.Style = ToolBarButtonStyle.Separator

        #create label1
        self.label1 = Label()
        self.label1.Text = "page:"
        self.label1.Size = Size(50,18)
        #self.label1.TextAlign = ContentAlignment.MiddleLeft;
        self.label1.Dock = DockStyle.Right

        #setup combobox
        self.combobox = ComboBox()
        self.combobox.Size = Size(50,18)
        self.combobox.Dock = DockStyle.Right
        self.combobox.DropDownStyle = ComboBoxStyle.DropDownList 
        # add items in ComboBox
        for i in range(10):
            self.combobox.Items.Add(str(i))
        self.combobox.SelectedIndexChanged += self.select
        
        # create dialogs
        self.openfiledialog = OpenFileDialog()
        self.printdialog = PrintDialog()

        # add controls
        self.toolbar.Buttons.Add(self.toolbar_btn1)
        self.toolbar.Buttons.Add(self.toolbar_btn2)
        self.toolbar.Buttons.Add(self.toolbar_btn3)
        self.toolbar.Buttons.Add(self.toolbar_btn4)
        self.toolbar.Buttons.Add(self.toolbar_btn5)
        self.toolbar.Controls.Add(self.label1)
        self.toolbar.Controls.Add(self.combobox)
        self.Controls.Add(self.toolbar)
        self.Controls.Add(self.label)

    # PushButton style click event
    def on_click(self, sender, event):
        btn = self.toolbar.Buttons.IndexOf(event.Button)
        if btn == 0:
            self.count += 1
            self.label.Text = "You clicked PushButton %s times" % self.count
        elif btn == 2:
            if event.Button.Pushed:
                self.label.Enabled = False
            else:
                self.label.Enabled = True

    # DropDownButton style click event
    def cc(self, sender, event):
        if sender is self.mi0:
            self.label.Text = "You selected dropdownbutton item %s" % "Red"
            self.label.BackColor = Color.Red
        if sender is self.mi1:
            self.label.Text = "You selected dropdownbutton item %s" % "Blue"
            self.label.BackColor = Color.Blue

    # ComboBox click event
    def select(self, sender, event):
        """select a item"""
        self.label.Text = "You select combobox item %s" % self.combobox.Text

# run application
form = ToolBarSample()
Application.EnableVisualStyles()
Application.Run(form)
