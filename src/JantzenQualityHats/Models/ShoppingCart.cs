﻿using JantzenQualityHats.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace JantzenQualityHats.Models
{
    public class ShoppingCart
    {
        public string ShoppingCartId { get; set; }
        public const string CartSessionKey = "cartId";
        public static ShoppingCart GetCart(HttpContext context)
        {
            var cart = new ShoppingCart();
            cart.ShoppingCartId = cart.GetCartId(context);
            return cart;
        }

        public void AddToCart(Hat hat, ApplicationDbContext db)
        {
            var cartItem = db.CartItems.SingleOrDefault(c => c.CartID == ShoppingCartId && c.Hat.HatID == hat.HatID);
            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    Hat = hat,
                    CartID = ShoppingCartId,
                    Count = 1,
                    DateCreated = DateTime.Now
                };
                db.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Count++;
            }
            db.SaveChanges();
        }

        public int RemoveFromCart(int id, ApplicationDbContext db)
        {
            var cartItem = db.CartItems.SingleOrDefault(cart => cart.CartID == ShoppingCartId && cart.Hat.HatID == id);
            int itemCount = 0;
            if (cartItem != null)
            {
                if (cartItem.Count > 1)
                {
                    cartItem.Count--;
                    itemCount = cartItem.Count;
                }
                else
                {
                    db.CartItems.Remove(cartItem);
                }
                db.SaveChanges();
            }
            return itemCount;
        }

        public void EmptyCart(ApplicationDbContext db)
        {
            var cartItems = db.CartItems.Where(cart => cart.CartID == ShoppingCartId);
            foreach (var cartItem in cartItems)
            {
                db.CartItems.Remove(cartItem);
            }
            db.SaveChanges();
        }

        public List<CartItem> GetCartItems(ApplicationDbContext db)
        {
            List<CartItem> cartItems = db.CartItems.Include(i => i.Hat).Where(cartItem => cartItem.CartID == ShoppingCartId).ToList();

            return cartItems;

        }

        public int GetCount(ApplicationDbContext db)
        {
            int? count =
                (from cartItems in db.CartItems where cartItems.CartID == ShoppingCartId select (int?)cartItems.Count).Sum();
            return count ?? 0;
        }

        public decimal GetTotal(ApplicationDbContext db)
        {
            decimal? total = (from cartItems in db.CartItems
                                where cartItems.CartID == ShoppingCartId
                                select (int?)cartItems.Count * cartItems.Hat.Price).Sum();
            return total ?? decimal.Zero;
        }

        public decimal GetGst(ApplicationDbContext db)
        {
            decimal? total = (from cartItems in db.CartItems
                              where cartItems.CartID == ShoppingCartId
                              select (int?)cartItems.Count * cartItems.Hat.Price).Sum();
            decimal? GST = (total/100)*15;
            return GST ?? decimal.Zero;
        }

        public decimal GetGrandTotal(ApplicationDbContext db)
        {
            decimal? total = (from cartItems in db.CartItems
                              where cartItems.CartID == ShoppingCartId
                              select (int?)cartItems.Count * cartItems.Hat.Price).Sum();
            decimal? gst = (total / 100) * 15;
            decimal? GrandTotal = total+gst;
            return GrandTotal ?? decimal.Zero;
        }

        public string GetCartId(HttpContext context)
        {
            if (context.Session.GetString(CartSessionKey) == null)
            {
                Guid tempCartId = Guid.NewGuid();
                context.Session.SetString(CartSessionKey, tempCartId.ToString());
            }
            return context.Session.GetString(CartSessionKey).ToString();
        }



    }
}
