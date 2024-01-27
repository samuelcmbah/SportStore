using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Moq;
using SportStore.Controllers;
using SportStore.Models;
using SportStore.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace SportsStore.Tests
{
    //These tests were written based on the Guided project source
    //however, moving forward, i want the project to reflect soley my skills
    //hence i wont be writing any more tests
    //btw, writing tests is not a religion
    public class HomeControllerTests
    {
        [Fact]
        public void Can_Use_Repository()
        {
            // Arrange
            //sets up a mock implementation of IStoreRepository using Moq
            Mock<IStoreRepository> mock = new Mock<IStoreRepository>();

            //configure the mock to return a collection of two Product objects when the AllProducts property is accessed
            mock.Setup(m => m.AllProducts).Returns((new Product[]
            {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"}
            }).AsQueryable<Product>());

            Mock<ILogger<HomeController>> mockLogger = new Mock<ILogger<HomeController>>();

            //create an instance of the HomeController class, passing the mock repository to its constructor
            HomeController controller = new HomeController(mockLogger.Object, mock.Object);

            // Act
            //invoke the Index action on the controller, attempting to cast the result as a ViewResult
            ProductsListViewModel result = (controller.Index(null) as ViewResult)?.ViewData.Model as ProductsListViewModel ?? new();

            // Assert
            //The result is converted to an array for easier assertion.
            //assert that the result is not null and that it contains two products.
            //check the names of the products to ensure they match the expected values
            Product[] prodArray = result.Products.ToArray() ?? Array.Empty<Product>();
            Assert.True(prodArray.Length == 2);
            Assert.Equal("P1", prodArray[0].Name);
            Assert.Equal("P2", prodArray[1].Name);
        }

        [Fact]
        public void Can_Paginate()
        {
            // Arrange
            Mock<IStoreRepository> mock = new Mock<IStoreRepository>();
            mock.Setup(m => m.AllProducts).Returns((new Product[] {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"},
                new Product {ProductID = 4, Name = "P4"},
                new Product {ProductID = 5, Name = "P5"}
            }).AsQueryable<Product>());

            Mock<ILogger<HomeController>> mockLogger = new Mock<ILogger<HomeController>>();
            HomeController controller = new HomeController(mockLogger.Object, mock.Object);
            controller.ProductPerPage = 3;
            // Act
            //The result is cast as a ViewResult, and its Model property is then cast as an IEnumerable<Product>.
            //If the cast is unsuccessful, an empty enumerable is provided.
            ProductsListViewModel result = (controller.Index(null, 2) as ViewResult)?.ViewData.Model as ProductsListViewModel ?? new();

            // Assert
            Product[] prodArray = result.Products.ToArray() ?? Array.Empty<Product>();
            Assert.True(prodArray.Length == 2);
            Assert.Equal("P4", prodArray[0].Name);
            Assert.Equal("P5", prodArray[1].Name);
        }

        [Fact]
        public void Can_Send_Pagination_View_Model()
        {
            // Arrange
            Mock<IStoreRepository> mock = new Mock<IStoreRepository>();
            mock.Setup(m => m.AllProducts).Returns((new Product[] {
                 new Product {ProductID = 1, Name = "P1"},
                 new Product {ProductID = 2, Name = "P2"},
                 new Product {ProductID = 3, Name = "P3"},
                 new Product {ProductID = 4, Name = "P4"},
                 new Product {ProductID = 5, Name = "P5"}
            }).AsQueryable<Product>());
            Mock<ILogger<HomeController>> mockLogger = new Mock<ILogger<HomeController>>();

            // Arrange
            HomeController controller = new HomeController(mockLogger.Object, mock.Object) { ProductPerPage = 3 };

            // Act
            ProductsListViewModel result = (controller.Index(null, 2) as ViewResult)?.ViewData.Model as ProductsListViewModel ?? new();
            // Assert
            var pageInfo = result.PagingInfo;
            Assert.Equal(2, pageInfo.CurrentPage);
            Assert.Equal(3, pageInfo.ItemsPerPage);
            Assert.Equal(5, pageInfo.TotalItems);
            Assert.Equal(2, pageInfo.TotalPages);
        }

        [Fact]
        public void Can_Filter_Products()
        {
            // Arrange
            // - create the mock repository
            Mock<IStoreRepository> mock = new Mock<IStoreRepository>();
            mock.Setup(m => m.AllProducts).Returns((new Product[] {
                 new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
                 new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
                 new Product {ProductID = 3, Name = "P3", Category = "Cat1"},
                 new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
                 new Product {ProductID = 5, Name = "P5", Category = "Cat3"}
             }).AsQueryable<Product>());
            Mock<Logger<HomeController>> mockLogger = new Mock<Logger<HomeController>>();

            // Arrange - create a controller and make the page size 3 items
            HomeController controller = new HomeController(mockLogger.Object, mock.Object);
            controller.ProductPerPage = 3;

            // Action
            Product[] result = ((controller.Index("Cat2", 1) as ViewResult)?.ViewData.Model
            as ProductsListViewModel ?? new()).Products.ToArray();
            // Assert
            Assert.Equal(2, result.Length);
            Assert.True(result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.True(result[1].Name == "P4" && result[1].Category == "Cat2");
        }

        [Fact]
        public void Generate_Category_Specific_Product_Count()
        {
            // Arrange
            Mock<IStoreRepository> mock = new Mock<IStoreRepository>();
            mock.Setup(m => m.AllProducts).Returns((new Product[] {
                 new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
                 new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
                 new Product {ProductID = 3, Name = "P3", Category = "Cat1"},
                 new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
                 new Product {ProductID = 5, Name = "P5", Category = "Cat3"}
            }).AsQueryable<Product>());

            Mock<ILogger<HomeController>> mockLogger = new(); 
            HomeController target = new HomeController(mockLogger.Object, mock.Object);
            target.ProductPerPage = 3;
            Func<IActionResult, ProductsListViewModel?> GetModel = result => (result as ViewResult)?.ViewData?.Model as ProductsListViewModel;
            //Func<IActionResult, ProductsListViewModel?> GetModel = result => result is ViewResult viewResult ? viewResult.ViewData?.Model as ProductsListViewModel : null;


            // Action
            int? res1 = GetModel(target.Index("Cat1"))?.PagingInfo.TotalItems;
            int? res2 = GetModel(target.Index("Cat2"))?.PagingInfo.TotalItems;
            int? res3 = GetModel(target.Index("Cat3"))?.PagingInfo.TotalItems;
            int? resAll = GetModel(target.Index(null))?.PagingInfo.TotalItems;
            // Assert
            Assert.Equal(2, res1);
            Assert.Equal(2, res2);
            Assert.Equal(1, res3);
            Assert.Equal(5, resAll);
        }
    }
}
