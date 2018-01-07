using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace client
{
    /// <summary>
    /// ContractCompiler.xaml 的交互逻辑
    /// </summary>
    public partial class ContractCompiler : Page
    {
        public ContractCompiler()
        {
            InitializeComponent();
        }
        public void ClearLog()
        {
            Action safelog = () =>
            {
                this.listDebug.Items.Clear();
            };
            this.Dispatcher.Invoke(safelog);
        }
        public void Log(string log)
        {
            Action<string> safelog = (_log) =>
            {
                this.listDebug.Items.Add(_log);
            };
            this.Dispatcher.Invoke(safelog, log);
        }
        public class HighlightCurrentLineBackgroundRenderer : IBackgroundRenderer
        {
            private TextEditor _editor;

            public HighlightCurrentLineBackgroundRenderer(TextEditor editor)
            {
                _editor = editor;
            }

            public KnownLayer Layer
            {
                get { return KnownLayer.Selection; }
            }

            public void Draw(TextView textView, DrawingContext drawingContext)
            {
                if (_editor.Document == null)
                    return;

                textView.EnsureVisualLines();
                var currentLine = _editor.Document.GetLineByOffset(_editor.CaretOffset);
                foreach (var rect in BackgroundGeometryBuilder.GetRectsForSegment(textView, currentLine))
                {
                    drawingContext.DrawRectangle(
                        new SolidColorBrush(Color.FromArgb(0x40, 0, 0, 0xFF)), null,
                        new Rect(rect.Location, new Size(textView.ActualWidth - 32, rect.Height)));
                }
            }
        }
        Result buildResult = null;
        ThinNeo.Debug.Helper.AddrMap debugInfo = null;

        public class Result
        {
            public string script_hash;
            public string srcfile;
            public byte[] avm;
            public string debuginfo;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ICSharpCode.AvalonEdit.TextEditor code = codeEdit;
            codeEdit.TextArea.TextView.BackgroundRenderers.Add(
    new HighlightCurrentLineBackgroundRenderer(code));
            codeEdit.TextArea.Caret.PositionChanged += (s, ee) =>
              {
                  if (this.debugInfo == null)
                      return;
                  var pos = codeEdit.CaretOffset;
                  var line = codeEdit.Document.GetLineByOffset(pos).LineNumber;
                  var addr = this.debugInfo.GetAddrBack(line);
                  if (addr >= 0)
                  {
                      foreach (ThinNeo.Compiler.Op item in this.listASM.Items)
                      {
                          if (item != null && item.addr == addr)
                          {
                              this.listASM.SelectedItem = item;
                              this.listASM.ScrollIntoView(item);
                              break;
                          }
                      }
                  }
              };
        }
        public static byte[] HexString2Bytes(string str)
        {
            if (str.IndexOf("0x") == 0)
                str = str.Substring(2);
            byte[] outd = new byte[str.Length / 2];
            for (var i = 0; i < str.Length / 2; i++)
            {
                outd[i] = byte.Parse(str.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return outd;
        }
        void updateASM(string hex)
        {
            listASM.Items.Clear();
            //build asm
            ThinNeo.Compiler.Op[] ops = null;
            try
            {
                var data = HexString2Bytes(hex);
                ops = ThinNeo.Compiler.Avm2Asm.Trans(data);
                foreach (var op in ops)
                {
                    var str = "";
                    try
                    {
                        str = op.ToString();
                    }
                    catch
                    {
                        str = "op fail:";
                    }
                    listASM.Items.Add(str);
                }
            }
            catch (Exception err)
            {
            }

        }
        private void listASM_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var op = this.listASM.SelectedItem as ThinNeo.Compiler.Op;
            if (op == null) return;
            var line = this.debugInfo.GetLineBack(op.addr);
            textAsm.Text = "srcline=" + line;
            if (line > 0)
            {
                var ioff = this.codeEdit.Document.Lines[line - 1].Offset;
                var len = this.codeEdit.Document.Lines[line - 1].Length;
                this.codeEdit.CaretOffset = ioff;
                //this.codeEdit.Select(ioff, 0);
                this.codeEdit.ScrollToLine(line - 1);
                codeEdit.TextArea.TextView.InvalidateLayer(KnownLayer.Background);
            }

        }
    }
}
