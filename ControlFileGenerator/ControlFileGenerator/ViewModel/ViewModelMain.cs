using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ControlFileGenerator.Model;
using System.Collections.ObjectModel;
using System.Configuration;

namespace ControlFileGenerator.ViewModel
{
    class ViewModelMain : ViewModelBase
    {
        public ObservableCollection<Person> CaseType { get; set; }
        public ObservableCollection<Person> TransferType { get; set; }
        public ObservableCollection<Person> AccountType { get; set; }
        public ObservableCollection<Person> AccountOption { get; set; }
        public RelayCommand GenerateITIControlFile { get; set; }

        /// <summary>
        /// SelectedItem is an object instead of a Person, only because we are allowing "CanUserAddRows=true" 
        /// NewItemPlaceHolder represents a new row, and this is not the same as Person class
        /// 
        /// Change 'object' to 'Person', and you will see the following:
        /// 
        /// System.Windows.Data Error: 23 : Cannot convert '{NewItemPlaceholder}' from type 'NamedObject' to type 'MvvmExample.Model.Person' for 'en-US' culture with default conversions; consider using Converter property of Binding. NotSupportedException:'System.NotSupportedException: TypeConverter cannot convert from MS.Internal.NamedObject.
        ///   at System.ComponentModel.TypeConverter.GetConvertFromException(Object value)
        ///   at System.ComponentModel.TypeConverter.ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, Object value)
        ///   at MS.Internal.Data.DefaultValueConverter.ConvertHelper(Object o, Type destinationType, DependencyObject targetElement, CultureInfo culture, Boolean isForward)'
        ///System.Windows.Data Error: 7 : ConvertBack cannot convert value '{NewItemPlaceholder}' (type 'NamedObject'). BindingExpression:Path=SelectedPerson; DataItem='ViewModelMain' (HashCode=47403907); target element is 'DataGrid' (Name=''); target property is 'SelectedItem' (type 'Object') NotSupportedException:'System.NotSupportedException: TypeConverter cannot convert from MS.Internal.NamedObject.
        ///   at MS.Internal.Data.DefaultValueConverter.ConvertHelper(Object o, Type destinationType, DependencyObject targetElement, CultureInfo culture, Boolean isForward)
        ///   at MS.Internal.Data.ObjectTargetConverter.ConvertBack(Object o, Type type, Object parameter, CultureInfo culture)
        ///   at System.Windows.Data.BindingExpression.ConvertBackHelper(IValueConverter converter, Object value, Type sourceType, Object parameter, CultureInfo culture)'
        /// </summary>

        object _SelectedCaseType;
        public object SelectedCasType
        {
            get
            {
                return _SelectedCaseType;
            }
            set
            {
                if (_SelectedCaseType != value)
                {
                    _SelectedCaseType = value;
                    RaisePropertyChanged("SelectedCasType");
                }
            }
        }

        object _SelectedTransferType;
        public object SelectedTransferType
        {
            get
            {
                return _SelectedTransferType;
            }
            set
            {
                if (_SelectedTransferType != value)
                {
                    _SelectedTransferType = value;
                    RaisePropertyChanged("SelectedTransferType");
                }
            }
        }

        object _SelectedAccountType;
        public object SelectedAccountType
        {
            get
            {
                return _SelectedAccountType;
            }
            set
            {
                if (_SelectedAccountType != value)
                {
                    _SelectedAccountType = value;
                    if (((ControlFileGenerator.Model.Person)(_SelectedAccountType)).AccountType == "New Account")
                    {
                        this.ISEnable = true;
                    }
                    else
                    {
                        this.ISEnable = false;
                    }
                    RaisePropertyChanged("SelectedAccountType");
                }
            }
        }


        object _SelectedAccountOption;
        public object SelectedAccountOption
        {
            get
            {
                return _SelectedAccountOption;
            }
            set
            {
                if (_SelectedAccountOption != value)
                {
                    _SelectedAccountOption = value;
                    RaisePropertyChanged("SelectedAccountOption");
                }
            }
        }

        string _TextPropertySeedValue;
        public string TextPropertySeedValue
        {
            get
            {
                return _TextPropertySeedValue;
            }
            set
            {
                if (_TextPropertySeedValue != value)
                {
                    _TextPropertySeedValue = value;
                    RaisePropertyChanged("TextPropertySeedValue");
                }
            }
        }

        string _TextPropertyCasePerFile;
        public string TextPropertyCasePerFile
        {
            get
            {
                return _TextPropertyCasePerFile;
            }
            set
            {
                if (_TextPropertyCasePerFile != value)
                {
                    _TextPropertyCasePerFile = value;
                    RaisePropertyChanged("TextPropertyCasePerFile");
                }
            }
        }

        string _TextPropertyDropLocation;
        public string TextPropertyDropLocation
        {
            get
            {
                return _TextPropertyDropLocation;
            }
            set
            {
                if (_TextPropertyDropLocation != value)
                {
                    _TextPropertyDropLocation = value;
                    RaisePropertyChanged("TextPropertyDropLocation");
                }
            }
        }

        string _TextPropertyNoOfCase;
        public string TextPropertyNoOfCase
        {
            get
            {
                return _TextPropertyNoOfCase;
            }
            set
            {
                if (_TextPropertyNoOfCase != value)
                {
                    _TextPropertyNoOfCase = value;
                    RaisePropertyChanged("TextPropertyNoOfCase");
                }
            }
        }

        bool _isEnable;
        public bool ISEnable
        {
            get
            {
                return _isEnable;
            }
            set
            {
                if (_isEnable != value)
                {
                    _isEnable = value;
                    RaisePropertyChanged("ISEnable");
                }
            }
        }

        public RelayCommand AddUserCommand { get; set; }

        public ViewModelMain()
        {
            try
            {
                CaseType = new ObservableCollection<Person>
                {
                new Person{CaseType = "Transfer IN"},
                new Person{CaseType = "Transfer Out"},                
                };

                TransferType = new ObservableCollection<Person>
                {
                new Person{TransferType = "Stock & ShareISA"},
                new Person{TransferType = "Cash ISA"},                
                };

                AccountType = new ObservableCollection<Person>
                {
                new Person{AccountType = "New Account"},
                new Person{AccountType = "Existing Account"},                
                };

                AccountOption = new ObservableCollection<Person>
                {
                new Person{AccountOption = "EIT"},
                new Person{AccountOption = "Non EIT"},                
                };

                int seed = Convert.ToInt32(System.DateTime.Now.Ticks % 10000 + 1) * 10000;
                TextPropertySeedValue = seed.ToString();
                TextPropertyCasePerFile = ConfigurationManager.AppSettings["NoOfCaseInFile"].ToString()?? string.Empty;
                TextPropertyNoOfCase = ConfigurationManager.AppSettings["NoOfCase"].ToString() ?? string.Empty;
                TextPropertyDropLocation = ConfigurationManager.AppSettings["OutPutFileLocation"].ToString() ?? string.Empty;

                GenerateITIControlFile = new RelayCommand(GenITIControlfile);
            }
            catch (System.Exception ex)
            { 
            
            }
        }

        void GenITIControlfile(object parameter)
        {
            try
            {
                int noofcase = Convert.ToInt32(this.TextPropertyNoOfCase);
                int seedvalue = Convert.ToInt32(this.TextPropertySeedValue);
                int Caseincontrolfile = Convert.ToInt32(this.TextPropertyCasePerFile);

                int nooffullcontrolfile = noofcase / Caseincontrolfile;
                int rest = noofcase % Caseincontrolfile;
                string drooplocation = this.TextPropertyDropLocation;

                string caseType = ((ControlFileGenerator.Model.Person)(this.SelectedCasType)).CaseType.ToString();
                string accountType = ((ControlFileGenerator.Model.Person)(this.SelectedAccountType)).AccountType.ToString();
                string transferType = ((ControlFileGenerator.Model.Person)(this.SelectedTransferType)).TransferType.ToString();
                string accountOption = ((ControlFileGenerator.Model.Person)(this.SelectedAccountOption)).AccountOption.ToString();


                Model.ControlFileGenerator cfg = new Model.ControlFileGenerator();
                if (nooffullcontrolfile > 0)
                {
                    for (int i = 0; i < nooffullcontrolfile; i++)
                    {
                        cfg.GenerateControlFile(seedvalue + (Caseincontrolfile *i), Caseincontrolfile, drooplocation, caseType, accountType, transferType);
                    }
                                   
                }
                if(rest > 0)
                {
                  cfg.GenerateControlFile(seedvalue + (Caseincontrolfile * nooffullcontrolfile), rest, drooplocation, caseType, accountType, transferType);
                }

            
            }
            catch (System.Exception ex)
            {

            }
        }

    }
}
