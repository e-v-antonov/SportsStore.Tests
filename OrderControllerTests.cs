using Moq;
using Xunit;
using SportsStore.Models;
using SportsStore.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SportsStore.Tests
{
    public class OrderControllerTests
    {
        [Fact]
        public void Cannot_Checkout_Empty_Cart()
        {
            //Создание имитированного хранилища заказов
            Mock<IOrderRepository> mock = new Mock<IOrderRepository>();

            //Создание пустой корзины
            Cart cart = new Cart();

            //Создание заказа
            Order order = new Order();

            //Создание экземпляра контроллера
            OrderController target = new OrderController(mock.Object, cart);

            //Действие
            ViewResult result = target.Checkout(order) as ViewResult;

            //Утверждение-проверка что заказ не был сохранен
            mock.Verify(m => m.SaveOrder(It.IsAny<Order>()), Times.Never);

            //Утверждение-проверка что метод возвращает стандартное представление
            Assert.True(string.IsNullOrEmpty(result.ViewName));

            //Утверждение-проверка что предствалению передана недопустимая модель
            Assert.False(result.ViewData.ModelState.IsValid);
        }

        [Fact]
        public void Can_Checkout_And_Submit_Order()
        {
            Mock<IOrderRepository> mock = new Mock<IOrderRepository>();

            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);

            OrderController target = new OrderController(mock.Object, cart);

            RedirectToActionResult result = target.Checkout(new Order()) as RedirectToActionResult;

            //Проверка что заказ был сохранен
            mock.Verify(m => m.SaveOrder(It.IsAny<Order>()), Times.Once);

            Assert.Equal("Completed", result.ActionName);
        }
    }
}
