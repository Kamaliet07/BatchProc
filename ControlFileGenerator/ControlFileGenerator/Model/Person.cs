using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ControlFileGenerator.Model
{
   public class Person : INotifyPropertyChanged
    {
       string _CaseType;
       public string CaseType
       {
           get 
           {
               return _CaseType;
           }
           set
           {
               if (_CaseType != value)
               {
                   _CaseType = value;
                   RaisePropertyChanged("CaseType");               
               }
           }      
       
       }

       string _TransferType;
       public string TransferType
       {
           get
           {
               return _TransferType;
           }
           set
           {
               if (_TransferType != value)
               {
                   _TransferType = value;
                   RaisePropertyChanged("TransferType");
               }
           }

       }

       string _AccountType;
       public string AccountType
       {
           get
           {
               return _AccountType;
           }
           set
           {
               if (_AccountType != value)
               {
                   _AccountType = value;
                   RaisePropertyChanged("AccountType");
               }
           }

       }

       string _AccountOption;
       public string AccountOption
       {
           get
           {
               return _AccountOption;
           }
           set
           {
               if (_AccountOption != value)
               {
                   _AccountOption = value;
                   RaisePropertyChanged("AccountOption");
               }
           }

       }


       public event PropertyChangedEventHandler PropertyChanged;
       void RaisePropertyChanged(string prop)
       {
           if (PropertyChanged != null) 
           {
               PropertyChanged(this, new PropertyChangedEventArgs(prop)); 
           }
       }
       
    }
}
