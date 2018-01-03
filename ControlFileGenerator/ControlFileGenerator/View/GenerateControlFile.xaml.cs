using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Reflection;
using System.Xml.Linq;
using Wpf_Customer.DAL;
using ControlFileGenerator.Model;

namespace LogViewer
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        private string _text = string.Empty;
        public string TEXT
        {
            get { return _text; }
            set { _text = value; }
        }

        public About()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StringBuilder sbText = new StringBuilder();
            Assembly assembly = Assembly.GetEntryAssembly();
            if (assembly != null)
            {
                object[] attributes = assembly.GetCustomAttributes(false);
                foreach (object attribute in attributes)
                {
                    Type type = attribute.GetType();
                    if (type == typeof(AssemblyTitleAttribute))
                    {
                        AssemblyTitleAttribute title = (AssemblyTitleAttribute)attribute;
                        lblText.Content = title.Title;
                    }
                    if (type == typeof(AssemblyFileVersionAttribute))
                    {
                        AssemblyFileVersionAttribute version = (AssemblyFileVersionAttribute)attribute;
                        //labelAssemblyVersion.Content = version.Version;
                    }
                    if (type == typeof(AssemblyCopyrightAttribute))
                    {
                        AssemblyCopyrightAttribute copyright = (AssemblyCopyrightAttribute)attribute;
                        sbText.AppendFormat("{0}\r", copyright.Copyright);
                    }
                    if (type == typeof(AssemblyCompanyAttribute))
                    {
                        AssemblyCompanyAttribute company = (AssemblyCompanyAttribute)attribute;
                        sbText.AppendFormat("{0}\r",company.Company);
                    }
                    if (type == typeof(AssemblyDescriptionAttribute))
                    {
                        AssemblyDescriptionAttribute description = (AssemblyDescriptionAttribute)attribute;
                        sbText.AppendFormat("{0}\r", description.Description);
                    }
                }
                //labelAssembly.Content = sbText.ToString();
            }

            string path = GlobalApplication.getResourcePath("Web") + @"Desktop\WaitingList.xsl";
            
            string TEXT = 
@"<log4net>
  <appender name=""RollingFileAppender"" type=""log4net.Appender.RollingFileAppender"">
    <file type=""log4net.Util.PatternString"" value=""c:\log\log.xml"" />
    <appendToFile value=""true"" />
    <datePattern value=""yyyyMMdd"" />
    <rollingStyle value=""Date"" />
    <layout type=""log4net.Layout.XmlLayoutSchemaLog4j"">
      <locationInfo value=""true"" />
    </layout>
  </appender>
  <root>
    <level value=""DEBUG"" />
    <appender-ref ref=""RollingFileAppender"" />
  </root>
</log4net>";
           
            this.RichTextBox1.AppendText(TEXT);
        }

        private string GetControlFormate()
        {
            throw new NotImplementedException();
        }

        public void CreateXML()
        {
            XDocument doc = new XDocument(new XDeclaration("1.0", "utf-16", "yes"),

               new XElement("RootNode", new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"), new XAttribute(XNamespace.Xmlns + "xsd", "http://www.w3.org/2001/XMLSchema"), new XAttribute("Name", "X_SoftwareUpdate"), new XAttribute("LocaleId", "1033"),

                   new XElement("ChildNode",

                       new XElement("UpdateXMLDescriptionItem", new XAttribute("PropertyName", "_Product"), new XAttribute("UIPropertyName", ""),

                           new XElement("MatchRules",

                               new XElement("string", "''Product:X''")

                               ))),

                       new XElement("ChildNode",

                       new XElement("UpdateXMLDescriptionItem", new XAttribute("PropertyName", "IsSuperseded"), new XAttribute("UIPropertyName", ""),

                           new XElement("MatchRules",

                               new XElement("string", "false")

                               ))),

                       new XElement("ChildNode",

                       new XElement("UpdateXMLDescriptionItem", new XAttribute("PropertyName", "_UpdateClassification"), new XAttribute("UIPropertyName", ""),

                           new XElement("MatchRules",

                               new XElement("string", "''UpdateClassification:X''")

                               )))));

            doc.Save(@"C:\test.xml");

        }
        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            //Close();
            CreateXML();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (this.chkAuto.IsChecked == true)
            {
                txtFname.IsEnabled = false;
                txtLname.IsEnabled = false;
                txtDob.IsEnabled = false;
                txtPost.IsEnabled = false;
                txtNino.IsEnabled = false;
            }
            else
            {
                txtFname.IsEnabled = true;
                txtLname.IsEnabled = true;
                txtDob.IsEnabled = true;
                txtPost.IsEnabled = true;
                txtNino.IsEnabled = true;
            
            }

        }        
    }
}
