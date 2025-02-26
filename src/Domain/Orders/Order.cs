﻿using Flunt.Validations;
using IWantApp.Endpoints.Products;

namespace IWantApp.Domain.Orders;

public class Order: Entity
{
    public string ClientId { get; private set; } = null!;
    public List<Product> Products { get; private set; } = null!;
    public decimal Total { get; private set; }
    public string DeliveryAddress { get; private set; } = null!;

    private Order() { }
    public Order(string clientId,string clientName,List<Product> products,string deliveryAddress)
    {
        ClientId = clientId;
        Products = products;
        DeliveryAddress = deliveryAddress;
        CreatedBy = clientName;
        EditedBy = null;
        CreatedOn = DateTime.UtcNow;
        EditedOn = null;

        Total = 0;
        foreach(var item in products)
        {
            Total += item.Price;
        }

        Validate( );
    }

    private void Validate()
    {
        var contract = new Contract<Order>( )
            .IsNotNull(ClientId,"Client")
            .IsTrue(Products != null && Products.Any( ),"Products")
            .IsNotNullOrEmpty(DeliveryAddress,"DeliveryAddress");
        AddNotifications(contract);
    }
}
