
##############################################################################
# Written by:  Neville Gao  <nevillegao@gmail.com>
# Date:        2009/09/21
# Description: progressbar.py wrapper script
#              Used by the progressbar-*.py tests
##############################################################################

from strongwind import *
from progressbar import *

# class to represent the main window.
class ProgressBarFrame(accessibles.Frame):

    def __init__(self, accessible):
        super(ProgressBarFrame, self).__init__(accessible)
        self.frame = self.findDocumentFrame('ProgressBarSample')
        self.filler = self.frame.findFiller('Silverlight Control')
        self.label = self.filler.findLabel(None)
        self.button = self.filler.findPushButton(None)
        self.progressBar = self.filler.findProgressBar('')

    def assertValue(self, expected_value):
        """
        Make sure the value of progressBar is expected
        """
        procedurelogger.expectedResult("update progressBar's value to %s" % expected_value)

        assert  self.progressBar.value == expected_value,\
                "actual value is %s, expected value is %s" % \
                (self.progressBar.value, expected_value)

    def setValue(self, value):
        """
        Set progressBar's value
        """
        procedurelogger.action("set progressBar's value to %s" % value)

        self.progressBar.value = value
