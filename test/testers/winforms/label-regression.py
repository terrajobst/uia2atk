#!/usr/bin/env python

##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/07/2008
# Description: Test accessibility of label widget 
#              Use the labelframe.py wrapper script
#              Test the samples/winforms/button_label_linklabel.py script
##############################################################################

# The docstring below  is used in the generated log file
"""
Test accessibility of label widget
"""

# imports
import sys
import os

from strongwind import *
from label import *
from helpers import *
from sys import argv
from os import path

app_path = None 
try:
  app_path = argv[1]
except IndexError:
  pass #expected

# open the label sample application
try:
  app = launchLabel(app_path)
except IOError, msg:
  print "ERROR:  %s" % msg
  exit(2)

sleep(config.SHORT_DELAY)

# make sure we got the app back
if app is None:
  exit(4)

# just an alias to make things shorter
lFrame = app.labelFrame

#check sensitive Label's default states
statesCheck(lFrame.sensitive_label, "Label")

#check insensitive Label's default states
statesCheck(lFrame.insensitive_label, "Label", 
                        invalid_states=["enabled","sensitive"])

#click button2 to change label text
lFrame.button2.click()
sleep(config.SHORT_DELAY)
lFrame.assertLabel('You have clicked me 1 times')

#click button2 again to change label's text value
lFrame.button2.click()
sleep(config.SHORT_DELAY)
lFrame.assertText('You have clicked me 2 times')

#check sensitive lable's states again after update text
statesCheck(lFrame.sensitive_label, "Label")

# make sure the label of a MessageBox is accessible
lFrame.button1.click()
sleep(config.SHORT_DELAY)
lFrame.assertMessageBox()
lFrame.assertMessageBoxText("successful clicked me")

# close the MessageBox
lFrame.message_box.altF4()
sleep(config.SHORT_DELAY)

print "INFO:  Log written to: %s" % config.OUTPUT_DIR

#close application frame window
lFrame.quit()
