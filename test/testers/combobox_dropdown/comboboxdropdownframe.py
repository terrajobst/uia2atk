
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        11/14/2008
# Description: combobox_dropdown.py wrapper script
#              Used by the combobox_dropdown-*.py tests
##############################################################################

import sys
import os
import actions
import states

from strongwind import *
from combobox_dropdown import *


# class to represent the main window.
class ComboBoxDropDownFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    LABEL1 = "You select 1"

    def __init__(self, accessible):
        super(ComboBoxDropDownFrame, self).__init__(accessible)
        self.label1 = self.findLabel(self.LABEL1)
        self.combobox = self.findComboBox(None)
        self.textbox = self.findText(None)

    #give 'click' action
    def click(self,accessible):
        accessible.click()

    #give 'press' action
    def press(self,accessible):
        accessible.press()
        sleep(config.SHORT_DELAY)

        procedurelogger.expectedResult('menu item list is showing')
        self.menu = self.findMenu(None)
        self.menuitem = dict([(x, self.findMenuItem(str(x))) for x in range(10)])

    #check the label after click listitem
    def assertLabel(self, itemname):
        procedurelogger.expectedResult('item "%s" is %s' % (itemname, 'select'))

        def resultMatches():
            return self.findLabel("You select %s" % itemname)
	assert retryUntilTrue(resultMatches)

    #assert Text implementation for MenuItem
    def assertItemText(self, textValue=None):
        procedurelogger.action('check MenuItem\'s Text Value')

        for textValue in range(10):
            procedurelogger.expectedResult('item "%s"\'s Text is %s' % (self.menuitem[textValue],textValue))
            assert self.menuitem[textValue].text == str(textValue)

    #assert Text value of TextBox after do click action
    def assertText(self, accessible, values):
        procedurelogger.expectedResult('the text of %s is %s' % (accessible,values))
        assert accessible.text == str(values)

    #assert Selection implementation for Menu
    def assertSelectionChild(self, accessible, childIndex):
        procedurelogger.action('selecte childIndex %s in "%s"' % (childIndex, accessible))

        accessible.selectChild(childIndex)

    def assertClearSelection(self, accessible):
        procedurelogger.action('clear selection in "%s"' % (accessible))

        accessible.clearSelection()

    #input value into text box
    def inputText(self, textbox, values):
        textbox.typeText(values)

    #enter Text Value for EditableText
    def enterTextValue(self, textbox, values):
        procedurelogger.action('in %s enter %s' % (textbox, values))
        textbox.text = values
    
    #close application main window after running test
    def quit(self):
        self.altF4()
