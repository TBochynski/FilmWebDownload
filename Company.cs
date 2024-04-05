using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls;

namespace eBayDEParser
{

    public class Company : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _Id = "";
        public string Id 
        { 
            get { return _Id; } 
            set {
                if (value != this._Id)
                {
                    this._Id = value;
                    this.OnPropertyChanged("Id");
                }
            } 
        }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                if (value != this._Name)
                {
                    this._Name = value;
                    this.OnPropertyChanged("Name");
                }
            }
        }

        private string _NameOrg;
        public string NameOrg
        {
            get { return _NameOrg; }
            set
            {
                if (value != this._NameOrg)
                {
                    this._NameOrg = value;
                    this.OnPropertyChanged("NameOrg");
                }
            }
        }

        private string _Register = "";
        public string Register
        {
            get { return _Register; }
            set
            {
                if (value != this._Register)
                {
                    this._Register = value;
                    this.OnPropertyChanged("Register");
                }
            }
        }

        private string _CreateBy = "";
        public string CreateBy
        {
            get { return _CreateBy; }
            set
            {
                if (value != this._CreateBy)
                {
                    this._CreateBy = value;
                    this.OnPropertyChanged("CreateBy");
                }
            }
        }


        private string _Category = "";
        public string Category
        {
            get { return _Category; }
            set
            {
                if (value != this._Category)
                {
                    this._Category = value;
                    this.OnPropertyChanged("Category");
                }
            }
        }

        private string _Country = "";
        public string Country
        {
            get { return _Country; }
            set
            {
                if (value != this._Country)
                {
                    this._Country = value;
                    this.OnPropertyChanged("Country");
                }
            }
        }

        public bool IsChecked { get; set; } = false;

        public override string ToString()
        {
            return $"{TrimWWw(Id)};{Trim(Name)};{Trim(NameOrg)};{Trim(Register)};{Trim(CreateBy)};{Trim(this.Category)};{Trim(this.Country)};";
        }

        public string TrimWWw(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            else
            {
                value.Replace("&amp;", "&").Replace(";", "").Replace(Environment.NewLine, " ").Replace("\n", " ").Replace("'", "").Trim();
                if(!value.StartsWith("http")) value = "https://" + value;
                return value;
            }
        }

        public string Trim(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            else return value.Replace("&amp;", "&").Replace(";", "").Replace(Environment.NewLine, " ").Replace("\n", " ").Replace("'", "").Trim();
        }


        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
    }

    public class MyViewModel : ViewModelBase
    {
        public MyViewModel() { }

        private ObservableCollection<Company> _data;

        public ObservableCollection<Company> database
        {
            get
            {
                if (this._data == null || this._data.Count != this.list.Count)
                {
                    this._data = new ObservableCollection<Company>(list);
                }
                return this._data;
            }
        }

        private List<Company> list = new List<Company>();

        public void Add(Company comp)
        {
            list.Add(comp);
            this._data.Add(comp);
            this.OnPropertyChanged("database");
        }

        public void Remove(Company comp)
        {
            list.Remove(comp);
            this._data.Remove(comp);
        }
    }
}
