using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.DataTransfer;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

// 빈 페이지 항목 템플릿에 대한 설명은 http://go.microsoft.com/fwlink/?LinkId=234238에 나와 있습니다.

namespace JonginLeeTool
{
    /// <summary>
    /// 자체에서 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private string mvvmup;
        private string mvvmdown;
        private string mvvmall;
        ObservableCollection<string> list = new ObservableCollection<string>();
        DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };

        public MainPage()
        {
            this.InitializeComponent();
            PropertyTypeComboBox.SelectedIndex = 0;
            OutputTypeButton1.IsChecked = true;
            DataMemberButton1.IsChecked = true;
            BindingListBox.ItemsSource = list;

            timer.Tick += timer_Tick;
            timer.Start();
        }

        void timer_Tick(object sender, object e)
        {



        }

        /// <summary>
        /// 이 페이지가 프레임에 표시되려고 할 때 호출됩니다.
        /// </summary>
        /// <param name="e">페이지에 도달한 방법을 설명하는 이벤트 데이터입니다. Parameter
        /// 속성은 일반적으로 페이지를 구성하는 데 사용됩니다.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void PropertyTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;

            if (cb != null)
            {
                switch (cb.SelectedIndex)
                {
                    case 0: // Auto
                        PropertyTypeTextBox.Text = "string";
                        break;
                    case 1: //Visible
                        PropertyTypeTextBox.Text = "int";
                        break;
                    case 2: // Hidden
                        PropertyTypeTextBox.Text = "bool";
                        break;
                    case 3: // Disabled
                        PropertyTypeTextBox.Text = "Datetime";
                        break;
                    case 4: // Disabled
                        PropertyTypeTextBox.Text = "";
                        break;
                    default:
                        PropertyTypeTextBox.Text = "";
                        break;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OutputTextblock.Text = GetMVVMResult();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DataPackage dataPackage = new DataPackage();
            dataPackage.RequestedOperation = DataPackageOperation.Copy;
            dataPackage.SetText(GetMVVMResult());
            Clipboard.SetContent(dataPackage);
        }


        private string GetMVVMResult()
        {
            string input = PropertyNameTextBox.Text;
            string output = "";
            mvvmdown = "";
            mvvmup = "";
            string outputTypeB = "";

            string datamemberinput = "";

            if (DataMemberButton2.IsChecked == true)
                datamemberinput = "[DataMember]\n";
            if (DataMemberButton3.IsChecked == true)
                datamemberinput = "[IgnoreDataMember]\n";


            if(input != null && input != "")
            {
                string[] parse = input.Split(',');


                for (int i = 0; i < parse.Length; i++)
                {
                    string parseinput = parse[i].Trim();

                    if (parseinput != "")
                    {
                        bool listcheck = false;
                        foreach (var data in list)
                        {
                            if (data == parseinput)
                                listcheck = true;
                        }

                        if (listcheck == false)
                            list.Add(parseinput);

                        string varinput = "private " + PropertyTypeTextBox.Text + " _" + parseinput + ";\n";
                        string propertyinput = datamemberinput + "public " + PropertyTypeTextBox.Text + " " + parseinput + "\n" +
                                     "{\n" +
                                     "\tget\n" +
                                     "\t{\n" +
                                     "\t\treturn _" + parseinput + ";\n" +
                                     "\t}\n" +
                                     "\tset\n" +
                                     "\t{\n" +
                                     "\t\tif (_" + parseinput + " != value)\n" +
                                     "\t\t{\n" +
                                     "\t\t\t_" + parseinput + " = value;\n" +
                                     "\t\t\t" + RaiseTypeTextBox.Text + "(\"" + parseinput + "\");\n" +
                                     "\t\t}\n" +
                                     "\t}\n" +
                                     "}\n\n"
                                     ;
                        mvvmup = mvvmup + varinput;
                        mvvmdown = mvvmdown + propertyinput;

                        outputTypeB = outputTypeB + varinput + propertyinput + "\n";
                    }
                }

            }
            if (OutputTypeButton1.IsChecked == true)
                output = mvvmup + "\n" + mvvmdown;
            else
                output = outputTypeB;

            return output;
        }

        private void PropertyNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            OutputPanelTestVisible();
        }

        private void PropertyTypeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            OutputPanelTestVisible();
        }

        private void RaiseTypeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            OutputPanelTestVisible();
        }

        private void OutputPanelTestVisible()
        {
            if (PropertyNameTextBox.Text.Trim() != "" && RaiseTypeTextBox.Text.Trim() != "" && PropertyTypeTextBox.Text.Trim() != "")
            {
                OutputPanel.IsHitTestVisible = true;
            }
            else
                OutputPanel.IsHitTestVisible = false;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            GetMVVMResult();
            DataPackage dataPackage = new DataPackage();
            dataPackage.RequestedOperation = DataPackageOperation.Copy;
            dataPackage.SetText(mvvmup);
            Clipboard.SetContent(dataPackage);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            GetMVVMResult();
            DataPackage dataPackage = new DataPackage();
            dataPackage.RequestedOperation = DataPackageOperation.Copy;
            dataPackage.SetText(mvvmdown);
            Clipboard.SetContent(dataPackage);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string copytext = "{Binding " + button.Tag + "}";
            DataPackage dataPackage = new DataPackage();
            dataPackage.RequestedOperation = DataPackageOperation.Copy;

            dataPackage.SetText(copytext);
            Clipboard.SetContent(dataPackage);
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            list.Clear();
        }

        private void PropertyNameTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {

            if (e.Key.Equals(Windows.System.VirtualKey.Enter))
            {
                OutputTextblock.Text = GetMVVMResult();

                DataPackage dataPackage = new DataPackage();
                dataPackage.RequestedOperation = DataPackageOperation.Copy;
                dataPackage.SetText(GetMVVMResult());
                Clipboard.SetContent(dataPackage);
            }
        }

        private async void Button_Click_6(object sender, RoutedEventArgs e)
        {
            await TranslateTask();
        }

        private async Task TranslateTask()
        {
            string enginput = TranslateEnglishTextBox.Text;
            string korinput = TranslateKoreanTextBox.Text;
            TranslateTextBoxInitialize();
            await Translate("en", "zh-CN", enginput, TranslateChineseTextBox);
            await Translate("en", "zh-TW", enginput, TranslateTaiwanTextBox);
            await Translate("en", "es", enginput, TranslateSpanishTextBox);
            await Translate("en", "pt", enginput, TranslatePortugueseTextBox);
            await Translate("en", "ru", enginput, TranslateRussianTextBox);
            await Translate("ko", "ja", korinput, TranslateJapanTextBox);
            await Translate("en", "de", enginput, TranslateGermanTextBox);
            await Translate("en", "fr", enginput, TranslateFranceTextBox);
            await Translate("en", "it", enginput, TranslateItalyTextBox);
        }

        private void TranslateTextBoxInitialize()
        {
            TranslateChineseTextBox.Text = "";
            TranslateTaiwanTextBox.Text = "";
            TranslateSpanishTextBox.Text = "";
            TranslatePortugueseTextBox.Text = "";
            TranslateRussianTextBox.Text = "";
            TranslateJapanTextBox.Text = "";
            TranslateGermanTextBox.Text = "";
            TranslateFranceTextBox.Text = "";
            TranslateItalyTextBox.Text = "";
        }

        private async Task Translate(string fromcode, string contrycode, string input, TextBox output)
        {
            HttpClient http = new System.Net.Http.HttpClient();
            HttpResponseMessage response = await http.GetAsync("http://translate.google.com/translate_a/t?client=t&hl=ko&sl=" + fromcode + "&tl=" + contrycode + "&ie=UTF-8&oe=UTF-8&multires=1&prev=btn&ssel=5&tsel=5&sc=1&q=" + input);
            await DisplayTextResult(response, output);

        }


        private async Task DisplayTextResult(HttpResponseMessage response, TextBox output)
        {
            string responseBodyAsText;

            // We cast the StatusCode to an int so we display the numeric value (e.g., "200") rather than the
            // name of the enum (e.g., "OK") which would often be redundant with the ReasonPhrase.
            output.Text = "";
            responseBodyAsText = await response.Content.ReadAsStringAsync();

            string aa = "[[";
            int a1 = responseBodyAsText.IndexOf(aa) + aa.Length;
            int a2 = responseBodyAsText.IndexOf("]]", a1);
            string all = responseBodyAsText.Substring(a1, a2 - a1);

            int z = 0;
            string saying = "";
            for (int i = 0; i < 10; i++)
            {
                string bb = "[\"";
                if( all.IndexOf(bb, z) != -1)
                {
                    int b1 = all.IndexOf(bb, z) + bb.Length;
                    int b2 = all.IndexOf("\",\"", b1);
                    string word = all.Substring(b1, b2-b1);
                    if (i == 0)
                        saying = word;
                    else
                        saying = saying + " " + word;

                    z = b2;
                }
            }

            saying = ReplaceLetter(saying, "\\u003e", ">");
            saying = ReplaceLetter(saying, "\\u003c", "<");

            output.Text += saying;
        }

        private string getTranslateCode(int languageIndex)
        {
            string result = "";
            string output = "";

            if (languageIndex == 0)//영어
                result = TranslateEnglishTextBox.Text;
            if (languageIndex == 1)//한국어
                result = TranslateKoreanTextBox.Text;
            if (languageIndex == 2)//중국어[간체]
                result = TranslateChineseTextBox.Text;
            if (languageIndex == 3)//중국어[번체]
                result = TranslateTaiwanTextBox.Text;
            if (languageIndex == 4)//스페인어
                result = TranslateSpanishTextBox.Text;
            if (languageIndex == 5)//포르투갈어
                result = TranslatePortugueseTextBox.Text;
            if (languageIndex == 6)//러시아어
                result = TranslateRussianTextBox.Text;
            if (languageIndex == 7)//일본어
                result = TranslateJapanTextBox.Text;
            if (languageIndex == 8)//독일어
                result = TranslateGermanTextBox.Text;
            if (languageIndex == 9)//프랑스어
                result = TranslateFranceTextBox.Text;
            if (languageIndex == 10)//이탈리아어
                result = TranslateItalyTextBox.Text;

            output = "if (languageIndex == " + languageIndex + ")"+getLanguageKoreanName(languageIndex)+"\n" +
                    "\treturn \"" + result + "\";";

            return output;
        }

        private string getLanguageKoreanName(int languageIndex)
        {
            string result = "";
            string output = "";

            if (languageIndex == 0)//영어
                result = "//영어";
            if (languageIndex == 1)//한국어
                result = "//한국어";
            if (languageIndex == 2)//중국어[간체]
                result = "//중국어[간체]";
            if (languageIndex == 3)//중국어[번체]
                result = "//중국어[번체]";
            if (languageIndex == 4)//스페인어
                result = "//스페인어";
            if (languageIndex == 5)//포르투갈어
                result = "//포르투갈어";
            if (languageIndex == 6)//러시아어
                result = "//러시아어";
            if (languageIndex == 7)//일본어
                result = "//일본어";
            if (languageIndex == 8)//독일어
                result = "//독일어";
            if (languageIndex == 9)//프랑스어
                result = "//프랑스어";
            if (languageIndex == 10)//이탈리아어
                result = "//이탈리아어";

            return result;
        }

        private string getTranslateAllCode()
        {
            string output = "";
            output = "public static string " + TranslatePropertyNameTextBox.Text + "\n" +
                    "{\n" +
                    "\tget\n" +
                    "\t{\n" +
                    "\t\t" + getTranslateCode(0) + "\n" +
                    "\t\t" + getTranslateCode(1) + "\n" +
                    "\t\t" + getTranslateCode(2) + "\n" +
                    "\t\t" + getTranslateCode(3) + "\n" +
                    "\t\t" + getTranslateCode(4) + "\n" +
                    "\t\t" + getTranslateCode(5) + "\n" +
                    "\t\t" + getTranslateCode(6) + "\n" +
                    "\t\t" + getTranslateCode(7) + "\n" +
                    "\t\t" + getTranslateCode(8) + "\n" +
                    "\t\t" + getTranslateCode(9) + "\n" +
                    "\t\t" + getTranslateCode(10) + "\n" +
                    "\t\treturn \"" + TranslateEnglishTextBox.Text + "\";\n" +
                    "\t}\n" +
                    "}\n";

            return output;
        }

        private void CopyToClipboard(string text)
        {
            if (text != "" && text != null)
            {
                DataPackage dataPackage = new DataPackage();
                dataPackage.RequestedOperation = DataPackageOperation.Copy;
                dataPackage.SetText(text);
                Clipboard.SetContent(dataPackage);
            }
        }

        private void LanButton2_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard(getTranslateCode(2));
        }

        private void LanButton3_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard(getTranslateCode(3));
        }

        private void LanButton4_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard(getTranslateCode(4));
        }

        private void LanButton5_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard(getTranslateCode(5));
        }

        private void LanButton6_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard(getTranslateCode(6));
        }

        private void LanButton7_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard(getTranslateCode(7));
        }

        private void LanButton8_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard(getTranslateCode(8));
        }

        private void LanButton9_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard(getTranslateCode(9));
        }

        private void LanButton10_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard(getTranslateCode(10));
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            TranslateOutputTextblock.Text = getTranslateAllCode();
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            TranslateOutputTextblock.Text = getTranslateAllCode();
            CopyToClipboard(getTranslateAllCode());
        }

        private async void Button_Click_9(object sender, RoutedEventArgs e)
        {
            try
            {
                AutoWorkStatus.Text = "작동중";
                string text = await Clipboard.GetContent().GetTextAsync();

                string aa = "public static string ";
                int a1 = text.IndexOf(aa) + aa.Length;
                int a2 = text.IndexOf("{", a1);
                string propertyname = text.Substring(a1, a2 - a1).Trim();

                string bb = "languageIndex == 0";
                string bb0 = "return \"";
                int b1 = text.IndexOf(bb, a2) + bb.Length;
                int b2 = text.IndexOf(bb0, b1) + bb0.Length;
                int b3 = text.IndexOf("\"", b2 + 1);
                string engword = text.Substring(b2, b3 - b2);

                string cc = "languageIndex == 1";
                string cc0 = "return \"";
                int c1 = text.IndexOf(cc, a2) + cc.Length;
                int c2 = text.IndexOf(cc0, c1) + cc0.Length;
                int c3 = text.IndexOf("\"", c2 + 1);
                string korword = text.Substring(c2, c3 - c2);

                TranslatePropertyNameTextBox.Text = propertyname;
                TranslateEnglishTextBox.Text = engword;
                TranslateKoreanTextBox.Text = korword;

                await TranslateTask();
                TranslateOutputTextblock.Text = getTranslateAllCode();
                CopyToClipboard(getTranslateAllCode());
                AutoWorkStatus.Text = "작동완료";
            }
            catch
            {
                AutoWorkStatus.Text = "에러났네?-_-;;";
            }



        }

        public string ReplaceLetter(string data, string replaceWhat, string replaceHow)
        {
            try
            {
                int z = 0;
                for (int i = 0; i < data.Length; i++)
                {
                    if (data.IndexOf(replaceWhat, z) != -1)
                    {
                        int a1 = data.IndexOf(replaceWhat, z);
                        data = data.Substring(0, a1) + replaceHow + data.Substring(a1 + replaceWhat.Length, data.Length - a1 - replaceWhat.Length);
                        z = a1;
                        z = z - replaceWhat.Length + replaceHow.Length;
                    }
                    else
                    {
                        break;
                    }
                }
                return data;
            }
            catch
            {
                return data;
            }
        }

    }
}
