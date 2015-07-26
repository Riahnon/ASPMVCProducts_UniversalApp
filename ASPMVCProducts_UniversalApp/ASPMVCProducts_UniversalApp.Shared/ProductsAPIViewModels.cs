using ProductsAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Windows.UI.Core;

namespace ASPMVCProducts_UniversalApp
{
    public class ProductListVM : ViewModelBase<ProductListDTO>
    {
        public ProductListVM(CoreDispatcher aDispatcher, ProductListDTO aModel)
            :base(aDispatcher,aModel )
        {}

        public int Id
        {
            get { return Model.Id; }
            set { Model.Id = value; }
        }
        public string Name 
        {
            get { return Model.Name; }
            set { Model.Name = value; }
        }

        public IEnumerable<ProductEntryVM> ProductEntries
        {
            get { return Model.ProductEntries.Select ( aEntry => new ProductEntryVM ( this.Dispatcher, aEntry )); }
        }

    }

    public class ProductEntryVM : ViewModelBase<ProductEntryDTO>
    {
        public ProductEntryVM(CoreDispatcher aDispatcher, ProductEntryDTO aModel)
            : base(aDispatcher, aModel)
        { }

        public int Id
        {
            get { return Model.Id; }
            set { Model.Id = value; }
        }
        public string ProductName
        {
            get { return Model.ProductName; }
            set { Model.ProductName = value; }
        }
        public int Amount
        {
            get { return Model.Amount; }
            set { Model.Amount = value; }
        }
        public string Comments
        {
            get { return Model.Comments; }
            set { Model.Comments = value; }
        }
    }

    public class ProductsAPIClientVM : ViewModelBase<ProductsAPIClient>
    {
        public ProductsAPIClientVM(CoreDispatcher aDispatcher, ProductsAPIClient aModel)
            :base(aDispatcher,aModel )
        {}

        public string ServerURL
        {
            get { return Model.ServerURL; }
        }

        public UserDTO LoggedInUser
        {
            get { return Model.LoggedInUser; }
        }

        public IEnumerable<ProductListVM> ProductLists
        {
            get { return Model.ProductLists.Select( aProductList => new ProductListVM( this.Dispatcher, aProductList )); }
        }

        public bool IsBusy
        {
            get { return Model.IsBusy; }
        }

        public async Task RegisterUser(RegisterUserDTO aUser)
        {
            await Model.RegisterUser(aUser);
        }

        public async Task Login(RegisterUserDTO aUser)
        {
            await Model.Login(aUser);
        }

        public async Task Logout()
        {
            await Model.Logout();
        }

        public async Task QueryProductLists()
        {
            await Model.QueryProductLists();
        }

        public async Task CreateProductList(ProductListDTO aProductList)
        {
            await Model.CreateProductList(aProductList);
        }

        public async Task DeleteProductList(ProductListDTO aProductList)
        {
            await Model.DeleteProductList(aProductList);
        }

        public async Task QueryProductEntries(ProductListDTO aList)
        {
            await Model.QueryProductEntries(aList);
        }

        public async Task CreateProductEntry(ProductListDTO aList, ProductEntryDTO aProductEntry)
        {
            await Model.CreateProductEntry(aList, aProductEntry);
        }

        public async Task DeleteProductEntry(ProductListDTO aList, ProductEntryDTO aProductEntry)
        {
            await Model.DeleteProductEntry(aList, aProductEntry);
        }
    }
}
