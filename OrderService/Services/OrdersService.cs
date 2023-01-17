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

        public async Task<List<OrderDto>> GetListAsync(DateTime? dateTimeFilter = null)
        {
            var query = _ordersContext.Orders
                .Include(x => x.OrderItems)
                .Where(x => x.OrderStatusId == OrderStatusEnum.Registered)
                .Select(x => new OrderDto()
                {
                    OrderId = x.Id,
                    CreatedAt = x.CreatedAt,
                    OrderStatus = x.OrderStatusId,
                    FullName = x.FullName,
                    OrderItems = x.OrderItems
                        .Select(x => new OrderItemDto() 
                        { 
                            RowId = x.RowId, 
                            Count = x.Count, 
                            ProductId = x.ProductId 
                        })
                        .ToList()
                });

            if (dateTimeFilter != null)
            {
                query = query.Where(x => x.CreatedAt.Date == dateTimeFilter.Value.Date);
            }

            return await query.ToListAsync();
        }

        public async Task<OrderDto?> GetByIdAsync(short id)
        {
            var order = await _ordersContext.Orders
                .Include(x=>x.OrderItems)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (order == null)
            {
                return null;
            }

            return MapToDto(order);
        }

        public async Task<OrderDto> CreateAsync(OrderCreateDto order)
        {
            await CheckOrderItemsRulesAsync(order.OrderItems
                .Select(x => new OrderItemDto() { Count = x.Count, ProductId = x.ProductId })
                .ToList());

            var newOrder = new Order
            {
                FullName = order.FullName,
                CreatedAt = DateTime.Now,
                OrderStatusId = order.OrderStatus,
                OrderItems = order.OrderItems.ConvertAll(orderOrderItem => new OrderItem
                {
                    ProductId = orderOrderItem.ProductId,
                    Count = orderOrderItem.Count,
                })
            };

            await _ordersContext.Orders.AddAsync(newOrder);

            await _ordersContext.SaveChangesAsync();

            return MapToDto(newOrder);
        }

        public async Task<OrderDto?> UpdateAsync(short orderId, OrderDto order)
        {
            if (order.OrderStatus != OrderStatusEnum.Registered)
            {
                throw new OrderUpdateException("Заказ может быть изменен только в статусе «Зарегистрирован».");
            }

            await CheckOrderItemsRulesAsync(order.OrderItems);

            var existingOrder = await _ordersContext.Orders
                .Include(x => x.OrderItems)
                .FirstOrDefaultAsync(x => x.Id == orderId);

            if (existingOrder == null)
            {
                return null;
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

            return MapToDto(existingOrder);
        }

        public async Task DeleteAsync(short orderId)
        {
            var existingOrder = await _ordersContext.Orders
              .Include(x => x.OrderItems)
              .FirstOrDefaultAsync(x => x.Id == orderId);

            if (existingOrder == null)
            {
                throw new OrderNotFoundException();
            }

            if (existingOrder.OrderStatusId != OrderStatusEnum.Registered)
            {
                throw new OrderUpdateException("Заказ может быть удален только в статусе «Зарегистрирован».");
            }

            foreach (var existingOrderItem in existingOrder.OrderItems.ToList())
            {
                _ordersContext.OrderItems.Remove(existingOrderItem);
            }

            _ordersContext.Orders.Remove(existingOrder);

            await _ordersContext.SaveChangesAsync();
        }

        private async Task CheckOrderItemsRulesAsync(List<OrderItemDto> orderItems) 
        {
            if (orderItems.Count() > 10)
            {
                throw new OrderCreateException("В одном заказе можно указать не больше 10 единиц товаров.");
            }

            var products = await _ordersContext.Products.Where(x => orderItems.Select(o => o.ProductId).Contains(x.Id)).ToListAsync();

            if (products.Count() != orderItems.Count())
            {
                throw new OrderCreateException("Указаны несуществующие в базе артикулы товаров");
            }

            if (products.Sum(x => x.Price * orderItems.First(z => z.ProductId == x.Id).Count) > 15000)
            {
                throw new OrderCreateException("Сумма заказа не должна превышать 15000 у.е.");
            }
        }

        private OrderDto MapToDto(Order order) 
        {
            return new OrderDto()
            {
                OrderId = order.Id,
                OrderStatus = order.OrderStatusId,
                CreatedAt = order.CreatedAt,
                FullName = order.FullName,
                OrderItems = order.OrderItems
                        .Select(x => new OrderItemDto()
                        {
                            RowId = x.RowId,
                            Count = x.Count,
                            ProductId = x.ProductId
                        })
                        .ToList()
            };
        } 
    }
}
