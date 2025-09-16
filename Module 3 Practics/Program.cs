using System;
using System.Collections.Generic;

namespace OnlineStoreSOLID
{
    // СРП
    class Product { public string Name; public double Price; }
    class OrderItem { public Product Product; public int Quantity; }

    class Order
    {
        public List<OrderItem> Items = new();
        public IPayment Payment;
        public IDelivery Delivery;
        public List<INotification> Notifications = new();

        public void AddItem(Product p, int q) => Items.Add(new OrderItem { Product = p, Quantity = q });

        public double CalculateTotal(IDiscount discount)
        {
            double total = 0;
            foreach (var i in Items) total += i.Product.Price * i.Quantity;
            return discount.Apply(total);
        }

        public void CompleteOrder(IDiscount discount)
        {
            double total = CalculateTotal(discount);
            Payment.ProcessPayment(total);
            Delivery.DeliverOrder(this);
            foreach (var n in Notifications) n.SendNotification($"Order total {total} processed.");
        }
    }

    // Платежи
    interface IPayment { void ProcessPayment(double amount); }
    class CreditCardPayment : IPayment { public void ProcessPayment(double a) => Console.WriteLine($"Paid {a} via Credit Card"); }
    class PayPalPayment : IPayment { public void ProcessPayment(double a) => Console.WriteLine($"Paid {a} via PayPal"); }

    // Доставка
    interface IDelivery { void DeliverOrder(Order o); }
    class CourierDelivery : IDelivery { public void DeliverOrder(Order o) => Console.WriteLine("Delivered by courier"); }
    class PostDelivery : IDelivery { public void DeliverOrder(Order o) => Console.WriteLine("Delivered via post"); }

    // Уведомления
    interface INotification { void SendNotification(string msg); }
    class EmailNotification : INotification { public void SendNotification(string msg) => Console.WriteLine($"[Email] {msg}"); }
    class SmsNotification : INotification { public void SendNotification(string msg) => Console.WriteLine($"[SMS] {msg}"); }

    // Скидочки
    interface IDiscount { double Apply(double total); }
    class TenPercentDiscount : IDiscount { public double Apply(double total) => total * 0.9; }

    // Основа
    class Program
    {
        static void Main()
        {
            var book = new Product { Name = "Book", Price = 100 };
            var laptop = new Product { Name = "Laptop", Price = 1000 };

            var order = new Order();
            order.AddItem(book, 2);
            order.AddItem(laptop, 1);

            order.Payment = new CreditCardPayment();
            order.Delivery = new CourierDelivery();
            order.Notifications.Add(new EmailNotification());
            order.Notifications.Add(new SmsNotification());

            order.CompleteOrder(new TenPercentDiscount());
        }
    }
}


// Ну и вывод

// Каждый класс выполняет одну функцию

// Добавление новых способов оплаты  доставки или скидок не требует изменять код

// Новые классы можно использовать как интерфейс

// Уведомления используют только нужный метод