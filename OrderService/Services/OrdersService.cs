using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Exceptions;
using OrderService.Models;
using OrderService.Services.Dto;

namespace OrderService
{
    public class OrdersService
    {
        private readonly OrdersContext _ordersContext;
        public OrdersService(OrdersContext ordersContext)
        {
            _ordersContext = ordersContext;
        }

        public async Task<List<OrderDto>> GetListAsync()
        {
            return await _ordersContext.Orders
                .Include(x => x.OrderItems)
                .Where(x => x.OrderStatusId == OrderStatusEnum.Registered)
                .Select(x => new OrderDto()
                {
                    OrderId = x.Id,
                    OrderStatus = x.OrderStatusId,
                    FullName = x.FullName,
                    OrderItems = x.OrderItems
                        .Select(x => new OrderItemDto() { Count = x.Count, ProductId = x.ProductId })
                        .ToList()
                })
                .ToListAsync();
        }

        public async Task<OrderDto?> GetByIdAsync(short id)
        {
            var order = await _ordersContext.Orders.FirstOrDefaultAsync(x => x.Id == id);

            if (order == null)
            {
                return null;
            }

            return new OrderDto()
            {
                OrderId = order.Id,
                OrderStatus = order.OrderStatusId,
                FullName = order.FullName,
                OrderItems = order.OrderItems
                        .Select(x => new OrderItemDto() { Count = x.Count, ProductId = x.ProductId })
                        .ToList()
            };
        }

        public async Task<Order> Create(OrderCreateDto order)
        {
            var newOrder = new Order
            {
                FullName = order.FullName,
                OrderStatusId = default,
                OrderItems = order.OrderItems.ConvertAll(orderOrderItem => new OrderItem
                {
                    ProductId = orderOrderItem.ProductId,
                    Count = orderOrderItem.Count,
                })
            };

            await _ordersContext.Orders.AddAsync(newOrder);

            await _ordersContext.SaveChangesAsync();

            return newOrder;
        }

        public async Task Update(short orderId, OrderDto order)
        {
            if (order.OrderStatus != OrderStatusEnum.Registered)
            {
                throw new OrderUpdateException("Заказ может быть изменен только в статусе «Зарегистрирован».");
            }

            var existingOrder = await _ordersContext.Orders
                .Include(x => x.OrderItems)
                .FirstOrDefaultAsync(x => x.Id == orderId);

            if (existingOrder == null)
            {
                return;
            }

            // Update parent
            _ordersContext.Entry(existingOrder).CurrentValues.SetValues(order);

            // Delete children
            foreach (var existingOrderItem in existingOrder.OrderItems.ToList())
            {
                if (!order.OrderItems.Any(c => c.RowId == existingOrderItem.RowId))
                    _ordersContext.OrderItems.Remove(existingOrderItem);
            }

            // Update and Insert children
            foreach (var orderItem in order.OrderItems)
            {
                var existingOrderItem = existingOrder.OrderItems
                    .Where(c => c.RowId == orderItem.RowId && c.RowId != default(int))
                    .SingleOrDefault();

                if (existingOrderItem != null)
                    // Update child
                    _ordersContext.Entry(existingOrderItem).CurrentValues.SetValues(orderItem);
                else
                {
                    // Insert child
                    var newChild = new OrderItem
                    {
                        OrderId = orderId,
                        Count = orderItem.Count,
                        ProductId = orderItem.ProductId,
                    };
                    existingOrder.OrderItems.Add(newChild);
                }
            }

            await _ordersContext.SaveChangesAsync();
        }

        public async Task Delete(short orderId)
        {
            var existingOrder = await _ordersContext.Orders
              .Include(x => x.OrderItems)
              .FirstOrDefaultAsync(x => x.Id == orderId);

            if (existingOrder == null)
            {
                return;
            }

            if (existingOrder.OrderStatusId != OrderStatusEnum.Registered)
            {
                throw new OrderUpdateException("Заказ может быть изменен только в статусе «Зарегистрирован».");
            }

            foreach (var existingOrderItem in existingOrder.OrderItems.ToList())
            {
                _ordersContext.OrderItems.Remove(existingOrderItem);
            }

            _ordersContext.Orders.Remove(existingOrder);

            await _ordersContext.SaveChangesAsync();
        }
    }
}
