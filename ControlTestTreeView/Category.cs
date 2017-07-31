using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ControlTestTreeView
{
    public class Category : INotifyPropertyChanged
    {
        public Category(String categoryName,ObservableCollection<Product> products)
        {
            CategoryName = categoryName;
            Products = products;
        }

        private ObservableCollection<Product> products;

        public ObservableCollection<Product> Products
        {
            get { return products; }
            set
            {
                products = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Products"));
            }
        }

        private string categoryName;

        public string CategoryName
        {
            get { return categoryName;}
            set
            {
                categoryName = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CategoryName"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }
    }
}