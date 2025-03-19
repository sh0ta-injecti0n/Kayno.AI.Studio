
# CLR統合設定
import clr
clr.AddReference('System.Windows.Forms')
from System.Windows.Forms import MessageBox

def show_message():
    MessageBox.Show('test: IronPythonからメッセージ！')

show_message()

