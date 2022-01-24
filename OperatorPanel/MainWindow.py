import PyQt5.QtWidgets as qtw
import PyQt5.QtGui as qtg

from MesApiConnector import MesApiConnector

class MainWindow(qtw.QWidget):
    def __init__(self):
        super().__init__()
        self.setWindowTitle("OeeProductionPanel")
        self.setLayout(qtw.QGridLayout())

        mesApiConnector = MesApiConnector("localhost", "44379")

        okCountLabel_label = qtw.QLabel("OK: ")
        okCountLabel_label.setFont(qtg.QFont('Verdana', 24))
        okCountLabel_label.setMinimumSize(100, 50)
        self.layout().addWidget(okCountLabel_label, 0, 0)

        okCountLabelValue_label = qtw.QLabel("0")
        okCountLabelValue_label.setFont(qtg.QFont('Verdana', 24))
        okCountLabelValue_label.setMinimumSize(100, 50)
        self.layout().addWidget(okCountLabelValue_label, 0, 1)

        nokCountLabel_label = qtw.QLabel("NOK: ")
        nokCountLabel_label.setFont(qtg.QFont('Verdana', 24))
        nokCountLabel_label.setMinimumSize(100, 50)
        self.layout().addWidget(nokCountLabel_label, 0, 2)

        nokCountLabelValue_label = qtw.QLabel("0")
        nokCountLabelValue_label.setFont(qtg.QFont('Verdana', 24))
        nokCountLabelValue_label.setMinimumSize(100, 50)
        self.layout().addWidget(nokCountLabelValue_label, 0, 3)

        msgBox = qtw.QMessageBox()
        okCount = 0
        nokCount = 0
        try:
            oeeData = mesApiConnector.GetOeeData()
            stationSummary = mesApiConnector.GetPartCounters(oeeData)
            self.okCount = stationSummary.okCount
            self.nokCount = stationSummary.nokCount
            lastWorkTime = mesApiConnector.GetLastWorkStatus(oeeData)
            okCountLabelValue_label.setText(str(self.okCount))
            nokCountLabelValue_label.setText(str(self.nokCount))
        except Exception as ex:
            msgBox.setText("Error: "+ ex)

        addPart_label = qtw.QLabel("Dodaj Sztukę:")
        addPart_label.setFont(qtg.QFont('Verdana', 24))
        addPart_label.setMinimumSize(400, 50)
        self.layout().addWidget(addPart_label, 1, 0, 1, 4)

        dmc_label = qtw.QLabel("DMC: ")
        dmc_label.setFont(qtg.QFont('Verdana', 24))
        dmc_label.setMinimumSize(100, 50)
        self.layout().addWidget(dmc_label, 2, 0)

        dmc_textBox = qtw.QLineEdit()
        dmc_textBox.setObjectName("dmc_textContent") 
        dmc_textBox.setFont(qtg.QFont('Verdana', 24))
        dmc_textBox.setMinimumSize(300, 50)
        self.layout().addWidget(dmc_textBox, 2, 1, 1, 3)

        ok_button = qtw.QPushButton("OK",
            clicked = lambda: press_ok_button())
        ok_button.setFont(qtg.QFont('Verdana', 24))
        ok_button.setMinimumSize(200, 230)
        self.layout().addWidget(ok_button, 3, 0,4,2)

        nok_button = qtw.QPushButton("NOK",
            clicked = lambda: press_nok_button())
        nok_button.setFont(qtg.QFont('Verdana', 24))
        nok_button.setMinimumSize(200, 230)
        self.layout().addWidget(nok_button,3,2,4,2)

        workStatusChoose_label = qtw.QLabel("Stan Pracy Maszyny: ")
        workStatusChoose_label.setFont(qtg.QFont('Verdana', 24))
        workStatusChoose_label.setMinimumSize(400, 50)
        self.layout().addWidget(workStatusChoose_label, 0, 4, 1, 4)
    
        workButtonBehind_label = qtw.QLabel("")
        workButtonBehind_label.setStyleSheet("background-color: green;")
        workButtonBehind_label.setMinimumSize(400, 100)
        workButtonBehind_label.setVisible(False)
        self.layout().addWidget(workButtonBehind_label, 1,4,2,4)

        buttonContainerWork = qtw.QWidget()
        buttonContainerWork.setLayout(qtw.QGridLayout())
        buttonContainerWork.setContentsMargins(0,0,0,0)

        work_button = qtw.QPushButton("Praca",
            clicked = lambda: press_work_button())
        work_button.setFont(qtg.QFont('Verdana', 24))
        work_button.setMinimumSize(400, 100)
        buttonContainerWork.layout().addWidget(work_button,0,0)
        self.layout().addWidget(buttonContainerWork,1,4,2,4)

        breakButtonBehind_label = qtw.QLabel("")
        breakButtonBehind_label.setStyleSheet("background-color: orange;")
        breakButtonBehind_label.setMinimumSize(400, 100)
        breakButtonBehind_label.setVisible(False)
        self.layout().addWidget(breakButtonBehind_label, 3,4,2,4)

        buttonContainerBreak = qtw.QWidget()
        buttonContainerBreak.setLayout(qtw.QGridLayout())
        buttonContainerBreak.setContentsMargins(0,0,0,0)

        break_button = qtw.QPushButton("Przerwa",
            clicked = lambda: press_break_button())
        break_button.setFont(qtg.QFont('Verdana', 24))
        break_button.setMinimumSize(400, 100)
        buttonContainerBreak.layout().addWidget(break_button,0,0)
        self.layout().addWidget(buttonContainerBreak,3,4,2,4)

        alarmButtonBehind_label = qtw.QLabel("")
        alarmButtonBehind_label.setStyleSheet("background-color: red;")
        alarmButtonBehind_label.setMinimumSize(400, 100)
        alarmButtonBehind_label.setVisible(False)
        self.layout().addWidget(alarmButtonBehind_label, 5,4,2,4)

        buttonContainerAlarm = qtw.QWidget()
        buttonContainerAlarm.setLayout(qtw.QGridLayout())
        buttonContainerAlarm.setContentsMargins(0,0,0,0)

        alarm_button = qtw.QPushButton("Awaria",
            clicked = lambda: press_alarm_button())
        alarm_button.setFont(qtg.QFont('Verdana', 24))
        alarm_button.setMinimumSize(400, 100)
        buttonContainerAlarm.layout().addWidget(alarm_button,0,0)
        self.layout().addWidget(buttonContainerAlarm,5,4,2,4)

        workStatus = lastWorkTime.workStatus
        if (workStatus == 30):
            workButtonBehind_label.setVisible(True)
        if (workStatus == 40):
            breakButtonBehind_label.setVisible(True)
        if (workStatus == 20):
            alarmButtonBehind_label.setVisible(True)


        self.show()

        def press_ok_button():
            try:
                mesApiConnector.PutNewPart(1, dmc_textBox.text())
                self.okCount += 1
                okCountLabelValue_label.setText(str(self.okCount))
            except Exception as ex:
                msgBox.setText("Error: "+ ex)

        def press_nok_button():
            try:
                mesApiConnector.PutNewPart(2, dmc_textBox.text())
                self.nokCount += 1
                nokCountLabelValue_label.setText(str(self.nokCount))
            except Exception as ex:
                msgBox.setText("Error: "+ ex)

        def press_work_button():
            try:
                mesApiConnector.PutNewWorkStatus(30)
                workButtonBehind_label.setVisible(True)
                breakButtonBehind_label.setVisible(False)
                alarmButtonBehind_label.setVisible(False)
            except Exception as ex:
                msgBox.setText("Error: "+ ex)

        def press_break_button():
            try:
                mesApiConnector.PutNewWorkStatus(40)
                workButtonBehind_label.setVisible(False)
                breakButtonBehind_label.setVisible(True)
                alarmButtonBehind_label.setVisible(False)
            except Exception as ex:
                msgBox.setText("Error: "+ ex)

        def press_alarm_button():
            try:
                mesApiConnector.PutNewWorkStatus(20)
                workButtonBehind_label.setVisible(False)
                breakButtonBehind_label.setVisible(False)
                alarmButtonBehind_label.setVisible(True)
            except Exception as ex:
                msgBox.setText("Error: "+ ex)

app = qtw.QApplication([])
mw = MainWindow()

app.exec_()