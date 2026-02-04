using System.Collections.Generic;
using UnityEngine;

public static class EconomicEntityUtilities
{

	     public static List<EconomicEntity> GenerateEntityNames()
	     {
		          List<string> roles = new List<string>();
		          foreach (var role in PepoLibraryRoles.HunterGatherer)
		          {
				roles.Add(role.roleName);
		          }
		          List<EconomicEntity> entities = new List<EconomicEntity>();
		          foreach (var role in PepoLibraryRoles.HunterGatherer)
		          {
				entities.Add(new EconomicEntity(role.roleName));
		          }
		          return entities;
	     }

	     //take a product and update the product state of the entity based on its personality variables
	     public static void UpdateProductState(EconomicEntity entity, Product product)
	     {

	     }
}
public static class MarketUtilities
{
	     public static Market TestEconomy()
	     {
		          var market = new Market();
		          var products = ProductUtilities.GenerateProductNames();
		          var entities = EconomicEntityUtilities.GenerateEntityNames();
		          foreach (var product in products)
		          {
				MarketUtilities.UpdatePrice(market.Products, product, Random.Range(1, 10));

				foreach (var entity in entities)
				{
					     entity.AddProduct(product, Random.Range(0, 10));
					     entity.AddDesiredProduct(product, Random.Range(0, 10));
					     entity.ShowProduct(product, Random.Range(1, 10));
				}
		          }
		          foreach (var entity in entities)
		          {
				market.AddParticipant(entity);
		          }
		          Debug.Log(MarketSupplyDemand(market));
		          Debug.Log(MarketPrices(market));
		          return market;
	     }
	     // Calculate the total value of a set of products based on their state in the market
	     public static float CalculateValueOfProducts(Dictionary<Product, int> products, Dictionary<Product, ProductState> productStates)
	     {
		          float totalValue = 0;
		          foreach (var product in products)
		          {
				if (productStates.ContainsKey(product.Key))
				{
					     totalValue += product.Value * productStates[product.Key].AveragePrice;
				}
		          }
		          return totalValue;
	     }

	     // Update the inventory of a product in a dictionary
	     public static void UpdateInventory(Dictionary<Product, ProductState> products, Product product, int amount)
	     {
		          if (products.ContainsKey(product))
		          {
				products[product].UpdateInventory(amount);
		          }
		          else
		          {
				products.Add(product, new ProductState(amount, 0));
		          }
	     }

	     // Update the desired inventory of a product in a dictionary
	     public static void UpdateDesiredInventory(Dictionary<Product, ProductState> products, Product product, int amount)
	     {
		          if (products.ContainsKey(product))
		          {
				products[product].UpdateDesiredInventory(amount);
		          }
		          else
		          {
				products.Add(product, new ProductState(0, amount));
		          }
	     }

	     // Update the price of a product in a dictionary
	     public static void UpdatePrice(Dictionary<Product, ProductState> products, Product product, float price)
	     {
		          if (products.ContainsKey(product))
		          {
				products[product].SeePrice(price);

		          }
		          else
		          {
				var productState = new ProductState(0, 0);
				productState.SeePrice(price);
				products.Add(product, productState);
		          }
		          //debug log product seen for price
		          UnityEngine.Debug.Log(product.Name + " seen for " + price);
	     }


	     public static string MarketSupplyDemand(Market market)
	     {
		          string marketString = "";
		          foreach (var entity in market.Participants.Keys)
		          {
				//this person is looking to buy this amount of this product
				marketString += entity.Name + "\n";
				foreach (var product in entity.Products.Keys)
				{
					     if (entity.Products[product].SupplyDemand != 0)
					     {
						          ParticipantSupplyDemand(entity, product);
					     }
				}
		          }
		          return marketString;
	     }
	     public static string ParticipantSupplyDemand(EconomicEntity entity, Product product)
	     {
		          string marketString = "";
		          if (entity.Products[product].SupplyDemand > 0)
		          {
				marketString += "Wants to buy " + entity.Products[product].SupplyDemand + " " + product.Name + " ";
				// for this price
				marketString += "for " + entity.Products[product].ExchangeValue + "\n";
		          }
		          else
		          {
				marketString += "Wants to sell " + entity.Products[product].SupplyDemand + " " + product.Name + " ";
				// for this price
				marketString += "for " + entity.Products[product].ExchangeValue + "\n";
		          }
		          return marketString;
	     }
	     public static string MarketPrices(Market market)
	     {

		          string marketString = "";
		          //for each product in the market display the price and price histort
		          foreach (var product in market.Products.Keys)
		          {
				marketString += product.Name + " is currently trading for about " + market.Products[product].ExchangeValue + "\n";
				// it has been trading for this price
				marketString += "It has been trading for " + market.Products[product].AveragePrice + " and has been seen " + market.Products[product].TimesSeen + " times" + "\n";
		          }
		          return marketString;
	     }
}



public static class ProductUtilities
{
	     public static List<Product> GenerateProductNames()
	     {
		          return new List<Product>
	  {
	      new Product("Garlic"),
	      new Product("Peaches"),
	      new Product("Fish"),
	      new Product("Egg"),
	      new Product("Wood"),
	  };
	     }
}

